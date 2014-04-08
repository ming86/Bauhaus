using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bauhaus.Models;
using System.Web.Security;
using System.Data.Entity;
using WebMatrix.WebData;

namespace Bauhaus.SeedSimple
{
    public class InitSecurityDb : DropCreateDatabaseIfModelChanges<BauhausEntities>
    {
        protected override void Seed(BauhausEntities context)
        {
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
        }
    }
}