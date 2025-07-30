#if !MVC5
using Microsoft.Extensions.Configuration;
#endif

namespace OAuth2.Client.Models
{
	public class KeycloakOptions : Options
	{
		private static readonly string _DEFAULTIDFIELD	= "sub";

#if !MVC5
		public KeycloakOptions(IConfigurationSection cfg)
			: base(cfg)
		{
			Name                = cfg["Name"]		?? throw new ArgumentNullException(cfg.Path+":Name");
			BaseURI				= cfg["BaseURI"]	?? throw new ArgumentNullException(cfg.Path+":BaseURI");
			IDField             = cfg["IDField"]    ?? _DEFAULTIDFIELD;
		}
#endif

		public KeycloakOptions(string name, string clientID, string clientSecret, string scope, string baseURI, string redirectURI, string? idField)
			: base(clientID, clientSecret, scope, redirectURI)
		{
			Name                = name;
			BaseURI				= baseURI;
			IDField             = idField ?? _DEFAULTIDFIELD;
		}

		/// <summary>
		/// Название провайдера
		/// </summary>
		internal string Name	{ get; }

		/// <summary>
		/// Базовый адрес
		/// </summary>
		internal string BaseURI { get; }

		/// <summary>
		/// Имя поля, из которого брать <see cref="IUserInfo.ID"/>
		/// </summary>
		internal string IDField { get; }
	}
}
