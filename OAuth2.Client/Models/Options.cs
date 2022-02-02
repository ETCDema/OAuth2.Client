using Microsoft.Extensions.Configuration;

namespace OAuth2.Client.Models
{
	public class Options
	{
#pragma warning disable CS8618
		public Options()
		{
		}
#pragma warning restore CS8618

		public Options(IConfigurationSection cfg)
		{
			ClientID            = cfg["ClientID"];
			ClientSecret        = cfg["ClientSecret"];
			Scope               = cfg["Scope"];
			RedirectURI         = cfg["RedirectURI"];
		}

		/// <summary>Client ID.</summary>
		public string ClientID			{ get; init; }

		/// <summary>Client secret.</summary>
		public string ClientSecret		{ get; init; }

		/// <summary>Scope - contains set of permissions which user should give to your application.</summary>
		public string Scope				{ get; init; }

		/// <summary>
		/// Redirect URI (URI user will be redirected to
		/// after authentication using third-party service).
		/// </summary>
		public string RedirectURI		{ get; init; }

		internal HttpMessageHandler? TestHandler		{ get; init; }
	}
}
