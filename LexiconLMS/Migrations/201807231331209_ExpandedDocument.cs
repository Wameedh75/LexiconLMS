namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandedDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "Description", c => c.String());
            AddColumn("dbo.Documents", "Deadline", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "Deadline");
            DropColumn("dbo.Documents", "Description");
        }
    }
}
