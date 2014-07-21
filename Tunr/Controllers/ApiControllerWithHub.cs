using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Tunr.Controllers
{
	public abstract class ApiControllerWithHub<THub> : ApiController
			where THub : IHub
	{
		Lazy<IHubContext> hub = new Lazy<IHubContext>(
			() => GlobalHost.ConnectionManager.GetHubContext<THub>()
		);


		protected IHubContext Hub
		{
			get { return hub.Value; }
		}
	}
}