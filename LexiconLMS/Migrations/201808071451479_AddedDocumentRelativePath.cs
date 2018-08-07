namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDocumentRelativePath : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "RelativePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "RelativePath");
        }
    }
}
