using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class Song
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid SongId { get; set; }
		public string SongFingerprint { get; set; }
		public string SongMD5 { get; set; }
		public TunrUser Owner { get; set; }
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
				SongID = this.SongId,
				//SongFingerPrint = this.SongFingerprint,
				SongMD5 = this.SongMD5,
				OwnerId = new Guid(this.Owner.Id),
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
		public Guid SongID { get; set; }
		//public string SongFingerPrint { get; set; }
		public Guid OwnerId { get; set; }
		public string SongMD5 { get; set; }
		public string Title { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public int TrackNumber { get; set; }
		public int DiscNumber { get; set; }
		public int Year { get; set; }
		public string Genre { get; set; }
		public double Length { get; set; }
	}
}