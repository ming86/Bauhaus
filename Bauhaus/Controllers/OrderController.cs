﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;
using System.IO;
using WebMatrix.WebData;
using OfficeOpenXml;
using System.Drawing;
using System.Globalization;
using Bauhaus.Helpers;
using System.Web.Script.Serialization;
using Newtonsoft.Json;


namespace Bauhaus.Controllers
{
    public class OrderController : Controller
    {
        private BauhausEntities db = new BauhausEntities();

        //
        // GET: /Order/
        [Authorize]
        public ActionResult Index(Boolean filter = false)
        {
            return View(SelectOrders("all",filter));
        }

        //
        // GET: /Order/
        [Authorize]
        public ActionResult Index2(Boolean filter = false)
        {
            return View(SelectOrders("all", filter));
        }

        //
        // GET: /Order/

        [Authorize]
        [HttpPost]
        public JsonResult GetOrder(int id)
        {
            System.Diagnostics.Debug.WriteLine("Get OrderS");
            Order order = (from x in db.Orders
                           where x.SapID == id
                           select x).FirstOrDefault();
            if (order == null)
                return Json(new { Status = 0, Message = "Not found" });
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("_OrderInfo", order) });
        }

        [Authorize]
        public ActionResult Details(long id)
        {
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                TempData["Type"] = "info";
                TempData["Message"] = "Not Found";
                return View("Index", "Home");
            }
            return View(order);


        }
        
        [Authorize]
        public ActionResult GeneralSearch(String id)
        {
            long idn;
            // Its a Number.
            if (long.TryParse(id, out idn))
            {
                //Its a Zone
                if (id.Length == 3)
                {
                    var orders = db.Orders.Where(order => order.Customer.SaleZone == idn)
                        .OrderBy(order => order.Customer.Name)
                        .ToList()
                        .AsQueryable();

                    if (orders != null && orders.Any())
                    {
                        ViewBag.Title = "Orders from Zone " + id;
                        return View("Index", orders);
                    }
                }
                    // Its a Unit
                else if(id.Length == 2)
                {
                    var orders = db.Orders.Where(order => order.Customer.Unit == idn)
                        .OrderBy(order => order.Customer.Name)
                        .ToList()
                        .AsQueryable();
                }
                else
                {
                    //Check Orders
                    Order ord = db.Orders.Find(idn);
                    if (ord != null)
                        return RedirectToAction("Details", new { id = idn });
                    else
                    {
                        //Check Customers
                        Customer cust = db.Customers.Find(idn);
                        if (cust != null)
                            return RedirectToAction("Details","Customer",new { id = idn});
                        else
                        {
                            //Check Shipments
                            Shipment shpP = new Shipment();
                            shpP = db.Shipments.Find(idn);
                            if (shpP != null)
                            {
                                return RedirectToAction("Details", "Shipment", new { id = idn });
                            }
                            //Check Deliveries
                            else
                            {
                                ord = (from x in db.Orders
                                       where x.Delivery.ID == idn
                                       select x).FirstOrDefault();
                                if (ord != null)
                                {
                                    return RedirectToAction("Details", new { id = idn });
                                }
                                else
                                {
                                    //Check Invoices
                                    var linqOrd = (from x in db.Orders
                                                   from y in x.Invoices
                                                   where y.ID == idn
                                                   select x).FirstOrDefault();
                                    if (linqOrd != null)
                                    {
                                        return RedirectToAction("Details", linqOrd);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // Contains Letters or wrong Format
            else
            {
                //Check Routes
                var orders =  db.Orders.Where(order => order.Customer.Route.Name == id).OrderBy(order=>order.Customer.Name).ToList().AsQueryable();
                if (orders != null && orders.Any())
                {
                    ViewBag.Title = "Orders from Route " + id; 
                    return View("Index", orders);
                }
            }
            TempData["Type"] = "info";
            TempData["Message"] = "Sorry. We couldn't Find any matching data.";
            String returnUrl = Request.UrlReferrer.ToString();
            if (String.IsNullOrEmpty(returnUrl))
                returnUrl = "~/";

            return Redirect(returnUrl);
        }
            
        [Authorize]
        public ActionResult VisibilitySearch(long id = 0)
        {
            if (id == 0)
            {
                TempData["Type"] = "warning";
                TempData["Message"] = "Invalid Number";
                return RedirectToAction("VisibilityIndex");
            }
            ICollection<Order> list = new List<Order> { };
            if (id == 0)
                return RedirectToAction("Index");
            // Check Ship-Tos<3
            var cust = (from x in db.Customers
                        where x.ID == id
                        select x).FirstOrDefault();
            if (cust != null)
                return View("VisibilityIndex", cust.Orders);
            else
            {
                // Check order
                var ord = (from x in db.Orders
                           where x.SapID == id
                           select x).FirstOrDefault();
                if (ord != null)
                {
                    list.Add(ord);
                    return View("VisibilityIndex", list);
                }
                else
                {
                    // Check Delivery
                    ord = (from x in db.Orders
                           where x.Delivery.ID == id
                           select x).FirstOrDefault();
                    if (ord != null)
                    {
                        list.Add(ord);
                        return View("VisibilityIndex", list);
                    }
                    else
                    {
                        // Check Shipments
                        list = (from x in db.Orders
                                where x.Shipment.ID == id
                                select x).ToList();
                        if (list != null)
                        {
                            return View("VisibilityIndex", list);
                        }
                        //Check Invoices
                        else
                        {
                            var linqOrd = (from x in db.Orders
                                           from y in x.Invoices
                                           where y.ID == id
                                           select x).FirstOrDefault();

                            if (linqOrd != null)
                            {
                                list.Add(linqOrd);
                                return View("VisibilityIndex", list);
                            }
                            else
                            {
                                TempData["Type"] = "info";
                                TempData["Message"] = "Sorry. We couldn't find that number.";
                                return RedirectToAction("VisibilityIndex");
                            }

                        }
                    }
                }
            }
        }
        [Authorize]
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

        [Authorize]
        public ActionResult VisibilityIndex(Boolean filter = false)
        {
            ViewBag.Title = "Order Tracking";
            return View(SelectOrders("all",filter));
        }

        [Authorize(Roles = "CSR,Admin")]
        public ActionResult RDDFConfirmation(Boolean filter = false)
        {
            ViewBag.Title = "RDDF Confirmation";
            return View(SelectOrders("assigned",filter));
        }

        [Authorize(Roles = "CSR,Admin")]
        public JsonResult RDDFBulkConfirmation(String jsonData, String newRDDF)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<String> orders = jss.Deserialize<List<String>>(jsonData);
            System.Diagnostics.Debug.WriteLine("Date = " + newRDDF.ToString());
            Boolean confirmLeadTime = false;
            DateTime RDDF;
            if(!String.IsNullOrEmpty(newRDDF))
            {
                if (!DateTime.TryParseExact(newRDDF, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out RDDF))
                {
                    return Json(new { Status = 0, Message = "Date is not in correct format.(dd/MM/yyyy)" });
                }
            }
            else
            {
                confirmLeadTime = true;
                RDDF = DateTime.Today;
            }
            
            if (orders != null && orders.Any())
            {
                foreach (String order in orders)
                {
                    long ordNumber = long.Parse(order);
                    Order ord = db.Orders.Find(ordNumber);
                    if (ord != null && ord.Customer != null)
                    {
                        if(!confirmLeadTime)
                            ord.RDDF.DSSDate = RDDF;
                        ord.RDDF.Confirmed = true;
                    }
                    
                }
                db.SaveChanges();
                return Json(new { Status = 1, Message = "Dates Confirmed." });

            }else
            {
                return Json(new { Status = 0, Message = "No orders selected." });
            }
        }

        /// <summary>
        /// Returns a list of orders matching the description
        /// </summary>
        /// <param name="desc">Description of orders to get</param>
        /// <returns>Iqueryable Order list</returns>
        [Authorize]
        public IQueryable<Order> SelectOrders(String desc, Boolean filter = false)
        {
            DateTime helper;
            IQueryable<Order> orders = null; 
            switch (desc)
            {
                case "all":
                    orders = (from x in db.Orders
                              select x);
                    ViewBag.Title = "All Orders";
                    break;

                case "openItems":
                    orders = (from x in db.Orders
                              where x.Status.OpenItem == true
                              select x);
                    ViewBag.Title = "Open Items";
                    break;

                case "15D": // Aging
                    helper = DateTime.Today.AddDays(-15);
                    orders = (from x in db.Orders
                              where x.DocDate < helper &&
                              x.Delivery == null
                              select x);
                    ViewBag.Title = "15 Days without DSS Orders";
                    break;

                case "3D": // Out of Lead Time
                    helper = DateTime.Today.AddBusinessDays(-3);
                    orders = (from x in db.Orders
                              where x.Delivery != null &&
                              x.Delivery.Date < helper &&
                              x.Shipment == null && !x.Invoices.Any()
                              select x);
                    ViewBag.Title = "3 Days non Shipped Orders";
                    break;
                case "5D": // Too many days invoiced
                    helper = DateTime.Today.AddDays(-5);
                    orders = (from x in db.Orders
                              where x.Invoices.FirstOrDefault() != null &&
                              x.Invoices.FirstOrDefault().Date < helper &&
                              x.POD == null
                              select x);
                    ViewBag.Title = "5 Days without POD Orders";
                    break;

                case "warning":
                    helper = DateTime.Today.AddDays(-15);
                    DateTime helper2 = DateTime.Today.AddBusinessDays(-3);
                    DateTime helper3 = DateTime.Today.AddDays(-5);

                    orders = (from x in db.Orders
                              where x.DocDate < helper &&
                              x.Delivery == null ||
                              x.Delivery != null &&
                              x.Delivery.Date < helper2 &&
                              x.Shipment == null && !x.Invoices.Any() ||
                              x.Invoices.FirstOrDefault() != null &&
                              x.Invoices.FirstOrDefault().Date < helper3 &&
                              x.POD == null
                              select x);
                    ViewBag.Title = "Under Warning Orders";
                    break;

                case "blocked":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 0 &&
                              x.Status.State == 1
                              select x);
                    ViewBag.Title = "Blocked Orders";
                    break;

                case "open":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 0 &&
                              x.Status.State == 0
                              select x);
                    ViewBag.Title = "Open Orders";
                    break;

                case "assigned":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1
                              select x);
                    ViewBag.Title = "Assigned Orders";
                    break;

                case "planned":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                             x.Status.State == 0 &&
                             x.Status.Reason == 1 ||
                             x.Status.Stage == 2
                              select x);
                    ViewBag.Title = "Planned Orders";
                    break;
                case "invoiced":
                    orders = (from x in db.Orders
                              where x.Status.Code == 50 &&
                              x.POD == null
                              select x);
                    ViewBag.Title = "Invoiced Orders";
                    break;

                case "onTransit":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 3
                              select x);
                    ViewBag.Title = "Transiting Orders";
                    break;

                case "onCustomer":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 4
                              select x);
                    ViewBag.Title = "On Customer Orders";
                    break;

                case "delivered":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 5
                              select x);
                    ViewBag.Title = "Delivered Orders";
                    break;

                case "pending":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 0
                              select x);
                    ViewBag.Title = "Pending Orders";
                    break;

                case "vfr":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 && 
                              x.Status.State == 1 &&
                              x.Status.Reason == 8
                              select x);
                    ViewBag.Title = "Pending for Vehicle Fill Rate";
                    break;

                case "vfrMakro":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 9
                              select x);
                    ViewBag.Title = "Makro Orders Pending for Vehicle Fill Rate";
                    break;

                case "vehicle":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 4
                              select x);
                    ViewBag.Title = "Pending for Vehicle Disposition";
                    break;

                case "deleted":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 10
                              select x);
                    ViewBag.Title = "Pending for Removal";
                    break;

                case "customerCapacity":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 2
                              select x);
                    ViewBag.Title = "Pending for Customer Capacity";
                    break;

                case "appointmentConfirmation":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 1
                              select x);
                    ViewBag.Title = "Pending for Appoinment Confirmation";
                    break;

                case "postponed":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 3
                              select x);
                    break;

                case "minimun":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 6
                              select x);
                    ViewBag.Title = "Pending for Minimal Quantity";
                    break;

                case "minimunMakro":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 7
                              select x);
                    ViewBag.Title = "Makro Orders pending for Minimal Quantity";
                    break;

                case "zsplit":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 11
                              select x);
                    ViewBag.Title = "Pending for ZSPLIT";
                    break;

                case "inventoryLack":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 5
                              select x);
                    ViewBag.Title = "Pending for Inventory Lack";
                    break;

                case "others":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1 &&
                              x.Status.Reason == 0
                              select x);
                    ViewBag.Title = "Orders Pending for Unknown Reasons";
                    break;

                case "allDistribution":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Status.State == 1
                              select x);
                    break;

                case "todayDSS":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.Delivery != null &&
                              x.Delivery.Date == DateTime.Today
                              select x);
                    ViewBag.Title = "Orders from Today DSS";
                    break;

                case "confirmedRDDF":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.RDDF.Confirmed == true
                              select x);
                    ViewBag.Title = "Confirmed RDDF Orders";
                    break;

                case "unconfirmedRDDF":
                    orders = (from x in db.Orders
                              where x.Status.Stage == 1 &&
                              x.RDDF.Confirmed == false
                              select x);
                    ViewBag.Title = "UnConfirmed RDDF Orders";
                    break;

                default:
                    orders = db.Orders;
                    ViewBag.Title = "Default: All Orders";
                    break;
            }

            if(filter)
            {
                UserProfile user = db.UserProfiles.Find(WebSecurity.CurrentUserId);
                if(User.IsInRole("CSR"))
                {
                    System.Diagnostics.Debug.WriteLine("User is CSR");
                    orders = orders.Where(x => x.Customer.MainCSROM.Name == user.FullName);
                }
                    
                if(User.IsInRole("CBD"))
                {
                    System.Diagnostics.Debug.WriteLine("User is CBD");
                    if(Session["FilterData"]!=null)
                    {
                        Models.Filter filterData = Session["FilterData"] as Models.Filter;
                        if(filterData.Settings.ContainsKey("Team"))
                        {
                            String teamFilter = filterData.Settings["Team"];
                            orders = orders.Where(x => x.Customer.Team == teamFilter);
                        }
                        if (filterData.Settings.ContainsKey("Area"))
                        {
                            int areaFilter = Int32.Parse(filterData.Settings["Area"]);
                            orders = orders.Where(x => x.Customer.SaleZone == areaFilter);
                        }
                        if (filterData.Settings.ContainsKey("Unit"))
                        {
                            int unitFilter = Int32.Parse(filterData.Settings["Unit"]);
                            orders = orders.Where(x => x.Customer.Unit == unitFilter);
                        }
                    }
                }
            }
            return orders.ToList().AsQueryable();
        }

        /// <summary>
        /// Return a view with the orders requested.
        /// </summary>
        /// <param name="desc">Requested Orders description</param>
        /// <param name="filter">TRUE if needs to be filtered by User Name</param>
        /// <returns>View with requested orders</returns>
        [Authorize]
        public ActionResult ViewOrders(String desc, Boolean filter = false)
        {
            return View("Index",SelectOrders(desc,filter));
        }

        /// <summary>
        /// Downloads a file containing the order matching the given description
        /// </summary>
        /// <param name="desc">Order Description</param>
        /// <param name="filter">TRUE if orders should be filtered by username ( False by default) </param>
        [Authorize]
        public void DownloadOrders(String desc, Boolean filter = false)
        {
            IQueryable<Order> final = SelectOrders(desc,filter);
            Boolean isCBD = User.IsInRole("CBD");

            var ordersF = from x in final
                          from y in x.Products
                          select new
                          {
                              x.SapID,
                              DocDate = x.DocDate.ToString("dd/MM/yy"),
                              ShipTo = x.Customer.ID,
                              x.Customer.Name,
                              x.CustomerPO,
                              SKU = y.SKU,
                              Description = y.Description,
                              CS = y.Qty.CS,
                              SU = y.Qty.SU,
                              Delivery = (x.Delivery == null) ? "N/A" : x.Delivery.ID.ToString(),
                              DeliveryDate = (x.Delivery == null) ? "N/A" : x.Delivery.Date.ToShortDateString(),
                              DeliveryCS = (x.Delivery == null) ? "N/A" : y.DSSQty.CS.ToString(),
                              DeliverySU = (x.Delivery == null) ? "N/A" : y.DSSQty.SU.ToString(),
                              Shipment = (x.Shipment == null) ? "N/A" : x.Shipment.ID.ToString(),
                              Invoices = (x.Invoices == null || !x.Invoices.Any()) ? "N/A" : x.Invoices.First().ID.ToString(),
                              Status = (isCBD)? x.Status.StageDescription():x.Status.StageDescription() + " " + x.Status.ReasonDescription(),
                          };
            using (ExcelPackage pck = new ExcelPackage())
            {
                // Create WorkSheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Orders");

                // Load Info
                ws.Cells["A1"].LoadFromCollection(ordersF.ToList(), true);

                // Format Headers
                using (ExcelRange rng = ws.Cells["A1:Q1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }

                // Write Back Response to Client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename= " + ViewBag.Title +" "+DateTime.Now +".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }

        /// <summary>
        /// Returns JSON object with orders count.
        /// </summary>
        /// <param name="desc">description orders must match</param>
        /// <param name="filter">TRUE if orders should be filtered by username</param>
        /// <returns>Json Object containing int</returns>
        [Authorize]
        public JsonResult GetOrders(String desc, Boolean filter = false)
        {
            IQueryable<Order> orders = SelectOrders(desc,filter);
            int count = orders.Count();
            int cs = (from order in orders
                      from product in order.Products
                      select product.Qty.CS).Sum(); 
            double su = Math.Round((from order in orders
                              from product in order.Products
                              select (double?)product.Qty.SU).Sum() ?? 0,2);
                
            return Json(new { Count = count,CS = cs, SU = su });
        }
        
        /// <summary>
        /// Updates Order Count of Dashboard Summary
        /// </summary>
        /// <param name="filter">True if orders should by filtered according roles</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult UpdateSummaryCount(Boolean filter = false)
        {
            int openItems = SelectOrders("openItems", filter).Count();
            int D15 = SelectOrders("15D", filter).Count();
            int D3 = SelectOrders("3D", filter).Count();
            int D5 = SelectOrders("5D", filter).Count();
            int blocked = SelectOrders("blocked", filter).Count();
            int open = SelectOrders("open", filter).Count();
            int assigned = SelectOrders("assigned", filter).Count();
            int planned = SelectOrders("planned", filter).Count();
            int transit = SelectOrders("onTransit", filter).Count();
            int customer = SelectOrders("onCustomer", filter).Count();
            int delivered = SelectOrders("delivered", filter).Count();
            int warning = openItems + D15 + D3 + D5;
            int total = blocked + open + assigned + planned + transit + customer + delivered;

            return Json(new
            {
                openItems = openItems,
                D15 = D15,
                D3 = D3,
                D5 = D5,
                Warning = warning,
                Blocked = blocked,
                Open = open,
                Assigned = assigned,
                Planned = planned,
                Transit = transit,
                Customer = customer,
                Delivered = delivered,
                Total = total
            });
        }

        /// <summary>
        /// Updates Complete Dashboard Summary
        /// </summary>
        /// <param name="filter">True if orders should by filtered according roles</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult UpdateSummaryComplete(Boolean filter = false)
        {
            IQueryable<Order> orders = SelectOrders("openItems", filter);
            //int openItems = orders.Count();
            //int openItemsCS = (from order in orders
            //                   from product in order.Products
            //                   select product.Qty.CS).Sum(); 
            //double openItemsSU = Math.Round((from order in orders
            //                                   from product in order.Products
            //                                   select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("15D", filter);
            //int D15 = orders.Count();
            //int D15CS = (from order in orders
            //                   from product in order.Products
            //                   select product.Qty.CS).Sum(); 
            //double D15SU = Math.Round((from order in orders
            //                               from product in order.Products
            //                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("3D", filter);
            //int D3 = orders.Count();
            //int D3CS = (from order in orders
            //             from product in order.Products
            //             select product.Qty.CS).Sum();
            //double D3SU = Math.Round((from order in orders
            //                           from product in order.Products
            //                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("5D", filter);
            //int D5 = orders.Count();
            //int D5CS = (from order in orders
            //             from product in order.Products
            //             select product.Qty.CS).Sum();
            //double D5SU = Math.Round((from order in orders
            //                           from product in order.Products
            //                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("blocked", filter);
            int blocked = orders.Count();
            int blockedCS = (from order in orders
                         from product in order.Products
                         select product.Qty.CS).Sum();
            double blockedSU = Math.Round((from order in orders
                                       from product in order.Products
                                       select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("open", filter);
            int open = orders.Count();
            int openCS = (from order in orders
                             from product in order.Products
                             select product.Qty.CS).Sum();
            double openSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("assigned", filter);
            int assigned = orders.Count();
            int assignedCS = (from order in orders
                             from product in order.Products
                             select product.Qty.CS).Sum();
            double assignedSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("planned", filter);
            int planned = orders.Count();
            int plannedCS = (from order in orders
                             from product in order.Products
                             select product.Qty.CS).Sum();
            double plannedSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("invoiced", filter);
            int invoiced = orders.Count();
            int invoicedCS = (from order in orders
                             from product in order.Products
                             select product.Qty.CS).Sum();
            double invoicedSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("onTransit", filter);
            //int transit = orders.Count();
            //int transitCS = (from order in orders
            //                 from product in order.Products
            //                 select product.Qty.CS).Sum();
            //double transitSU = Math.Round((from order in orders
            //                               from product in order.Products
            //                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("onCustomer", filter);
            //int customer = orders.Count();
            //int customerCS = (from order in orders
            //                 from product in order.Products
            //                 select product.Qty.CS).Sum();
            //double customerSU = Math.Round((from order in orders
            //                               from product in order.Products
            //                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //orders = SelectOrders("delivered", filter);
            //int delivered = orders.Count();
            //int deliveredCS = (from order in orders
            //                 from product in order.Products
            //                 select product.Qty.CS).Sum();
            //double deliveredSU = Math.Round((from order in orders
            //                               from product in order.Products
            //                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            //int warning = openItems + D15 + D3 + D5;
            //int warningCS = openItemsCS + D15CS + D3CS + D5CS;
            //double warningSU = openItemsSU + D15SU + D3SU + D5SU;

            int total = blocked + open + assigned + planned + invoiced;
            //transit + customer + delivered;
            int totalCS = blockedCS + openCS + assignedCS + plannedCS + invoicedCS;
            //transitCS + customerCS + deliveredCS;
            double totalSU = blockedSU + openSU + assignedSU + plannedSU + invoicedSU;
            //transitSU + customerSU + deliveredSU;

            return Json(new { 
                //openItems = openItems,
                //openItemsCS = openItemsCS,
                //openItemsSU = openItemsSU,
                //D15 = D15,
                //D15CS = D15CS,
                //D15SU = D15SU,
                //D3 = D3,
                //D3CS = D3CS,
                //D3SU = D3SU,
                //D5 = D5,
                //D5CS = D5CS,
                //D5SU = D5SU,
                //Warning = warning,
                //WarningCS = warningCS,
                //WarningSU = warningSU,
                Blocked = blocked,
                BlockedCS = blockedCS,
                BlockedSU = blockedSU,
                Open = open,
                OpenCS = openCS,
                OpenSU = openSU,
                Assigned = assigned,
                AssignedCS = assignedCS,
                AssignedSU = assignedSU,
                Planned = planned,
                PlannedCS = plannedCS,
                PlannedSU = plannedSU,
                Invoiced = invoiced,
                InvoicedCS = invoicedCS,
                InvoicedSU = invoicedSU,
                //Transit = transit,
                //TransitCS = transitCS,
                //TransitSU = transitSU,
                //Customer = customer,
                //CustomerCS = customerCS,
                //CustomerSU = customerSU,
                //Delivered = delivered,
                //DeliveredCS = deliveredCS,
                //DeliveredSU = deliveredSU,
                Total = total,
                TotalCS = totalCS,
                TotalSU = totalSU
            });
        }

        /// <summary>
        /// Updates Distribution Summary with order Count only.
        /// </summary>
        /// <param name="filter">True if orders should be filtered.</param>
        /// <returns></returns>
        [Authorize]
        public JsonResult UpdateSummaryDistCount(Boolean filter = false)
        {
            IQueryable<Order> orders = SelectOrders("planned", filter);
            double plannedSU = Math.Round((from order in orders
                                               from product in order.Products
                                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("appointmentConfirmation", filter);
            int appointment = orders.Count();
            double appointmentSU = Math.Round((from order in orders
                                               from product in order.Products
                                               select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("pending", filter);
            int pending = orders.Count();
            double pendingSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("customerCapacity", filter);
            int customerCapacity = orders.Count();
            double customerCapacitySU = Math.Round((from order in orders
                                                    from product in order.Products
                                                    select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("postponed", filter);
            int postponed = orders.Count();
            double postponedSU = Math.Round((from order in orders
                                             from product in order.Products
                                             select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("vehicle", filter);
            int vehicle = orders.Count();
            double vehicleSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("inventoryLack", filter);
            int inventoryLack = orders.Count();
            double inventoryLackSU = Math.Round((from order in orders
                                                 from product in order.Products
                                                 select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("minimun", filter);
            int minimun = orders.Count();
            double minimunSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("minimunMakro", filter);
            int minimunMakro = orders.Count();
            double minimunMakroSU = Math.Round((from order in orders
                                                from product in order.Products
                                                select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("vfr", filter);
            int vfr = orders.Count();
            double vfrSU = Math.Round((from order in orders
                                       from product in order.Products
                                       select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("vfrMakro", filter);
            int vfrMakro = orders.Count();
            double vfrMakroSU = Math.Round((from order in orders
                                            from product in order.Products
                                            select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("deleted", filter);
            int deleted = orders.Count();
            double deletedSU = Math.Round((from order in orders
                                           from product in order.Products
                                           select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("zsplit", filter);
            int zsplit = orders.Count();
            double zsplitSU = Math.Round((from order in orders
                                          from product in order.Products
                                          select (double?)product.Qty.SU).Sum() ?? 0, 2);

            orders = SelectOrders("others", filter);
            int others  = orders.Count();
            double othersSU = Math.Round((from order in orders
                                          from product in order.Products
                                          select (double?)product.Qty.SU).Sum() ?? 0, 2);

            int allDistribution = pending + appointment + customerCapacity + postponed + vehicle + inventoryLack + minimun +
                minimunMakro + vfr + vfrMakro + deleted + zsplit + others;

            double allDistributionSU = Math.Round(pendingSU + appointmentSU + customerCapacitySU + postponedSU + vehicleSU +
                inventoryLackSU + minimunSU + minimunMakroSU + vfrSU + vfrMakroSU + deletedSU + zsplitSU + othersSU,2);

            return Json(new
            {
                PlannedDSU = plannedSU,
                AppointmentConfirmation = appointment,
                AppointmentConfirmationSU = appointmentSU,
                Pending = pending,
                PendingSU = pendingSU,
                CustomerCapacity = customerCapacity,
                CustomerCapacitySU = customerCapacitySU,
                Postponed = postponed,
                PostponedSU = postponedSU,
                Vehicle = vehicle,
                VehicleSU = vehicleSU,
                InventoryLack = inventoryLack,
                InventoryLackSU = inventoryLackSU,
                Minimun = minimun,
                MinimunSU = minimunSU,
                MinimunMakro = minimunMakro,
                MinimunMakroSU = minimunMakroSU,
                VFR = vfr,
                VFRSU = vfrSU,
                VFRMakro = vfrMakro,
                VFRMakroSU = vfrMakroSU,
                Deleted = deleted,
                DeletedSU = deletedSU,
                ZSPLIT = zsplit,
                ZSPLITSU = zsplitSU,
                Others = others,
                OthersSU = othersSU,
                AllDistribution = allDistribution,
                AllDistributionSU = allDistributionSU
            });
        }

        /// <summary>
        /// Handles KPIs update for Index KPI Tab. Provides not only Arrays of Historical indicators for graph but
        /// individual last value for quick visualization
        /// </summary>
        /// <returns>Json Object Containing: Status, Message,CSOT,CSOTGraph,OT,OTGraph</returns>
        [Authorize]
        public JsonResult UpdateKPIS()
        {
            DateTime helper = DateTime.Today.AddDays(-45);
            var csot = (from x in db.HistIndicators
                        where x.Name == "CSOT" &&
                        x.Date > helper
                        select x).ToList().Select(x => new
                        {
                            date = x.Date.ToString("yyyy-MM-dd"),
                            value = x.Value
                        });

            var ot = (from x in db.HistIndicators
                        where x.Name == "OT" &&
                        x.Date > helper
                        select x).ToList().Select(x => new
                        {
                            date = x.Date.ToString("yyyy-MM-dd"),
                            value = x.Value
                        });

                Double CSOTvalue = (csot.LastOrDefault() != null)? csot.LastOrDefault().value:0;

                Double OTvalue = (ot.LastOrDefault() != null)? ot.LastOrDefault().value:0;

                return Json(new { Status = 1, Message = "KPIs Updated", CSOT = CSOTvalue, CSOTGraph = csot, OT = OTvalue, OTGraph = ot });
        }
        [Authorize]
        public void DownloadOrderInformation(long id)
        {
            Order order = db.Orders.Find(id);
            var products = from x in order.Products
                           select new
                           {
                               SKU = x.SKU,
                               Description = x.Description,
                               CS = x.Qty.CS,
                               SU = x.Qty.SU,
                               DssCS = x.DSSQty.CS,
                               DssSU = x.DSSQty.SU
                           };


            using (ExcelPackage pck = new ExcelPackage())
            {
                // Create WorkSheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Order Details");

                // Load Info
                ws.Cells["A1"].Value = "Order #";
                ws.Cells["B1"].Value = order.SapID;
                ws.Cells["A2"].Value = "Status";
                ws.Cells["B2"].Value = order.Status.StageDescription()+" "+order.Status.ReasonDescription();
                ws.Cells["A3"].Value = "PO";
                ws.Cells["B3"].Value = order.CustomerPO;
                ws.Cells["A4"].Value = "Doc. Date";
                ws.Cells["B4"].Value = order.DocDate;
                ws.Cells["A5"].Value = "DSS Date";
                ws.Cells["B5"].Value = order.RDDF.DSSDate;
                ws.Cells["C1"].Value = "Delivery #";
                ws.Cells["D1"].Value = (order.Delivery != null)? order.Delivery.ID.ToString():"N/A";
                ws.Cells["C2"].Value = "Delivery Date";
                ws.Cells["D2"].Value = (order.Delivery != null) ? order.Delivery.Date.ToString() : "N/A";
                ws.Cells["C3"].Value = "Shipment #";
                ws.Cells["D3"].Value = (order.Shipment != null) ? order.Shipment.ID.ToString() : "N/A";
                ws.Cells["C4"].Value = "Shipment Date";
                ws.Cells["D4"].Value = (order.Shipment != null) ? order.Shipment.Date.ToString() : "N/A";
                ws.Cells["C5"].Value = "Carrier";
                ws.Cells["D5"].Value = (order.Shipment.Carrier != null) ? order.Shipment.Carrier.Name.ToString() : "N/A";
                ws.Cells["E1"].Value = "Invoices";
                ws.Cells["F1"].Value = (order.Invoices.Any()) ? JsonConvert.SerializeObject(order.Invoices.ToList()) : "N/A";
                ws.Cells["E2"].Value = "POD Date";
                ws.Cells["F2"].Value = (order.POD != null) ? order.POD.Date.ToString() : "N/A";
                ws.Cells["A6"].LoadFromCollection(products.ToList(), true);

                // Format Headers
                using (ExcelRange rng = ws.Cells["A6:F6"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }
                using (ExcelRange rng = ws.Cells["A1:A5"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }
                using (ExcelRange rng = ws.Cells["E1:E2"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }
                using (ExcelRange rng = ws.Cells["C1:C5"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }
                // Write Back Response to Client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename= Order_" + order.SapID + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }
    }
}