namespace Tunr.Migrations
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Tunr.Models;

    public partial class SongsToSql : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AlphaTokens", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AlphaTokens", new[] { "User_Id" });
            CreateTable(
                "dbo.SongModels",
                c => new
                    {
                        SongId = c.Guid(nullable: false),
                        Fingerprint = c.String(),
                        Md5Hash = c.String(),
                        FileName = c.String(),
                        FileType = c.String(),
                        FileSize = c.Long(nullable: false),
                        AudioChannels = c.Int(nullable: false),
                        AudioBitrate = c.Int(nullable: false),
                        AudioSampleRate = c.Int(nullable: false),
                        Duration = c.Double(nullable: false),
                        TagTitle = c.String(),
                        TagAlbum = c.String(),
                        TagPerformersJson = c.String(),
                        TagAlbumArtistsJson = c.String(),
                        TagComposersJson = c.String(),
                        TagGenresJson = c.String(),
                        TagYear = c.Int(nullable: false),
                        TagTrack = c.Int(nullable: false),
                        TagTrackCount = c.Int(nullable: false),
                        TagDisc = c.Int(nullable: false),
                        TagDiscCount = c.Int(nullable: false),
                        TagComment = c.String(),
                        TagLyrics = c.String(),
                        TagConductor = c.String(),
                        TagBeatsPerMinute = c.Int(nullable: false),
                        TagGrouping = c.String(),
                        TagCopyright = c.String(),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.SongId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            DropTable("dbo.AlphaTokens");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AlphaTokens",
                c => new
                    {
                        AlphaTokenId = c.Guid(nullable: false, identity: true),
                        CreatedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UsedTime = c.DateTimeOffset(precision: 7),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.AlphaTokenId);
            
            DropForeignKey("dbo.SongModels", "Owner_Id", "dbo.AspNetUsers");
            DropIndex("dbo.SongModels", new[] { "Owner_Id" });
            DropTable("dbo.SongModels");
            CreateIndex("dbo.AlphaTokens", "User_Id");
            AddForeignKey("dbo.AlphaTokens", "User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
