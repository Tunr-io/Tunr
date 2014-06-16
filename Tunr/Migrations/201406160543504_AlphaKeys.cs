namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlphaKeys : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.AlphaTokenId)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AlphaTokens", "User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AlphaTokens", new[] { "User_Id" });
            DropTable("dbo.AlphaTokens");
        }
    }
}
