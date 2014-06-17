using System;
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
					// Check to make sure our alpha token is valid.
					var token = db.AlphaTokens.Where(x => x.AlphaTokenId == new Guid(model.AlphaToken)).Where(x => x.User == null).Select(x => x).FirstOrDefault();
					if (token != null)
					{
						var user = new TunrUser() { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName };
						var result = await UserManager.CreateAsync(user, model.Password);
						if (result.Succeeded)
						{
							token.User = db.Users.Where(x => x.Id == user.Id).Select(x => x).FirstOrDefault();
							token.UsedTime = DateTimeOffset.Now;
							await db.SaveChangesAsync();

							// Send them a welcome mail!
							var myMessage = new SendGridMessage();
							myMessage.From = new System.Net.Mail.MailAddress("monolith@tunr.io", "Tunr.io");
							myMessage.AddTo(user.Email);
							myMessage.Subject = "Welcome to the Tunr Pre-Alpha!";
							myMessage.Text = "Hey " + user.DisplayName + "! Thanks for taking the time to test out Tunr for me."
								+ "\n\nThe first thing you'll need to do is upload some music. You can do this with the Tunr sync client!"
								+ "\nYou can find it at the following URL:"
								+ "\nhttp://tunr.io/downloads/TunrSync.zip"
								+ "\n\nSimply extract the archive, and run TunrSync.exe in the command prompt with the following arguments:"
								+ "\n TunrSync.exe your@emailaddress.com yourpassword C:\\Path\\To\\Your\\Music\\Folder"
								+ "\n\nAfter you've uploaded some tracks, head to http://play.tunr.io to give them a listen!"
								+ "\n\nHaving trouble? Reach out to me! hayden@outlook.com or any other means you know of."
								+ "\n\nPlease make me aware of any bugs you find! That's all I ask in return :)"
								+ "\n\nThanks again for testing!"
								+ "\n\nHayden";

							var credentials = new NetworkCredential("***REMOVED***", "***REMOVED***");
							var transportWeb = new Web(credentials);
							await transportWeb.DeliverAsync(myMessage);

							return Created(new Uri("/api/Users/" + user.Id, UriKind.Relative), user.toViewModel());
						}
						else
						{
							return BadRequest(result.Errors.First());
						}
					}
					else
					{
						return BadRequest("Invalid Alpha Token provided.");
					}
				}
			}
			return BadRequest(ModelState);
		}
    }
}
