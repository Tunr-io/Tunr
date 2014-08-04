using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class PlaylistItemModel
	{
		[Key]
		public Guid PlaylistItemId { get; set; }
		public Guid SongId { get; set; }
		public int Order { get; set; }
		public DateTimeOffset ModifiedTime { get; set; }
	}
}