namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SongMD5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Songs", "SongMD5", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Songs", "SongMD5");
        }
    }
}
