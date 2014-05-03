﻿using System;
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

namespace Tunr.Controllers
{
	[Authorize]
	[RoutePrefix("api/Library")]
	public class LibraryController : ApiController
	{
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

				var songs = db.Songs.Where(u => u.Owner.Id == user.Id).Select(u => u).ToList().Select(u => u.toViewModel());
				return songs;
			}
		}

		// GET api/Library/Artists
		[Route("Artists")]
		public IEnumerable<string> GetArtists()
		{
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.Where(u => u.Id == uid).Select(u => u).FirstOrDefault();

				var artists = db.Songs.Where(u => u.Owner.Id == user.Id).Select(u => u.Artist).Distinct().ToList();
				return artists;
			}
		}

		// GET api/Library/{song}
		[AllowAnonymous]
		[Route("{songid}")]
		public HttpResponseMessage Get(Guid songid)
		{
			Process ffmpeg = new Process();
			ProcessStartInfo startinfo = new ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/ffmpeg.exe"), "-i - -vn -ar 44100 -ac 2 -ab 128k -f mp3 - ");
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
			response.Content = new StreamContent(ffmpeg.StandardOutput.BaseStream);
			response.Content.Headers.ContentType = new MediaTypeHeaderValue("audio/mpeg");

			// Read everything from the blob in a separate thread.
			Task.Run(() =>
			{
				try
				{
					System.Diagnostics.Debug.WriteLine("Reading from blob...");
					var blobstream = blockBlob.OpenRead();
					byte[] buf = new byte[1024 * 32];
					while (true)
					{
						int sz = blobstream.Read(buf, 0, buf.Length);
						//System.Diagnostics.Debug.WriteLine("read " + sz + " bytes from blob.");
						if (sz <= 0) break;
						ffmpeg.StandardInput.BaseStream.Write(buf, 0, sz);
						ffmpeg.StandardInput.BaseStream.Flush();
						//.Diagnostics.Debug.WriteLine("WROTE " + sz + " bytes to ffmpeg.");
						if (sz > buf.Length) break;
					}
					blobstream.Close();
					ffmpeg.StandardInput.BaseStream.Close();
				}
				catch (Exception)
				{
					System.Diagnostics.Debug.WriteLine("something weird happened.");
				}
			});

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

						var song = new Song()
						{
							Owner = user,
							Title = tagFile.Tag.Title,
							Artist = tagFile.Tag.Performers.First(),
							Album = tagFile.Tag.Album,
							Genre = tagFile.Tag.FirstGenre,
							Year = (int)tagFile.Tag.Year,
							TrackNumber = (int)tagFile.Tag.Track,
							DiscNumber = (int)tagFile.Tag.Disc,
							SongFingerprint = fingerprint,
							Length = 0 //Todo
						};

						db.Songs.Add(song);
						db.SaveChanges();

						// Upload the song to blob storage ...
						// Retrieve storage account from connection string.
						CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
							CloudConfigurationManager.GetSetting("StorageConnectionString"));

						// Create the blob client.
						CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

						// Retrieve reference to a previously created container.
						CloudBlobContainer container = blobClient.GetContainerReference("songs");

						// Retrieve reference to a blob named "myblob".
						CloudBlockBlob blockBlob = container.GetBlockBlobReference(song.SongId.ToString());

						// Create or overwrite the "myblob" blob with contents from a local file.
						using (var fileStream = System.IO.File.OpenRead(file.LocalFileName))
						{
							blockBlob.UploadFromStream(fileStream);
						}

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
	public class AudioStream
	{
		private readonly Stream _source_stream;
		public AudioStream(Stream src)
		{
			_source_stream = src;
		}

		public async void WriteToStream(Stream outputStream, HttpContent content, TransportContext context)
		{
			try
			{
				var buffer = new byte[1024];
				while (true)
				{
					int sz = await _source_stream.ReadAsync(buffer, 0, buffer.Length);
					System.Diagnostics.Debug.WriteLine("READDDD " + sz + " bytes from ffmpeg.");
					if (sz <= 0) break;
					await outputStream.WriteAsync(buffer, 0, sz);
					await outputStream.FlushAsync();
				}
			}
			catch (HttpException ex)
			{
				return;
			}
			finally
			{
				outputStream.Close();
			}
		}
	}


}