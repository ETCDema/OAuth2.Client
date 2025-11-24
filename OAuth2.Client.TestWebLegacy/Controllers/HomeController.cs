
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using OAuth2.Client.Models;
using OAuth2.Client.TestWebLegacy.Models;

namespace OAuth2.Client.TestWebLegacy.Controllers
{
	/// <summary>
	/// Single test controller
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		/// Show page with clients and data of selected client
		/// </summary>
		/// <param name="cname">Selected client</param>
		/// <returns></returns>
		[Route("/{cname?}")]
		public ActionResult Index(string cname = null)
		{
			return View(_createModel(cname, null));
		}

		/// <summary>
		/// Auth callback (return URL)
		/// </summary>
		/// <param name="cname">Selected client</param>
		/// <param name="code">Code from service</param>
		/// <param name="state">State data</param>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		[Route("/By/{cname}")]
		public async Task<ActionResult> By(string cname, string code, string state)
		{
			var vm              = _createModel(cname, state);
			try
			{
				vm.Log.Add("From "+cname+": "+code);
				if (!string.IsNullOrEmpty(state))
					vm.Log.Add("+ ["+state+"]");

				var client      = (OAuth2Based<UserInfo>)vm.Current ?? throw new NotSupportedException("Client "+cname+" not supported");

				vm.User         = await client.GetUserInfoAsync(Request.QueryString, (dir, v1, v2) =>
				{
					vm.Log.Add(dir+v1);
					if (v2!=null)
						vm.Log.Add(dir+v2);
				});
				vm.LoginHint    = client.GetHint(vm.User);
			} catch(Exception ex)
			{
				vm.Error        = ex.Message;
				while (ex!=null)
				{
					vm.Log.Add("FAIL: ["+ex.GetType().Name+"] "+ex.Message);
					vm.Log.Add("@ "+ex.StackTrace);
					ex			= ex.InnerException;
				}
			}

			return View("Index", vm);
		}

		/// <summary>
		/// Create new client model with state
		/// </summary>
		/// <param name="cname">Selected client</param>
		/// <param name="state">State data</param>
		/// <returns></returns>
		private ClientsModel _createModel(string cname, string state)
		{
			var clients         = MvcApplication.Clients;
			var m 				= new ClientsModel(clients)
			{
				StateValue      = state,				
			};

			if (!string.IsNullOrEmpty(cname))
				m.Current       = m.Clients.FirstOrDefault(c => c.Name == cname);

			return m;
		}
	}
}