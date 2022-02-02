using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	public class Google : Google<UserInfo>
	{
		public Google(Options opt)
			: base(opt)
		{
		}
	}

	public class Google<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		private RestClient? _client;

		public Google(Options opt)
			: base(opt)
		{
		}

		public override string Name =>	"Google";

		protected override RestClient NewAccessCodeClient()
		{
			return _client ?? (_client = new RestClient(NewOptions("https://accounts.google.com")));
		}

		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://www.googleapis.com"));
		}

		protected override void InitLoginURIRequest(RestRequest request, string? state)
		{
			request.Resource    = "/o/oauth2/auth";
		}

		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource		= "/o/oauth2/token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/oauth2/v1/userinfo";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;
			var u				= new TUserInfo()
			{
				ID				= data.TryGet("id")!,
				Email			= data.TryGet("email"),
				FirstName		= data.TryGet("given_name"),
				LastName		= data.TryGet("family_name"),
				AvatarURL		= data.TryGet("picture")?.Replace("=s96-c", "")
			};

			return u;
		}
	}
}
