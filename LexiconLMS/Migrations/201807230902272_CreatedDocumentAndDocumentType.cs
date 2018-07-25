namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedDocumentAndDocumentType : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Documents", "Type_Id", "dbo.DocumentTypes");
            DropForeignKey("dbo.Documents", "Module_Id", "dbo.Modules");
            DropForeignKey("dbo.Documents", "Course_Id", "dbo.Courses");
            DropIndex("dbo.Documents", new[] { "User_Id" });
            DropIndex("dbo.Documents", new[] { "Type_Id" });
            DropIndex("dbo.Documents", new[] { "Module_Id" });
            DropIndex("dbo.Documents", new[] { "Course_Id" });
            DropTable("dbo.DocumentTypes");
            DropTable("dbo.Documents");
        }
    }
}
