using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;
using WebMatrix.WebData;
using System.Web.Security;
using Bauhaus.Filters;
using System.Data.OleDb;
using System.Data.SqlClient;
using OfficeOpenXml;
using System.Drawing;
using System.Data;
using System.Globalization;
using PercentageProgressBar;
using Bauhaus.Helpers;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using System.Text;
using System.Web.Hosting;

namespace Bauhaus.Controllers
{
    public class AdminController : Controller
    {
        private BauhausEntities db = new BauhausEntities();
        //
        // GET: /Admin/

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            DateTime updated = (from x in db.Reports
                                where x.Name == "ZSKU.txt" || x.Name == "ZSKU.xlsx"
                                select x.CreationDate).ToList().LastOrDefault();
            if (updated == null)
            {
                if (db.Reports.ToList().Any())
                    updated = db.Reports.ToList().LastOrDefault().CreationDate;
                else
                    updated = DateTime.MinValue;

            }

            ViewBag.Title = "Dashboard";

            return View(updated);
        }

        [Authorize(Roles = "Admin")]
        //[InitializeSimpleMembership]
        public ActionResult ManageUsers()
        {
            ViewBag.Title = "Manage Users";
            //Initialize Iteration Structure.
            IEnumerable<UserProfile> users;
            using (BauhausEntities db = new BauhausEntities())
            {
                //Request User List From DB.
                users = db.UserProfiles.ToList();
            }

            //Pass it as a variable through ViewBag.
            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            return View(users);
        }

