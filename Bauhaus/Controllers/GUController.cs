using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bauhaus.Controllers
{
    public class GUController : Controller
    {
        private Bauhaus.Models.BauhausEntities db = new Models.BauhausEntities();
        //
        // GET: /CBD/

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
