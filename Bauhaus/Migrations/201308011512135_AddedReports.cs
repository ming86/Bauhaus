namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        ReportID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        processed = c.Boolean(nullable: false),
                        uploader = c.String(),
                        comment = c.String(),
                    })
                .PrimaryKey(t => t.ReportID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reports");
        }
    }
}
