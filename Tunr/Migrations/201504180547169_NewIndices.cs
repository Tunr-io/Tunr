namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewIndices : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SongModels", "Md5Hash", c => c.String(maxLength: 36));
            CreateIndex("dbo.SongModels", "Md5Hash");
        }
        
        public override void Down()
        {
            DropIndex("dbo.SongModels", new[] { "Md5Hash" });
            AlterColumn("dbo.SongModels", "Md5Hash", c => c.String());
        }
    }
}
