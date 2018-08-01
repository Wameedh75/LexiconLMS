namespace LexiconLMS.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Documents : DbMigration
    {
        public override void Up() {
            CreateTable(
                "dbo.Documents",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Description = c.String(),
                    Deadline = c.DateTime(),
                    CourseId = c.Int(),
                    ModuleId = c.Int(),
                    TypeId = c.Int(nullable: false),
                    UserId = c.String(nullable: false, maxLength: 128),
                    FileName = c.String(nullable: false),
                    MimeType = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.DocumentTypes", t => t.TypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.CourseId)
                .Index(t => t.ModuleId)
                .Index(t => t.TypeId)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.DocumentTypes",
                c => new {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                })
                .PrimaryKey(t => t.Id);


            CreateTable(
                "dbo.DocumentTypeCanCreate",
                c => new {
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
                c => new {
                    DocumentTypeId = c.Int(nullable: false),
                    IdentityRoleId = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => new { t.DocumentTypeId, t.IdentityRoleId })
                .ForeignKey("dbo.DocumentTypes", t => t.DocumentTypeId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.IdentityRoleId, cascadeDelete: true)
                .Index(t => t.DocumentTypeId)
                .Index(t => t.IdentityRoleId);

        }

        public override void Down() {
            DropForeignKey("dbo.Documents", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "TypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.DocumentTypeCanView", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanView", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.DocumentTypeCanCreate", "IdentityRoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.DocumentTypeCanCreate", "DocumentTypeId", "dbo.DocumentTypes");
            DropForeignKey("dbo.Documents", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.Documents", "CourseId", "dbo.Courses");
            DropIndex("dbo.DocumentTypeCanView", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanView", new[] { "DocumentTypeId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "IdentityRoleId" });
            DropIndex("dbo.DocumentTypeCanCreate", new[] { "DocumentTypeId" });
            DropIndex("dbo.Documents", new[] { "UserId" });
            DropIndex("dbo.Documents", new[] { "TypeId" });
            DropIndex("dbo.Documents", new[] { "ModuleId" });
            DropIndex("dbo.Documents", new[] { "CourseId" });
            DropTable("dbo.DocumentTypeCanView");
            DropTable("dbo.DocumentTypeCanCreate");
            DropTable("dbo.DocumentTypes");
            DropTable("dbo.Documents");
        }
    }
}
