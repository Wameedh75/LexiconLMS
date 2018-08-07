namespace LexiconLMS.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Activities : DbMigration
    {
        public override void Up() {
            CreateTable(
                    "dbo.Activities",
                    c => new {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Starttime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId, cascadeDelete: true)
                .Index(t => t.ModuleId);

        }

        public override void Down() {
            DropForeignKey("dbo.Activities", "ModuleId", "dbo.Modules");
            DropIndex("dbo.Activities", new[] { "ModuleId" });
            DropTable("dbo.Activities");
        }
    }
}
