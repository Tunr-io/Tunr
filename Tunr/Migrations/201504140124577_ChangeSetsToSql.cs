namespace Tunr.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeSetsToSql : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChangeModels",
                c => new
                    {
                        ChangeId = c.Guid(nullable: false),
                        SongId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        ChangeSetModel_ChangeSetId = c.Guid(),
                    })
                .PrimaryKey(t => t.ChangeId)
                .ForeignKey("dbo.ChangeSetModels", t => t.ChangeSetModel_ChangeSetId)
                .Index(t => t.ChangeSetModel_ChangeSetId);
            
            CreateTable(
                "dbo.ChangeSetModels",
                c => new
                    {
                        ChangeSetId = c.Guid(nullable: false),
                        LastModifiedTime = c.DateTimeOffset(nullable: false, precision: 7),
                        IsFresh = c.Boolean(nullable: false),
                        Owner_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ChangeSetId)
                .ForeignKey("dbo.AspNetUsers", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChangeSetModels", "Owner_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChangeModels", "ChangeSetModel_ChangeSetId", "dbo.ChangeSetModels");
            DropIndex("dbo.ChangeSetModels", new[] { "Owner_Id" });
            DropIndex("dbo.ChangeModels", new[] { "ChangeSetModel_ChangeSetId" });
            DropTable("dbo.ChangeSetModels");
            DropTable("dbo.ChangeModels");
        }
    }
}
