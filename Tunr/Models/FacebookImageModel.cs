using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class FacebookSearchResponse
	{
		public IEnumerable<FacebookSearchResult> data;
	}
	public class FacebookSearchResult
	{
		public string category;
		public string name;
		public string id;
	}
	public class FacebookImageResponse
	{
		public IEnumerable<FacebookImageResponseItem> data;
	}

	public class FacebookImageResponseItem
	{
		public string id;
		public IEnumerable<FacebookImageResponseImage> images;
		public FacebookImageResponseLikes likes;
	}

	public class FacebookImageResponseImage {
		public int height;
		public int width;
		public string source;
	}

	public class FacebookImageResponseLikes
	{
		public IEnumerable<FacebookImageResponseLike> data;
	}
	public class FacebookImageResponseLike
	{
		public string id;
		public string name;
	}
}