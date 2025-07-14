using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for live.com with base UserInfo model
	/// </summary>
	public class MicrosoftLive : MicrosoftLive<UserInfo>
	{
		public MicrosoftLive(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for live.com
	/// </summary>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class MicrosoftLive<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient? _client;

		/// <inheritdoc/>
		public MicrosoftLive(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name =>	"MicrosoftLive";

		public override string? GetHint(IUserInfo info)
		{
			return info.Email;
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://login.live.com"));
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://graph.microsoft.com/v1.0"));
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "/oauth20_authorize.srf";

			if (!string.IsNullOrEmpty(hint)) request.AddParameter("login_hint", hint);
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/oauth20_token.srf";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/me";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);

			var mainRContent    = ctx.RawContent;
			var mainContent		= ctx.Content!;

			// Аватар нужно получать отдельным запросом и упакуем его в data url т.к. он у нас имеет двоичный формат в base64encoded строке
			await _getAvatarAsync(ctx, "/me/photo/$value", "#AvatarURL", mainContent, cancellationToken).ConfigureAwait(false);

			ctx.RawContent      = mainRContent;
			ctx.Content         = mainContent;
		}

		/// <summary>
		/// Get avatar content for user
		/// </summary>
		/// <param name="ctx">Context</param>
		/// <param name="url">URL for get avatar</param>
		/// <param name="n">Name of avatar content in token data</param>
		/// <param name="data">Token data</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task _getAvatarAsync(Ctx ctx, string url, string n, TokensData data, CancellationToken cancellationToken)
		{
			ctx.Request         = new RestRequest(url);
			await AddAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
			await base.ExecuteAndVerifyAsync(ctx, true, cancellationToken).ConfigureAwait(false);
			if (ctx.RawContent!=null) data.Add(n, ctx.RawContent);
		}

		/// <inheritdoc/>
		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;
			var id              = data.TryGet("id")!;

			var u               = new TUserInfo()
			{
				ID              = id,
				Email           = data.TryGet("userPrincipalName"),
				FirstName       = data.TryGet("givenName"),
				LastName        = data.TryGet("surname"),
				AvatarURL		= data.TryGet("#AvatarURL"),
			};

			return u;
		}
	}
}
