using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Sitecore.HabitatHome.Commerce.Website
{
    public class CustomRoutes
    {
        public virtual void Process(PipelineArgs args)
        {
            RegisterRoute();
        }
        public static void RegisterRoute()
        {
            var route = RouteTable.Routes.MapRoute(
                name: "MyApp",
                url: "api/cxa/{controller}/{action}",
                namespaces: new[] { "Sitecore.HabitatHome.Feature.Customers.Controllers", "Commerce.XA.Feature.Account.Controllers" });
        }
    }
}