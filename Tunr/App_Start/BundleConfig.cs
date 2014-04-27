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
				
				));

			bundles.Add(new ScriptBundle("~/bundles/ts").Include(
				"~/Scripts/ts/Animator.js",
				"~/Scripts/ts/Component.js",
				"~/Scripts/ts/components/Login.js",
				"~/Scripts/ts/components/LibraryPane.js",
				"~/Scripts/ts/components/PlaylistPane.js",
				"~/Scripts/ts/Tunr.js"
				));

			bundles.Add(new StyleBundle("~/bundles/css").Include(
				 "~/Content/css/Tunr.css"));

			// Set EnableOptimizations to false for debugging. For more information,
			// visit http://go.microsoft.com/fwlink/?LinkId=301862
			// BundleTable.EnableOptimizations = true;
		}
	}
}
