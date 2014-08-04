using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class PlaylistModel
	{
		[Key]
		public Guid PlaylistId { get; set; }
		public virtual TunrUser User { get; set; }
		public string Title { get; set; }
		public virtual ICollection<PlaylistItemModel> Items { get; set; }
		public DateTimeOffset ModifiedTime { get; set; }

		public PlaylistViewModel ToViewModel()
		{
			TunrUserViewModel userVm = null;
			if (this.User != null)
			{
				userVm = this.User.toViewModel();
			}
			return new PlaylistViewModel()
			{
				PlaylistId = this.PlaylistId,
				Title = this.Title,
				User = userVm,
				Items = this.Items,
				ModifiedTime = this.ModifiedTime
			};
		}
	}

	public class PlaylistViewModel
	{
		public Guid PlaylistId { get; set; }
		public TunrUserViewModel User { get; set; }
		public string Title { get; set; }
		public ICollection<PlaylistItemModel> Items { get; set; }
		public DateTimeOffset ModifiedTime { get; set; }
	}
}