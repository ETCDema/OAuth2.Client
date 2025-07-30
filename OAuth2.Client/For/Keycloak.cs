using System;
using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for Keycloak with base UserInfo model
	/// </summary>
	public class Keycloak(KeycloakOptions opt) : Keycloak<UserInfo>(opt)
	{
	}

	/// <summary>
	/// OAuth2 client for Keycloak
	/// </summary>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class Keycloak<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient _client = default!;
		private string _idField;

		public Keycloak(KeycloakOptions opt)
			: base(opt)
		{
			Name                = opt.Name;
			_idField            = opt.IDField;
		}

		public override string Name		{ get; }

		public override string? GetHint(IUserInfo info)
		{
			return info.Email;
		}

		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions(((KeycloakOptions)Options).BaseURI));
		}

		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		protected override RestClient NewUserInfoClient()
		{
			return NewAccessCodeClient();
		}

		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "auth";
			if (!string.IsNullOrEmpty(hint))
			{
				request.AddParameter("login_hint", hint);
			}
		}

		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource = "token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource = "userinfo";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}

		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			TokensData content  = ctx.Content!;
			string[]? array     = content.TryGet("name")?.Split([' '], 2);
			return new TUserInfo
			{
				ID              = content.TryGet(_idField)		?? throw new NullReferenceException($"Поле {_idField} не содержит значения"),
				Email           = content.TryGet("email"),
				FirstName       = content.TryGet("given_name")  ?? ((array?.Length > 1) ? array[1] : content.TryGet("preferred_username")),
				LastName        = content.TryGet("family_name") ?? ((array?.Length > 0) ? array[0] : "")
			};
		}
	}
}
