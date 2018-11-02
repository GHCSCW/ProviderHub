using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;

namespace ProviderHubServiceNew
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Auth", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SaveProvider",
                url: "Provider/Save/{type}/{id}",
                defaults: new { controller = "Provider", action = "Save", type = 0, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SaveFacility",
                url: "Facility/Save/{type}/{id}",
                defaults: new { controller = "Facility", action = "Save", type = 0, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "SaveVendor",
                url: "Vendor/Save/{type}/{id}",
                defaults: new { controller = "Vendor", action = "Save", type = 0, id=UrlParameter.Optional }
            );
        }
    }
}
