using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.TestWeb.Services
{
	public class FakeClient : OAuth2Based<UserInfo>
	{
		private RestClient? _client;
		private int _num;

		public FakeClient(Options opt)
			: base(opt)
		{
			_num                = 0;
		}

		public override string Name		=> "FakeClient";

		protected override RestClient NewAccessCodeClient()
		{
			return _client ?? (_client = new RestClient(new RestClientOptions()));
		}

		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		protected override RestClient NewUserInfoClient()
		{
			return NewAccessCodeClient();
		}

		public override Task<string> GetLoginURIAsync(string? state = null, CancellationToken cancellationToken = default)
		{
			var url             = Options.RedirectURI+"?code="+(_num++)+"@"+Guid.NewGuid();
			if (!string.IsNullOrEmpty(state))
				url             += "&state="+state;

			return Task.FromResult(url);
		}

		protected override Task<UserInfo> GetUserInfoAsync(Ctx ctx, CancellationToken cancellationToken)
		{
			return Task.FromResult(new UserInfo
			{
				ID              = "By"+ctx.Params.Get("state"),
				FirstName       = "Fake",
				LastName		= "User",
				Email			= "fake@user.mail",
				ProviderName	= Name
			});
		}

		internal override Task<UserInfo> GetUserInfoAsync(IQueryCollection parameters, Action<string, string, string?> onReq, CancellationToken cancellationToken = default)
		{
			onReq(">>>", "Get AccessToken by ClientID+ClientSecret+Code request", null);
			onReq("<<<", "application/json", "JSON with access token");

			onReq(">>>", "Get UserInfo by AccessToken request", null);
			onReq("<<<", "application/json", "JSON with user info");

			return Task.FromResult(new UserInfo
			{
				ID              = "By"+parameters["state"],
				FirstName       = "Fake",
				LastName        = "User",
				Email           = "fake@user.mail",
				ProviderName    = Name
			});
		}

		protected override void InitLoginURIRequest(RestRequest request, string? state)
		{
			throw new NotSupportedException();
		}

		protected override UserInfo ParseUserInfo(Ctx ctx)
		{
			throw new NotSupportedException();
		}
	}
}
