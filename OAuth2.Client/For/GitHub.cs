using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for GitHub with base UserInfo model
	/// </summary>
	public class GitHub : GitHub<UserInfo>
	{
		public GitHub(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for GitHub
	/// </summary>
	/// <remarks>
	/// Login hint not supported.
	/// </remarks>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class GitHub<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		private RestClient? _client;

		/// <inheritdoc/>
		public GitHub(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name =>	"GitHub";

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://github.com"));
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://api.github.com"));
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource    = "/login/oauth/authorize";
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/login/oauth/access_token";
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource        = "/user";
			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);

			// Если в основных данных нет почтового адреса, то попробуем получить его отдельно
			if (ctx.Content!=null && string.IsNullOrEmpty(ctx.Content.TryGet("email")))
			{
				// Сохраним текущие данные
				var mainRContent= ctx.RawContent;
				var mainContent	= ctx.Content!;

				// Получаем список почтовых адресов отдельным запросом
				ctx.Request     = new RestRequest("/user/emails");
				ctx.Request.AddHeader("Accept", "application/json");
				await AddAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
				await base.ExecuteAndVerifyAsync(ctx, false, cancellationToken).ConfigureAwait(false);

				// Добавим в основные данные найденный почтовый адрес
				var data        = ctx.Content!;
				mainContent.Add("#email", GitHub<TUserInfo>._find(data, ".primary") ?? GitHub<TUserInfo>._find(data, ".verified") ?? data.TryGet("0.email"));

				// Вернем обратно основные данные
				ctx.RawContent	= mainRContent;
				ctx.Content		= mainContent;
			}
		}

		/// <summary>
		/// Find value in token data
		/// </summary>
		/// <param name="data">Token data</param>
		/// <param name="attr">Name suffix</param>
		/// <returns></returns>
		private static string? _find(TokensData data, string attr)
		{
			var i				= 0;
			string? v;
			while ((v = data.TryGet(i+attr))!=null && v!="true") i++;
			return v!=null ? data.TryGet(i+".email") : null;
		}

		/// <inheritdoc/>
		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;
			var names           = data.TryGet("name")?.Split(' ');

			var u               = new TUserInfo()
			{
				ID              = data.TryGet("id")!,
				Email           = data.TryGet("#email") ?? data.TryGet("email"),
				FirstName       = names?.Length>0 ? names[0] : data.TryGet("login"),
				LastName        = names?.Length>1 ? names[1] : "",
				AvatarURL		= data.TryGet("avatar_url")
			};

			return u;
		}
	}
}
