using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebTruyenHay
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "PaymentCallback",
            url: "Payment/Callback",
            defaults: new { controller = "YourController", action = "PaymentCallback" }
         );
            routes.MapRoute(
            name: "ReadTruyen",
            url: "truyen/ReadTruyen/{id}/{sothutu}",
            defaults: new { controller = "Truyen", action = "ReadTruyen", sothutu = UrlParameter.Optional }
        );

            routes.MapRoute(
            name: "ToggleTheme",
            url: "truyen/ToggleTheme",
            defaults: new { controller = "truyen", action = "ToggleTheme" }
        );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
