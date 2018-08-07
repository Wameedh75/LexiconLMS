namespace LexiconLMS.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Courses : DbMigration
    {
        public override void Up() {
            CreateTable(
                    "dbo.Courses",
                    c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.AspNetUsers", "CourseId", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "CourseId");
            AddForeignKey("dbo.AspNetUsers", "CourseId", "Courses", "Id");

        }

        public override void Down() {
            DropTable("dbo.Courses");
            DropForeignKey("dbo.AspNetUsers", "CourseId", "Courses");
            DropIndex("dbo.AspNetUsers", "CourseId");
        }
    }
}
