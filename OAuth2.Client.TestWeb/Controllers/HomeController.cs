using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using OAuth2.Client.Models;
using OAuth2.Client.TestWeb.Models;

namespace OAuth2.Client.TestWeb.Controllers
{
	public class HomeController : Controller
	{
		[Route("/{cname?}")]
		public IActionResult Index(string? cname = null)
		{
			return View(_createModel(cname, null));
		}

		[Route("/By/{cname}")]
		public async Task<IActionResult> By(string cname, string code, string? state)
		{
			var vm              = _createModel(cname, state);
			try
			{
				vm.Log.Add("From "+cname+": "+code);
				if (!string.IsNullOrEmpty(state))
					vm.Log.Add("+ ["+state+"]");

				var client      = (OAuth2Based<UserInfo>)vm.Current!;

				vm.User         = await client.GetUserInfoAsync(Request.Query, (dir, v1, v2) =>
				{
					vm.Log.Add(dir+v1);
					if (v2!=null)
						vm.Log.Add(dir+v2);
				});
			} catch(Exception ex)
			{
				vm.Error        = ex.Message;
				vm.Log.Add("FAIL: ["+ex.GetType().Name+"] "+ex.Message);
				vm.Log.Add("@ "+ex.StackTrace);
			}

			return View("Index", vm);
		}

		private ClientsModel _createModel(string? cname, string? state)
		{
			var m 				= new ClientsModel(HttpContext.RequestServices.GetServices<IClient>())
			{
				StateValue      = state
			};

			if (!string.IsNullOrEmpty(cname))
				m.Current		= m.Clients.FirstOrDefault(c => c.Name == cname);

			return m;
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}