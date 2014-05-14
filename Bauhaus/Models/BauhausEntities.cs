using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Bauhaus.Models
{
    public class BauhausEntities : DbContext
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<POD> PODs { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<CarryFee> CarryFees { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<HistIndicator> HistIndicators { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<RDDF> RDDFs { get; set; }
        public DbSet<Input> Inputs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Filter> Filters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
                     
    }
}