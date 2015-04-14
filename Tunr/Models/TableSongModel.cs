using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
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
	public class TableSong : TableEntity
	{
		/// <summary>
		/// I set the rowkey
		/// </summary>
		[JsonProperty("songId")]
		[IgnoreProperty]
		public Guid SongId
		{
			get
			{
				return Guid.Parse(this.RowKey);
			}
			set
			{
				this.RowKey = value.ToString();
			}
		}
		
		/// <summary>
		/// I set the PartitionKey
		/// </summary>
		[IgnoreProperty]
		public Guid OwnerId
		{
			get
			{
				return Guid.Parse(this.PartitionKey);
			}
			set
			{
				this.PartitionKey = value.ToString();
			}
		}

		/// <summary>
		/// Audio data fingerprint of the audio file.
		/// </summary>
		public string Fingerprint { get; set; }

		/// <summary>
		/// MD5 hash of the first few KB of the file. Used to prevent duplicates.
		/// </summary>
		[JsonProperty("md5Hash")]
		public string Md5Hash { get; set; }

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
		public int AudioBitrate { get; set; }

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
		[IgnoreProperty]
		[JsonProperty("tagPerformers")]
		public List<string> TagPerformers { get; set; }

		/// <summary>
		/// Tag: List of track album artists.
		/// </summary>
		[IgnoreProperty]
		[JsonProperty("tagAlbumArtists")]
		public List<string> TagAlbumArtists { get; set; }

		/// <summary>
		/// Tag: List of track composers.
		/// </summary>
		[IgnoreProperty]
		[JsonProperty("tagComposers")]
		public List<string> TagComposers { get; set; }

		/// <summary>
		/// Tag: List of track genres.
		/// </summary>
		[IgnoreProperty]
		[JsonProperty("tagGenres")]
		public List<string> TagGenres { get; set; }

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

		public override IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
		{
			var results = base.WriteEntity(operationContext);
			// Serialize lists to JSON
			results.Add("TagPerformers", new EntityProperty(JsonConvert.SerializeObject(this.TagPerformers)));
			results.Add("TagAlbumArtists", new EntityProperty(JsonConvert.SerializeObject(this.TagAlbumArtists)));
			results.Add("TagComposers", new EntityProperty(JsonConvert.SerializeObject(this.TagComposers)));
			results.Add("TagGenres", new EntityProperty(JsonConvert.SerializeObject(this.TagGenres)));
			return results;
		}

		public override void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
		{
			base.ReadEntity(properties, operationContext);
			// Deserialize list values from JSON
			this.TagPerformers = JsonConvert.DeserializeObject<List<string>>(properties["TagPerformers"].StringValue);
			this.TagAlbumArtists = JsonConvert.DeserializeObject<List<string>>(properties["TagAlbumArtists"].StringValue);
			this.TagComposers = JsonConvert.DeserializeObject<List<string>>(properties["TagComposers"].StringValue);
			this.TagGenres = JsonConvert.DeserializeObject<List<string>>(properties["TagGenres"].StringValue);
		}
	}
}