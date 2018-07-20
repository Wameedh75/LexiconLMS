namespace LexiconLMS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NavpropertyinMoudel2 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Modules", name: "CourseId", newName: "Course_Id");
            RenameIndex(table: "dbo.Modules", name: "IX_CourseId", newName: "IX_Course_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Modules", name: "IX_Course_Id", newName: "IX_CourseId");
            RenameColumn(table: "dbo.Modules", name: "Course_Id", newName: "CourseId");
        }
    }
}
