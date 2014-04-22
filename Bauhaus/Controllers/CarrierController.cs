using Bauhaus.Models;
using Bauhaus.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Bauhaus.Controllers
{
    public class CarrierController : Controller
    {
        BauhausEntities db = new BauhausEntities();
        //
        // GET: /Carrier/
        [Authorize(Roles="Carrier , Admin")]
        public ActionResult Index()
        {
            ViewBag.Title = "Shipments";
            BauhausEntities db = new BauhausEntities();

            IQueryable<Shipment> shipments;

            if (User.IsInRole("Carrier"))
            {
                UserProfile user = db.UserProfiles.Find(WebSecurity.CurrentUserId);

                shipments = (from x in db.Shipments
                             where x.Carrier.Name == user.FullName
                             select x).ToList().AsQueryable();

            }
            else
            {
                shipments = db.Shipments.ToList().AsQueryable();
            }

            return View(shipments);
        }

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

        /// <summary>
        /// Returns rendered partial view _ShipmentInfo on Json Object.
        /// </summary>
        /// <param name="id">ID of shipment to be loaded in the view</param>
        /// <returns>JSON with status, Message and rendered view.</returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult GetShipment(long id)
        {

            Shipment shp = db.Shipments.Find(id);
            if (shp == null)
                return Json(new { Status = 0, Message = "Not found" });
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("_ShipmentInfo", shp) });
        }
        /// <summary>
        /// Creates a new vehicle and assigns it to a carrier company.
        /// </summary>
        /// <param name="carrier">Carrier ID</param>
        /// <param name="plate">Vehicle Plate</param>
        /// <param name="type">Vehicle Capacity Type</param>
        /// <param name="driversName">Drivers Name</param>
        /// <param name="driversTelephone">Drivers Telephone</param>
        /// <returns>JSON with Status and Message of operation.</returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult RegisterVehicle(long carrier ,string plate, string type, string driversName, string driversTelephone)
        {
            Carrier carr = db.Carriers.Find(carrier);

            if(carr==null)
                return Json(new { Status = 0, Message = "Carrier not Found." });


            Contact Ndriver = carr.Drivers.Where(item => item.Name == driversName).FirstOrDefault();
            if ( Ndriver == null)
            {
                Ndriver = new Contact();
                Ndriver.Area = "Driver";
                Ndriver.Name = driversName;
                Ndriver.Telephone = driversTelephone;
                carr.Drivers.Add(Ndriver);
            }
                
            Vehicle Veh = carr.Vehicles.Where(item=>item.Plate == plate).FirstOrDefault();
            if (Veh != null)
                if (Veh.Driver.Name == driversName)
                    return Json(new { Status = 0, Message = "Vehicle Already Registered." });
                else
                    Veh.Driver = Ndriver;
            else
            {
                Veh = new Vehicle();
                Veh.Driver = Ndriver;
                Veh.Plate = plate;
                Veh.Type = type;
                carr.Vehicles.Add(Veh);
            }
            db.SaveChanges();
            return Json(new { Status = 1, Message = "Ok" });
        }

        /// <summary>
        /// Takes Shipment id and assigns vehicle id
        /// </summary>
        /// <param name="ShipmentId">Shipment id</param>
        /// <param name="VehicleId">Vehicle id to assign</param>
        /// <returns>JSON witn status and Message</returns>
        [Authorize(Roles="Carrier , Admin")]
        public JsonResult AssignVehicle(long ShipmentId, int VehicleId)
        {
            Shipment shpmt = db.Shipments.Find(ShipmentId);
            if (shpmt != null)
            {
                shpmt.Vehicle = db.Vehicles.Find(VehicleId);
                if (shpmt.Vehicle != null)
                {
                    db.SaveChanges();
                    return Json(new { Status = 1, Message = "Vehicle Assigned." });
                }
                else
                    return Json(new { Status = 0, Message = "Vehicle not Found." });
            }
            else
                return Json(new { Status = 0, Message = "Shipment not Found." });
        }

        /// <summary>
        /// Takes Shipment id and assigns vehicle id
        /// </summary>
        /// <param name="ShipmentId">Shipment id</param>
        /// <param name="VehicleId">Vehicle id to assign</param>
        /// <returns>JSON witn status and Message</returns>
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult SaveAssignVehicle(long ShipmentId, long carrier, string plate, string type, string driversName, string driversTelephone)
        {
            Shipment shpmt = db.Shipments.Find(ShipmentId);
            if (shpmt != null)
            {
                Carrier carr = db.Carriers.Find(carrier);

                if (carr == null)
                    return Json(new { Status = 0, Message = "Carrier not Found." });


                Contact Ndriver = carr.Drivers.Where(item => item.Name == driversName).FirstOrDefault();
                if (Ndriver == null)
                {
                    Ndriver = new Contact();
                    Ndriver.Area = "Driver";
                    Ndriver.Name = driversName;
                    Ndriver.Telephone = driversTelephone;
                    carr.Drivers.Add(Ndriver);
                }

                Vehicle Veh = carr.Vehicles.Where(item => item.Plate == plate).FirstOrDefault();
                if (Veh != null)
                {
                    if (Veh.Driver.Name != driversName)
                    {
                        Veh.Driver = Ndriver;
                    }
                        
                }
                else
                {
                    Veh = new Vehicle();
                    Veh.Driver = Ndriver;
                    Veh.Plate = plate;
                    Veh.Type = type;
                    carr.Vehicles.Add(Veh);
                }
                shpmt.Vehicle = Veh;
                db.SaveChanges();
                return Json(new { Status = 1, Message = "Ok" });
            }
            else
                return Json(new { Status = 0, Message = "Shipment not Found." });
        }


        /// <summary>
        /// Edits pointed vehicle with given data.
        /// </summary>
        /// <param name="id">Vehicle Id</param>
        /// <param name="plate">Vehicle Plate</param>
        /// <param name="status">Vehicle Status</param>
        /// <param name="type">Vehicle Type</param>
        /// <param name="driversName">Drivers Name</param>
        /// <param name="driversTelephone">Drivers Telephone</param>
        /// <returns>Json with Status and Message</returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult EditVehicle(int id, string plate, string type,
                                        string driversName, string driversTelephone )
        {
            Vehicle auxVehicle = db.Vehicles.Find(id);
            if (auxVehicle != null)
            {
                auxVehicle.Plate = plate;
                auxVehicle.Type = type;
                auxVehicle.Driver.Name = driversName;
                auxVehicle.Driver.Telephone = driversTelephone;

                db.SaveChanges();

                return Json(new { Status = 1, Message = "Vehicle Edited." });
            }
            else
                return Json(new { Status = 0, Message = "Vehicle not Found." });
        }

        /// <summary>
        /// Takes vehicle id and deletes that vehicle from carriers list.
        /// </summary>
        /// <param name="id">vehicles id to be deleted</param>
        /// <returns>Json with Status and Message</returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult DeleteVehicle(int id)
        {

            Vehicle veh = db.Vehicles.Find(id);
            if(veh!=null)
            {
                var shipments = (from x in db.Shipments
                                 where x.Vehicle.Plate == veh.Plate
                                 select x);
                if(shipments.Any())
                {
                    foreach (Shipment shp in shipments)
                    {
                        shp.Vehicle = null;
                        shp.Carrier.Vehicles.Remove(veh);
                    }
                }
                

                db.Vehicles.Remove(veh);
                db.SaveChanges();
                return Json(new { Status = 1, Message = "Vehicle Deleted." });
            }
            else
                return Json(new { Status = 0, Message = "Vehicle not Found." });
        }

        /// <summary>
        /// Takes orders belonging to a selected shipment and customer changing States
        /// </summary>
        /// <param name="shipment">Orders Shipment ID</param>
        /// <param name="customer">Orders Customer ID </param>
        /// <param name="stage">New Stage</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult UpdateStage(long shipment, long customer, int stage)
        {
            List<Order> orders = (from x in db.Orders
                                  where x.Customer.ID == customer &&
                                  x.Shipment != null && x.Shipment.ID == shipment
                                  select x).ToList();
            if (orders == null)
            {
                return Json(new { Status = 0, Message = "Customer has no Orders." });
            }

            if(stage < 2 || stage > 5)
            {
                return Json(new { Status = 0, Message = "Invalid Stage." });
            }

            foreach (Order ord in orders)
            {
                if(stage == 5)
                {
                    if(ord.POD == null)
                    {
                        ord.POD = new POD();
                        ord.POD.Date = DateTime.Today;
                        //ord.calculateIndicators();
                    }
                }
                ord.Status.Stage = stage;
                ord.Status.State = 0;
                ord.Status.Reason = 0;
            }

            db.SaveChanges();

            return Json( new { Status = 1, Message = "Orders Updated."});
        }

        /// <summary>
        /// Updates all customer order that belongs to selected shipment and stage with a new reason.
        /// </summary>
        /// <param name="shipment">Shipment ID where to get the orders</param>
        /// <param name="stage">Customer ID whose orders will be changed</param>
        /// <param name="reason">New status</param>
        /// <param name="comment">New comments asociated with status</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Carrier , Admin")]
        public JsonResult UpdateReason(long shipment, int stage, int reason, string comment)
        {
            Shipment shpmt = db.Shipments.Find(shipment);
            if(shpmt == null)
            {
                return Json(new { Status = 0, Message = "Shipment not Found." });
            }

            List<Order> orders = (from x in db.Orders
                                  where x.Shipment != null &&
                                  x.Shipment.ID == shipment &&
                                  x.Status.Stage == stage
                                  select x).ToList();
            if (!orders.Any())
            {
                return Json(new { Status = 0, Message = "No orders on selected Stage." });
            }
            else
            {
                foreach (Order ord in orders)
                {
                    ord.Status.State = 1;
                    ord.Status.Reason = reason;
                }
            }

            Input inp = new Input();
            inp.Author = User.Identity.Name;
            inp.Time = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            inp.Stage = stage;
            inp.State = 1;
            inp.Reason = reason;
            inp.Comment = comment;

            if (shpmt.TransitData == null)
            {
                shpmt.TransitData = new List<Input> { };
            }
            shpmt.TransitData.Add(inp);

            db.SaveChanges();

            return Json(new { Status = 1, Message = "Orders Updated." });
        }

        /// <summary>
        /// Takes a Carriers ID and return a list of every vehicle/driver the company have registered.
        /// </summary>
        /// <param name="id">Carriers ID</param>
        /// <returns>Json Object containing Status, Message and Partia View</returns>
        [Authorize(Roles="Carrier , Admin")]
        public JsonResult GetVehEditModal(long id)
        {
            System.Diagnostics.Debug.WriteLine("Getting Vehicles");
            Shipment shp = db.Shipments.Find(id);
            if (shp == null)
                return Json(new { Status = 0, Message = "Not found" });
            return Json(new { Status = 1, Message = "Ok", Content = RenderPartialViewToString("_VehEditModal", shp ) });
        }

        /// <summary>
        /// Returns partial view with vehicles Grid.
        /// </summary>
        /// <param name="id">Vehicles Carrier Id</param>
        /// <returns>Rendered partial view with vehicles grid</returns>
        [Authorize(Roles = "Carrier , Admin")]
        public ActionResult GetVehiclesGrid(long id,int page = 1,int elements = 5)
        {
            
            IEnumerable<Vehicle> vehicles = db.Carriers.Find(id).Vehicles.ToList();
            int totalPages = vehicles.Count() / elements;
            vehicles = vehicles.Skip(page * elements - elements).Take(elements);
            TempData["Page"] = page;
            return PartialView("_VehiclesGrid",vehicles);
        }

        /// <summary>
        /// Return a vehicle structure for data loading purposes
        /// </summary>
        /// <param name="id">Id of Vehicle to fetch</param>
        /// <returns></returns>
        public JsonResult getVehicle(int id)
        {
            Vehicle veh = db.Vehicles.Find(id);
            if (veh == null)
                return Json(new { Status = 0, Message = "Couldn't Find Vehicle" });
            return Json(new { Status = 1, Message = "Ok", Content = veh });
        }

        
    }
}
