namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
	using Tunr.Models;
	using System.Linq;
	using Microsoft.WindowsAzure.Storage.Table;
	using System.Collections.Generic;
    
    public partial class LibrarySizeConcurrencyFix : DbMigration
    {
        public override void Up()
        {
			// Update everyone's library size to actually be correct ...
			using (var db = new ApplicationDbContext())
			{
				var azureStorageContext = new AzureStorageContext();
				var users = db.Users.ToList();
				foreach (var user in users)
				{
					TableQuery<Tunr.Models.Song> songQuery = new TableQuery<Tunr.Models.Song>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.Id.ToString()));
					IEnumerable<Tunr.Models.Song> songs = azureStorageContext.SongTable.ExecuteQuery(songQuery);
					var calculatedLibrarySize = songs.Sum(s => s.FileSize);
					user.LibrarySize = calculatedLibrarySize;
				}
				db.SaveChanges();
			}
        }
        
        public override void Down()
        {
        }
    }
}
