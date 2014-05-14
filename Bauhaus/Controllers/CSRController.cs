using Bauhaus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;
using Bauhaus.Helpers;

namespace Bauhaus.Controllers
{
    public class CSRController : Controller
    {
        //
        // GET: /CSR/

        BauhausEntities db = new BauhausEntities();

        public ActionResult Index()
        {
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

    }
}
