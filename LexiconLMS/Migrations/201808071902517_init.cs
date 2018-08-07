namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Documents", name: "Course_Id", newName: "CourseId");
            RenameColumn(table: "dbo.Documents", name: "Module_Id", newName: "ModuleId");
            RenameColumn(table: "dbo.Documents", name: "Type_Id", newName: "TypeId");
            RenameColumn(table: "dbo.Documents", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.Documents", name: "IX_Type_Id", newName: "IX_TypeId");
            RenameIndex(table: "dbo.Documents", name: "IX_User_Id", newName: "IX_UserId");
            RenameIndex(table: "dbo.Documents", name: "IX_Course_Id", newName: "IX_CourseId");
            RenameIndex(table: "dbo.Documents", name: "IX_Module_Id", newName: "IX_ModuleId");
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        ChatId = c.String(nullable: false, maxLength: 128),
                        ChatName = c.String(),
                        Date = c.DateTime(nullable: false),
                        CourseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChatId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ChatId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Chats", t => t.ChatId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.ChatId);
            
            CreateTable(
                "dbo.Attachments",
                c => new
                    {
                        MessageId = c.String(nullable: false, maxLength: 128),
                        FileName = c.String(maxLength: 255),
                        ContentType = c.String(maxLength: 100),
                        FileType = c.Int(nullable: false),
                        Content = c.Binary(),
                    })
                .PrimaryKey(t => t.MessageId)
                .ForeignKey("dbo.Messages", t => t.MessageId)
                .Index(t => t.MessageId);
            
            CreateTable(
                "dbo.ChatApplicationUsers",
                c => new
                    {
                        Chat_ChatId = c.String(nullable: false, maxLength: 128),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Chat_ChatId, t.ApplicationUser_Id })
                .ForeignKey("dbo.Chats", t => t.Chat_ChatId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Chat_ChatId)
                .Index(t => t.ApplicationUser_Id);
            
            AddColumn("dbo.Documents", "FullPath", c => c.String());
            AddColumn("dbo.Documents", "RelativePath", c => c.String());
            AddColumn("dbo.Documents", "ActivityId", c => c.Int());
            CreateIndex("dbo.Documents", "ActivityId");
            AddForeignKey("dbo.Documents", "ActivityId", "dbo.Activities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ChatApplicationUsers", "Chat_ChatId", "dbo.Chats");
            DropForeignKey("dbo.Messages", "ChatId", "dbo.Chats");
            DropForeignKey("dbo.Messages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Attachments", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Chats", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.Documents", "ActivityId", "dbo.Activities");
            DropIndex("dbo.ChatApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.ChatApplicationUsers", new[] { "Chat_ChatId" });
            DropIndex("dbo.Attachments", new[] { "MessageId" });
            DropIndex("dbo.Messages", new[] { "ChatId" });
            DropIndex("dbo.Messages", new[] { "UserId" });
            DropIndex("dbo.Chats", new[] { "CourseId" });
            DropIndex("dbo.Documents", new[] { "ActivityId" });
            DropColumn("dbo.Documents", "ActivityId");
            DropColumn("dbo.Documents", "RelativePath");
            DropColumn("dbo.Documents", "FullPath");
            DropTable("dbo.ChatApplicationUsers");
            DropTable("dbo.Attachments");
            DropTable("dbo.Messages");
            DropTable("dbo.Chats");
            RenameIndex(table: "dbo.Documents", name: "IX_ModuleId", newName: "IX_Module_Id");
            RenameIndex(table: "dbo.Documents", name: "IX_CourseId", newName: "IX_Course_Id");
            RenameIndex(table: "dbo.Documents", name: "IX_UserId", newName: "IX_User_Id");
            RenameIndex(table: "dbo.Documents", name: "IX_TypeId", newName: "IX_Type_Id");
            RenameColumn(table: "dbo.Documents", name: "UserId", newName: "User_Id");
            RenameColumn(table: "dbo.Documents", name: "TypeId", newName: "Type_Id");
            RenameColumn(table: "dbo.Documents", name: "ModuleId", newName: "Module_Id");
            RenameColumn(table: "dbo.Documents", name: "CourseId", newName: "Course_Id");
        }
    }
}
