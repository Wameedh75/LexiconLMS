namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class start : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Modules", "CourseId", "dbo.Courses");
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Deadline = c.DateTime(),
                        Course_Id = c.Int(),
                        Module_Id = c.Int(),
                        Type_Id = c.Int(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.Course_Id)
                .ForeignKey("dbo.Modules", t => t.Module_Id)
                .ForeignKey("dbo.DocumentTypes", t => t.Type_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Course_Id)
                .Index(t => t.Module_Id)
                .Index(t => t.Type_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.DocumentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            
            AddColumn("dbo.Modules", "Course_Id", c => c.Int());
            AddColumn("dbo.Modules", "Course_Id1", c => c.Int());
            CreateIndex("dbo.Modules", "Course_Id");
            CreateIndex("dbo.Modules", "Course_Id1");
            AddForeignKey("dbo.Modules", "Course_Id1", "dbo.Courses", "Id");
            AddForeignKey("dbo.Modules", "Course_Id", "dbo.Courses", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Modules", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.Documents", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "Type_Id", "dbo.DocumentTypes");
            DropForeignKey("dbo.DocumentTypeCanView", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanView", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.DocumentTypeCanCreate", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanCreate", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.Documents", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.Documents", "Course_Id", "dbo.Courses");
            DropForeignKey("dbo.Modules", "Course_Id1", "dbo.Courses");
            DropIndex("dbo.DocumentTypeCanView", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanView", new[] { "DocumentTypeId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "DocumentTypeId" });
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropIndex("dbo.Documents", new[] { "Type_Id" });
            DropIndex("dbo.Documents", new[] { "Module_Id" });
            DropIndex("dbo.Documents", new[] { "Course_Id" });
            DropIndex("dbo.Modules", new[] { "Course_Id1" });
            DropIndex("dbo.Modules", new[] { "Course_Id" });
            DropColumn("dbo.Modules", "Course_Id1");
            DropColumn("dbo.Modules", "Course_Id");
            DropTable("dbo.DocumentTypeCanView");
            DropTable("dbo.DocumentTypeCanCreate");
            DropTable("dbo.DocumentTypes");
            DropTable("dbo.Documents");
            AddForeignKey("dbo.Modules", "CourseId", "dbo.Courses", "Id", cascadeDelete: true);
        }
    }
}
