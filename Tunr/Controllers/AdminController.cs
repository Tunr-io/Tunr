using Microsoft.AspNet.Identity;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Tunr.Models;

namespace Tunr.Controllers
{
    [RoutePrefix("Admin")]
    public class AdminController : ApiController
    {
        // GET: Admin
        [HttpGet]
        [Route("MigrateFromTableStorage")]
        public HttpResponseMessage MigrateFromTableStorage()
        {
            using (var db = new ApplicationDbContext())
            {
                var uid = User.Identity.GetUserId();
                var user = db.Users.Where(u => u.Id == uid).FirstOrDefault();

                if (!user.IsAdmin)
                {
                    string result = "Hello world! Time is: " + DateTime.Now;
                    var unauthresp = new HttpResponseMessage(HttpStatusCode.OK);
                    unauthresp.Content = new StringContent(result, Encoding.UTF8, "text/plain");
                    return unauthresp;
                }
            }
            string response = "";
            // Now move every song to SQL
            // Update everyone's library size to actually be correct ...
            response += "Starting migration...\n";
            using (var db = new ApplicationDbContext())
            {
                var azureStorageContext = new AzureStorageContext();
                var users = db.Users.ToList();
                foreach (var user in users)
                {
                    var userSongs = new List<SongModel>();
                    TableQuery<TableSong> songQuery = new TableQuery<TableSong>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.Id.ToString()));
                    IEnumerable<TableSong> songs = azureStorageContext.SongTable.ExecuteQuery(songQuery);
                    var calculatedLibrarySize = songs.Sum(s => s.FileSize);
                    user.LibrarySize = calculatedLibrarySize;
                    foreach (var song in songs)
                    {
                        userSongs.Add(new SongModel()
                        {
                            AudioBitrate = song.AudioBitrate,
                            AudioChannels = song.AudioChannels,
                            AudioSampleRate = song.AudioSampleRate,
                            Duration = song.Duration,
                            FileName = song.FileName,
                            FileSize = song.FileSize,
                            FileType = song.FileType,
                            Fingerprint = song.Fingerprint,
                            Md5Hash = song.Md5Hash,
                            Owner = user,
                            SongId = song.SongId,
                            TagAlbum = song.TagAlbum,
                            TagAlbumArtists = song.TagAlbumArtists,
                            TagBeatsPerMinute = song.TagBeatsPerMinute,
                            TagComment = song.TagComment,
                            TagComposers = song.TagComposers,
                            TagConductor = song.TagConductor,
                            TagCopyright = song.TagCopyright,
                            TagDisc = song.TagDisc,
                            TagDiscCount = song.TagDiscCount,
                            TagGenres = song.TagGenres,
                            TagGrouping = song.TagGrouping,
                            TagLyrics = song.TagLyrics,
                            TagPerformers = song.TagPerformers,
                            TagTitle = song.TagTitle,
                            TagTrack = song.TagTrack,
                            TagTrackCount = song.TagTrackCount,
                            TagYear = song.TagYear
                        });
                    }
                    db.Songs.AddRange(userSongs);
                    db.SaveChanges();
                    response += "Added " + userSongs.Count + " for " + user.Email + ".\n";
                }
            }
            response += "Done!\n";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(response, Encoding.UTF8, "text/plain");
            return resp;
        }
    }
}