namespace pathos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix001 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Chapters", "Project_ProjectID", "dbo.Projects");
            DropIndex("dbo.Chapters", new[] { "Project_ProjectID" });
            AddColumn("dbo.Chapters", "ProjectID", c => c.Int(nullable: false));
            AddForeignKey("dbo.Chapters", "ProjectID", "dbo.Projects", "ProjectID", cascadeDelete: true);
            CreateIndex("dbo.Chapters", "ProjectID");
            DropColumn("dbo.Chapters", "Project_ProjectID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Chapters", "Project_ProjectID", c => c.Int(nullable: false));
            DropIndex("dbo.Chapters", new[] { "ProjectID" });
            DropForeignKey("dbo.Chapters", "ProjectID", "dbo.Projects");
            DropColumn("dbo.Chapters", "ProjectID");
            CreateIndex("dbo.Chapters", "Project_ProjectID");
            AddForeignKey("dbo.Chapters", "Project_ProjectID", "dbo.Projects", "ProjectID", cascadeDelete: true);
        }
    }
}
