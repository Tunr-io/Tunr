namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Song : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongId = c.Guid(nullable: false, identity: true),
                        SongFingerprint = c.String(),
                        Title = c.String(),
                        Artist = c.String(),
                        Album = c.String(),
                        TrackNumber = c.Int(nullable: false),
                        DiscNumber = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        Genre = c.String(),
                        Length = c.Int(nullable: false),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SongId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Songs", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Songs", new[] { "Owner_Id" });
            DropTable("dbo.Songs");
        }
    }
}
