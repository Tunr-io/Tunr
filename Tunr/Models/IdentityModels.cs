using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tunr.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class TunrUser : IdentityUser
	{
		public string DisplayName { get; set; }
		public bool IsAdmin { get; set; }
		public long LibrarySize { get; set; }
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<TunrUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}

		public TunrUserViewModel toViewModel()
		{
			return new TunrUserViewModel()
			{
				DisplayName = this.DisplayName
			};
		}
	}

	public class TunrUserViewModel
	{
		public string DisplayName { get; set; }
	}

	public class EmailConfirmBindingModel
	{
		public string userId { get; set; }
		public string code { get; set; }
	}
}