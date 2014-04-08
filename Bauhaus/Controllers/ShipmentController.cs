using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;
using WebMatrix.WebData;
using System.IO;

namespace Bauhaus.Controllers
{
    public class ShipmentController : Controller
    {
        private BauhausEntities db = new BauhausEntities();

        //
        // GET: /Shipment/

        public ActionResult Index()
        {
            ViewBag.Title = "Shipment Index";
            return View(db.Shipments.ToList());
        }

        //
        // GET: /Shipment/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.Title = "Shipment Details";
            return View(db.Shipments.Find(id));
        }

        //
        // GET: /Shipment/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Shipment/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                db.Shipments.Add(shipment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(shipment);
        }

        //
        // GET: /Shipment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        //
        // POST: /Shipment/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shipment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shipment);
        }

        //
        // GET: /Shipment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Shipment shipment = db.Shipments.Find(id);
            if (shipment == null)
            {
                return HttpNotFound();
            }
            return View(shipment);
        }

        //
        // POST: /Shipment/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipment shipment = db.Shipments.Find(id);
            db.Shipments.Remove(shipment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        
    }
}