using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;
using Bauhaus.Helpers;
using System.IO;
using OfficeOpenXml;
using System.Drawing;
using WebMatrix.WebData;
using Newtonsoft.Json;

namespace Bauhaus.Controllers
{
    public class CustomerController : Controller
    {
        private BauhausEntities db = new BauhausEntities();

        //
        // GET: /Customer/

        public ActionResult Index(bool filter = false)
        {
            ViewBag.Title = "Customers";
            if(filter)
            {
                UserProfile user = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                if(User.IsInRole("CSR"))
                    return View(db.Customers.Where(x => x.MainCSROM.Name == user.FullName).OrderBy(x => x.Name));
                if(User.IsInRole("CBD"))
                    return View(db.Customers.Where(x => x.CBDRep.Name == user.FullName).OrderBy(x => x.Name));
                if (User.IsInRole("GU"))
                    return View(db.Customers.Where(x => x.GU.Name == user.FullName).OrderBy(x => x.Name));
            }
            return View(db.Customers.ToList().OrderBy(x => x.Name));
        }

        
        /// <summary>
        /// Returns a view with customers details.
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public ActionResult Details(long id, string returnUrl)
        {
            ViewBag.Title = "Customer Details";
            Customer customer = db.Customers.Find(id);
            return View(customer);
        }

        
        /// <summary>
        /// Allows disposing of current Context.
        /// </summary>
        /// <param name="disposing">Wether or not base dispose should be used.</param>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


        /// <summary>
        /// Return a string containing a rendered partial view.
        /// </summary>
        /// <param name="viewName">View to Render</param>
        /// <param name="model">Model that needs to be passed to the view</param>
        /// <returns>Rendered View</returns>
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }


        [HttpPost]
        public JsonResult GetCustomer(int id)
        {
            Customer cust = db.Customers.Find(id);
            if (cust == null)
                return Json(new { Status = 0, Message = "Not found" });
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("_CustomerInfo", cust) });
        }

        /// <summary>
        /// Method that Download to a Excel File all CLient's orders.
        /// </summary>
        public void DownloadCustomerOrders(long id)
        {
            Customer cust = db.Customers.Find(id);
            var orders = from x in cust.Orders
                         select new
                         {
                             x.SapID,
                             DocDate = x.DocDate.ToString("dd/MM/yy"),
                             x.CustomerPO,
                             OrderCS = x.Products.Sum(prod=>prod.Qty.CS),
                             Delivery = (x.Delivery == null)? "Sin Asignar":x.Delivery.ID.ToString(),
                             DeliveryCS = (x.Delivery == null)? "Sin Asignar":x.Products.Sum(prod=>prod.DSSQty.CS).ToString(),
                             Shipment = (x.Shipment == null)? "Sin Asignar":x.Shipment.ID.ToString(),
                             Status = x.Status.StageDescription() + " " + x.Status.ReasonDescription(),
                             Products = JsonConvert.SerializeObject(x.Products.Select(y => new{y.Qty.CS, y.Description}).ToList()),
                            
                         };


            using (ExcelPackage pck = new ExcelPackage())
            {
                // Create WorkSheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Orders");

                // Load Info
                ws.Cells["A1"].LoadFromCollection(orders.ToList(), true);

                // Format Headers
                using (ExcelRange rng = ws.Cells["A1:H1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }

                // Write Back Response to Client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename= Status_"+cust.Name+".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }

        [HttpPost]
        public ContentResult UpdateObservation(long ID, String newObservation)
        {
            int type = 0;
            string result = "Observation Updated.";
            Customer cust = db.Customers.Find(ID);
            if (cust != null)
            {
                cust.Observation = newObservation;
                db.SaveChanges();
            }
            else
            {
                type = 1;
                result = "Observation Update Failed.";
            }


            return new ContentResult { Content = type + "|" + result };
        }

    }
}