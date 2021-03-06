﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Tunr.Models;
using Tunr.Providers;

namespace Tunr
{
	public partial class Startup
	{
		// Enable the application to use OAuthAuthorization. You can then secure your Web APIs
		static Startup()
		{
			PublicClientId = "tunr";

			OAuthOptions = new OAuthAuthorizationServerOptions
			{
				TokenEndpointPath = new PathString("/Token"),
				//AuthorizeEndpointPath = new PathString("/Account/Authorize"), 
				Provider = new ApplicationOAuthProvider(PublicClientId),
				AccessTokenExpireTimeSpan = TimeSpan.FromDays(90),
				AllowInsecureHttp = false
			};
		}

		public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

		public static string PublicClientId { get; private set; }

		// For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
		public void ConfigureAuth(IAppBuilder app)
		{
			// Configure the db context and user manager to use a single instance per request
			app.CreatePerOwinContext(ApplicationDbContext.Create);
			app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

			// Enable the application to use a cookie to store information for the signed in user
			//app.UseCookieAuthentication(new CookieAuthenticationOptions
			//{
			//	AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
			//	Provider = new CookieAuthenticationProvider
			//	{
			//		OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, TunrUser>(
			//			validateInterval: TimeSpan.FromDays(90),
			//			regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
			//	}
			//});
			app.UseCookieAuthentication(new CookieAuthenticationOptions());

			// Use a cookie to temporarily store information about a user logging in with a third party login provider
			app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
			
			// Enable the application to use bearer tokens to authenticate users
			app.UseOAuthBearerTokens(OAuthOptions);

			// Uncomment the following lines to enable logging in with third party login providers
			//app.UseMicrosoftAccountAuthentication(
			//    clientId: "",
			//    clientSecret: "");

			//app.UseTwitterAuthentication(
			//    consumerKey: "",
			//    consumerSecret: "");

			//app.UseFacebookAuthentication(
			//    appId: "",
			//    appSecret: "");

			//app.UseGoogleAuthentication();
		}
	}
}
