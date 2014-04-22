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

namespace Bauhaus.Controllers
{
    public class AdminController : Controller
    {
        private BauhausEntities db = new BauhausEntities();
        //
        // GET: /Admin/

        [Authorize(Roles="Admin")]
        public ActionResult Index()
        {
            DateTime updated = (from x in db.Reports
                                where x.Name == "ZVORF.xlsx"
                                || x.Name == "ZSKU.xlsx"
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
            if(users.Length != 0)
                System.Web.Security.Roles.RemoveUsersFromRole(users, roleName);
            System.Web.Security.Roles.DeleteRole(roleName);
            return RedirectToAction("ManageRoles","Admin");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult UserToRole(string roleName, string userName)
        {
            if (!Roles.RoleExists(roleName))
                return Json(new { Status = 0, Message = "Wanted role does not exist." });
            if(!WebSecurity.UserExists(userName))
                return Json(new { Status = 0, Message = "User does not exist." });
            
            String [] roles = System.Web.Security.Roles.GetRolesForUser(userName);
            if(roles.Length>0)
                System.Web.Security.Roles.RemoveUserFromRoles(userName, roles);

            System.Web.Security.Roles.AddUserToRole(userName,roleName);

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
            return View(db.Logs.ToList().OrderByDescending(log=>log.Date));
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
            foreach(UserProfile user in users)
            {
                if(!visits.ContainsKey(user.UserName))
                    visits.Add(user.UserName, 0);
            }

            List<Log> logs = db.Logs.Where(x => x.Type == "Warning" &&
                                                x.Description.Contains("Logged In") &&
                                                x.Date.Month == DateTime.Today.Month
                                                ).ToList();
            
            foreach(Log lg in logs)
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
                ws.Cells["A1"].LoadFromCollection(visits.OrderBy(x=>x.Value).ToList(), true);

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
    }
}
