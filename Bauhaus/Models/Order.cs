using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Globalization;

namespace Bauhaus.Models
{
    public class Order
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long SapID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name="Doc. Date")]
        public DateTime DocDate { get; set; }
        public string Type { get; set; }
        public string PayTerm { get; set; }
        [Required]
        public virtual Customer Customer { get; set; }
        [Display(Name="Cust. PO")]
        public string CustomerPO { get; set; }
        public virtual RDDF RDDF { get; set; }
        public int Plant { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual Delivery Delivery { get; set; }
        public virtual Status Status { get; set; }
        public virtual Shipment Shipment { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set;}
        public virtual POD POD { get; set; }


        /// <summary>
        /// Calculates KPIs on Orders with POD
        /// </summary>
        /// <returns>Return 0 if Normal Flow. 1 if Error encountered.</returns>
        public int CalculateIndicators()
        {
            // Order Needs POD for indicators to be calculated
            if (this.POD == null)
                return 1;
            else
            {
                if (this.POD.CSOT == null)
                {
                    this.POD.CSOT = new Indicator();
                    if (this.POD.Date <= this.RDDF.Original)   // Inside Time Window.
                    {
                        this.POD.CSOT.Value = true; // HIT
                        this.POD.CSOT.Confirmed = true;
                    }
                    else
                    {
                        this.POD.CSOT.Value = false; // MISS
                    }
                }

                if (this.POD.OT == null)
                {
                    this.POD.OT = new Indicator();
                    if (this.POD.Date <= this.RDDF.DSSDate || // Inside Time Window
                        this.Products.Sum(x=>x.DSSQty.CS) < 200)
                    {
                        this.POD.OT.Value = true;
                        this.POD.OT.Confirmed = true;
                    }
                    else
                    {
                        this.POD.OT.Value = false;
                    }

                }

                return 0;
            }
        }

        /// <summary>
        /// Add product to Order if product is Missing.
        /// </summary>
        /// <param name="Material">Material Code to add</param>
        /// <param name="Description">Material Description</param>
        /// <param name="Brand">Product Brand</param>
        /// <param name="Category">Product Category</param>
        /// <param name="CS">CS</param>
        /// <param name="SU">SU</param>
        /// <param name="DSSCS">DSS CS</param>
        /// <param name="DSSSU">DSS SU</param>
        /// <param name="Weight">Weight</param>
        /// <param name="Volume">Volume</param>
        /// <returns>0 if </returns>
        public int CheckProduct(String Material, String Description, String Brand, String Category, String CS, String SU, String DSSCS, String DSSSU, String Weight, String Volume)
        {
            // Initialization
            CultureInfo firstCulture = new CultureInfo("en-US");
            CultureInfo culture = new CultureInfo("es-VE");
            if (this.Products == null)
                this.Products = new List<Product>();
            long auxLong;
            Double auxDouble;
            // Parse SKU
            if (!long.TryParse(Material, out auxLong))
            {
                return 1;
            }
            // Find Product
            Product product = this.Products.Where(x => x.SKU == auxLong).FirstOrDefault();

            // New Product
            if (product == null)
            {
                product = new Product();
                product.SKU = auxLong;
                product.Description = Description.Trim();
                product.Category = Category.Trim();
                product.Brand = Brand.Trim();
                product.Qty = new Quantity();
                product.DSSQty = new Quantity();

                // Parse CS
                if (!double.TryParse(CS, NumberStyles.Number, firstCulture, out auxDouble))
                    if (!double.TryParse(CS, NumberStyles.Number, culture, out auxDouble))
                    {
                        return 1;
                    }

                product.Qty.CS = auxDouble;

                // Parse SU
                if (!double.TryParse(SU, NumberStyles.Number, firstCulture, out auxDouble))
                    if (!double.TryParse(SU, NumberStyles.Number, culture, out auxDouble))
                    {
                        return 1;
                    }

                product.Qty.SU = auxDouble;

                //Parse  Weight
                if (!double.TryParse(Weight, NumberStyles.Number, firstCulture, out auxDouble))
                    if (!double.TryParse(SU, NumberStyles.Number, culture, out auxDouble))
                    {
                        return 1;
                    }

                product.Qty.NetWeight = auxDouble;

                // Parse Volume
                if (!double.TryParse(Volume, NumberStyles.Number, firstCulture, out auxDouble))
                    if (!double.TryParse(Volume, NumberStyles.Number, culture, out auxDouble))
                    {
                        return 1;
                    }

                product.Qty.Volume = auxDouble;

                if (!String.IsNullOrWhiteSpace(DSSCS))
                {
                    // Parse Dss CS
                    if (!double.TryParse(DSSCS, NumberStyles.Number, firstCulture, out auxDouble))
                        if (!double.TryParse(DSSCS, NumberStyles.Number, culture, out auxDouble))
                        {
                            return 1;
                        }

                    product.DSSQty.CS = auxDouble;

                    // Parse Dss SU
                    if (!double.TryParse(DSSSU, NumberStyles.Number, firstCulture, out auxDouble))
                        if (!double.TryParse(DSSSU, NumberStyles.Number, culture, out auxDouble))
                        {
                            return 1;
                        }
                    product.DSSQty.SU = auxDouble;

                }
                this.Products.Add(product);

                return 0;

            }
            else
            // Product Exist.
            {
                // Initialize Quantities
                if (product.Qty == null)
                    product.Qty = new Quantity();

                if (product.DSSQty == null)
                    product.DSSQty = new Quantity();

                if (!String.IsNullOrWhiteSpace(DSSCS))
                {
                    // Parse Dss Cs
                    if (!double.TryParse(DSSCS, NumberStyles.Number, firstCulture, out auxDouble))
                        if (!double.TryParse(DSSCS, NumberStyles.Number, culture, out auxDouble))
                        {
                            return 1;
                        }
                    product.DSSQty.CS = auxDouble;

                    // Parse Dss Su
                    if (!double.TryParse(DSSSU, NumberStyles.Number, firstCulture, out auxDouble))
                        if (!double.TryParse(DSSSU, NumberStyles.Number, culture, out auxDouble))
                        {
                            return 1;
                        }
                    product.DSSQty.SU = auxDouble;
                }
                return 0;
            }
        }
    }
}