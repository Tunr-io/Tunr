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
	public class Song : TableEntity
	{
		/// <summary>
		/// I set the rowkey
		/// </summary>
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
		public string SongFingerprint { get; set; }
		public string SongMD5 { get; set; }
		/// <summary>
		/// I set the PartitionKey
		/// </summary>
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
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public int TrackNumber { get; set; }
		public int DiscNumber { get; set; }
		public int Year { get; set; }
		public string Genre { get; set; }
		public double Length { get; set; }

		public SongViewModel toViewModel()
		{
			return new SongViewModel()
			{
				SongId = this.SongId,
				//SongFingerPrint = this.SongFingerprint,
				SongMD5 = this.SongMD5,
				OwnerId = this.OwnerId,
				Title = this.Title,
				Artist = this.Artist,
				Album = this.Album,
				TrackNumber = this.TrackNumber,
				DiscNumber = this.DiscNumber,
				Year = this.Year,
				Genre = this.Genre,
				Length = this.Length
			};
		}
	}

	public class SongViewModel
	{
		[JsonProperty("songId")]
		public Guid SongId { get; set; }
		//public string SongFingerPrint { get; set; }
		[JsonProperty("ownerId")]
		public Guid OwnerId { get; set; }
		[JsonProperty("songMD5")]
		public string SongMD5 { get; set; }
		[JsonProperty("title")]
		public string Title { get; set; }
		[JsonProperty("artist")]
		public string Artist { get; set; }
		[JsonProperty("album")]
		public string Album { get; set; }
		[JsonProperty("trackNumber")]
		public int TrackNumber { get; set; }
		[JsonProperty("discNumber")]
		public int DiscNumber { get; set; }
		[JsonProperty("year")]
		public int Year { get; set; }
		[JsonProperty("genre")]
		public string Genre { get; set; }
		[JsonProperty("length")]
		public double Length { get; set; }
	}
}