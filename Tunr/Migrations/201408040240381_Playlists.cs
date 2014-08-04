namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Playlists : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlaylistModels",
                c => new
                    {
                        PlaylistId = c.Guid(nullable: false),
                        Title = c.String(),
                        ModifiedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.PlaylistId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.PlaylistItemModels",
                c => new
                    {
                        PlaylistItemId = c.Guid(nullable: false),
                        SongId = c.Guid(nullable: false),
                        Order = c.Int(nullable: false),
                        ModifiedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        PlaylistModel_PlaylistId = c.Guid(),
                    })
                .PrimaryKey(t => t.PlaylistItemId)
                .ForeignKey("dbo.PlaylistModels", t => t.PlaylistModel_PlaylistId)
                .Index(t => t.PlaylistModel_PlaylistId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlaylistModels", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PlaylistItemModels", "PlaylistModel_PlaylistId", "dbo.PlaylistModels");
            DropIndex("dbo.PlaylistItemModels", new[] { "PlaylistModel_PlaylistId" });
            DropIndex("dbo.PlaylistModels", new[] { "User_Id" });
            DropTable("dbo.PlaylistItemModels");
            DropTable("dbo.PlaylistModels");
        }
    }
}
