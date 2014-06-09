using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bauhaus.Models;

namespace Bauhaus.Controllers
{
    public class ContactController : Controller
    {
        private BauhausEntities db = new BauhausEntities();

        [Authorize]
        public ActionResult Index()
        {
            return View(db.Contacts.OrderBy(x=>x.Area).ToList());
        }

        [HttpPost]
        [Authorize]
        public ContentResult Create(long customer,String area, String name, String telephone, String email)
        {
            int type;
            string result;

            if (area!=null && name!=null && telephone!=null && telephone!=null && email!=null)
            {
                Customer cust = db.Customers.Find(customer);
                Contact contact = new Contact();
                contact.Area = area;
                contact.Name = name;
                contact.Telephone = telephone;
                contact.Email = email;
                cust.Contacts.Add(contact);
                db.Contacts.Add(contact);
                db.SaveChanges();
                type = 0;
                result = "Contact Saved.";
            }
            else
            {
                type = 1;
                result = "Contact Saving Failed.";
            }

            return new ContentResult { Content = type + "|" + result };
        }

        //
        // GET: /Contact/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        //
        // POST: /Contact/Edit/5
        [Authorize]
        [HttpPost]
        public ContentResult Update(int id, String area, String name, String telephone, String email)
        {

            int type;
            string result;
            if (area!=null && name!=null && telephone!=null && email!=null)
            {
                Contact contact = db.Contacts.Find(id);
                contact.Area = area;
                contact.Name = name;
                contact.Telephone = telephone;
                contact.Email = email;
                db.Entry(contact).State = EntityState.Modified;
                db.SaveChanges();
                type = 0;
                result = "Contact Saved.";
                
            }
            else
            {
                type = 1;
                result = "Contact Saving Failed.";
            }

            return new ContentResult { Content = type + "|" + result };
        }

        //
        // GET: /Contact/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            System.Diagnostics.Debug.WriteLine("Deleting Contact: " + id);
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
                return Json(new { Status = 0, Message = "Not found" });
            else
            {
                db.Contacts.Remove(contact);
                db.SaveChanges();
            }
            return Json(new { Status = 1, Message = "OK" });
        }

        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}