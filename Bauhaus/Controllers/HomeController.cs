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
        public ActionResult AutoEnrole()
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
                Log log = new Log();
                log.Source = Request.UserHostAddress;
                log.UserName = User.Identity.Name;
                log.Type = "FeedBack";
                log.Description = message;
                log.Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
                db.Logs.Add(log);
                db.SaveChanges();
                String inner = (e.InnerException.Message != null) ? e.InnerException.Message : "";
                return Json(new { Status = 0, Message = e.Message + ". " + inner });
            }

            Log log2 = new Log();
            log2.Source = Request.UserHostAddress;
            log2.UserName = User.Identity.Name;
            log2.Type = "FeedBack";
            log2.Description = message;
            log2.Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Venezuela Standard Time"));
            db.Logs.Add(log2);
            db.SaveChanges();
            return Json(new { Status = 1, Message = "FeedBack Sent." });
        }
    }
}