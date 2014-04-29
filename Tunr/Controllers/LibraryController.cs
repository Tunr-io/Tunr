using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Tunr.Controllers
{
	//[Authorize]
	public class LibraryController : ApiController
	{
		// GET api/<controller>
		public IEnumerable<string> Get()
		{
			return new string[] { "value1", "value2" };
		}

		// GET api/<controller>/5
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<controller>
		public async Task<HttpResponseMessage> Post()
		{
			// Check if the request contains multipart/form-data.
			if (!Request.Content.IsMimeMultipartContent())
			{
				throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
			}

			string root = HttpContext.Current.Server.MapPath("~/App_Data/uploads");
			var provider = new MultipartFormDataStreamProvider(root);

			try
			{
				// Read the form data.
				await Request.Content.ReadAsMultipartAsync(provider);

				// This illustrates how to get the file names.
				foreach (MultipartFileData file in provider.FileData)
				{
					System.Diagnostics.Debug.WriteLine(file.Headers.ContentDisposition.FileName);
					System.Diagnostics.Debug.WriteLine(file.Headers.ContentType);
					System.Diagnostics.Debug.WriteLine("Server file path: " + file.LocalFileName);
					TagLib.File tagFile;
					string extension = file.Headers.ContentDisposition.FileName.Substring(file.Headers.ContentDisposition.FileName.LastIndexOf(".") + 1);
					extension = extension.Replace("\"", "").ToLower();
					string type;
					if (extension.Equals("mp3"))
					{
						type = "audio/mpeg";
					}
					else if (extension.Equals("flac"))
					{
						type = "audio/flac";
					}
					else if (extension.Equals("m4a") || extension.Equals("mp4"))
					{
						type = "audio/mp4";
					}
					else if (extension.Equals("aac"))
					{
						type = "audio/aac";
					}
					else if (extension.Equals("ogg"))
					{
						type = "audio/ogg";
					}
					else
					{
						continue;
					}
					tagFile = TagLib.File.Create(file.LocalFileName, type, TagLib.ReadStyle.Average);
					System.Diagnostics.Debug.WriteLine(tagFile.Tag.Title + " by " + tagFile.Tag.Artists.First());
				}
				return Request.CreateResponse(HttpStatusCode.OK);
			}
			catch (System.Exception e)
			{
				return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
			}
		}

		// PUT api/<controller>/5
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/<controller>/5
		public void Delete(int id)
		{
		}
	}
}