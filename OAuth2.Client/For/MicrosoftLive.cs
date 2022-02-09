using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	public class MicrosoftLive : MicrosoftLive<UserInfo>
	{
		public MicrosoftLive(Options opt)
			: base(opt)
		{
		}
	}

	public class MicrosoftLive<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		private RestClient? _client;

		public MicrosoftLive(Options opt)
			: base(opt)
		{
		}

		public override string Name =>	"MicrosoftLive";

		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://login.live.com"));
		}

		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://graph.microsoft.com/v1.0"));
		}

		protected override void InitLoginURIRequest(RestRequest request, string? state)
		{
			request.Resource    = "/oauth20_authorize.srf";
		}

		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/oauth20_token.srf";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

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

		private async Task _getAvatarAsync(Ctx ctx, string url, string n, TokensData data, CancellationToken cancellationToken)
		{
			ctx.Request         = new RestRequest(url);
			await AddAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
			await base.ExecuteAndVerifyAsync(ctx, true, cancellationToken).ConfigureAwait(false);
			if (ctx.RawContent!=null) data.Add(n, ctx.RawContent);
		}

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
