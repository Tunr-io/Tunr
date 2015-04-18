namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoubleBitrate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SongModels", "AudioBitrate", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SongModels", "AudioBitrate", c => c.Int(nullable: false));
        }
    }
}
