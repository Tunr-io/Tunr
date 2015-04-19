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
using Microsoft.Xbox.Music.Platform.Client;
using Microsoft.Xbox.Music.Platform.Contract.DataModel;
using Facebook;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tunr.Controllers
{
	[Authorize]
	[RoutePrefix("api/Library")]
	public class LibraryController : ApiControllerWithHub<TunrHub>
	{
		public static readonly int c_md5size = 128 * 1024;
		private AzureStorageContext AzureStorageContext { get; set; }
		public LibraryController()
		{
			AzureStorageContext = new AzureStorageContext();
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
		public IEnumerable<SongModel> Get()
		{
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.Where(u => u.Id == uid).FirstOrDefault();

                return db.Songs.Where(s => s.Owner.Id == user.Id).ToList();
			}
		}

		/// <summary>
		/// Fetches album art for the given track id.
		/// </summary>
		/// <param name="id">Track GUID</param>
		/// <returns>Redirects to an album art image.</returns>
		[Route("{id}/AlbumArt")]
		[AllowAnonymous] // HACK: Really shouldn't allow anonymous, but something's up with the cookie auth.
		public async Task<HttpResponseMessage> GetAlbumArt(Guid id)
		{
            using (var db = new ApplicationDbContext()) {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["XboxClientId"]))
                {
                    var song = db.Songs.SingleOrDefault(s => s.SongId == id);
                    if (song == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Cannot locate specified track ID.");
                    }
                    IXboxMusicClient client = XboxMusicClientFactory.CreateXboxMusicClient(ConfigurationManager.AppSettings["XboxClientId"], ConfigurationManager.AppSettings["XboxClientSecret"]);
                    var response = await client.SearchAsync(Namespace.music, song.TagPerformers.FirstOrDefault() + " " + song.TagAlbum, filter: SearchFilter.Albums, maxItems: 1, country: "US");
                    if (response.Albums != null && response.Albums.TotalItemCount > 0)
                    {
                        var resp = Request.CreateResponse();
                        resp.StatusCode = HttpStatusCode.Redirect;
                        resp.Headers.Location = new Uri(response.Albums.Items[0].ImageUrl);
                        return resp;
                    }
                }
                // If our look-up fails, we just spit out the default album art.
                var defaultResp = Request.CreateResponse();
                defaultResp.StatusCode = HttpStatusCode.Redirect;
                defaultResp.Headers.Location = new Uri("/Content/svg/album_cover.svg", UriKind.Relative);
                return defaultResp;
            }
		}

		/// <summary>
		/// Returns a bunch of information and art on the specified track's artist.
		/// </summary>
		/// <param name="id">Track GUID</param>
		/// <returns>List of images and various information.</returns>
		[Route("{id}/ArtistInfo")]
		[AllowAnonymous] // HACK: Really shouldn't allow anonymous, but something's up with the cookie auth.
		public async Task<IHttpActionResult> GetImage(Guid id)
		{
            using (var db = new ApplicationDbContext())
            {
                var song = db.Songs.SingleOrDefault(s => s.SongId == id);
                if (song == null) {
				    return BadRequest("Cannot locate specified track ID.");
			    }
			    var artistInfo = new ArtistInfo()
			    {
				    ImageUrls = new List<string>()
			    };
			    try
			    {
				    // Fetch images from Facebook
				    var fbClient = new FacebookClient();
				    dynamic tokenResult = fbClient.Get("oauth/access_token", new
				    {
					    client_id = ConfigurationManager.AppSettings["FacebookClientId"],
					    client_secret = ConfigurationManager.AppSettings["FacebookClientSecret"],
					    grant_type = "client_credentials"
				    });
				    fbClient.AccessToken = tokenResult.access_token;

				    FacebookSearchResponse fbRequest = fbClient.Get<FacebookSearchResponse>("search", new { q = song.TagPerformers.FirstOrDefault(), type = "page" });
				    IEnumerable<FacebookSearchResult> fbResults = fbRequest.data.Where(x => x.category.Equals("Musician/band"));
				    dynamic pageId = fbResults.FirstOrDefault().id;

				    FacebookImageResponse fbImageRequest = fbClient.Get<FacebookImageResponse>(pageId + "/photos/uploaded?fields=images&limit=100");
				    Random rnd = new Random();
				    var hiresFbImages = fbImageRequest.data.Where(x => x.images.Count() > 0).Where(x => x.images.First().width * x.images.First().height > 750000).OrderBy(x => rnd.Next()).Take(5);
				    artistInfo.ImageUrls.AddRange(hiresFbImages.Select(x => x.images.First().source));
			
				    // Fetch images from Xbox Music
				    IXboxMusicClient client = XboxMusicClientFactory.CreateXboxMusicClient(ConfigurationManager.AppSettings["XboxClientId"], ConfigurationManager.AppSettings["XboxClientSecret"]);
				    var response = await client.SearchAsync(Namespace.music, song.TagPerformers.FirstOrDefault(), filter: SearchFilter.Artists, maxItems: 1, country: "US");
				    if (response.Artists.TotalItemCount > 0)
				    {
					    artistInfo.ImageUrls.Add(response.Artists.Items[0].ImageUrl);
				    }
			    }
			    catch (Exception) { }

			    Random rand = new Random();
			    artistInfo.ImageUrls = artistInfo.ImageUrls.OrderBy(x => rand.Next()).ToList();
			    return Ok<ArtistInfo>(artistInfo);
            }
        }

        [Route("{id}/Download")]
        [AllowAnonymous] // HACK: Really shouldn't allow anonymous, but something's up with the cookie auth.
        public HttpResponseMessage GetDownload(Guid id)
        {
            using (var db = new ApplicationDbContext())
            {
                var song = db.Songs.SingleOrDefault(s => s.SongId == id);
                if (song == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, new ApplicationException("Specified song could not be found"));
                }
                HttpResponseMessage response = new HttpResponseMessage();
                
                var storageContainer = AzureStorageContext.UploadsContainer;
                var storageBlob = storageContainer.GetBlockBlobReference(song.SongId.ToString());

                response.Content = new StreamContent(storageBlob.OpenRead());
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                var disposition = new ContentDispositionHeaderValue("attachment");
                disposition.FileName = song.FileName;
                disposition.Size = song.FileSize;
                response.Content.Headers.ContentDisposition = disposition;
                response.Content.Headers.ContentLength = song.FileSize;

                return response;
            }
        }

        [Route("Sync")]
        public IHttpActionResult GetSyncBase()
        {
            using (var db = new ApplicationDbContext())
            {
                var uid = User.Identity.GetUserId();
                var user = db.Users.SingleOrDefault(u => u.Id == uid);
                if (user == null)
                {
                    return InternalServerError(new Exception("User could not be found"));
                }

                // Pull the last changeset by this user
                var changeSet = db.ChangeSets.Where(c => c.Owner.Id == user.Id).OrderByDescending(c => c.LastModifiedTime).Take(1).FirstOrDefault();

                if (changeSet != null)
                {
                    // If this changeset was fresh, it is no longer.
                    // Update in a concurrency-safe way
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        changeSet.IsFresh = false;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            saveFailed = true;
                            e.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                }

                // Pull the user's library
                var songs = db.Songs.Where(s => s.Owner.Id == user.Id).ToList();

                return Ok<SyncBaseModel>(new SyncBaseModel()
                {
                    LastSyncId = changeSet == null ? Guid.Empty : (Guid?)changeSet.ChangeSetId,
                    Library = songs
                });
            }
        }

		/// <summary>
		/// Fetches all 
		/// </summary>
		/// <returns></returns>
		[Route("Sync/{lastChangeId}")]
		public IHttpActionResult GetSync(Guid lastChangeId)
		{
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.SingleOrDefault(u => u.Id == uid);
				if (user == null)
				{
					return InternalServerError(new Exception("User could not be found"));
				}

                // Find the changeset that they gave us
                // if they give us an empty ID, get all changesets
                IOrderedQueryable<ChangeSetModel> newSets;
                if (lastChangeId.Equals(Guid.Empty))
                {
                    newSets = db.ChangeSets.Where(c => c.Owner.Id == user.Id).OrderBy(c => c.LastModifiedTime);
                } else
                {
                    var baseChangeSet = db.ChangeSets.SingleOrDefault(c => c.ChangeSetId == lastChangeId && c.Owner.Id == user.Id);
                    if (baseChangeSet == null)
                    {
                        return NotFound();
                    }
                    newSets = db.ChangeSets.Where(c => c.Owner.Id == user.Id && c.LastModifiedTime > baseChangeSet.LastModifiedTime).OrderBy(c => c.LastModifiedTime);
                }

                // Pull all of this user's changesets
                var changes = newSets.Select(c => new
                {
                    ChangeSetId = c.ChangeSetId,
                    IsFresh = c.IsFresh,
                    LastModifiedTime = c.LastModifiedTime,
                    Changes = c.Changes.Select(c2 => new
                    {
                        Type = c2.Type,
                        Song = db.Songs.Where(s => s.SongId == c2.SongId).FirstOrDefault()
                    })
                }).ToList();

                // Make sure we mark all the old changesets as un-fresh
                foreach (var change in changes)
                {
                    bool saveFailed;
                    do
                    {
                        saveFailed = false;
                        db.ChangeSets.SingleOrDefault(c => c.ChangeSetId == change.ChangeSetId).IsFresh = false;
                        try
                        {
                            db.SaveChanges();
                        }
                        catch (DbUpdateConcurrencyException e)
                        {
                            saveFailed = true;
                            e.Entries.Single().Reload();
                        }
                    } while (saveFailed);
                }

				return Ok<IEnumerable<object>>(changes);
			}
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
						TagLib.File tagFile;
						string fileName = file.Headers.ContentDisposition.FileName.Replace("\"", "");
						string extension = fileName.Substring(fileName.LastIndexOf(".") + 1);
						extension = extension.ToLower();
						string type;
						switch (extension)
						{
							case "mp3":
								type = "audio/mpeg";
								break;
							case "flac":
								type = "audio/flac";
								break;
							case "m4a":
							case "mp4":
								type = "audio/mp4";
								break;
							case "aac":
								type = "audio/aac";
								break;
							case "ogg":
								type = "audio/ogg";
								break;
							default:
								return BadRequest("Format not supported.");
						}
						tagFile = TagLib.File.Create(file.LocalFileName, type, TagLib.ReadStyle.Average);

                        // Run fingerprint calculation...
                        string fingerprint = "";
                        using (System.Diagnostics.Process fpcalc = new System.Diagnostics.Process())
                        {
                            fpcalc.StartInfo = new System.Diagnostics.ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/fpcalc.exe"), file.LocalFileName);
                            fpcalc.StartInfo.RedirectStandardOutput = true;
                            fpcalc.StartInfo.UseShellExecute = false;
                            StringBuilder fp = new StringBuilder();
                            fpcalc.Start();

                            while (!fpcalc.StandardOutput.EndOfStream)
                            {
                                fp.Append(fpcalc.StandardOutput.ReadLine());
                            }
                            fingerprint = fp.ToString();
                            fingerprint = fingerprint.Substring(fingerprint.IndexOf("FINGERPRINT=") + 12);
                        }

                        // Calculate file's MD5 hash (only the first few KB to save time)
                        string fileMd5Hash = "";
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
                            fileMd5Hash = BitConverter.ToString(md5h.ComputeHash(buf)).Replace("-", "");
                        }

                        // Calculate MD5 hash of raw audio
                        string audioMd5Hash = "";
                        using (System.Diagnostics.Process ffhash = new System.Diagnostics.Process())
                        {
                            string args = "-i \"" + file.LocalFileName + "\" -acodec copy -vn -f md5 - ";
                            ffhash.StartInfo = new System.Diagnostics.ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/ffmpeg.exe"), args);
                            ffhash.StartInfo.RedirectStandardOutput = true;
                            ffhash.StartInfo.UseShellExecute = false;
                            StringBuilder ffhashOutput = new StringBuilder();
                            ffhash.Start();
                            while (!ffhash.StandardOutput.EndOfStream)
                            {
                                ffhashOutput.Append(ffhash.StandardOutput.ReadLine());
                            }
                            audioMd5Hash = ffhashOutput.ToString();
                            audioMd5Hash = audioMd5Hash.Substring(audioMd5Hash.IndexOf("MD5=") + 4, 32);
                        }

                        // Grab a little bit more data
                        string codecName = "";
                        int channels = 0;
                        int sampleRate = 0;
                        double duration = 0;
                        double bitrate = 0;
                        using (System.Diagnostics.Process ffprobe = new System.Diagnostics.Process())
                        {
                            ffprobe.StartInfo = new System.Diagnostics.ProcessStartInfo(HostingEnvironment.MapPath("~/App_Data/executables/ffprobe.exe"), "-v quiet -print_format json -show_format -show_streams \"" + file.LocalFileName + "\"");
                            ffprobe.StartInfo.RedirectStandardOutput = true;
                            ffprobe.StartInfo.UseShellExecute = false;
                            StringBuilder ffprobeOutput = new StringBuilder();
                            ffprobe.Start();
                            while (!ffprobe.StandardOutput.EndOfStream)
                            {
                                ffprobeOutput.Append(ffprobe.StandardOutput.ReadLine());
                            }
                            var ffprobeObject = JObject.Parse(ffprobeOutput.ToString());
                            duration = ffprobeObject.SelectToken("format").Value<double>("duration");
                            bitrate = (double)(ffprobeObject.SelectToken("format").Value<long>("bit_rate") / (double)1024);
                            // Find the first audio stream to get audio stream data
                            foreach (JObject stream in ffprobeObject.SelectToken("streams"))
                            {
                                if (stream.Value<string>("codec_type") == "audio")
                                {
                                    codecName = stream.Value<string>("codec_name");
                                    channels = stream.Value<int>("channels");
                                    sampleRate = stream.Value<int>("sample_rate");
                                    break;
                                }
                            }
                        }

                        var song = new SongModel()
						{
							SongId = Guid.NewGuid(),
							Owner = user,
							FileName = Path.GetFileName(fileName),
							//FileType = extension,
                            FileType = codecName,
							FileSize = new FileInfo(file.LocalFileName).Length,
							//AudioBitrate = tagFile.Properties.AudioBitrate,
                            AudioBitrate = bitrate,
							//AudioChannels = tagFile.Properties.AudioChannels,
                            AudioChannels = channels,
							//AudioSampleRate = tagFile.Properties.AudioSampleRate,
                            AudioSampleRate = sampleRate,
							Fingerprint = fingerprint,
                            FileMd5Hash = fileMd5Hash,
							AudioMd5Hash = audioMd5Hash,
							Duration = tagFile.Properties.Duration.TotalSeconds,
							TagTitle = tagFile.Tag.Title,
							TagAlbum = tagFile.Tag.Album,
							TagPerformers = new List<string>(tagFile.Tag.Performers),
							TagAlbumArtists = new List<string>(tagFile.Tag.AlbumArtists),
							TagComposers = new List<string>(tagFile.Tag.Composers),
							TagGenres = new List<string>(tagFile.Tag.Genres),
							TagYear = (int)tagFile.Tag.Year,
							TagTrack = (int)tagFile.Tag.Track,
							TagTrackCount = (int)tagFile.Tag.TrackCount,
							TagDisc = (int)tagFile.Tag.Disc,
							TagDiscCount = (int)tagFile.Tag.DiscCount,
							TagComment = tagFile.Tag.Comment,
							TagLyrics = tagFile.Tag.Lyrics,
							TagConductor = tagFile.Tag.Conductor,
							TagBeatsPerMinute = (int)tagFile.Tag.BeatsPerMinute,
							TagGrouping = tagFile.Tag.Grouping,
							TagCopyright = tagFile.Tag.Copyright
						};

						tagFile.Dispose();

                        // Make sure this file isn't already there
                        var existing = db.Songs.SingleOrDefault(s => s.Owner.Id == user.Id && s.AudioMd5Hash == song.AudioMd5Hash);
						if (existing != null)
						{
							File.Delete(file.LocalFileName);
							return BadRequest("This song already exists.");
						}

						// Retrieve reference to a blob
						CloudBlockBlob blockBlob = AzureStorageContext.UploadsContainer.GetBlockBlobReference(song.SongId.ToString());

						// Create or overwrite blob with contents from a local file.
						using (var fileStream = File.OpenRead(file.LocalFileName))
						{
							blockBlob.UploadFromStream(fileStream);
						}

                        // Insert the song into database...
                        db.Songs.Add(song);
                        db.SaveChanges();

                        // Is there a fresh change set?
                        var freshChangeSet = db.ChangeSets.SingleOrDefault(c => c.Owner.Id == user.Id && c.IsFresh == true);
						if (freshChangeSet != null)
						{
							freshChangeSet.Changes.Add(new ChangeModel()
                            {
                                ChangeId = Guid.NewGuid(),
                                SongId = song.SongId,
                                Type = ChangeModel.ChangeType.Create
                            });
							freshChangeSet.LastModifiedTime = DateTimeOffset.Now;
                            db.SaveChanges();
						}
						else if (freshChangeSet == null) // If null, we have no fresh changeset, need to make a new one.
						{
							var newChangeSet = new ChangeSetModel()
							{
								ChangeSetId = Guid.NewGuid(),
								Owner = user,
								Changes = new List<ChangeModel>()
                                {
                                    new ChangeModel()
                                    {
                                        ChangeId = Guid.NewGuid(),
                                        SongId = song.SongId,
                                        Type = ChangeModel.ChangeType.Create
                                    }
                                },
								IsFresh = true,
								LastModifiedTime = DateTimeOffset.Now,
							};
                            db.ChangeSets.Add(newChangeSet);
                            db.SaveChanges();
						}

						// Update user's library size in a concurrency-safe way
						bool saveFailed;
						do
						{
							saveFailed = false;
							user.LibrarySize += song.FileSize;
							try
							{
								db.SaveChanges();
							}
							catch (DbUpdateConcurrencyException e)
							{
								saveFailed = true;
								e.Entries.Single().Reload();
							}
						} while (saveFailed);

						// Push the new song to SignalR clients
						this.Hub.Clients.Group(user.Id).newSong(song);

						File.Delete(file.LocalFileName);
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
}