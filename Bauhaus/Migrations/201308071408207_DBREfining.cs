namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBREfining : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "Status", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "QtyCS", c => c.Single(nullable: false));
            AlterColumn("dbo.Orders", "QtySU", c => c.Single(nullable: false));
            DropColumn("dbo.Orders", "PriceDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "PriceDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "QtySU", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "QtyCS", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "Status", c => c.String());
        }
    }
}
