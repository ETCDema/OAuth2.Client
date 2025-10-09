#if !MVC5
using Microsoft.Extensions.Configuration;
#endif

using System.Net.Http;

namespace OAuth2.Client.Models
{
	public class VKIDOptions: Options
	{
#if !MVC5
		public VKIDOptions(IConfigurationSection cfg)
			: base(cfg)
		{
			UsePublicInfo       = !"false".Equals(cfg["usePublicInfo"], StringComparison.OrdinalIgnoreCase);
		}
#endif

		public VKIDOptions(string clientID, string clientSecret, string scope, string redirectURI, bool usePublicInfo)
			: base(clientID, clientSecret, scope, redirectURI)
		{
			UsePublicInfo       = usePublicInfo;
		}

		internal VKIDOptions(string clientID, string clientSecret, string scope, string redirectURI, HttpMessageHandler testHandler)
			: base(clientID, clientSecret, scope, redirectURI, testHandler)
		{
			UsePublicInfo		= false;
		}

		public bool UsePublicInfo		{ get; }
	}
}
