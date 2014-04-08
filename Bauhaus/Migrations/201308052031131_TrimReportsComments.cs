namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrimReportsComments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reports", "path", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reports", "path");
        }
    }
}
