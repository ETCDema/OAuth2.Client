using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for Yandex with base UserInfo model
	/// </summary>
	public class Yandex : Yandex<UserInfo>
	{
		public Yandex(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for Yandex
	/// </summary>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class Yandex<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private static readonly string _AVATARBASEURI   = "https://avatars.yandex.net/get-yapic/";
		private static readonly string _LARGE           = "islands-200";

		private RestClient? _client;

		/// <inheritdoc/>
		public Yandex(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name =>	"Yandex";

		public override string? GetHint(IUserInfo info)
		{
			return info.Email;
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://oauth.yandex.ru"));
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://login.yandex.ru"));
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "/authorize";

			if (!string.IsNullOrEmpty(hint)) request.AddParameter("login_hint", hint);
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/info";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
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
