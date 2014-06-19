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
        public JsonResult Create(String area, String name, String telephone, String email)
        {
            if (area!=null && name!=null && telephone!=null || email!=null)
            {
                Contact contact = new Contact();
                contact.Area = area;
                contact.Name = name;
                contact.Telephone = telephone;
                contact.Email = email;
                db.Contacts.Add(contact);
                db.SaveChanges();
                return Json(new { Status = 1, Message = "Contact Saved." });
            }
            else
            {
                return Json(new { Status = 1, Message = "Contact must have Area, Name and either Tlf or Mail." });
            }
        }

        //
        // GET: /Contact/Edit/5
        [Authorize]
        public JsonResult GetContact(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
                return Json(new { Status = 0, Message = "Contact not found"});
            return Json(new { Status = 1, Message = "Ok", Contact = contact});
        }

        [Authorize]
        public ActionResult GetContactsGrid()
        {
            return PartialView("_ContactsGrid", db.Contacts.OrderBy(x => x.Area).ToList());
        }

        //
        // POST: /Contact/Edit/5
        [Authorize]
        [HttpPost]
        public JsonResult Update(int id, String area, String name, String telephone, String email)
        {
            if (area != null && name != null && telephone != null || email != null)
            {
                Contact contact = db.Contacts.Find(id);
                contact.Area = area;
                contact.Name = name;
                contact.Telephone = telephone;
                contact.Email = email;
                db.SaveChanges();
                return Json(new { Status = 1, Messsage = "Contact Saved" });
            }
            else
                return Json(new { Status = 0, Message = "Contact Saving Failed." });
        }

        //
        // GET: /Contact/Delete/5
        [Authorize]
        public JsonResult Delete(int id)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
                return Json(new { Status = 0, Message = "Not found" });
            else
            {
                db.Contacts.Remove(contact);
                db.SaveChanges();
                return Json(new { Status = 1, Message = "Contact Removed" });
            }
            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}