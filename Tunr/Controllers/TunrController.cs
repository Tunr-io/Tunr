using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Tunr.Controllers
{
	[RequireHttps]
	public class TunrController : Controller
	{
		public ActionResult Index()
		{
			ViewBag.HeaderHtml = Server.HtmlDecode(System.Configuration.ConfigurationManager.AppSettings["HeaderHtml"]); 
			return View();
		}
	}
}
