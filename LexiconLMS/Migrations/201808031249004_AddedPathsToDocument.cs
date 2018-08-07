namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPathsToDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "FullPath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "FullPath");
        }
    }
}
