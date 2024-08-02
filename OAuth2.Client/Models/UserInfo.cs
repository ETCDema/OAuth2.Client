namespace OAuth2.Client.Models
{
	/// <summary>
	/// User info model
	/// </summary>
	public interface IUserInfo
	{
		/// <summary>Unique identifier.</summary>
		string ID				{ get; set; }

		/// <summary>
		/// Friendly name of <see cref="IUserInfo"/> provider (which is, in its turn, the client of OAuth/OAuth2 provider).
		/// </summary>
		/// <remarks>
		/// Supposed to be unique per OAuth/OAuth2 client.
		/// </remarks>
		string ProviderName		{ get; set; }

		/// <summary>Email address.</summary>
		string? Email			{ get; set; }

		/// <summary>First name.</summary>
		string? FirstName		{ get; set; }

		/// <summary>Last name.</summary>
		string? LastName		{ get; set; }

		/// <summary>Contains URI of avatar.</summary>
		string? AvatarURL		{ get; set; }
	}

	/// <summary>
	/// Base Userinfo model
	/// </summary>
	public class UserInfo: IUserInfo
	{
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

		/// <summary>Constructor.</summary>
		public UserInfo()
		{
		}

		/// <summary>Unique identifier.</summary>
		public string ID				{ get; set; }

		/// <summary>
		/// Friendly name of <see cref="IUserInfo"/> provider (which is, in its turn, the client of OAuth/OAuth2 provider).
		/// </summary>
		/// <remarks>
		/// Supposed to be unique per OAuth/OAuth2 client.
		/// </remarks>
		public string ProviderName		{ get; set; }

#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

		/// <summary>Email address.</summary>
		public string? Email			{ get; set; }

		/// <summary>First name.</summary>
		public string? FirstName		{ get; set; }

		/// <summary>Last name.</summary>
		public string? LastName			{ get; set; }

		/// <summary>Contains URI of avatar.</summary>
		public string? AvatarURL		{ get; set; }
	}
}
