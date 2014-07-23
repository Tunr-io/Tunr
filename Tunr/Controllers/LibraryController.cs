using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Tunr.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using Microsoft.WindowsAzure.Storage.Table;
using Tunr.Hubs;

namespace Tunr.Controllers
{
	[Authorize]
	[RoutePrefix("api/Library")]
	public class LibraryController : ApiControllerWithHub<TunrHub>
	{
		public static readonly int c_md5size = 128 * 1024;
		private CloudTable azure_table { get; set; }
		public LibraryController()
		{
			// Retrieve the storage account from the connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
				CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the table client.
			CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

			// Create the table if it doesn't exist.
			CloudTable table = tableClient.GetTableReference("songs");
			table.CreateIfNotExists();

			this.azure_table = table;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
		}

		// GET api/Library
		[Route("")]
		public IEnumerable<SongViewModel> Get()
		{
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.Where(u => u.Id == uid).Select(u => u).FirstOrDefault();

				TableQuery<Song> query = new TableQuery<Song>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.Id.ToString()));
				var songs = this.azure_table.ExecuteQuery(query);
				var song_viewmodels = songs.Select(s => s.toViewModel());
				return song_viewmodels;
			}
		}

		// GET api/Library/Artists
		//[Route("Artists")]
		//public IEnumerable<string> GetArtists()
		//{
		//	using (var db = new ApplicationDbContext())
		//	{
		//		var uid = User.Identity.GetUserId();
		//		var user = db.Users.Where(u => u.Id == uid).Select(u => u).FirstOrDefault();

		//		var artists = db.Songs.Where(u => u.Owner.Id == user.Id).Select(u => u.Artist).Distinct().ToList();
		//		return artists;
		//	}
		//}

		// GET api/Library/{song}
		[AllowAnonymous]
		[Route("{songid}")]
		public HttpResponseMessage Get(Guid songid)
		{
			// This could take awhile.
			HttpContext.Current.Server.ScriptTimeout = 600;

			Process ffmpeg = new Process();
			ProcessStartInfo startinfo = new ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/ffmpeg.exe"), "-i - -vn -ar 44100 -ac 2 -ab 192k -f mp3 - ");
			startinfo.RedirectStandardError = true;
			startinfo.RedirectStandardOutput = true;
			startinfo.RedirectStandardInput = true;
			startinfo.UseShellExecute = false;
			startinfo.CreateNoWindow = true;
			ffmpeg.StartInfo = startinfo;
			ffmpeg.ErrorDataReceived += ffmpeg_ErrorDataReceived;
			
			//ffmpeg.BeginOutputReadLine();
			
			// Our response is a stream
			var response = Request.CreateResponse();
			response.StatusCode = HttpStatusCode.OK;

			//response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "track.mp3" };

			// Retrieve storage account from connection string.
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
				CloudConfigurationManager.GetSetting("StorageConnectionString"));

			// Create the blob client.
			CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

			// Retrieve reference to a previously created container.
			CloudBlobContainer container = blobClient.GetContainerReference("songs");

			// Retrieve reference to a blob
			CloudBlockBlob blockBlob = container.GetBlockBlobReference(songid.ToString());

			ffmpeg.Start();
			ffmpeg.BeginErrorReadLine();
			blockBlob.BeginDownloadToStream(ffmpeg.StandardInput.BaseStream, (result) =>
			{
				blockBlob.EndDownloadToStream(result);
				// Close the stream.
				ffmpeg.StandardInput.Close();
			}, new Object[] { blockBlob, ffmpeg.StandardInput.BaseStream });

			// Intermediary stream to handle ffmpeg output before browser (chrome) is ready to consume
			ProducerConsumerStream pcstream = new ProducerConsumerStream();

			// Thread to read from FFMpeg...
			Task.Run(() =>
			{
				byte[] ffmpegbuf = new byte[1024 * 32];
				long length = 0;
				while (true)
				{
					int sz = ffmpeg.StandardOutput.BaseStream.Read(ffmpegbuf, 0, ffmpegbuf.Length);
					//System.Diagnostics.Debug.WriteLine("read " + sz + " bytes from blob.");
					if (sz <= 0) break;
					pcstream.Write(ffmpegbuf, 0, sz);
					pcstream.Flush();
					length += sz;
					//System.Diagnostics.Debug.WriteLine("WROTE " + sz + " bytes to intermediary.");
					if (sz > ffmpegbuf.Length) break;
				}
				System.Diagnostics.Debug.WriteLine("WRITE ENDED");
				pcstream.streamLength = length;
			});

			// Thread to write to browser...
			response.Content = new PushStreamContent((stream, content, context) =>
			{
				byte[] buf = new byte[1024 * 8];
				long length = 0;
				while (true)
				{
					int sz = pcstream.Read(buf, 0, buf.Length);
					//System.Diagnostics.Debug.WriteLine("READ  " + sz + " bytes from intermediary.");
					//if (sz <= 0) break;
					stream.Write(buf, 0, sz);
					stream.Flush();
					length += sz;
					//.Diagnostics.Debug.WriteLine("WROTE " + sz + " bytes to ffmpeg.");
					if (pcstream.streamLength != 0 && length >= pcstream.streamLength) break;
					if (sz > buf.Length) break;
				}
				System.Diagnostics.Debug.WriteLine("READ ENDED.");
				stream.Close();
			}, new MediaTypeHeaderValue("audio/mpeg"));
			
			System.Diagnostics.Debug.WriteLine("Returned response.");
			return response;
		}

		void ffmpeg_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.Data);
		}

		void ffmpeg_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine(e.Data);
		}

		// POST api/Library
		// Add a song to the user's collection
		[Route("")]
		public async Task<IHttpActionResult> Post()
		{
			// Check if the request contains multipart/form-data.
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.Where(u => u.Id == uid).Select(u => u).FirstOrDefault();
				if (user == null)
				{
					return InternalServerError(new Exception("User could not be found"));
				}
				string root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
				var provider = new MultipartFormDataStreamProvider(root);

				try
				{
					// Read the form data.
					await Request.Content.ReadAsMultipartAsync(provider);

					// This illustrates how to get the file names.
					foreach (MultipartFileData file in provider.FileData)
					{
						System.Diagnostics.Debug.WriteLine(file.Headers.ContentDisposition.FileName);
						System.Diagnostics.Debug.WriteLine(file.Headers.ContentType);
						System.Diagnostics.Debug.WriteLine("Server file path: " + file.LocalFileName);
						TagLib.File tagFile;
						string extension = file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf(".") + 1);
						extension = extension.Replace("\"", "").ToLower();
						string type;
						if (extension.Equals("mp3"))
						{
							type = "audio/mpeg";
						}
						else if (extension.Equals("flac"))
						{
							type = "audio/flac";
						}
						else if (extension.Equals("m4a") || extension.Equals("mp4"))
						{
							type = "audio/mp4";
						}
						else if (extension.Equals("aac"))
						{
							type = "audio/aac";
						}
						else if (extension.Equals("ogg"))
						{
							type = "audio/ogg";
						}
						else
						{
							return BadRequest("Format not supported.");
						}
						tagFile = TagLib.File.Create(file.LocalFileName, type, TagLib.ReadStyle.Average);

						// Run fingerprint calculation...
						System.Diagnostics.Process fpcalc = new System.Diagnostics.Process();
						fpcalc.StartInfo = new System.Diagnostics.ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/fpcalc.exe"), file.LocalFileName);
						fpcalc.StartInfo.RedirectStandardOutput = true;
						fpcalc.StartInfo.UseShellExecute = false;
						StringBuilder fp = new StringBuilder();
						fpcalc.Start();
						while (!fpcalc.HasExited)
						{
							fp.Append(fpcalc.StandardOutput.ReadToEnd());
						}
						string fingerprint = fp.ToString();
						fingerprint = fingerprint.Substring(fingerprint.IndexOf("FINGERPRINT=") + 12);

						//string sHash = "";
						//using (StreamReader sr = new StreamReader(file.LocalFileName)) {
						//	MD5CryptoServiceProvider md5h = new MD5CryptoServiceProvider();
						//	sHash = BitConverter.ToString(md5h.ComputeHash(sr.BaseStream)).Replace("-","");
						//}
						string sHash = "";
						using (StreamReader sr = new StreamReader(file.LocalFileName))
						{
							var buf = new byte[LibraryController.c_md5size];
							int bytesRead = 0;
							while (true)
							{
								var read = sr.BaseStream.Read(buf, 0, LibraryController.c_md5size);
								bytesRead += read;
								if (bytesRead == LibraryController.c_md5size || read == 0)
								{
									break;
								}
							}

							MD5CryptoServiceProvider md5h = new MD5CryptoServiceProvider();
							sHash = BitConverter.ToString(md5h.ComputeHash(buf)).Replace("-", "");
						}

						var song = new Song()
						{
							SongId = Guid.NewGuid(),
							OwnerId = new Guid(user.Id),
							Title = tagFile.Tag.Title,
							Artist = tagFile.Tag.Performers.First(),
							Album = tagFile.Tag.Album,
							Genre = tagFile.Tag.FirstGenre,
							Year = (int)tagFile.Tag.Year,
							TrackNumber = (int)tagFile.Tag.Track,
							DiscNumber = (int)tagFile.Tag.Disc,
							SongFingerprint = fingerprint,
							SongMD5 = sHash,
							Length = tagFile.Properties.Duration.TotalSeconds
						};

						// Make sure this file isn't already there
						var existing = this.azure_table.CreateQuery<Song>().Where(x => x.PartitionKey == user.Id.ToString()).Where(x => x.SongMD5 == song.SongMD5).FirstOrDefault();
						if (existing != null)
						{
							return BadRequest("This song already exists.");
						}

						// Upload the song to blob storage ...
						// Retrieve storage account from connection string.
						CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
							CloudConfigurationManager.GetSetting("StorageConnectionString"));

						// Create the blob client.
						CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

						// Retrieve reference to a previously created container.
						CloudBlobContainer container = blobClient.GetContainerReference("songs");
						container.CreateIfNotExists();

						// Retrieve reference to a blob named "myblob".
						CloudBlockBlob blockBlob = container.GetBlockBlobReference(song.SongId.ToString());

						// Create or overwrite the "myblob" blob with contents from a local file.
						using (var fileStream = System.IO.File.OpenRead(file.LocalFileName))
						{
							blockBlob.UploadFromStream(fileStream);
						}

						// Insert the song into table storage...
						// Create the TableOperation that inserts the customer entity.
						TableOperation insertOperation = TableOperation.Insert(song);

						// Execute the insert operation.
						this.azure_table.Execute(insertOperation);

						// Push the new song to SignalR clients
						this.Hub.Clients.Group(user.Id).newSong(song.toViewModel());

						System.IO.File.Delete(file.LocalFileName);
					}
					return Ok();
				}
				catch (System.Exception e)
				{
					return InternalServerError(e);
				}
			}
		}
	}

	class ProducerConsumerStream : Stream
	{
		private readonly MemoryStream innerStream;
		private long readPosition;
		private long writePosition;
		public long streamLength = 0;

		public ProducerConsumerStream()
		{
			innerStream = new MemoryStream();
		}

		public override bool CanRead { get { return true; } }

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite { get { return true; } }

		public override void Flush()
		{
			lock (innerStream)
			{
				innerStream.Flush();
			}
		}

		public override long Length
		{
			get
			{
				lock (innerStream)
				{
					return innerStream.Length;
				}
			}
		}

		public override long Position
		{
			get { throw new NotSupportedException(); }
			set { throw new NotSupportedException(); }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			lock (innerStream)
			{
				innerStream.Position = readPosition;
				int red = innerStream.Read(buffer, offset, count);
				readPosition = innerStream.Position;

				return red;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			lock (innerStream)
			{
				innerStream.Position = writePosition;
				innerStream.Write(buffer, offset, count);
				writePosition = innerStream.Position;
			}
		}
	}
}