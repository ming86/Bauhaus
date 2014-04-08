namespace Bauhaus.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        SapID = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        type = c.String(),
                        SoldTo = c.Int(nullable: false),
                        ShipTo = c.Int(nullable: false),
                        DocDate = c.DateTime(nullable: false),
                        CusPODate = c.DateTime(nullable: false),
                        PriceDate = c.DateTime(nullable: false),
                        RDDF = c.DateTime(nullable: false),
                        RDDT = c.DateTime(nullable: false),
                        QtyCS = c.Int(nullable: false),
                        QtySU = c.Int(nullable: false),
                        NewRDD = c.DateTime(nullable: false),
                        Blking = c.Int(nullable: false),
                        Value = c.Single(nullable: false),
                        Invoice_invoideID = c.Int(nullable: false),
                        Invoice_date = c.DateTime(nullable: false),
                        Invoice_POD = c.DateTime(nullable: false),
                        Invoice_netQtyCs = c.Int(nullable: false),
                        Invoice_netQtySu = c.Int(nullable: false),
                        Delivery_deliveryID = c.Int(),
                        Shipment_shipmentID = c.Int(),
                    })
                .PrimaryKey(t => t.SapID)
                .ForeignKey("dbo.Deliveries", t => t.Delivery_deliveryID)
                .ForeignKey("dbo.Shipments", t => t.Shipment_shipmentID)
                .Index(t => t.Delivery_deliveryID)
                .Index(t => t.Shipment_shipmentID);
            
            CreateTable(
                "dbo.Deliveries",
                c => new
                    {
                        deliveryID = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        qtycs = c.Int(nullable: false),
                        qtysu = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.deliveryID);
            
            CreateTable(
                "dbo.Shipments",
                c => new
                    {
                        shipmentID = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        transportName = c.String(),
                        vehicleType = c.String(),
                        city = c.String(),
                        route = c.String(),
                    })
                .PrimaryKey(t => t.shipmentID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Orders", new[] { "Shipment_shipmentID" });
            DropIndex("dbo.Orders", new[] { "Delivery_deliveryID" });
            DropForeignKey("dbo.Orders", "Shipment_shipmentID", "dbo.Shipments");
            DropForeignKey("dbo.Orders", "Delivery_deliveryID", "dbo.Deliveries");
            DropTable("dbo.Shipments");
            DropTable("dbo.Deliveries");
            DropTable("dbo.Orders");
        }
    }
}
