using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.TestWeb.Services
{
	/// <summary>
	/// Test fake client - just return something without request any service.
	/// </summary>
	public class FakeClient : OAuth2Based<UserInfo>
	{
		private RestClient? _client;
		private int _num;

		/// <inheritdoc/>
		public FakeClient(Options opt)
			: base(opt)
		{
			_num                = 0;
		}

		/// <inheritdoc/>
		public override string Name		=> "FakeClient";

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(new RestClientOptions());
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
		public override Task<string> GetLoginURIAsync(string? state = null, string? hint = null, CancellationToken cancellationToken = default)
		{
			var url             = Options.RedirectURI+"?code="+(_num++)+"@"+Guid.NewGuid();
			if (!string.IsNullOrEmpty(state))
				url             += "&state="+state;

			if (!string.IsNullOrEmpty(hint))
				url             += "&login_hint="+hint;

			return Task.FromResult(url);
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		protected override UserInfo ParseUserInfo(Ctx ctx)
		{
			throw new NotSupportedException();
		}
	}
}
