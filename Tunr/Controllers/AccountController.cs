﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using Tunr.Models;
using SendGrid;
using System.Web.Http.Cors;
using System.Configuration;

namespace Tunr.Controllers
{
	[RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
		private ApplicationUserManager _userManager;

		public AccountController()
		{
		}

		public AccountController(ApplicationUserManager userManager)
		{
			UserManager = userManager;
		}

		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		[HttpPost]
		[Route("AddAlphaToken")]
		public async Task<IHttpActionResult> AddAlphaToken()
		{
			using (var db = new ApplicationDbContext())
			{
				var uid = User.Identity.GetUserId();
				var user = db.Users.Where(u => u.Id == uid).Select(u => u).FirstOrDefault();
				if (user.IsAdmin == false)
				{
					return BadRequest("You do not have permission to generate new alpha keys.");
				}
				var key = new AlphaToken() { AlphaTokenId = new Guid(), CreatedTime = DateTimeOffset.Now };
				db.AlphaTokens.Add(key);
				await db.SaveChangesAsync();
				return Ok(key);
			}
		}

		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		[Route("Register")]
		[EnableCors(origins: "*", headers: "*", methods: "*")]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if (ModelState.IsValid)
			{
				using (var db = new ApplicationDbContext())
				{	
					var user = new TunrUser() { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName, LibrarySize = 0 };
					var result = await UserManager.CreateAsync(user, model.Password);
					if (result.Succeeded)
					{
						var confirmationCode = UserManager.GenerateEmailConfirmationToken(user.Id);
						string uri = Url.Route("DefaultApi", new { controller = "Account/ConfirmEmail/", userId = user.Id.ToString(), code = confirmationCode });
						string absoluteUrl = new Uri(Request.RequestUri, uri).AbsoluteUri;

						// Send them a welcome mail!
						if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SendGridUsername"]))
						{
							var myMessage = new SendGridMessage();
							myMessage.From = new System.Net.Mail.MailAddress("support@tunr.io", "Tunr Support");
							myMessage.AddTo(user.Email);
							myMessage.Subject = "Tunr Activation";
							myMessage.Text = "Hey " + user.DisplayName + "! Welcome to Tunr."
								+ "\n\nBefore you start listening, we need you to activate your account."
								+ "\nJust click the URL below to activate and log in."
								+ "\n\n" + absoluteUrl
								+ "\n\nAfter you activate and log in, you can begin uploading your music and listening!"
								+ "\n\nIf you have any trouble, want to share your thoughts, or just want to talk, shoot us an email at support@tunr.io . We'd love to hear from you."
								+ "\n\n We <3 Music!"
								+ "\n\n The Tunr team";

							var credentials = new NetworkCredential(ConfigurationManager.AppSettings["SendGridUsername"], ConfigurationManager.AppSettings["SendGridPassword"]);
							var transportWeb = new Web(credentials);
							await transportWeb.DeliverAsync(myMessage);
						}

						return Created(new Uri("/api/Users/" + user.Id, UriKind.Relative), user.toViewModel());
					}
					else
					{
						return BadRequest(result.Errors.First());
					}
				}
			}
			return BadRequest(ModelState);
		}

		[HttpGet]
		[Route("ConfirmEmail")]
		public async Task<IHttpActionResult> ConfirmEmail([FromUri] EmailConfirmBindingModel emailConfirmModel)
		{
			if (emailConfirmModel.userId == null || emailConfirmModel.code == null)
			{
				return BadRequest("The user or code provided is invalid.");
			}
			var result = await UserManager.ConfirmEmailAsync(emailConfirmModel.userId, emailConfirmModel.code);
			if (result.Succeeded)
			{
				return Redirect(new Uri(Request.RequestUri, "/"));
			}
			return InternalServerError(new ApplicationException("Could not validate e-mail."));
		}
    }
}
