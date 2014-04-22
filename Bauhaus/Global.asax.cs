using Bauhaus.Models;
using Bauhaus.SeedSimple;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebMatrix.WebData;

namespace Bauhaus
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            Database.SetInitializer<BauhausEntities>(new InitSecurityDb());
            //Database.SetInitializer(new DontDropDbJustCreateTablesIfModelChanged<BauhausEntities>());
            BauhausEntities context = new BauhausEntities();
            context.Database.Initialize(true);
            if (!WebSecurity.Initialized)
                WebSecurity.InitializeDatabaseConnection("BauhausEntities",
                     "UserProfile", "UserId", "UserName", autoCreateTables: true);
            if (!Roles.RoleExists("Admin"))
                Roles.CreateRole("Admin");
            if (!Roles.RoleExists("CSR"))
                Roles.CreateRole("CSR");
            if (!Roles.RoleExists("Carrier"))
                Roles.CreateRole("Carrier");
            if (!Roles.RoleExists("CBD"))
                Roles.CreateRole("CBD");
            if (!Roles.RoleExists("Planner"))
                Roles.CreateRole("Planner");
        }
    }
}