namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DocumentChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "ActivityId", c => c.Int());
            CreateIndex("dbo.Documents", "ActivityId");
            AddForeignKey("dbo.Documents", "ActivityId", "dbo.Activities", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Documents", "ActivityId", "dbo.Activities");
            DropIndex("dbo.Documents", new[] { "ActivityId" });
            DropColumn("dbo.Documents", "ActivityId");
        }
    }
}
