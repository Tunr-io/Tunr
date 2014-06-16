using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Tunr.Models
{
	public class ApplicationDbContext : IdentityDbContext<TunrUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", throwIfV1Schema: false)
		{
		}

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public DbSet<Song> Songs { get; set; }
		public DbSet<AlphaToken> AlphaTokens { get; set; }
	}
}