using System.Web.Mvc;
using System.Web.Routing;

namespace OAuth2.Client.TestWebLegacy
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{cname}",
				defaults: new { controller = "Home", action = "Index", cname = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "DefaultBy",
				url: "By/{cname}",
				defaults: new { controller = "Home", action = "By" }
			);
		}
	}
}

