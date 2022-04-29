﻿namespace OAuth2.Client.Models
{
	public class UserInfo
	{
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

		/// <summary>
		/// Constructor.
		/// </summary>
		public UserInfo()
		{
		}

		/// <summary>
		/// Unique identifier.
		/// </summary>
		public string ID				{ get; set; }

		/// <summary>
		/// Friendly name of <see cref="UserInfo"/> provider (which is, in its turn, the client of OAuth/OAuth2 provider).
		/// </summary>
		/// <remarks>
		/// Supposed to be unique per OAuth/OAuth2 client.
		/// </remarks>
		public string ProviderName		{ get; set; }

#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

		/// <summary>
		/// Email address.
		/// </summary>
		public string? Email			{ get; set; }

		/// <summary>
		/// First name.
		/// </summary>
		public string? FirstName		{ get; set; }

		/// <summary>
		/// Last name.
		/// </summary>
		public string? LastName			{ get; set; }

		/// <summary>
		/// Contains URI of avatar.
		/// </summary>
		public string? AvatarURL		{ get; set; }
	}
}
