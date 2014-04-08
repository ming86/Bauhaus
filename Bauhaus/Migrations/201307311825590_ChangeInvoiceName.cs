namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeInvoiceName : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        invoiceID = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        POD = c.DateTime(nullable: false),
                        netQtyCs = c.Int(nullable: false),
                        netQtySu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.invoiceID);
            
            AddColumn("dbo.Orders", "Invoice_invoiceID", c => c.Int());
            AddForeignKey("dbo.Orders", "Invoice_invoiceID", "dbo.Invoices", "invoiceID");
            CreateIndex("dbo.Orders", "Invoice_invoiceID");
            DropColumn("dbo.Orders", "Invoice_invoideID");
            DropColumn("dbo.Orders", "Invoice_date");
            DropColumn("dbo.Orders", "Invoice_POD");
            DropColumn("dbo.Orders", "Invoice_netQtyCs");
            DropColumn("dbo.Orders", "Invoice_netQtySu");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Invoice_netQtySu", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "Invoice_netQtyCs", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "Invoice_POD", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Invoice_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Invoice_invoideID", c => c.Int(nullable: false));
            DropIndex("dbo.Orders", new[] { "Invoice_invoiceID" });
            DropForeignKey("dbo.Orders", "Invoice_invoiceID", "dbo.Invoices");
            DropColumn("dbo.Orders", "Invoice_invoiceID");
            DropTable("dbo.Invoices");
        }
    }
}
