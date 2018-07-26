namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDocumentTypePrivileges : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentTypeCanCreate",
                c => new
                    {
                        DocumentTypeId = c.Int(nullable: false),
                        IdentityRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.DocumentTypeId, t.IdentityRoleId })
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.IdentityRoleId, cascadeDelete: true)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.IdentityRoleId);
            
            CreateTable(
                "dbo.DocumentTypeCanView",
                c => new
                    {
                        DocumentTypeId = c.Int(nullable: false),
                        IdentityRoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.DocumentTypeId, t.IdentityRoleId })
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.IdentityRoleId, cascadeDelete: true)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.IdentityRoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DocumentTypeCanView", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanView", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.DocumentTypeCanCreate", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanCreate", "DocumentTypeId", "dbo.DocumentTypes");
            DropIndex("dbo.DocumentTypeCanView", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanView", new[] { "DocumentTypeId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "DocumentTypeId" });
            DropTable("dbo.DocumentTypeCanView");
            DropTable("dbo.DocumentTypeCanCreate");
        }
    }
}
