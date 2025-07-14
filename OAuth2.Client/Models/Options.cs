using System.Net.Http;

#if !MVC5
using Microsoft.Extensions.Configuration;
#endif

namespace OAuth2.Client.Models
{
	/// <summary>
	/// Options model for client
	/// </summary>
	public class Options
	{
		/// <summary>
		/// Create options by separate values
		/// </summary>
		/// <param name="clientID">Client ID for use in request</param>
		/// <param name="clientSecret">Client secret for use in service request</param>
		/// <param name="scope">Required service scope</param>
		/// <param name="redirectURI">Return from service URL after auth</param>
		public Options(string clientID, string clientSecret, string scope, string redirectURI)
		{
			ClientID            = clientID;
			ClientSecret        = clientSecret;
			Scope               = scope;
			RedirectURI         = redirectURI;
		}

#if !MVC5
		/// <summary>
		/// Create options from config
		/// </summary>
		/// <param name="cfg">Config section</param>
		/// <exception cref="ArgumentNullException"></exception>
		public Options(IConfigurationSection cfg)
		{
			ClientID            = cfg["ClientID"]		?? throw new ArgumentNullException(cfg.Path+":ClientID");
			ClientSecret        = cfg["ClientSecret"]	?? throw new ArgumentNullException(cfg.Path+":ClientSecret");
			Scope               = cfg["Scope"];
			RedirectURI         = cfg["RedirectURI"]	?? throw new ArgumentNullException(cfg.Path+":RedirectURI");
		}
#endif

		/// <summary>
		/// For testing only
		/// </summary>
		/// <param name="clientID">Client ID for use in request</param>
		/// <param name="clientSecret">Client secret for use in service request</param>
		/// <param name="scope">Required service scope</param>
		/// <param name="redirectURI">Return from service URL after auth</param>
		/// <param name="testHandler">Handler for test requests</param>
		internal Options(string clientID, string clientSecret, string scope, string redirectURI, HttpMessageHandler testHandler)
			: this(clientID, clientSecret, scope, redirectURI)
		{
			TestHandler         = testHandler;
		}

		/// <summary>Client ID.</summary>
		public string ClientID			{ get; }

		/// <summary>Client secret.</summary>
		public string ClientSecret		{ get; }

		/// <summary>Scope - contains set of permissions which user should give to your application.</summary>
		public string? Scope			{ get; }

		/// <summary>Redirect URI (URI user will be redirected to after authentication using third-party service).</summary>
		public string RedirectURI		{ get; }

		internal HttpMessageHandler? TestHandler		{ get; }
	}
}