        [Authorize(Roles = "Admin")]
        //[InitializeSimpleMembership]
        public ActionResult ManageRoles()
        {
            ViewBag.Title = "Manage Roles";
            //Obtain all roles from Web Security
            var roles = System.Web.Security.Roles.GetAllRoles();

            return View(roles);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRole(string roleName)
        {
            if (!System.Web.Security.Roles.RoleExists(roleName))
            {
                System.Web.Security.Roles.CreateRole(roleName);
            }

            var roles = System.Web.Security.Roles.GetAllRoles();

            //Return View "Manage Roles with Local Object "roles".
            return View("ManageRoles", roles);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DeleteRole(string roleName)
        {
            if (roleName == "Admin")
            {
                return View("ManageRoles");
            }

            var users = System.Web.Security.Roles.GetUsersInRole(roleName);
            if (users.Length != 0)
                System.Web.Security.Roles.RemoveUsersFromRole(users, roleName);
            System.Web.Security.Roles.DeleteRole(roleName);
            return RedirectToAction("ManageRoles", "Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult UserToRole(string roleName, string userName)
        {
            if (!Roles.RoleExists(roleName))
                return Json(new { Status = 0, Message = "Wanted role does not exist." });
            if (!WebSecurity.UserExists(userName))
                return Json(new { Status = 0, Message = "User does not exist." });

            String[] roles = System.Web.Security.Roles.GetRolesForUser(userName);
            if (roles.Length > 0)
                System.Web.Security.Roles.RemoveUserFromRoles(userName, roles);

            System.Web.Security.Roles.AddUserToRole(userName, roleName);

            //Security Log Details
            Log log = new Log(Request.UserHostAddress, User.Identity.Name, "Warning", "Assigned " + roleName + " role to user " + userName);
            db.Logs.Add(log);
            // 

            db.SaveChanges();

            return Json(new { Status = 1, Message = "Role Assigned Correctly." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult DeleteUser(string userName)
        {
            Log log = new Log(Request.UserHostAddress, User.Identity.Name, "Warning", "Deleted User '" + userName + "'");
            db.Logs.Add(log);
            db.SaveChanges();
            if (WebSecurity.UserExists(userName))
            {
                String[] roles = Roles.GetRolesForUser(userName);
                if (roles.Count() > 0)
                {
                    Roles.RemoveUserFromRoles(userName, roles);
                }
                ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(userName); // deletes record from webpages_Membership table
                ((SimpleMembershipProvider)Membership.Provider).DeleteUser(userName, true); // deletes record from UserProfile table

                return Json(new { Status = 1, Type = "info", Message = "User Removed" });
            }
            else
            {
                return Json(new { Status = 0, Type = "warning", Message = "User not Found" });
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult LogIndex()
        {
            ViewBag.Title = "Security Log";
            return View(db.Logs.ToList().OrderByDescending(log => log.Date));
        }


        /// <summary>
        /// Provides Visit Stadistics for current month in a excel File
        /// </summary>
        [Authorize(Roles = "Admin")]
        public void DownloadVisitStadistics()
        {
            ViewBag.Title = "Visit Stadistics Month " + DateTime.Today.Month;
            List<UserProfile> users = db.UserProfiles.ToList();
            Dictionary<String, int> visits = new Dictionary<string, int>();
            foreach (UserProfile user in users)
            {
                if (!visits.ContainsKey(user.UserName))
                    visits.Add(user.UserName, 0);
            }

            List<Log> logs = db.Logs.Where(x => x.Description.Contains("Logged In") &&
                                                x.Date.Month == DateTime.Today.Month
                                                ).ToList();

            foreach (Log lg in logs)
            {
                if (visits.ContainsKey(lg.UserName))
                    visits[lg.UserName] += 1;
                else
                    System.Diagnostics.Debug.WriteLine(lg.UserName + " is not present in dictionary");
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                // Create WorkSheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Visits");

                // Load Info
                ws.Cells["A1"].LoadFromCollection(visits.OrderBy(x => x.Value).ToList(), true);

                // Format Headers
                using (ExcelRange rng = ws.Cells["A1:B1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }

                ws.Cells["A1"].Value = "User";
                ws.Cells["B1"].Value = "# of Visits";


                // Write Back Response to Client
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment; filename= " + ViewBag.Title + ".xlsx");
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.End();
            }
        }


        /// <summary>
        /// Backups all Customers contacts and comments
        /// </summary>
        [Authorize(Roles="Admin")]
        public void backupContactsComments()
        {
            ViewBag.Title = "Customer 's Contacts & Comments";

            var contacts = from x in db.Customers
                           from y in x.Contacts
                           select new
                           {
                               Customer = x.ID,
                               Area = y.Area,
                               Name =y.Name,
                               Tel = y.Telephone,
                               Email = y.Email
                           };

            int ccount = contacts.Count();

            var contactsE = from x in db.Contacts
                            where !(from y in db.Customers
                                    from z in y.Contacts
                                    select z.ID).Contains(x.ID)
                            select new
                            {
                                Customer = "",
                                Area = x.Area,
                                Name = x.Name,
                                Tel = x.Telephone,
                                Email = x.Email
                            };

            var comments = from x in db.Customers
                           select new
                           {
                               Customer = x.ID,
                               Comment = x.Observation
                           };

             using (ExcelPackage pck = new ExcelPackage())
            {
                // Create WorkSheet
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Contacts");
                ExcelWorksheet ws2 = pck.Workbook.Worksheets.Add("Comments");


                // Load Info
                ws.Cells["A1"].LoadFromCollection(contacts.ToList(), true);
                ws2.Cells["A1"].LoadFromCollection(comments.ToList(), true);
                ccount = ccount + 1;
                ws.Cells["A" + ccount].LoadFromCollection(contactsE.ToList(), false);

                // Format Headers
                using (ExcelRange rng = ws.Cells["A1:E1"])
                {
                    rng.Style.Font.Bold = true;
                    rng.Style.Font.Color.SetColor(Color.White);
                    rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    rng.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                }

                using (ExcelRange rng = ws2.Cells["A1:B1"])
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
                Response.Flush();
                Response.End();
            }

        }

        [Authorize(Roles = "Admin")]
        public ActionResult DataControls()
        {
            return View();
        }

        
        [Authorize(Roles = "Admin")]
        public void Backup(String tableNames = "__MigrationHistory", Boolean promptDownload = false)
        {
            //Check if Directory Exist
            if (!System.IO.Directory.Exists(Server.MapPath("~/Content/BackupScripts")))
            {
                System.Diagnostics.Debug.WriteLine("Creating Directory");
                System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/BackupScripts"));
            }
            

            String FileName = "backupScript"+DateTime.Today.ToString("ddMMyyyy")+".sql";

            String Path = string.Format("{0}/{1}", Server.MapPath("~/Content/BackupScripts"),
                                        FileName);

            try
            {
                if (System.IO.File.Exists(Path))
                    System.IO.File.Delete(Path);
            }
            catch(IOException)
            {
                Path = string.Format("{0}/{1}", Server.MapPath("~/Content/BackupScripts"),
                                        "1"+FileName);
            }

            // Define Tables to BackUp
            String[] Tables = tableNames.Split(',');
            StringBuilder sb = new StringBuilder();

            Server srv = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection("BDC-SQLP040\\PRDNP4012", "BauhausDB_User", "gladOS146")); // Server Config
            //Server srv = new Server(); // Localhost Config
            Database dbs = srv.Databases["BauhausDB"];
            ScriptingOptions options = new ScriptingOptions();
            options.ScriptData = true;
            options.ScriptDrops = false;
            options.FileName = Path;
            options.EnforceScriptingOptions = true;
            options.ScriptSchema = false;
            options.IncludeHeaders = true;
            options.AppendToFile = true;
            options.Indexes = true;
            options.WithDependencies = true;
            options.IncludeDatabaseContext = true;

            foreach (var tbl in Tables)
            {
                dbs.Tables[tbl].EnumScript(options);
            }

            if(promptDownload)
            {
                // Write Back Response to Client
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "text/x-sql";
                Response.AddHeader("content-disposition", "attachment; filename= " + FileName + ";");
                Response.TransmitFile(Path);
                Response.Flush();
                // Delete File from server
                System.IO.File.Delete(Path);
                Response.End();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult Erase()
        {
            // Emergency Backup
            Backup();

            // Erase All
            Server srv = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection("BDC-SQLP040\\PRDNP4012", "BauhausDB_User", "gladOS146")); // Server Config
            // Server srv = new Server(); // Localhost Config
            String deleteScript = "USE [BauhausDB]\r\nGO\r\nEXEC sp_MSForEachTable 'DISABLE TRIGGER ALL ON ?'\r\nGO\r\nEXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'\r\nGO\r\nEXEC sp_MSForEachTable 'DELETE FROM ?'\r\nGO\r\nEXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'\r\nGO\r\nEXEC sp_MSForEachTable 'ENABLE TRIGGER ALL ON ?'\r\nGO";
            try
            {
                srv.ConnectionContext.ExecuteNonQuery(deleteScript);
            }
            catch(SqlException e)
            {
                return Json(new { Status = 0, Message = "DB Cleaning Script Failed under "+e.Message+". Please contact an Administrator." });
            }
                
            

            //Restore Model Data
            DirectoryInfo dir = new DirectoryInfo(Server.MapPath("~/Content/BackupScripts"));
            FileInfo file = dir.GetFiles().OrderByDescending(f => f.LastWriteTime).FirstOrDefault();
            if(file!=null)
            {
                String script = file.OpenText().ReadToEnd();
                srv.ConnectionContext.ExecuteNonQuery(script,ExecutionTypes.ContinueOnError);
            }
            else
                return Json(new { Status = 0, Message = "DB Metadata Restoring Failed. Metadata Script was not found or was not created." });
            
            return Json(new { Status = 1, Message = "System Restarted Successfully"});
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Restore()
        {
            //Check if file selected
            if (Request.Files["fileUpload"].ContentLength > 0)
            {
                //Check if Directory Exist
                if (!System.IO.Directory.Exists(Server.MapPath("~/Content/BackupScripts")))
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/Content/BackupScripts"));

                string path = string.Format("{0}/{1}", Server.MapPath("~/Content/BackupScripts"),
                                       Request.Files["fileUpload"].FileName);

                if (Path.GetExtension(path) != ".sql")
                {
                    TempData["Type"] = "warning";
                    TempData["Message"] = "Incorrect File Extension, please use only '.sql' files.";
                    return View("DataControls");
                }

                Request.Files["fileUpload"].SaveAs(path);
                FileInfo fil = new FileInfo(path);
                String script = fil.OpenText().ReadToEnd();
                Server srv = new Server(new Microsoft.SqlServer.Management.Common.ServerConnection("BDC-SQLP040\\PRDNP4012", "BauhausDB_User", "gladOS146"));
                srv.ConnectionContext.ExecuteNonQuery(script,ExecutionTypes.ContinueOnError);
                TempData["Type"] = "success";
                TempData["Message"] = "Script loaded successfully";
            }
            else
            {
                TempData["Type"] = "warning";
                TempData["Message"] = "No File Selected.";

            }
            return View("DataControls");
        }
    }
}
