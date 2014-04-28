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

namespace Tunr.Controllers
{
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


		// POST: /Account/Register
		[HttpPost]
		[AllowAnonymous]
		public async Task<IHttpActionResult> Register(RegisterBindingModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new TunrUser() { UserName = model.Email, Email = model.Email, DisplayName = model.DisplayName };
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return Created(new Uri("/api/Users/" + user.Id,UriKind.Relative), user.toViewModel());
				}
				else
				{
					return BadRequest(result.Errors.First());
				}
			}
			return BadRequest(ModelState);
		}
    }
}
