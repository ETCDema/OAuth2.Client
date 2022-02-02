using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	public class Yandex : Yandex<UserInfo>
	{
		public Yandex(Options opt)
			: base(opt)
		{
		}
	}

	public class Yandex<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		private static readonly string _AVATARBASEURI   = "https://avatars.yandex.net/get-yapic/";
		private static readonly string _LARGE           = "islands-200";

		private RestClient? _client;

		public Yandex(Options opt)
			: base(opt)
		{
		}

		public override string Name =>	"Yandex";

		protected override RestClient NewAccessCodeClient()
		{
			return _client ?? (_client = new RestClient(NewOptions("https://oauth.yandex.ru")));
		}

		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://login.yandex.ru"));
		}

		protected override void InitLoginURIRequest(RestRequest request, string? state)
		{
			request.Resource    = "/authorize";
		}

		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/info";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;
			var names			= data.TryGet("real_name")?.Split(' ');
			var pic             = data.TryGet("default_avatar_id");	// Вместо аватара к нам приходит его идентификатор и нужно сформировать полный URL

			var u               = new TUserInfo()
			{
				ID              = data.TryGet("id")!,
				Email           = data.TryGet("default_email"),
				FirstName       = data.TryGet("first_name") ?? (names?.Length>0 ? names[0] : data.TryGet("display_name")),
				LastName        = data.TryGet("last_name")  ?? (names?.Length>1 ? names[1] : ""),
				AvatarURL		= !string.IsNullOrEmpty(pic) ? _AVATARBASEURI+pic+"/"+_LARGE : null,
			};

			return u;
		}
	}
}
