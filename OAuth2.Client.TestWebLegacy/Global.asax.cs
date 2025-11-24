using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using OAuth2.Client.For;
using OAuth2.Client.Models;
using OAuth2.Client.TestWeb.Services;


namespace OAuth2.Client.TestWebLegacy
{
	public class MvcApplication : HttpApplication
	{
		public static readonly List<IClient> Clients	= new List<IClient>();

		/// <summary>Инициализация приложения</summary>
		protected void Application_Start()
		{
			ServicePointManager.SecurityProtocol		|= SecurityProtocolType.Tls12;

			AreaRegistration.RegisterAllAreas();
			RouteConfig.RegisterRoutes(RouteTable.Routes);

			Clients.Add(new FakeClient(new Options("Fake-ID", "Fake-Secret", "fake:profile fake:email", "http://localhost:8050/By/FakeClient")));
		}
	}
}
