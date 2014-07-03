namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSongs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Songs", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Songs", new[] { "Owner_Id" });
            DropTable("dbo.Songs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Guid(nullable: false, identity: true),
                        SongFingerprint = c.String(),
                        SongMD5 = c.String(),
                        Title = c.String(),
                        Artist = c.String(),
                        Album = c.String(),
                        TrackNumber = c.Int(nullable: false),
                        DiscNumber = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Genre = c.String(),
                        Length = c.Double(nullable: false),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SongId);
            
            CreateIndex("dbo.Songs", "Owner_Id");
            AddForeignKey("dbo.Songs", "Owner_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
