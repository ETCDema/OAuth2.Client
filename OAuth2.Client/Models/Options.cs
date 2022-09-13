using Microsoft.Extensions.Configuration;

namespace OAuth2.Client.Models
{
	public class Options
	{
		public Options(string clientID, string clientSecret, string scope, string redirectURI)
		{
			ClientID            = clientID;
			ClientSecret        = clientSecret;
			Scope               = scope;
			RedirectURI         = redirectURI;
		}

		public Options(IConfigurationSection cfg)
		{
			ClientID            = cfg["ClientID"];
			ClientSecret        = cfg["ClientSecret"];
			Scope               = cfg["Scope"];
			RedirectURI         = cfg["RedirectURI"];
		}

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
		public string Scope				{ get; }

		/// <summary>
		/// Redirect URI (URI user will be redirected to
		/// after authentication using third-party service).
		/// </summary>
		public string RedirectURI		{ get; }

		internal HttpMessageHandler? TestHandler		{ get; }
	}
}
