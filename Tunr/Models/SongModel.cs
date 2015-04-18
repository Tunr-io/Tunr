using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SongModel
    {
        /// <summary>
		/// I set the rowkey
		/// </summary>
		[JsonProperty("songId")]
        [Key]
        public Guid SongId { get; set; }

        /// <summary>
        /// I set the PartitionKey
        /// </summary>
        public TunrUser Owner { get; set; }

        /// <summary>
        /// Audio data fingerprint of the audio file.
        /// </summary>
        public string Fingerprint { get; set; }

        /// <summary>
        /// MD5 hash of the first few KB of the file. Used to prevent duplicates.
        /// </summary>
        [JsonProperty("fileMd5Hash")]
        [Index]
        [StringLength(36)]
        public string FileMd5Hash { get; set; }

        /// <summary>
        /// MD5 hash of the file audio contents. Used to prevent duplicates.
        /// </summary>
        [JsonProperty("AudioMd5Hash")]
        [Index]
        [StringLength(36)]
        public string AudioMd5Hash { get; set; }

        /// <summary>
        /// Full name of the file originally uploaded.
        /// </summary>
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        /// <summary>
        /// Type of this file - 'mp3', 'flac', etc.
        /// </summary>
        [JsonProperty("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// Size of the file in bytes.
        /// </summary>
        [JsonProperty("fileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// Number of audio channels.
        /// </summary>
        [JsonProperty("audioChannels")]
        public int AudioChannels { get; set; }

        /// <summary>
        /// Bitrate of the audio.
        /// </summary>
        [JsonProperty("audioBitrate")]
        public double AudioBitrate { get; set; }

        /// <summary>
        /// Sample rate of the audio.
        /// </summary>
        [JsonProperty("audioSampleRate")]
        public int AudioSampleRate { get; set; }

        /// <summary>
        /// Duration of the song in seconds.
        /// </summary>
        [JsonProperty("duration")]
        public double Duration { get; set; }

        /// <summary>
        /// Tag: Track title.
        /// </summary>
        [JsonProperty("tagTitle")]
        public string TagTitle { get; set; }

        /// <summary>
        /// Tag: Track album.
        /// </summary>
        [JsonProperty("tagAlbum")]
        public string TagAlbum { get; set; }

        /// <summary>
        /// Tag: List of track performers.
        /// </summary>
        [JsonProperty("tagPerformers")]
        [NotMapped]
        public ICollection<string> TagPerformers
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(TagPerformersJson);
            }
            set
            {
                TagPerformersJson = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// Backing SQL store for list of performers, in JSON
        /// </summary>
        public string TagPerformersJson { get; set; }

        /// <summary>
        /// Tag: List of track album artists.
        /// </summary>
        [JsonProperty("tagAlbumArtists")]
        [NotMapped]
        public ICollection<string> TagAlbumArtists
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(TagAlbumArtistsJson);
            }
            set
            {
                TagAlbumArtistsJson = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// Backing SQL store for list of album artists, in JSON
        /// </summary>
        public string TagAlbumArtistsJson { get; set; }

        /// <summary>
        /// Tag: List of track composers.
        /// </summary>
        [JsonProperty("tagComposers")]
        [NotMapped]
        public ICollection<string> TagComposers
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(TagComposersJson);
            }
            set
            {
                TagComposersJson = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// Backing SQL store for list of composers, in JSON
        /// </summary>
        public string TagComposersJson { get; set; }

        /// <summary>
        /// Tag: List of track genres.
        /// </summary>
        [JsonProperty("tagGenres")]
        [NotMapped]
        public ICollection<string> TagGenres
        {
            get
            {
                return JsonConvert.DeserializeObject<List<string>>(TagGenresJson);
            }
            set
            {
                TagGenresJson = JsonConvert.SerializeObject(value);
            }
        }

        /// <summary>
        /// Backing SQL store for list of genres, in JSON
        /// </summary>
        public string TagGenresJson { get; set; }

        /// <summary>
        /// Tag: Track year.
        /// </summary>
        [JsonProperty("tagYear")]
        public int TagYear { get; set; }

        /// <summary>
        /// Tag: Track number.
        /// </summary>
        [JsonProperty("tagTrack")]
        public int TagTrack { get; set; }

        /// <summary>
        /// Tag: Total album track count.
        /// </summary>
        [JsonProperty("tagTrackCount")]
        public int TagTrackCount { get; set; }

        /// <summary>
        /// Tag: Track disc number.
        /// </summary>
        [JsonProperty("tagDisc")]
        public int TagDisc { get; set; }

        /// <summary>
        /// Tag: Total album disc count.
        /// </summary>
        [JsonProperty("tagDiscCount")]
        public int TagDiscCount { get; set; }

        /// <summary>
        /// Tag: Track comment.
        /// </summary>
        [JsonProperty("tagComment")]
        public string TagComment { get; set; }

        /// <summary>
        /// Tag: Track lyrics.
        /// </summary>
        [JsonProperty("tagLyrics")]
        public string TagLyrics { get; set; }

        /// <summary>
        /// Tag: Track conductor.
        /// </summary>
        [JsonProperty("tagConductor")]
        public string TagConductor { get; set; }

        /// <summary>
        /// Tag: Track BPM.
        /// </summary>
        [JsonProperty("tagBeatsPerMinute")]
        public int TagBeatsPerMinute { get; set; }

        /// <summary>
        /// Tag: Track grouping.
        /// </summary>
        [JsonProperty("tagGrouping")]
        public string TagGrouping { get; set; }

        /// <summary>
        /// Tag: Track copyright.
        /// </summary>
        [JsonProperty("tagCopyright")]
        public string TagCopyright { get; set; }
    }
}