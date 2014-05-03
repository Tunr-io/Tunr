using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace Tunr
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/js").Include(
				"~/Scripts/js/jquery-2.1.0.min.js"
				));

			bundles.Add(new ScriptBundle("~/bundles/ts").Include(
				"~/Scripts/ts/Animator.js",
				"~/Scripts/ts/Component.js",
				"~/Scripts/ts/components/Login.js",
				"~/Scripts/ts/components/LibraryPane.js",
				"~/Scripts/ts/components/PlaylistPane.js",
				"~/Scripts/ts/components/PlayingPane.js",
				"~/Scripts/ts/data/Library.js",
				"~/Scripts/ts/data/Playlist.js",
				"~/Scripts/ts/API.js",
				"~/Scripts/ts/Misc.js",
				"~/Scripts/ts/Tunr.js"
				));

			bundles.Add(new StyleBundle("~/bundles/css").Include(
				 "~/Content/css/Tunr.css", new CssRewriteUrlTransform()));

			// Set EnableOptimizations to false for debugging. For more information,
			// visit http://go.microsoft.com/fwlink/?LinkId=301862
			// BundleTable.EnableOptimizations = true;
		}
	}
}
