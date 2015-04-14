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

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<TunrUser>()
				.Property(s => s.LibrarySize)
				.IsConcurrencyToken();

            modelBuilder.Entity<ChangeSetModel>()
                .Property(c => c.IsFresh)
                .IsConcurrencyToken();
		} 

		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}

		public DbSet<SongModel> Songs { get; set; }
		public DbSet<PlaylistModel> Playlists { get; set; }
        public DbSet<ChangeSetModel> ChangeSets { get; set; }
        public DbSet<ChangeModel> Changes { get; set; }
	}
}