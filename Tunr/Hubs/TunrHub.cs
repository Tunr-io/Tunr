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
	}
}