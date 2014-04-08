namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CustomerChanges : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "ContactTelephone", c => c.String());
            AddColumn("dbo.Customers", "MainTelephone", c => c.String());
            AddColumn("dbo.Customers", "Email", c => c.String());
            AlterColumn("dbo.Customers", "PayTerm", c => c.String());
            DropColumn("dbo.Customers", "Telephone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customers", "Telephone", c => c.String());
            AlterColumn("dbo.Customers", "PayTerm", c => c.Time(nullable: false));
            DropColumn("dbo.Customers", "Email");
            DropColumn("dbo.Customers", "MainTelephone");
            DropColumn("dbo.Customers", "ContactTelephone");
        }
    }
}
