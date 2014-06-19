using Bauhaus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace Bauhaus.Controllers
{
    public class HomeController : Controller
    {
        BauhausEntities db = new BauhausEntities();

        [Authorize]
        public ActionResult Index()
        {
            if (!WebSecurity.UserExists("admin"))
            {
                WebSecurity.CreateUserAndAccount("admin", "bauhaus", propertyValues: new
                {
                    FullName = "System Administrator",
                    Email = "bauhaus.admin@pg.com",
                    Active = true
                });
                if (!Roles.RoleExists("Admin"))
                    Roles.CreateRole("Admin");
                Roles.AddUserToRole("admin", "Admin");

            }
            try
            {
                string[] userRoles = Roles.GetRolesForUser();
                if (userRoles == null)
                    return RedirectToAction("LogOff", "Account");
                else
                {
                    if (userRoles.Length == 0)
                    {
                        return View("Unassigned");
                    }
                    else
                        return RedirectToAction("Index", userRoles[0]);
                }
            }
            catch (Exception e)
            {
                if (e is System.InvalidOperationException || e is NullReferenceException)
                {
                    String[] cookies = Request.Cookies.AllKeys;
                    foreach(String cookie in cookies)
                    {
                        Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                    }
                    return RedirectToAction("Index");
                }
                else
                    throw;
            }
        }

        [Authorize]
        public ActionResult Unassigned()
        {
            ViewBag.Title = "Unassigned Role";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Project Scope and Vision.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Us to find out More.";

            return View();
        }

        [Authorize]
        public ActionResult AutoEnroll()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult MasterKeyCheck(string masterKey)
        {
            string storedMasterKey = "bauhaus";
            if (storedMasterKey == masterKey)
            {
                if (!System.Web.Security.Roles.RoleExists("Admin"))
                    System.Web.Security.Roles.CreateRole("Admin");

                System.Web.Security.Roles.AddUserToRole(User.Identity.Name, "Admin");
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public JsonResult SendFeedback(String message)
        {
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("meneses.bs@pg.com", "Brahyam Meneses");
            msg.To.Add(new MailAddress("meneses.brahyam@gmail.com"));
            msg.Subject = "Bauhaus Feedback";
            msg.Body = message;

            SmtpClient mClient = new SmtpClient();

            try
            {
                mClient.Send(msg);
            }
            catch (SmtpException e)
            {
                String inner = (e.InnerException.Message != null) ? e.InnerException.Message : "";
                Log log = new Log(Request.UserHostAddress,User.Identity.Name,"FeedBack",message);
                Log log2 = new Log(Request.UserHostAddress, User.Identity.Name, "Error", e.Message + ". " + inner);
                db.Logs.Add(log);
                db.Logs.Add(log2);
                db.SaveChanges();
                return Json(new { Status = 0, Message = e.Message + ". " + inner });
            }

            Log log3 = new Log(Request.UserHostAddress, User.Identity.Name, "FeedBack", message);
            db.SaveChanges();
            return Json(new { Status = 1, Message = "FeedBack Sent." });
        }
    }
}