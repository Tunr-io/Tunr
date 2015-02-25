namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLibrarySize : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "LibrarySize", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LibrarySize");
        }
    }
}
