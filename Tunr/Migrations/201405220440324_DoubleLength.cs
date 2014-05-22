namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoubleLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Songs", "Length", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Songs", "Length", c => c.Int(nullable: false));
        }
    }
}
