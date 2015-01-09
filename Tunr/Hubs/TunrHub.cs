using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.Identity;
using Tunr.Models;

namespace Tunr.Hubs
{
	[Authorize]
	public class TunrHub : Hub
	{
		private ApplicationDbContext Db = new ApplicationDbContext();
		public override System.Threading.Tasks.Task OnConnected()
		{
			var userId = IdentityExtensions.GetUserId(Context.User.Identity);
			var user = Db.Users.Where(u => u.Id == userId).FirstOrDefault();
			Groups.Add(Context.ConnectionId, userId);
			return base.OnConnected();
		}
		internal void NewSong(TunrUser user, SongViewModel song)
		{
			Clients.Group(user.Id).newSong(song);
		}

		/// <summary>
		/// Fetches a list of playlists that belong to the current user.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PlaylistViewModel> GetPlaylists()
		{
			var userId = IdentityExtensions.GetUserId(Context.User.Identity);
			var playlists = Db.Playlists.Where(p => p.User.Id == userId).ToList().Select(p => p.ToViewModel());
			return playlists;
		}

		/// <summary>
		/// Creates a new playlist with the specified title and optional list of Song IDs
		/// </summary>
		/// <param name="title">New playlist title</param>
		/// <param name="songIds">A list of Song IDs this playlist should contain</param>
		/// <returns>true on success</returns>
		public Guid NewPlaylist(string title, List<Guid> songIds) {
			var userId = IdentityExtensions.GetUserId(Context.User.Identity);
			var newPlaylist = new PlaylistModel()
			{
				PlaylistId = Guid.NewGuid(),
				User = Db.Users.Where(u => u.Id == userId).First(),
				Title = title,
				ModifiedTime = DateTimeOffset.Now,
				Items = new List<PlaylistItemModel>()
			};

			if (songIds != null)
			{
				for (int i = 0; i < songIds.Count(); i++)
				{
					var playlistItem = new PlaylistItemModel()
					{
						PlaylistItemId = Guid.NewGuid(),
						SongId = songIds[i],
						Order = i,
						ModifiedTime = DateTimeOffset.Now
					};
					newPlaylist.Items.Add(playlistItem);
				}
			}
			
			Db.Playlists.Add(newPlaylist);
			Db.SaveChanges();
			return newPlaylist.PlaylistId;
		}
		
		/// <summary>
		/// Appends the specified song to the end of the specified playlist
		/// </summary>
		/// <param name="playlistId">ID of the playlist the song is being added to</param>
		/// <param name="songId">ID of the song to append</param>
		/// <returns></returns>
		public bool AddToPlaylist(Guid playlistId, Guid songId)
		{
			var userId = IdentityExtensions.GetUserId(Context.User.Identity);
			var user = Db.Users.Where(u => u.Id == userId).FirstOrDefault();
			var playlist = Db.Playlists.Where(p => p.PlaylistId == playlistId).FirstOrDefault();
			if (playlist != null)
			{
				int order = 0;
				if (playlist.Items != null && playlist.Items.Count > 0)
				{
					order = playlist.Items.OrderByDescending(i => i.Order).First().Order + 1;
				}
				var playlistItem = new PlaylistItemModel()
				{
					PlaylistItemId = Guid.NewGuid(),
					SongId = songId,
					Order = order,
					ModifiedTime = DateTimeOffset.Now
				};
				playlist.Items.Add(playlistItem);
				Db.SaveChanges();
				return true;
			}
			return false;
		}
	}
}