using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Bauhaus.Controllers
{
    public class CBDController : Controller
    {
        public Bauhaus.Models.BauhausEntities db = new Models.BauhausEntities();
        //
        // GET: /CBD/
        [Authorize]
        public ActionResult Index()
        {
            if(Session["FilterData"] == null)
            {
                Models.Filter filter = db.Filters.Find(WebSecurity.CurrentUserId);
                if (filter == null)
                    return View("FilterSetup");
                else
                    Session["FilterData"] = filter;
            }

            ViewBag.Title = "Dashboard";
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
            return View(updated);
        }

        [Authorize]
        public ActionResult FilterSetup()
        {
            return View();
        }

        /// <summary>
        /// Saves filter information in Session Variable and over DB if choosed to remember settings.
        /// </summary>
        /// <param name="user">user ID</param>
        /// <param name="team">Sales Team</param>
        /// <param name="area">Sales Zone</param>
        /// <param name="remember">True if filter should be saved to DB</param>
        /// <returns>Json with Status, Message or ErrorMessage</returns>
        [Authorize]
        public JsonResult SaveFilter(String team, String area, Boolean remember)
        {
            Bauhaus.Models.Filter filter = db.Filters.Find(WebSecurity.CurrentUserId);
            if (filter == null)
            {
                filter = new Models.Filter();
                filter.Settings = new Dictionary<string, string>();
                if(remember)
                    db.Filters.Add(filter);
            }
            else
            {
                if (!remember)
                    db.Filters.Remove(filter);
            }

            team = team.Trim();
            if(team!="All")
                filter.Settings.Add("Team", team);

            int areaNum;
            if (Int32.TryParse(area,out areaNum))
                if (area.Length == 2)
                    filter.Settings.Add("Unit", areaNum.ToString());
                else if (area.Length == 3)
                    filter.Settings.Add("Area", areaNum.ToString());
                
            Session["FilterData"] = filter;

            db.SaveChanges();

            return Json(new { Status = 1, Message = "Filter Saved Correctly" });
        }

        
        /// <summary>
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        [Authorize]
        public JsonResult LoadFilter()
        {
            String team, area;
            Models.Filter filter;

            if( Session["FilterData"] != null)
                filter = Session["FilterData"] as Models.Filter;
            else
            {
                filter = db.Filters.Find(WebSecurity.CurrentUserId);
                if(filter == null)
                    return Json(new { Status = 1, Message = "No filter Founded", Team = "All", Area = "All" });
            }
                if (filter.Settings.ContainsKey("Team"))
                    team = filter.Settings["Team"];
                else
                    team = "All";

                if (filter.Settings.ContainsKey("Area"))
                    area = filter.Settings["Area"];
                else if(filter.Settings.ContainsKey("Unit"))
                    area = filter.Settings["Unit"];
                else
                    area = "All";

                return Json(new { Status = 1, Message = "Filter Loaded from DB corretly", Team = team, Area = area });
            
        }
    }
}
