namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ump : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "FileName", c => c.String(nullable: false));
            AddColumn("dbo.Documents", "MimeType", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "MimeType");
            DropColumn("dbo.Documents", "FileName");
        }
    }
}
