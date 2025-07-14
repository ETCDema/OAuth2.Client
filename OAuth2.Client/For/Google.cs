using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for Google with base UserInfo model
	/// </summary>
	public class Google : Google<UserInfo>
	{
		public Google(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for Google
	/// </summary>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class Google<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient? _client;

		/// <inheritdoc/>
		public Google(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name =>	"Google";

		public override string? GetHint(IUserInfo info)
		{
			return info.Email;
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://accounts.google.com"));
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://www.googleapis.com"));
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "/o/oauth2/auth";

			if (!string.IsNullOrEmpty(hint)) request.AddParameter("login_hint", hint);
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource		= "/o/oauth2/token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/oauth2/v1/userinfo";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
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
