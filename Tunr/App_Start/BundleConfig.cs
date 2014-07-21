﻿using System;
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
			bundles.Add(new ScriptBundle("~/Content/bundles/js").Include(
				"~/Scripts/js/jquery-2.1.0.min.js",
				"~/Scripts/js/jquery.signalR-2.1.0.min.js"
				));

			var tsBundle = new ScriptBundle("~/Content/bundles/ts");
			tsBundle.Include(
				"~/Scripts/ts/Animator.js",
				"~/Scripts/ts/TiltEffect.js",
				"~/Scripts/ts/Component.js",
				"~/Scripts/ts/components/Login.js",
				"~/Scripts/ts/components/LibraryPane.js",
				"~/Scripts/ts/components/PlaylistPane.js",
				"~/Scripts/ts/components/PlayingPane.js",
				"~/Scripts/ts/data/Library.js",
				"~/Scripts/ts/data/Playlist.js",
				"~/Scripts/ts/API.js",
				"~/Scripts/ts/TunrHub.js",
				"~/Scripts/ts/Misc.js",
				"~/Scripts/ts/Tunr.js"
				);
#if DEBUG
			tsBundle.Include("~/Scripts/ts/Debug.js");
#endif
			bundles.Add(tsBundle);

			bundles.Add(new StyleBundle("~/Content/bundles/css").Include(
				 "~/Content/css/Tunr.css", new CssRewriteUrlTransform()));

			// Set EnableOptimizations to false for debugging. For more information,
			// visit http://go.microsoft.com/fwlink/?LinkId=301862
#if DEBUG
			BundleTable.EnableOptimizations = false;
#else
			BundleTable.EnableOptimizations = true;
#endif
		}
	}
}
