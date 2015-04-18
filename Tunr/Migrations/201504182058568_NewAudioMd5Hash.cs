namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewAudioMd5Hash : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.SongModels", "Md5Hash", "FileMd5Hash");
            AddColumn("dbo.SongModels", "AudioMd5Hash", c => c.String(maxLength: 36));
            CreateIndex("dbo.SongModels", "AudioMd5Hash");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.SongModels", "FileMd5Hash", "Md5Hash");
            DropIndex("dbo.SongModels", new[] { "AudioMd5Hash" });
            DropColumn("dbo.SongModels", "AudioMd5Hash");
        }
    }
}
