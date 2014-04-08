using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;
using OfficeOpenXml;
using System.IO;
using System.Globalization;
using System.Data.Entity.Validation;
using System.Threading;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Bauhaus.Helpers;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace Bauhaus.Controllers
{
    public class ReportController : Controller
    {
        private BauhausEntities db = new BauhausEntities();
        private Thread current;

        //
        // GET: /Report/
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.Title = "Manage Reports";
            return View(db.Reports.ToList().OrderByDescending(x => x.CreationDate));
        }

        /* Method: UploadReport
         * Desc: Handles report uploading and updating Reports DB table whenever existing reports are
         * overwriten. Also provides methods for cleaning reports format and updating creation date.
         */
        [Authorize(Roles = "Admin")]
        public ActionResult UploadReport()
        {
            System.Diagnostics.Debug.WriteLine("Upload Report");
            Stopwatch sw = Stopwatch.StartNew();
            if (Request.Files["fileUpload"].ContentLength > 0)
            {
                //Check if Directory Exist
                if (!System.IO.Directory.Exists(Server.MapPath("~/Content/UploadedReports")))
                {
                    System.Diagnostics.Debug.WriteLine("Creating Directory");
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/UploadedReports"));
                }

                //Initialize File in Server
                Report stReport;
                string path = string.Format("{0}/{1}", Server.MapPath("~/Content/UploadedReports"),
                                        Request.Files["fileUpload"].FileName);

                // Check File Extension
                if(Path.GetExtension(path)!= ".xlsx")
                {
                    TempData["Type"] = "warning";
                    TempData["Message"] = "Incorrect Formar, please use '.xlsx' files.";
                    return RedirectToAction("Index");
                }

                // Check if exist in DB
                string fileName = Request.Files["fileUpload"].FileName;
                if ((stReport = db.Reports.Where(b => b.Name == fileName).FirstOrDefault()) != null)
                {
                    System.Diagnostics.Debug.WriteLine("File in DB");
                    // Check if same report its being processed.
                    if (stReport.Status == 2)
                    {
                        TempData["Type"] = "warning";
                        TempData["Message"] = "Same Report its being processed right now. Please Wait a few minutes and try uploading again.";
                        return RedirectToAction("Index");
                    }
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    db.Reports.Remove(stReport);
                    db.SaveChanges();
                }
                // File not in Db
                else
                {
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }

                // Registering Log Action
                Log log = new Log();
                log.Source = Request.UserHostAddress;
                log.UserName = User.Identity.Name;
                log.Type = "Information";
                log.Description = "Uploaded Report '" + Request.Files["fileUpload"].FileName;
                log.Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
                db.Logs.Add(log);


                Request.Files["fileUpload"].SaveAs(path);

                // Adding Report to DB

                //Handle Report Adding
                stReport = new Report();
                stReport.Name = Request.Files["fileUpload"].FileName;
                stReport.CreationDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
                stReport.Status = 0;
                stReport.Uploader = User.Identity.Name;
                stReport.Path = path;
                db.Reports.Add(stReport);


                // Clean and Update Date Procedures.

                if (!CleanReport(stReport))
                {
                    TempData["Type"] = "danger";
                    TempData["Message"] = "Report Couldn't be Formatted Properly, Please Review Layout and/or Sheet Name to be the same as report.";
                    stReport.Status = 3;
                    stReport.Remark = "Report Couldn't be Formatted Properly.";
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                // Try to Process Report Automatically
                var processing = (from x in db.Reports
                                  where x.Status == 2
                                  select x).FirstOrDefault();

                if (processing != null)
                {
                    TempData["Type"] = "warning";
                    TempData["Message"] = "There is another report Processing Right Now. Please try to process Manually in a few Minutes.";
                    return RedirectToAction("Index");
                }


                stReport.Status = 2;
                db.SaveChanges();

                //Thread Report processing.

                current = new Thread(() =>
                    {
                        try
                        {
                            ProcessReport(stReport.ReportID);
                            if (stReport.Name == "ZSKU.xlsx")
                                CleanOrders(stReport.ReportID);
                            sw.Stop();
                            CalculateKPIs();
                            System.Diagnostics.Debug.WriteLine("Time Taken Cust Method: {0}ms", sw.Elapsed.TotalMilliseconds);
                        }
                        catch (Exception e)
                        {
                            db.Dispose();
                            db = new BauhausEntities();
                            LogError("ZSKU", 0, "Critical", e.ToString());
                            stReport.Remark = e.Message;
                            stReport.Status = 3;
                            db.SaveChanges();
                            TempData["Type"] = "Danger";
                            TempData["Message"] = "Report Processing Error: " + e.Message;
                        }
                    });

                current.Start();

                TempData["Type"] = "success";
                TempData["Message"] = "Your Report Has Begun Processing Correctly.";

            }
            else
            {
                TempData["Type"] = "danger";
                TempData["Message"] = "It Seems to be a Problem with Upload Request. Contact your IT administrator.";
            }


            return RedirectToAction("Index");
        }

        /* Method: ReportCleaning
         * Desc: Handles the formating of all reports uploaded to the system.
         * Args: Report: Report to be cleaned.
         */
        [Authorize]
        private bool CleanReport(Report report)
        {
            bool result = false;
            var reportFile = new FileInfo(report.Path);

            using (var package = new ExcelPackage(reportFile))
            {
                // Get the Workbook in the stored File.
                var workbook = package.Workbook;
                if (workbook != null)
                {
                    if (workbook.Worksheets.Count > 0)
                    {

                        foreach (ExcelWorksheet cWs in workbook.Worksheets)
                        {
                            String sheetName = cWs.Name.ToLower();

                            switch (sheetName)
                            {
                                case "lead times":
                                    result = true;
                                    break;
                                case "toda la data de activos":
                                    result = true;
                                    break;
                                case "info om-transporte":
                                    result = true;
                                    break;
                                case "zsku":
                                    cWs.DeleteRow(1, 3, true); // Delete Heading.
                                    cWs.DeleteRow(2, 1, true); // Delete 2nd Row.
                                    result = true;
                                    break;
                                case "pendiente":
                                    result = true;
                                    break;
                                case "caletas":
                                    result = true;
                                    break;
                                case "zvorf":
                                    result = true;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                //Save Excel File
                package.Save();
            }

            return result;
        }

        /// <summary>
        /// Stores Data from a Report in DB.
        /// </summary>
        /// <param name="id">ID of Report to be Processed</param>
        /// <returns>Report Index View</returns>
        [Authorize]
        public void ProcessReport(int id)
        {

            System.Diagnostics.Debug.WriteLine("Processing");

            Report stReport = db.Reports.Find(id);

            db.SaveChanges();

            var reportFile = new FileInfo(stReport.Path);

            using (var package = new ExcelPackage(reportFile))
            {
                // Get the Workbook in the stored File.
                var workbook = package.Workbook;
                if (workbook != null)
                {
                    if (workbook.Worksheets.Count > 0)
                    {
                        Boolean ready = false;
                        foreach (ExcelWorksheet cWs in workbook.Worksheets)
                        {
                            if (cWs.Dimension == null)
                                break;

                            // Finishes Sheet Loop if founded.
                            if (ready)
                                break;

                            

                            // Casing Procedures Due report Name
                            String sheetName = cWs.Name.ToLower();
                            System.Diagnostics.Debug.WriteLine(sheetName);
                            switch (sheetName)
                            {
                                // Routes information
                                case "lead times":
                                    if (ProcessRoutes(cWs, stReport) == 0)
                                    {
                                        stReport.Status = 1;
                                    }
                                    else
                                    {
                                        stReport.Status = 3;
                                    }
                                    break;
                                // MAESTRO
                                case "toda la data de activos":
                                    if (ProcessCustomerData(cWs, stReport) == 0)
                                    {
                                        stReport.Status = 1;
                                    }
                                    else
                                    {
                                        stReport.Status = 3;
                                    }
                                    db.SaveChanges();
                                    break;

                                // Summary de Jose Tarazona
                                case "info om-transporte":
                                    System.Diagnostics.Debug.WriteLine("Located Customers Report");
                                    if (ProcessCustomerData(cWs, stReport) == 0)
                                    {
                                        stReport.Status = 1;
                                    }
                                    else
                                    {
                                        stReport.Status = 3;
                                    }
                                    db.SaveChanges();
                                    break;

                                // ORDENES EN MANO BDC
                                case "pendiente":
                                    if (ProcessOrdenesEnMano(cWs, stReport) != 0)
                                        stReport.Status = 0;
                                    else
                                        stReport.Status = 1;
                                    break;

                                case "zvorf":
                                    if (ProcessOrdenesEnMano(cWs, stReport) != 0)
                                        stReport.Status = 0;
                                    else
                                        stReport.Status = 1;
                                    break;

                                case "zsku":
                                    int count = ProcessZsku(cWs, stReport);
                                    if (count != 0)
                                    {
                                        stReport.Status = 3;
                                        stReport.Remark = "Completed with " + count.ToString() + " Format Error(s).";
                                    }
                                    else
                                        stReport.Status = 1;
                                    break;

                                case "caletas":
                                    if (ProcessCarryFees(cWs, stReport) == 0)
                                        stReport.Status = 1;
                                    else
                                        stReport.Status = 0;
                                    break;

                                default:
                                    break;
                            }
                        }

                        if (stReport.Status != 1)
                        {
                            stReport.Status = 3;
                            if (String.IsNullOrEmpty(stReport.Remark))
                                stReport.Remark = "Report Processing Failed, See log for details.";
                        }


                        package.Save();
                        System.Diagnostics.Debug.WriteLine("Report Date: " + stReport.CreationDate.ToString());
                        db.Entry(stReport).State = System.Data.Entity.EntityState.Modified;
                        try
                        {

                            db.SaveChanges();
                        }
                        catch (System.Data.Entity.Validation.DbEntityValidationException e)
                        {
                            var outputLines = new List<string>();
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                outputLines.Add(string.Format(
                                    "{0}: Entity of type \"{1}\" in state \"{2}\" has the following validation errors:",
                                    DateTime.Now, eve.Entry.Entity.GetType().Name, eve.Entry.State));
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    outputLines.Add(string.Format(
                                        "- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage));
                                }
                            }
                            LogError(stReport.Name, 0, "Entity Validation", outputLines.ToString());
                        }

                    }
                }
                // Dispose Resources.
                package.Dispose();
                RemoveReport(stReport.ReportID, 0);
            }
        }

        /// <summary>
        /// Removes the report from DB
        /// </summary>
        /// <param name="stReport">Report to be removed</param>
        /// <param name="option">0  to Remove only File or 1 to remove file and db record</param>
        /// 
        private void RemoveReport(int stReport, int option)
        {
            Report selected = db.Reports.Find(stReport);
            if (selected != null)
            {
                if (System.IO.File.Exists(selected.Path))
                    System.IO.File.Delete(selected.Path);
                if (option != 0)
                    db.Reports.Remove(selected);
            }
        }
        /// <summary>
        /// Handles Carry fees processing. Adds Carry Fess with their respective Routes and
        /// Transportation Companys.
        /// </summary>
        /// <param name="cWs">Current WorkcSheet</param>
        /// <param name="stReport">Report To Be Processed</param>
        /// <returns>Error Count During Processing</returns>
        private int ProcessCarryFees(ExcelWorksheet cWs, Report stReport)
        {
            int cCount = 350;
            Dictionary<String, String> Map = MapReport(cWs);

            System.Diagnostics.Debug.WriteLine("Caletas");
            ReportStructure columns = new ReportStructure();
            columns.CarryFees();

            for (int i = 2; i <= cWs.Dimension.End.Row; i++)
            {
                long carrN = long.Parse(cWs.Cells[Map[columns.CarrierNumber] + i].Text);
                Carrier auxCarrier = db.Carriers.Find(carrN);

                if (auxCarrier != null)
                {
                    String auxRoute = cWs.Cells[Map[columns.Route] + i].Text;
                    String auxVeh = cWs.Cells[Map[columns.VehicleType] + i].Text;

                    CarryFee newCarryFee = (from x in auxCarrier.CarryFees
                                            where x.Route == auxRoute &&
                                            x.VehicleType == auxVeh
                                            select x).FirstOrDefault();
                    if (newCarryFee == null)
                    {
                        newCarryFee = new CarryFee();
                        newCarryFee.Route = auxRoute;
                        newCarryFee.VehicleType = auxVeh;
                        newCarryFee.Cost = cWs.Cells[Map[columns.CarryFee] + i].Text;

                        auxCarrier.CarryFees.Add(newCarryFee);
                    }
                    else
                    {
                        if (newCarryFee.Cost != cWs.Cells[Map[columns.CarryFee] + i].Text)
                        {
                            newCarryFee.Cost = cWs.Cells[Map[columns.CarryFee] + i].Text;
                        }
                    }

                }

                if (i % cCount == 0)
                {
                    stReport.Remark = ((int)((double)((double)i / cWs.Dimension.End.Row) * 100)) + "%";
                    db.SaveChanges();
                    db.Dispose();
                    db = new BauhausEntities();
                }
            }
            return 0;
        }

        /// <summary>
        /// Handles the processing of LeadTime values report, creating routes
        /// and linking each route to a leadtime.
        /// </summary>
        /// <param name="cWs">Current WorkSheet</param>
        /// <param name="stReport">Report to be Processed</param>
        /// <returns>Error Count during Saving.</returns>

        private int ProcessRoutes(ExcelWorksheet cWs, Report stReport)
        {
            int commitCount = 350;

            Dictionary<String, String> Mapping = MapReport(cWs);
            System.Diagnostics.Debug.WriteLine("Data de Rutas");
            ReportStructure columnsRoutes = new ReportStructure();
            columnsRoutes.RoutesData();

            for (int i = 2; i <= cWs.Dimension.End.Row; i++)
            {
                String routeName = cWs.Cells[Mapping[columnsRoutes.Route] + i].Text;
                System.Diagnostics.Debug.WriteLine(routeName);
                Route nRoute = db.Routes.Where(route => route.Name == routeName).FirstOrDefault();
                // Route Founded.
                if (nRoute != null)
                {
                    nRoute.Plant = cWs.Cells[Mapping[columnsRoutes.Plant] + i].Text;
                    System.Diagnostics.Debug.WriteLine(int.Parse(cWs.Cells[Mapping[columnsRoutes.LeadTime] + i].Text));
                    nRoute.LeadTime = new TimeSpan(int.Parse(cWs.Cells[Mapping[columnsRoutes.LeadTime] + i].Text), 0, 0, 0, 0);
                    System.Diagnostics.Debug.WriteLine(nRoute.LeadTime);
                }
                // Route Unkwon.
                else
                {
                    nRoute = new Route();
                    System.Diagnostics.Debug.WriteLine(int.Parse(cWs.Cells[Mapping[columnsRoutes.LeadTime] + i].Text));
                    nRoute.LeadTime = new TimeSpan(int.Parse(cWs.Cells[Mapping[columnsRoutes.LeadTime] + i].Text), 0, 0, 0, 0);
                    System.Diagnostics.Debug.WriteLine(nRoute.LeadTime);
                    nRoute.Plant = cWs.Cells[Mapping[columnsRoutes.Plant] + i].Text;
                    nRoute.Name = cWs.Cells[Mapping[columnsRoutes.Route] + i].Text;

                    db.Routes.Add(nRoute);
                }

                if (i % commitCount == 0)
                {
                    stReport.Remark = ((int)((double)((double)i / cWs.Dimension.End.Row) * 100)) + "%";
                    db.SaveChanges();
                    db.Dispose();
                    db = new BauhausEntities();
                }
            }
            db.SaveChanges();
            stReport.Status = 0;
            return 0;
        }

        /// <summary>
        /// Handles the processing of all customer data reports.
        /// </summary>
        /// <param name="cWs">Current WorkSheet from wich to extract data</param>
        /// <param name="stReport">Report Information</param>
        /// <returns> 0 if all processing went right, # of Errors encountered.</returns>
        private int ProcessCustomerData(ExcelWorksheet cWs, Report stReport)
        {
            int commitCount = 350;
            Dictionary<String, String> Mapping = MapReport(cWs);
            System.Diagnostics.Debug.WriteLine("Data de Clientes");
            Customers columnsMaestro = new Customers();
            String errors = columnsMaestro.SelfCheck(Mapping);
            if (!String.IsNullOrWhiteSpace(errors))
            {
                System.Diagnostics.Debug.WriteLine("Aborting process due to missing columns");
                LogError("OEM", 0, "Missing Columns", errors);
                stReport.Remark = "An error ocurred, missing columns: " + errors;
                return 1;
            }

            for (int i = 2; i <= cWs.Dimension.End.Row; i++)
            {
                bool nuevo = false;
                long CustNumber = long.Parse(cWs.Cells[Mapping[columnsMaestro.ShipTo] + i].Text);
                System.Diagnostics.Debug.WriteLine(CustNumber);
                Customer tempCust = db.Customers.Find(CustNumber);
                if (tempCust == null)
                {
                    tempCust = new Models.Customer();
                    tempCust.ID = CustNumber;
                    tempCust.Contacts = new List<Contact> { };
                    nuevo = true;
                }
                tempCust.Name = cWs.Cells[Mapping[columnsMaestro.CustomerName] + i].Text;
                tempCust.Region = cWs.Cells[Mapping[columnsMaestro.Region] + i].Text;
                tempCust.City = cWs.Cells[Mapping[columnsMaestro.City] + i].Text;
                tempCust.Address = cWs.Cells[Mapping[columnsMaestro.Address] + i].Text;
                tempCust.SaleZone = (cWs.Cells[Mapping[columnsMaestro.SaleZone] + i].Text != "") ? Convert.ToInt32(cWs.Cells[Mapping[columnsMaestro.SaleZone] + i].Text) : 0;
                tempCust.Team = cWs.Cells[Mapping[columnsMaestro.Team] + i].Text;

                // REGISTER CBD REP
                tempCust.CBDRep = updateContact(tempCust.CBDRep,"CBD",cWs.Cells[Mapping[columnsMaestro.CBDRep] + i].Text, cWs.Cells[Mapping[columnsMaestro.CBDRepTel] + i].Text, "");

                // REGISTER GU
                tempCust.GU = updateContact(tempCust.GU, "GU", cWs.Cells[Mapping[columnsMaestro.GU] + i].Text, cWs.Cells[Mapping[columnsMaestro.GUTel] + i].Text, "");
         
                //tempCust.PayTerm = cWs.Cells[Mapping[columnsMaestro.PaymentTerm] + i].Text;
                // tempCust.PayDescription = cWs.Cells[Mapping[columnsMaestro.PaymentTermDescription] + i].Text;
                tempCust.MaxVEH = cWs.Cells[Mapping[columnsMaestro.MaxVehicle] + i].Text;
                //Route
                System.Diagnostics.Debug.WriteLine(cWs.Cells[Mapping[columnsMaestro.Route] + i].Text);
                String routeName = cWs.Cells[Mapping[columnsMaestro.Route] + i].Text;
                tempCust.Route = db.Routes.Where(route => route.Name == routeName).FirstOrDefault();

                // REGISTER MAIN CSR OM
                tempCust.MainCSROM = updateContact(tempCust.MainCSROM, "CSR OM", cWs.Cells[Mapping[columnsMaestro.MainOMCSR] + i].Text, "", "");
                
                // REGISTER BACKUP CSR OM
                tempCust.BackupCSROM = updateContact(tempCust.BackupCSROM, "CSR OM", cWs.Cells[Mapping[columnsMaestro.BackupOMCSR] + i].Text, "", "");
                
                // REGISTER MAIN CSR AR
                tempCust.MainCSRAR = updateContact(tempCust.MainCSRAR, "CSR AR", cWs.Cells[Mapping[columnsMaestro.MainARCSR] + i].Text, "", "");
                
                // REGISTER BACKUP CSR AR
                tempCust.BackupCSRAR = updateContact(tempCust.BackupCSRAR, "CSR AR", cWs.Cells[Mapping[columnsMaestro.BackupARCSR] + i].Text, "", "");
                
                // Register || Update Initial Contacts
                Contact cont;
                if (nuevo)
                {
                    cont = updateContact(null,"Principal",tempCust.Name,cWs.Cells[Mapping[columnsMaestro.MainTelephone] + i].Text,cWs.Cells[Mapping[columnsMaestro.GeneralEmail] + i].Text);
                    tempCust.Contacts.Add(cont);
                    cont = updateContact(null, "Recepción",cWs.Cells[Mapping[columnsMaestro.ContactName] + i].Text ,cWs.Cells[Mapping[columnsMaestro.ContactTelephone] + i].Text, "");
                    tempCust.Contacts.Add(cont);
                }else
                {
                    cont = tempCust.Contacts.Where(x => x.Area == "Principal").First();
                    cont = updateContact(cont,"Principal",tempCust.Name,cWs.Cells[Mapping[columnsMaestro.MainTelephone] + i].Text,cWs.Cells[Mapping[columnsMaestro.GeneralEmail] + i].Text);
                    cont = tempCust.Contacts.Where(x => x.Area == "Recepción").First();
                    cont = updateContact(cont, "Recepción", cWs.Cells[Mapping[columnsMaestro.ContactName] + i].Text, cWs.Cells[Mapping[columnsMaestro.ContactTelephone] + i].Text, "");
                }

                if (nuevo)
                    db.Customers.Add(tempCust);

                if (i % commitCount == 0)
                {
                    stReport.Remark = ((int)((double)((double)i / cWs.Dimension.End.Row) * 100)) + "%";
                    db.SaveChanges();
                    db.Dispose();
                    db = new BauhausEntities();
                }
            }
            stReport.Status = 0;
            return 0;
        }


        /// <summary>
        /// Updates given Contact with Data, if Names differ will retrieve from DB new contact or create it.
        /// Returning updated or new Contact.
        /// </summary>
        /// <param name="cont">Contact to be updated</param>
        /// <param name="area">new Contact Area</param>
        /// <param name="name">new Contact Name</param>
        /// <param name="tel">new Contact Telephone</param>
        /// <param name="mail">new Contact Mail</param>
        /// <returns>Returns Updated Contact or Null if new Name is Empty</returns>
        public Contact updateContact(Contact cont, String area, String name, String tel, String mail)
        {
            if (!String.IsNullOrWhiteSpace(name))
            {
                if(cont == null)
                {
                    cont = new Contact();
                }

                if (cont.Name != name)
                {
                    Contact auxCont = db.Contacts.Where(x => x.Name == name).FirstOrDefault();
                    if (auxCont != null)
                    {
                        if (auxCont.Telephone != tel && !String.IsNullOrWhiteSpace(tel))
                            auxCont.Telephone = tel;
                        if (auxCont.Email != mail && !String.IsNullOrWhiteSpace(mail))
                            auxCont.Email = mail;
                    }
                    else
                    {
                        auxCont = new Contact();
                        auxCont.Area = area;
                        auxCont.Name = name;
                        auxCont.Telephone = tel;
                        auxCont.Email = mail;
                        db.Contacts.Add(auxCont);
                    }
                    return auxCont;
                }
                else
                {
                    if (cont.Telephone != tel && !String.IsNullOrWhiteSpace(tel))
                        cont.Telephone = tel;
                    if (cont.Email != mail && !String.IsNullOrWhiteSpace(mail))
                        cont.Email = mail;
                    return cont;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Handles the processing of "Ordenes en Mano" Report form BDC.
        /// </summary>
        /// <param name="cWs">Current WorkSheet</param>
        /// <param name="stReport">Report to be Processed</param>
        /// <returns>Error Count while Saving</returns>
        private int ProcessOrdenesEnMano(ExcelWorksheet cWs, Report stReport)
        {
            int commitCount = 350;
            Dictionary<String, String> Mapping = MapReport(cWs);
            System.Diagnostics.Debug.WriteLine("Ordenes en Mano");
            OEM columnsOEM = (cWs.Name.ToLower() == "pendiente") ? new OEM(true) : new OEM(false);
            String errors = columnsOEM.SelfCheck(Mapping);
            // Check for missing Columnsf
            if (!String.IsNullOrEmpty(errors))
            {
                System.Diagnostics.Debug.WriteLine("Aborting process due to missing columns");
                LogError("OEM", 0, "Missing Columns", errors);
                stReport.Remark = "An error ocurred, missing columns: " + errors;
                return 1;
            }



            for (int i = 2; i <= cWs.Dimension.End.Row; i++)
            {
                long ordN;
                if (long.TryParse(cWs.Cells[Mapping[columnsOEM.OrderNumber] + i].Text, out ordN))
                {
                    Order auxOrder = db.Orders.Find(ordN);
                    System.Diagnostics.Debug.WriteLine("Order " + i);
                    // Order Found
                    if (auxOrder != null)
                    {
                        // Update Route
                        String routeName = cWs.Cells[Mapping[columnsOEM.Route] + i].Text;
                        if (auxOrder.Customer.Route == null || auxOrder.Customer.Route != null && auxOrder.Customer.Route.Name != routeName)
                        {
                            auxOrder.Customer.Route = db.Routes.Where(route => route.Name == routeName).FirstOrDefault();
                        }

                        // Update Vehicle Type
                        if (auxOrder.VehicleType != cWs.Cells[Mapping[columnsOEM.VehicleType] + i].Text)
                        {
                            auxOrder.VehicleType = cWs.Cells[Mapping[columnsOEM.VehicleType] + i].Text;
                        }

                        string shipStatus = RemoveSimbols(RemoveDiacritics(cWs.Cells[Mapping[columnsOEM.ShipmentStatus] + i].Text.ToLower()), 1);

                        // Skip Iteration if Order is already invoiced.
                        if (auxOrder.Status.Code > 40)
                            continue;


                        // Allocate Shipment Status
                        switch (shipStatus)
                        {
                            case "planificado":

                                System.Diagnostics.Debug.WriteLine("Planificado");
                                long shpN;
                                if (long.TryParse(cWs.Cells[Mapping[columnsOEM.ShipmentNumber] + i].Text, out shpN))
                                {

                                    //New Shipment (Didnt Find it or Null from before.)
                                    if (auxOrder.Shipment == null || auxOrder.Shipment.ID != shpN)
                                    {
                                        auxOrder.Shipment = db.Shipments.Find(shpN);
                                        if (auxOrder.Shipment == null)
                                        {
                                            auxOrder.Shipment = new Shipment();
                                            auxOrder.Shipment.ID = shpN;
                                            auxOrder.Shipment.Date = DateTime.Today;
                                        }

                                    }


                                    // Check Turn
                                    //String turn = cWs.Cells[Mapping[columnsOEM.Turn] + i].Text;

                                    //if (String.IsNullOrEmpty(auxOrder.Shipment.Turn) || auxOrder.Shipment.Turn != turn)
                                    //{

                                    //    auxOrder.Shipment.Turn = turn;
                                    //    auxOrder.Shipment.Date = (turn != "T1" && turn != "T2" && turn != "T3" && turn != "" && turn != "0") ?
                                    //        DateTime.Today.AddDays(1) : DateTime.Today;
                                    //}

                                    // Initialize Order List
                                    if (auxOrder.Shipment.Orders == null)
                                    {
                                        auxOrder.Shipment.Orders = new List<Order> { };
                                    }

                                    // Add Order To List
                                    if (!auxOrder.Shipment.Orders.Contains(auxOrder))
                                    {
                                        auxOrder.Shipment.Orders.Add(auxOrder);
                                    }

                                    //Assign Carrier
                                    String carrName = cWs.Cells[Mapping[columnsOEM.CarrierName] + i].Text;

                                    if (auxOrder.Shipment.Carrier == null ||
                                        RemoveSimbols(RemoveDiacritics(auxOrder.Shipment.Carrier.Name.ToLower()), 0) != RemoveSimbols(RemoveDiacritics(carrName.ToLower()), 0))
                                    {
                                        string newCarr = RemoveSimbols(RemoveDiacritics(carrName.ToLower()), 0);

                                        auxOrder.Shipment.Carrier = (from x in db.Carriers
                                                                     where x.Name.ToLower() == newCarr
                                                                     select x).FirstOrDefault();
                                    }

                                    // Calculate CarryFee
                                    if (String.IsNullOrEmpty(auxOrder.Shipment.CarryFee) &&
                                        !String.IsNullOrEmpty(auxOrder.VehicleType) &&
                                        auxOrder.Customer.Route != null &&
                                        auxOrder.Shipment.Carrier != null)
                                    {
                                        auxOrder.Shipment.CarryFee = (from x in auxOrder.Shipment.Carrier.CarryFees
                                                                      where x.Route == auxOrder.Customer.Route.Name &&
                                                                      x.VehicleType == auxOrder.VehicleType
                                                                      select x.Cost).FirstOrDefault();
                                    }

                                    auxOrder.Status.Stage = 2;
                                    auxOrder.Status.State = 0;
                                    auxOrder.Status.Reason = 1;
                                }
                                break;

                            case "confirmaciondecita":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 1;
                                break;

                            case "vfr80":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 8;
                                break;

                            case "pendienteporeliminar":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 10;
                                break;

                            case "capacidaddecliente":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 2;
                                break;

                            case "vfr80makro":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 9;
                                break;

                            case "ordenminimamakro":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 7;
                                break;

                            case "ordenminima":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 6;
                                break;

                            case "zsplit":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 11;
                                break;

                            case "pedidopospuesto":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 3;
                                break;

                            case "vehiculo":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 4;
                                break;

                            case "pendiente":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 0;
                                auxOrder.Status.Reason = 0;
                                break;

                            case "faltadeinventario":
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 5;
                                break;

                            default:
                                LogError("OEM", i, "Reason not Recognized", "The reason : " + cWs.Cells[Mapping[columnsOEM.ShipmentStatus] + i].Text + " is not registered");
                                auxOrder.Status.Comment = cWs.Cells[Mapping[columnsOEM.ShipmentStatus] + i].Text;
                                auxOrder.Status.Stage = 1;
                                auxOrder.Status.State = 1;
                                auxOrder.Status.Reason = 0;
                                break;
                        }

                    }

                    if (i % commitCount == 0)
                    {
                        stReport.Remark = " " + ((int)((double)((double)i / cWs.Dimension.End.Row) * 100)) + "%";
                        db.SaveChanges();
                        db.Dispose();
                        db = new BauhausEntities();
                    }
                }
            }
            stReport.Status = 1;
            return 0;
        }


        /// <summary>
        /// Stores in DB orders contained in ZSKU report.
        /// </summary>
        /// <param name="cWs">Current Worcksheet</param>
        /// <param name="stReport">Report to be Processed</param>
        /// <returns>0 if report was processed correctly else error code.</returns>
        private int ProcessZsku(ExcelWorksheet cWs, Report stReport)
        {
            int commitCount = 300;
            int errorCount = 0;


            ReportStructure columns = new ReportStructure();
            columns.ZSKU();
            ///////////////////////////////////////////////////
            // Iterate on all Orders
            for (int i = 2; i <= cWs.Dimension.End.Row; i += 2)
            {
                // Variable Declarations
                CultureInfo culture = new CultureInfo("en-US");
                Order tempOrder = null;
                Customer tempCustomer = null;
                long custNumber = 0;
                int tempStatus = 0;

                System.Diagnostics.Debug.WriteLine("Order " + i);
                // Check if Null Order.
                if (string.IsNullOrWhiteSpace(cWs.Cells[columns.OrderNumber + i].Text))
                    break;
                // Parse Order Number
                long ordNumber = Int64.Parse(cWs.Cells[columns.OrderNumber + i].Text);


                System.Diagnostics.Debug.WriteLine("Seeing Order: " + ordNumber);

                // Look for Customer

                tempCustomer = (long.TryParse(cWs.Cells[columns.ShipTo + i].Text, out custNumber)) ? db.Customers.Find(custNumber) : null;

                // Customer Found
                if (tempCustomer != null)
                {
                    if (tempCustomer.Orders.Any())
                    {
                        // Look for Order in Client.
                        tempOrder = (from x in tempCustomer.Orders
                                     where x.SapID == ordNumber
                                     select x).FirstOrDefault();
                    }

                    //Order Found
                    if (tempOrder != null)
                    {
                        tempStatus = int.Parse(cWs.Cells[columns.Status + i].Text);
                        // Status Greater Than Old.
                        if (tempStatus > tempOrder.Status.Code)
                        {
                            System.Diagnostics.Debug.WriteLine("New Status > Old Status");
                            errorCount += ReplaceOrderZSKU(cWs, i, tempOrder, columns);
                            // Same Report
                            if (stReport.ReportID == tempOrder.Status.Report)
                            {
                                tempOrder.Status.OpenItem = true;
                            }
                            // Other Report
                            else
                            {
                                tempOrder.Status.Report = stReport.ReportID;
                                tempOrder.Status.OpenItem = false;
                            }
                        }
                        else
                        {// Same Status
                            bool newInvoice = false;

                            System.Diagnostics.Debug.WriteLine("New Status <= Old Status");
                            if (tempStatus == 50)
                            {
                                // New Invoice Registered
                                if (CheckInvoices(cWs, i, tempOrder, columns))
                                {
                                    System.Diagnostics.Debug.WriteLine("Invoice Added");
                                    newInvoice = true;
                                }
                            }
                            if (stReport.ReportID == tempOrder.Status.Report && !newInvoice)
                                tempOrder.Status.OpenItem = true;
                            else
                            {
                                tempOrder.Status.Report = stReport.ReportID;
                                tempOrder.Status.OpenItem = false;
                            }
                        }
                    }
                    // Order not Found
                    else
                    { // Register Order
                        System.Diagnostics.Debug.WriteLine("Creating New Order");
                        tempOrder = RegisterOrderZSKU(cWs, i, columns, tempCustomer);
                        if (tempOrder != null)
                        {
                            tempOrder.Status.Report = stReport.ReportID;
                        }
                        else
                        {
                            errorCount += 1;
                        }
                    }
                }
                // Customer not Found
                else
                {

                    if (RegisterClient(cWs, i, columns))
                    {
                        System.Diagnostics.Debug.WriteLine("New Client Registered");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed Adding a new Client");
                    }

                }

                if (i % commitCount == 0)
                {
                    System.Diagnostics.Debug.WriteLine("/////////////////////////////////////////");
                    stReport.Remark = ((int)((double)((double)i / cWs.Dimension.End.Row) * 100)) + "%";
                    db.Entry(stReport).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null)
                        {
                            if (ex.InnerException.InnerException != null)
                            {
                                LogError("ZSKU", i, "SQL", "Trying to add Order already in Database.");
                                long badOrder;
                                if (long.TryParse(Regex.Match(ex.InnerException.InnerException.Message, @"\d+").Value, out badOrder))
                                {
                                    db.Dispose();
                                    db = new BauhausEntities();
                                    Order ord = db.Orders.Find(badOrder);
                                    if (ord != null)
                                    {
                                        db.Orders.Remove(ord);
                                        db.SaveChanges();
                                        i -= commitCount;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }

                    db.Dispose();
                    db = new BauhausEntities();
                }

            }

            return errorCount;
        }


        /// <summary>
        /// Takes an Order wich is not in DB and register all data associated to the report structure.
        /// </summary>
        /// <param name="cWs">Current Excel Work Sheet</param>
        /// <param name="i">Report Line where the order is</param>
        /// <param name="columns">Report Structure being analysed</param>
        /// <param name="tempCustomer">Customer to be entitled with the order</param>
        /// <returns>New Order registered</returns>
        private Order RegisterOrderZSKU(ExcelWorksheet cWs, int i, ReportStructure columns, Customer tempCustomer)
        {
            try
            {
                // New Structure
                Order tempOrder = new Order();
                CultureInfo culture = new CultureInfo("en-US");
                int auxInt;
                if (Int32.TryParse(cWs.Cells[columns.OrderNumber + i].Text, out auxInt))
                    tempOrder.SapID = auxInt;
                else
                {
                    LogError("ZSKU", i, "NumberFormat", "Couldn't Parse Order Sap Number");
                    return null;
                }
                System.Diagnostics.Debug.WriteLine("Registering Order: " + tempOrder.SapID);
                DateTime auxDate;
                if (DateTime.TryParseExact(
                                cWs.Cells[columns.OrderDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                    tempOrder.DocDate = auxDate;
                else
                {
                    LogError("ZSKU", i, "DateFormat", "Couldn't Parse Document Date");
                    return null;
                }
                tempOrder.Type = (!String.IsNullOrWhiteSpace(cWs.Cells[columns.OrderType + i].Text)) ?
                    cWs.Cells[columns.OrderType + i].Text : "Manual";
                tempOrder.CustomerPO = cWs.Cells[columns.CustomerPONumber + i].Text;
                tempOrder.PayTerm = cWs.Cells[columns.PaymentTerm + i].Text;

                // Add Status
                tempOrder.Status = new Status();
                if (Int32.TryParse(cWs.Cells[columns.Status + i].Text, out auxInt))
                    tempOrder.Status.Code = auxInt;
                else
                {
                    LogError("ZSKU", i, "NumberFormat", "Couldn't Parse Order Status");
                    return null;
                }
                switch (tempOrder.Status.Code)
                {
                    case 10:
                        tempOrder.Status.Stage = 0;
                        tempOrder.Status.State = 1;
                        tempOrder.Status.Reason = 1;
                        break;
                    case 20:
                        tempOrder.Status.Stage = 0;
                        tempOrder.Status.State = 0;
                        tempOrder.Status.Reason = 0;
                        break;
                    case 30:
                        if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.ShipmentNumber + i].Text))
                        {
                            tempOrder.Status.Stage = 2;
                            tempOrder.Status.State = 0;
                            tempOrder.Status.Reason = 0;
                        }
                        else
                        {
                            tempOrder.Status.Stage = 1;
                            tempOrder.Status.State = 0;
                            tempOrder.Status.Reason = 0;
                        }

                        break;
                    case 40:
                        tempOrder.Status.Stage = 2;
                        tempOrder.Status.State = 0;
                        tempOrder.Status.Reason = 0;
                        break;
                    case 50:
                        tempOrder.Status.Stage = 3;
                        tempOrder.Status.State = 0;
                        tempOrder.Status.Reason = 0;
                        break;
                    default:
                        break;
                }

                tempOrder.Status.OpenItem = false;

                System.Diagnostics.Debug.WriteLine("Adding Customer : " + cWs.Cells[columns.ShipTo + i].Text + " to new Order ");
                // Add Customer
                tempOrder.Customer = tempCustomer;
                tempOrder.Quantities = new Quantity();

                //Auxiliar Variables to hold double values on TryParse
                Double aux1, aux2;

                tempOrder.Quantities.QtyCS = (Double.TryParse(cWs.Cells[columns.OrderQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                tempOrder.Quantities.QtySU = (Double.TryParse(cWs.Cells[columns.OrderQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                tempOrder.RDDF = new RDDF();
                if (DateTime.TryParseExact(cWs.Cells[columns.RDDF + i].Text,
                    "dd'.'MM'.'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out auxDate))
                {
                    tempOrder.RDDF.Original = auxDate;
                    tempOrder.RDDF.DSSDate = auxDate;
                }


                else
                {
                    LogError("ZSKU", i, "DateFormat", "Couldn't Parse RDDF");
                    return null;
                }

                tempOrder.Quantities.NetWeight = (Double.TryParse(cWs.Cells[columns.OrderWeight + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                tempOrder.Quantities.Volume = (Double.TryParse(cWs.Cells[columns.OrderVolume + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                // Delivery
                if (cWs.Cells[columns.DeliveryNumber + i].Text != "")
                {
                    tempOrder.Delivery = new Delivery();
                    tempOrder.Delivery.ID = Int32.Parse(cWs.Cells[columns.DeliveryNumber + i].Text);
                    System.Diagnostics.Debug.WriteLine("With Delivery :" + tempOrder.Delivery.ID);
                    if (DateTime.TryParseExact(
                                cWs.Cells[columns.DeliveryDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                        tempOrder.Delivery.Date = auxDate;
                    else
                    {
                        LogError("ZSKU", i, "DateFormat", "Couldn't Parse Delivery Date");
                        return null;
                    }
                    tempOrder.Delivery.Quantities = new Quantity();
                    tempOrder.Delivery.Quantities.QtyCS = (Double.TryParse(cWs.Cells[columns.DeliveryQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                    tempOrder.Delivery.Quantities.QtySU = (Double.TryParse(cWs.Cells[columns.DeliveryQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                    //Register DSS suggested RDDF
                    if (tempOrder.Customer.Route != null && tempOrder.Customer.Route.LeadTime != null)
                        tempOrder.RDDF.DSSDate = tempOrder.Delivery.Date.AddBusinessDays(tempOrder.Customer.Route.LeadTime.Days);
                }

                // Shipment
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.ShipmentNumber + i].Text))
                {
                    long ShpN;

                    if (long.TryParse(cWs.Cells[columns.ShipmentNumber + i].Text, out ShpN))
                    {
                        tempOrder.Shipment = db.Shipments.Find(ShpN);

                        if (tempOrder.Shipment == null)
                        {
                            tempOrder.Shipment = new Shipment();
                            tempOrder.Shipment.ID = ShpN;
                            if (DateTime.TryParseExact(
                                cWs.Cells[columns.ShipmentDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                                tempOrder.Shipment.Date = auxDate;
                            else
                            {
                                LogError("ZSKU", i, "DateFormat", "Couldn't Parse Shipment Date");
                                return null;
                            }
                            long CarrN;
                            tempOrder.Shipment.Carrier = (long.TryParse(cWs.Cells[columns.CarrierNumber + i].Text, out CarrN)) ?
                                db.Carriers.Find(CarrN) : null;
                            tempOrder.Shipment.Turn = cWs.Cells[columns.Turn + i].Text;
                            tempOrder.Shipment.TransitData = new List<Input> { };
                            tempOrder.Shipment.Orders = new List<Order> { };
                            tempOrder.Shipment.Orders.Add(tempOrder);
                            tempOrder.Shipment.OrdersToGo = 1;
                            // Add to DB.
                            db.Shipments.Add(tempOrder.Shipment);


                        }
                        else
                        {// Add Order to shipment list
                            if (!tempOrder.Shipment.Orders.Contains(tempOrder))
                            {
                                tempOrder.Shipment.Orders.Add(tempOrder);
                                tempOrder.Shipment.OrdersToGo += 1;
                            }
                        }

                        // Update Order Carry Fee.
                        // WORK WITH CS VOLUME AND WEIGHT TO DETERMINE WHICH KIND OF TRUCK IS.
                    }
                }

                // Invoice
                tempOrder.Invoices = new List<Invoice> { };
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.InvoiceNumber + i].Text))
                {
                    Invoice invo = new Invoice();
                    invo.ID = Int64.Parse(cWs.Cells[columns.InvoiceNumber + i].Text);
                    if (DateTime.TryParseExact(
                                cWs.Cells[columns.InvoiceDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                        invo.Date = auxDate;
                    else
                    {
                        LogError("ZSKU", i, "DateFormat", "Couldn't Parse Invoice Date");
                        return null;
                    }
                    invo.QtyCS = (Double.TryParse(cWs.Cells[columns.DeliveryQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                    invo.QtySU = (Double.TryParse(cWs.Cells[columns.DeliveryQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;
                    tempOrder.Invoices.Add(invo);
                }

                tempOrder.VehicleType = cWs.Cells[columns.VehicleType + i].Text;

                // Register POD
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.PODDate + i].Text))
                {
                    tempOrder.POD = new POD();
                    if (DateTime.TryParseExact(
                                cWs.Cells[columns.PODDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                        tempOrder.POD.Date = auxDate;
                    else
                    {
                        LogError("ZSKU", i, "DateFormat", "Couldn't Parse POD Date");
                        return null;
                    }

                    //Finish Shipment
                    if (tempOrder.Shipment != null)
                        tempOrder.Shipment.OrdersToGo -=1;

                    // UPdate Status
                    tempOrder.Status.Stage = 5;
                    tempOrder.Status.State = 0;
                    tempOrder.Status.Reason = 0;

                    // KPI Generation
                    if (tempOrder.calculateIndicators() != 0)
                        LogError("ZSKU", i, "Error", "Failed to Calculate Indicators");
                    
                }
                db.Orders.Add(tempOrder);

                return tempOrder;

            }
            catch (Exception e)
            {
                LogError("ZSKU", i, "Critical", e.ToString());
                return null;
            }
        }

        /// <summary>
        /// Calculates CSOT value for given order comparing POD and Original RDDF Date
        /// </summary>
        /// <param name="ord">Order to calculate CSOT to</param>
        /// <returns>True if process completed succesfully. false otherwise</returns>
        public bool CalculateCSOT(Order ord)
        {

            if (ord.POD.CSOT == null)
            {
                ord.POD.CSOT = new Indicator();
                if (ord.POD.Date <= ord.RDDF.DSSDate)   // Inside Time Window.
                {
                    ord.POD.CSOT.Value = true; // HIT
                    ord.POD.CSOT.Confirmed = true;
                }
                else
                {
                    ord.POD.CSOT.Value = false; // MISS
                }
                System.Diagnostics.Debug.WriteLine("CSOT " + ord.POD.CSOT);
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("CSOT Failed");
                return false;
            }

        }

        /// <summary>
        /// Calculates OT value for given order comparing POD and Original RDDF Date
        /// </summary>
        /// <param name="ord">Order to calculate CSOT to</param>
        /// <returns>True if process completed succesfully. false otherwise</returns>
        public bool CalculateOT(Order ord)
        {
            if (ord.POD.OT == null)
            {
                ord.POD.OT = new Indicator();

                if (ord.POD.Date <= ord.RDDF.DSSDate || // Inside Time Window
                    ord.Quantities.QtyCS < 200 || // Small Orders
                    (ord.POD.Date.Day - ord.RDDF.DSSDate.Day < 4 && // Weekend and Time Window < 4
                    ord.POD.Date.DayOfWeek == DayOfWeek.Saturday || ord.POD.Date.DayOfWeek == DayOfWeek.Sunday))
                {
                    ord.POD.OT.Value = true;
                    ord.POD.OT.Confirmed = true;
                }
                else
                {
                    ord.POD.OT.Value = false;
                }
                System.Diagnostics.Debug.WriteLine("OT " + ord.POD.OT);
                return true;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("OT Failed");
                return false;
            }

        }

        /// <summary>
        /// Counts totals and Updates CSOT Values.
        /// </summary>
        public void RegisterCSOT()
        {

            DateTime today = DateTime.Today;
            DateTime first = new DateTime(today.Year, today.Month, 1);
            DateTime last = first.AddMonths(1);

            int Total = (from x in db.Orders
                         where x.POD != null &&
                         first <= x.POD.Date &&
                         x.POD.Date <= last
                         select x).Count();



            int TotalHits = (from x in db.Orders
                             where x.POD != null &&
                             x.POD.CSOT != null &&
                             x.POD.CSOT.Value == true &&
                             first <= x.POD.Date &&
                             x.POD.Date <= last
                             select x).Count();

            DateTime helper = DateTime.Today;

            HistIndicator CSOT = (from x in db.HistIndicators
                                  where x.Name == "CSOT" &&
                                  x.Date == helper
                                  select x).FirstOrDefault();
            if (CSOT == null)
            {
                CSOT = new HistIndicator();
                CSOT.Name = "CSOT";
                CSOT.Date = DateTime.Today;
                db.HistIndicators.Add(CSOT);
            }

            if (Total != 0)
                CSOT.Value = Math.Round(((double)((double)TotalHits / Total) * 100), 2);
            else
            {
                LogError("CSOT Update Calculation", 0, "Critical", "There are 0 delivered Orders.");
                CSOT.Value = 0;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RegisterOT()
        {
            DateTime today = DateTime.Today;
            DateTime first = new DateTime(today.Year, today.Month, 1);
            DateTime last = first.AddMonths(1);

            int Total = (from x in db.Orders
                         where x.POD != null &&
                         first <= x.POD.Date &&
                         x.POD.Date <= last
                         select x).Count();



            int TotalHits = (from x in db.Orders
                             where x.POD != null &&
                             x.POD.OT != null &&
                             x.POD.OT.Value == true &&
                             first <= x.POD.Date &&
                             x.POD.Date <= last
                             select x).Count();

            DateTime helper = DateTime.Today;

            HistIndicator OT = (from x in db.HistIndicators
                                where x.Name == "OT" &&
                                x.Date == helper
                                select x).FirstOrDefault();
            if (OT == null)
            {
                OT = new HistIndicator();
                OT.Name = "OT";
                OT.Date = DateTime.Today;
                db.HistIndicators.Add(OT);
            }

            if (Total != 0)
                OT.Value = Math.Round(((double)((double)TotalHits / Total) * 100), 2);
            else
            {
                LogError("OT Update Calculation", 0, "Critical", "There are 0 delivered Orders.");
                OT.Value = 0;
            }

            db.SaveChanges();
        }

        /// <summary>
        /// Calculate all KPIs with current HIT/MISS Order Values.
        /// </summary>
        private void CalculateKPIs()
        {
            RegisterCSOT();
            RegisterOT();
            //KPIsAnalysis();
        }
        /// <summary>
        /// Loops over orders Analysing MISS values on CSOT and OT
        /// </summary>
        private void KPIsAnalysis()
        {
            IEnumerable<Order> orders = (from x in db.Orders
                                         where x.POD != null &&
                                         x.POD.CSOT == null ||
                                         x.POD.OT == null ||
                                         x.POD.CSOT != null &&
                                         x.POD.CSOT.Value == false ||
                                         x.POD.OT != null &&
                                         x.POD.OT.Value == false
                                         select x);
            if (orders != null)
            {
                foreach (Order ord in orders)
                {
                    //CSOT
                    if (ord.POD.CSOT == null)
                    {
                        ord.POD.CSOT = new Indicator();
                        if (ord.POD.Date <= ord.RDDF.Original)   // Inside Time Window.
                        {
                            ord.POD.CSOT.Value = true; // HIT
                            ord.POD.CSOT.Confirmed = true;
                        }
                        else
                        {
                            ord.POD.CSOT.Value = false; // MISS
                        }

                    }
                    // CSOT MISS Analysis
                    if (ord.POD.CSOT.Value == false)
                    {
                        // Check for any problems reported on transit.
                        if (ord.Shipment.TransitData.Any())
                        {
                            IEnumerable<Input> inputs = ord.Shipment.TransitData.Where(x => x.State == 1);
                            if (inputs != null)
                            {
                                CodeHelpers ch1 = new CodeHelpers();
                                // Concatenate Transit Issues as reasons.
                                foreach (Input inp in inputs)
                                {
                                    ord.POD.CSOT.Reason += " " + ch1.ReasonResolver(inp.Stage, inp.State, inp.Reason);
                                }
                                ord.POD.CSOT.Confirmed = true;
                            }
                            else
                            {
                                // Associate Order Comments as reasons for delay
                                if (!String.IsNullOrWhiteSpace(ord.Status.Comment))
                                {
                                    ord.POD.CSOT.Reason = ord.Status.Comment;
                                }
                                // Reason Unknow.
                                else
                                    ord.POD.CSOT.Reason = "Unknown Reason";
                            }
                        }
                    }

                    //OT
                    if (ord.POD.OT == null)
                    {
                        ord.POD.OT = new Indicator();
                        if (ord.POD.Date <= ord.RDDF.DSSDate || // Inside Time Window
                            ord.Quantities.QtyCS < 200 || // Small Orders
                            (ord.POD.Date.Day - ord.RDDF.DSSDate.Day < 4 && // Weekend and Time Window < 4
                            ord.POD.Date.DayOfWeek == DayOfWeek.Saturday || ord.POD.Date.DayOfWeek == DayOfWeek.Sunday))
                        {
                            ord.POD.OT.Value = true;
                            ord.POD.OT.Confirmed = true;
                        }
                        else
                        {
                            ord.POD.OT.Value = false;
                        }
                    }
                    // OT MISS Analysis
                    if (ord.POD.OT.Value == false)
                    {
                        // Check for any problems reported on transit.
                        if (ord.Shipment.TransitData.Any())
                        {
                            IEnumerable<Input> inputs = ord.Shipment.TransitData.Where(x => x.State == 1);
                            if (inputs != null)
                            {
                                CodeHelpers ch = new CodeHelpers();
                                // Concatenate Transit Issues as reasons.
                                foreach (Input inp in inputs)
                                {
                                    ord.POD.OT.Reason += " " + ch.ReasonResolver(inp.Stage, inp.State, inp.Reason);
                                }
                                ord.POD.OT.Confirmed = true;
                            }
                            else
                            {
                                // Associate Order Comments as reasons for dekay
                                if (!String.IsNullOrWhiteSpace(ord.Status.Comment))
                                {
                                    ord.POD.OT.Reason = ord.Status.Comment;
                                }
                                // Reason Unknow.
                                else
                                    ord.POD.OT.Reason = "Unknown Reason";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cWs"></param>
        /// <param name="i"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private bool RegisterClient(ExcelWorksheet cWs, int i, ReportStructure columns)
        {
            return false;
        }

        /// <summary>
        /// Checks if the invoice number of the current order is already registered or needs to be added
        /// </summary>
        /// <param name="cWs">Current Work Sheet</param>
        /// <param name="i">Report Line of Current Order</param>
        /// <param name="tempOrder">Order Being Checked</param>
        /// <param name="columns">Report Structure being analysed</param>
        /// <returns>True if a new invoice have been added to the order, false otherwise</returns>
        private bool CheckInvoices(ExcelWorksheet cWs, int i, Order tempOrder, ReportStructure columns)
        {
            CultureInfo culture = new CultureInfo("en-US");
            Invoice tempInvoice = null;
            bool added = false;
            long invID = long.Parse(cWs.Cells[columns.InvoiceNumber + i].Text);

            if (tempOrder.Invoices.Any())
            {
                tempInvoice = (from x in tempOrder.Invoices
                               where x.ID == invID
                               select x).FirstOrDefault();
            }

            if (tempInvoice == null)
            {
                tempInvoice = new Invoice();
                tempInvoice.ID = long.Parse(cWs.Cells[columns.InvoiceNumber + i].Text);
                DateTime auxDate;
                if (DateTime.TryParseExact(
                                cWs.Cells[columns.InvoiceDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                    tempInvoice.Date = auxDate;
                else
                {
                    LogError("ZSKU", i, "Invoice DateFormat", "Date could not be parsed, check report data.");
                    return false;
                }


                Double aux1, aux2;
                tempInvoice.QtyCS = (Double.TryParse(cWs.Cells[columns.DeliveryQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                tempInvoice.QtySU = (Double.TryParse(cWs.Cells[columns.DeliveryQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;
                tempOrder.Invoices.Add(tempInvoice);
                added = true;
            }

            return added;

        }


        /// <summary>
        /// Replaces all order data updating or downgrading depending on fields present.
        /// </summary>
        /// <param name="cWs">Current Work Sheet</param>
        /// <param name="i">Current Row</param>
        /// <param name="ord">Order to be replaced</param>
        /// <param name="columns">Report Structure being analised</param>
        /// <returns>0 if succesfully replaced order, 1 if error ocurred</returns>
        private int ReplaceOrderZSKU(ExcelWorksheet cWs, int i, Order ord, ReportStructure columns)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Replacing Order: " + i);
                CultureInfo culture = new CultureInfo("en-US");

                // Add Status
                ord.Status.Code = Int32.Parse(cWs.Cells[columns.Status + i].Text);
                switch (ord.Status.Code)
                {
                    case 10:
                        ord.Status.Stage = 0;
                        ord.Status.State = 1;
                        ord.Status.Reason = 1;
                        break;
                    case 20:
                        ord.Status.Stage = 0;
                        ord.Status.State = 0;
                        ord.Status.Reason = 0;
                        break;
                    case 30:
                        if (ord.Status.Stage < 1)
                        {
                            ord.Status.Stage = 1;
                            ord.Status.State = 0;
                            ord.Status.Reason = 0;
                        }
                        if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.ShipmentNumber + i].Text))
                        {
                            ord.Status.Stage = 2;
                            ord.Status.State = 0;
                            ord.Status.Reason = 0;
                        }
                        break;
                    case 40:
                        goto case 30;
                    case 50:
                        if (ord.Status.Stage < 3)
                        {
                            ord.Status.Stage = 3;
                            ord.Status.State = 0;
                            ord.Status.Reason = 0;
                        }
                        break;
                    default:
                        break;
                }
                Double aux1, aux2;
                ord.Quantities.QtyCS = (Double.TryParse(cWs.Cells[columns.OrderQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                ord.Quantities.QtySU = (Double.TryParse(cWs.Cells[columns.OrderQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                DateTime auxDate;
                if (DateTime.TryParseExact(cWs.Cells[columns.RDDF + i].Text, "dd'.'MM'.'yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out auxDate))
                    ord.RDDF.Original = auxDate;
                else
                {
                    LogError("ZSKU", i, "DateFormat", "Couldn't Parse RDDF");
                    return 1;
                }

                ord.Quantities.NetWeight = (Double.TryParse(cWs.Cells[columns.OrderWeight + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                ord.Quantities.Volume = (Double.TryParse(cWs.Cells[columns.OrderVolume + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                // Delivery
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.DeliveryNumber + i].Text))
                {
                    if (ord.Delivery == null)
                    {
                        ord.Delivery = new Delivery();
                        ord.Delivery.ID = long.Parse(cWs.Cells[columns.DeliveryNumber + i].Text);
                    }

                    if (DateTime.TryParseExact(
                        cWs.Cells[columns.DeliveryDate + i].Text, "dd'.'MM'.'yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeLocal,
                        out auxDate))
                        ord.Delivery.Date = auxDate;
                    else
                    {
                        LogError("ZSKU", i, "DateFormat", "Couldn't Parse Delivery Date");
                        return 1;
                    }
                    ord.Delivery.Quantities = new Quantity();

                    ord.Delivery.Quantities.QtyCS = (Double.TryParse(cWs.Cells[columns.DeliveryQtyCS + i].Text, NumberStyles.Number, culture, out aux1)) ? aux1 : 0;
                    ord.Delivery.Quantities.QtySU = (Double.TryParse(cWs.Cells[columns.DeliveryQtySU + i].Text, NumberStyles.Number, culture, out aux2)) ? aux2 : 0;

                    // Correcting DSS Default Date
                    if (ord.Customer.Route != null && ord.Customer.Route.LeadTime != null)
                        ord.RDDF.DSSDate = ord.Delivery.Date.AddBusinessDays(ord.Customer.Route.LeadTime.Days);
                }
                else
                {
                    // Order Downgraded - Remove Delivery
                    if (ord.Delivery != null)
                    {
                        db.Deliveries.Remove(ord.Delivery);
                        ord.Delivery = null;
                        ord.Status.Stage = 0;
                        ord.Status.State = 0;
                        ord.Status.Reason = 0;
                    }
                }

                // Shipment
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.ShipmentNumber + i].Text))
                {
                    long auxShp = long.Parse(cWs.Cells[columns.ShipmentNumber + i].Text);

                    if (ord.Shipment == null || ord.Shipment.ID != auxShp)
                    {
                        ord.Shipment = db.Shipments.Find(auxShp);

                        if (ord.Shipment == null)
                        {
                            ord.Shipment = new Shipment();
                            ord.Shipment.ID = auxShp;
                            if (DateTime.TryParseExact(
                                cWs.Cells[columns.ShipmentDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                                ord.Shipment.Date = auxDate;
                            else
                            {
                                LogError("ZSKU", i, "DateFormat", "Couldn't Parse Shipment Date");
                                return 1;
                            }
                            ord.Shipment.TransitData = new List<Input> { };
                            ord.Shipment.Orders = new List<Order> { };
                            ord.Shipment.OrdersToGo = 1;
                            ord.Shipment.Orders.Add(ord);
                        }
                        else
                        {
                            if (!ord.Shipment.Orders.Contains(ord))
                                ord.Shipment.OrdersToGo += 1;
                            ord.Shipment.Orders.Add(ord);
                        }
                    }

                    long auxCarr;

                    // If Carrier Number Present. Assign.
                    if (long.TryParse(cWs.Cells[columns.CarrierNumber + i].Text, out auxCarr))
                    {
                        if (ord.Shipment.Carrier == null || ord.Shipment.Carrier.ID != auxCarr)
                        {
                            ord.Shipment.Carrier = db.Carriers.Find(auxCarr);
                            if (ord.Shipment.Carrier == null)
                            {
                                ord.Shipment.Carrier = new Carrier();
                                ord.Shipment.Carrier.ID = auxCarr;
                                ord.Shipment.Carrier.Name = "Unknown";
                            }
                        }
                    }

                    // Carry Fee Assignment
                    if (ord.Shipment.CarryFee == null &&
                        !String.IsNullOrWhiteSpace(ord.VehicleType) &&
                        ord.Customer.Route != null)
                    {

                        ord.Shipment.CarryFee = (from x in ord.Shipment.Carrier.CarryFees
                                                 where x.Route == ord.Customer.Route.Name &&
                                                 x.VehicleType == ord.VehicleType
                                                 select x.Cost).FirstOrDefault();
                    }
                }

                // Invoices
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.InvoiceNumber + i].Text))
                {
                    CheckInvoices(cWs, i, ord, columns);
                }

                // Update POD
                if (!String.IsNullOrWhiteSpace(cWs.Cells[columns.PODDate + i].Text))
                {
                    if (ord.POD == null)
                    {
                        ord.POD = new POD();
                        if (DateTime.TryParseExact(
                                cWs.Cells[columns.PODDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                            ord.POD.Date = auxDate;
                        else
                        {
                            LogError("ZSKU", i, "DateFormat", "Couldn't Parse POD Date");
                            return 1;
                        }
                        if (ord.Shipment != null)
                            ord.Shipment.OrdersToGo = 0;
                        ord.Status.Stage = 5;
                        ord.Status.State = 0;
                        ord.Status.Reason = 0;
                    }
                    else
                    {
                        // Update POD if Different
                        DateTime aux = DateTime.Today;
                        if (DateTime.TryParseExact(
                                cWs.Cells[columns.PODDate + i].Text, "dd'.'MM'.'yyyy",
                                CultureInfo.InvariantCulture,
                                DateTimeStyles.AssumeLocal,
                                out auxDate))
                        {
                            ord.POD.Date = aux;
                            if (ord.POD.Date != aux)
                                ord.POD.Date = aux;
                        }
                        else
                        {
                            LogError("ZSKU", i, "DateFormat", "Couldn't Parse POD Date");
                            return 1;
                        }

                    }
                    // KPI Generation
                    if (ord.POD.CSOT == null || ord.POD.OT == null)
                    {
                        if(ord.calculateIndicators() != 0)
                        {
                            LogError("ZSKU", i, "Indicator", "Failed to Calculate Indicators");
                        }
                    }
                }

                return 0;
            }
            catch (Exception e)
            {
                LogError("ZSKU", i, e.Message, e.ToString());
                return 1;
            }
        }

        /// <summary>
        ///  Translate from plant number to plant String.
        /// </summary>
        /// <param name="plant">Plant Number</param>
        /// <returns>String Containing Plant Description</returns>

        public String TranslatePlantNumber(int plant)
        {
            string trans = "";
            switch (plant)
            {
                case 9453:
                    trans = "Barquisimeto";
                    break;
                case 3360:
                    trans = "Clover";
                    break;
                case 9468:
                    trans = "Guatire";
                    break;
                case 9876:
                    trans = "Wella";
                    break;
                default:
                    trans = plant.ToString();
                    break;
            }
            return trans;
        }

        /// <summary>
        /// Takes a Worksheet and Maps the columns contained inside to a dictionary.
        /// </summary>
        /// <param name="cWs">CurrentWorksheet</param>
        /// <returns>Dictionary containing column Name and location column</returns>
        public Dictionary<String, String> MapReport(ExcelWorksheet cWs)
        {
            Dictionary<String, String> ColumnMappings = new Dictionary<String, String>();
            for (int i = cWs.Dimension.Start.Column; i <= cWs.Dimension.End.Column; i++)
            {
                if (!String.IsNullOrWhiteSpace(cWs.Cells[1, i].Text))
                {
                    System.Diagnostics.Debug.WriteLine("Seeing " + cWs.Cells[1, i].Text + " on column " + GetExcelColumnName(i));
                    System.Diagnostics.Debug.WriteLine("Removing Diacritics " + RemoveSimbols(RemoveDiacritics(cWs.Cells[1, i].Text), 1));
                    if (!ColumnMappings.ContainsKey(cWs.Cells[1, i].Text))
                    {
                        ColumnMappings.Add(RemoveSimbols(RemoveDiacritics(cWs.Cells[1, i].Text.ToLower()), 1), GetExcelColumnName(i));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Warning, Report Contains Duplicate Column " + cWs.Cells[1, i].Text);
                        ColumnMappings.Add(RemoveSimbols(RemoveDiacritics(cWs.Cells[1, i].Text.ToLower()), 1) + "2", GetExcelColumnName(i));
                    }
                }
            }
            return ColumnMappings;
        }

        /// <summary>
        ///     Calculates columns letters based on maxColumnSize of Excel
        ///     Being a base 26 number.
        /// </summary>
        /// <param name="columnNumber">Column Position</param>
        /// <returns>Letter that represents excel column</returns>
        private string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// Remove Accentuation from words.
        /// </summary>
        /// <param name="text">text to be formated</param>
        /// <returns>String without accents</returns>
        private static string RemoveDiacritics(string text)
        {
            return string.Concat(
                text.Normalize(NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                ).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Remove simbols and spaces if requested.
        /// </summary>
        /// <param name="text">String to be Formated</param>
        /// <param name="opt">0 to keep Spaces/1 to Remove Them.</param>
        /// <returns>String formated without Simbols</returns>
        private static string RemoveSimbols(string text, int opt)
        {
            if (opt == 0)
                return Regex.Replace(text, @"[^\p{L}\p{N} ]+", "");
            else
                return Regex.Replace(text, @"[^\p{L}\p{N}]+", "");
        }

        /// <summary>
        /// Remove all orders that havent been updated by last Report.
        /// </summary>
        /// <param name="reportID">Newest Order Report</param>
        /// <returns> 0 if everything went OK</returns>
        private int CleanOrders(int reportID)
        {
            System.Diagnostics.Debug.WriteLine("Cleaning Orders");
            BauhausEntities db = new BauhausEntities();
            var orders = (from x in db.Orders
                          where x.Status.Report != reportID
                          && x.POD == null
                          select x).ToList();
            foreach (Order ord in orders.ToList())
            {
                System.Diagnostics.Debug.WriteLine("Deleting Order :" + ord.SapID);

                if (ord.Invoices.Any())
                {
                    foreach (Invoice inv in ord.Invoices.ToList())
                    {
                        db.Invoices.Remove(inv);
                        ord.Invoices.Remove(inv);
                    }

                }

                if (ord.Shipment != null)
                {
                    // Remove Inputs on transit Data
                    foreach(Input inp in ord.Shipment.TransitData.ToList())
                    {
                        ord.Shipment.TransitData.Remove(inp);
                        db.Inputs.Remove(inp);
                    }

                    ord.Shipment.Orders.Remove(ord);
                    if (!ord.Shipment.Orders.Any())
                        db.Shipments.Remove(ord.Shipment);
                    ord.Shipment = null;
                }

                if (ord.Delivery != null)
                {
                    db.Deliveries.Remove(ord.Delivery);
                    ord.Delivery = null;
                }

                db.Statuses.Remove(ord.Status);
                ord.Status = null;

                db.Orders.Remove(ord);
            }
            db.SaveChanges();
            return 0;
        }

        /// <summary>
        /// Report Execution Abortion
        /// </summary>
        /// <param name="id">Report to be Aborted</param>
        /// <returns>View Index with report list.</returns>
        [Authorize(Roles = "Admin")]
        public ActionResult Abort(int id)
        {
            System.Diagnostics.Debug.WriteLine("Aborting");
            Report stReport = db.Reports.Find(id);
            if (stReport == null || stReport.Status != 2)
            {
                TempData["Type"] = "warning";
                TempData["Message"] = "Report cant be aborted.";
                return View("Index", db.Reports.ToList().OrderBy(x => x.CreationDate));
            }

            stReport.Status = 3;
            stReport.Remark = "Aborted Execution";
            Log lg = new Log();
            lg.Source = Request.UserHostAddress;
            lg.Type = "Warning";
            lg.Description = "Aborted Execution of " + stReport.Name + " Report";
            lg.UserName = User.Identity.Name;
            lg.Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            db.Logs.Add(lg);

            if (current != null && current.IsAlive)
            {
                current.Abort();
            }

            db.SaveChanges();
            TempData["Type"] = "info";
            TempData["Message"] = "Report Aborted Successfully.";
            return View("Index", db.Reports.ToList().OrderBy(x => x.CreationDate));
        }

        /// <summary>
        /// Return report remark and status+
        /// </summary>
        /// <param name="id">Reprot ID</param>
        /// <returns>String that contains status simbol and remark</returns>
        [HttpPost]
        public ContentResult RefreshProgress(int id)
        {
            TagBuilder p = new TagBuilder("small");
            TagBuilder i = new TagBuilder("i");
            Report rp = db.Reports.Find(id);
            if (current != null && current.ThreadState == System.Threading.ThreadState.Aborted)
            {
                i.AddCssClass("fa fa-bug");
                p.InnerHtml = "Report Aborted";
            }
            else
            {
                if (rp == null)
                {
                    i.AddCssClass("fa fa-fire-extinguisher");
                    p.InnerHtml = "Report Not Available";
                }
                else
                {
                    switch (rp.Status)
                    {
                        case 0:
                            i.AddCssClass("fa fa-clock-o");
                            break;
                        case 1:
                            i.AddCssClass("fa fa-check-circle");
                            break;
                        case 2:
                            i.AddCssClass("fa fa-cog fa-spin");
                            break;
                        case 3:
                            i.AddCssClass("fa fa-exclamation-circle");
                            break;
                        default:
                            i.AddCssClass("fa fa-bug");
                            break;
                    }

                    p.InnerHtml = rp.Remark;

                }
            }
            return new ContentResult { Content = i.ToString() + p.ToString() };
        }

        /// <summary>
        /// Save Error information to Log Database.
        /// </summary>
        /// <param name="reportName">Name of report where error generated</param>
        /// <param name="line">Line of report where error generated</param>
        /// <param name="type">Error type. e.g.(Format,Missing,Critical)</param>
        /// <param name="message">Error description</param>
        private void LogError(String reportName, int line, String type, String message)
        {
            Log lg = new Log();
            lg.Source = "System";
            lg.Type = "Error";
            lg.Description = "<<" + type + "," + line + ">> " + message;
            lg.UserName = User.Identity.Name;
            lg.Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            db.Logs.Add(lg);
        }

    }
}