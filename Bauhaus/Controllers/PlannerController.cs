using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;

namespace Bauhaus.Controllers
{
    public class PlannerController : Controller
    {
        BauhausEntities db = new BauhausEntities();
        //
        // GET: /Planner/

        public ActionResult Index()
        {
            DateTime updated = (from x in db.Reports
                                where x.Name == "ZVORF.xlsx"
                                || x.Name == "ZSKU.xlsx"
                                select x.CreationDate).ToList().Last();
            if (updated == null)
            {
                if (db.Reports.ToList().Any())
                    updated = db.Reports.ToList().Last().CreationDate;
            }

            return View(updated);
        }

    }
}
