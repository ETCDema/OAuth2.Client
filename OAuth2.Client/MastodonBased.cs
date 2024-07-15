using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client
{
	/// <summary>
	/// Для реализации клиентов к сервисам, сделанным на основе Mastodon
	/// </summary>
	/// <remarks>
	/// Login hint not supported.
	/// </remarks>
	/// <typeparam name="TUserInfo">Тип модели пользоввателя</typeparam>
	public abstract class MastodonBased<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		/// <inheritdoc/>
		public MastodonBased(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "/oauth/authorize";
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/oauth/token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/api/v1/accounts/verify_credentials";
			ctx.Request.Method          = Method.Get;
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;
			var names           = data.TryGet("display_name")?.Split(' ');

			var u               = new TUserInfo()
			{
				ID              = data.TryGet("id")!,
				Email           = data.TryGet("email"),
				FirstName       = (names?.Length>0 && !string.IsNullOrEmpty(names[0]) ? names[0] : data.TryGet("username")),
				LastName        = (names?.Length>1 ? names[1] : ""),
				AvatarURL       = data.TryGet("avatar_static") ?? data.TryGet("avatar")
			};

			return u;
		}
	}
}
