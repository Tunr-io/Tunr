using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Xml.Serialization;

namespace Tunr.Controllers
{
	[RoutePrefix("api/LibraryData")]
    public class LibraryDataController : ApiController
    {
		private readonly string APIKEY = "***REMOVED***";
		private readonly string SECRET = "***REMOVED***";

		[Route("{artist}/{album}/art")]
		public HttpResponseMessage GetAlbumArt(string artist, string album)
		{
			var response = Request.CreateResponse();
			
			LastFmLib.API20.Settings.AuthData.ApiKey = new LastFmLib.MD5Hash(APIKEY);
			LastFmLib.API20.Settings.AuthData.ApiSecret = new LastFmLib.MD5Hash(SECRET);
			var test = new LastFmLib.LastFmClient();
			var data = test.GetAlbumMetaData(artist,album);

			response.StatusCode = HttpStatusCode.Redirect;
			if (data.CoverLarge == null || data.CoverLarge.AbsoluteUri == "http://cdn.last.fm/flatness/catalogue/noimage/2/default_album_medium.png")
			{
				response.Headers.Location = new Uri(ConvertRelativeUrlToAbsoluteUrl("~/Content/svg/album_cover.svg"));
			}
			else
			{
				response.Headers.Location = data.CoverLarge;
			}
			return response;
		}

		public string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl) {
		   return string.Format("{0}://{1}{2}{3}", (Request.RequestUri.Scheme == Uri.UriSchemeHttps) ? "https" : "http", Request.RequestUri.Host, (Request.RequestUri.Port == 80) ? "" : ":"+Request.RequestUri.Port.ToString(), VirtualPathUtility.ToAbsolute(relativeUrl));
		}
    }
}
