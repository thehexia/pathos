namespace pathos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttributesToProjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chapters",
                c => new
                    {
                        ChapterID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 140),
                        Location = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 140),
                        Project_ProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChapterID)
                .ForeignKey("dbo.Projects", t => t.Project_ProjectID, cascadeDelete: true)
                .Index(t => t.Project_ProjectID);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 140),
                        Author = c.String(nullable: false),
                        Description = c.String(maxLength: 140),
                    })
                .PrimaryKey(t => t.ProjectID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Chapters", new[] { "Project_ProjectID" });
            DropForeignKey("dbo.Chapters", "Project_ProjectID", "dbo.Projects");
            DropTable("dbo.Projects");
            DropTable("dbo.Chapters");
        }
    }
}
