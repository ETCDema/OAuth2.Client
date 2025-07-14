using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for vk.com with base UserInfo model
	/// </summary>
	public class VK : VK<UserInfo>
	{
		public VK(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for vk.com
	/// </summary>
	/// <remarks>
	/// Login hint not supported.
	/// </remarks>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class VK<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient? _client;

		/// <inheritdoc/>
		public VK(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name		=> "VK";

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://oauth.vk.com"));
		}

		/// <inheritdoc/>
		protected override RestClient NewAccessTokenClient()
		{
			return NewAccessCodeClient();
		}

		/// <inheritdoc/>
		protected override RestClient NewUserInfoClient()
		{
			return new RestClient(NewOptions("https://api.vk.com"));
		}

		/// <inheritdoc/>
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			request.Resource	= "/authorize";
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource= "/access_token";
			ctx.Request.Method	= Method.Get;
			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
			var data			= ctx.Content;
			if (data!=null)
			{
				// Идентификатор пользователя и его почта придут вместе с access token - сохраним их
				ctx.Params.Add("#user-id",	data.Get("user_id"))
						  .Add("#email",	data.TryGet("email"));
			}
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource= "/method/users.get";
			ctx.Request.Method	= Method.Post;
			ctx.Request	.AddParameter("v",			"5.131")
						.AddParameter("user_ids",	ctx.Params.Get("#user-id"))
						.AddParameter("fields",		"first_name,last_name,has_photo,photo_max_orig");

			await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <inheritdoc/>
		protected override Task AddAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.AddParameter("access_token", ctx.AccessToken);
			return Task.CompletedTask;
		}

		/// <inheritdoc/>
		protected override TUserInfo ParseUserInfo(Ctx ctx)
		{
			var data            = ctx.Content!;

			var u               = new TUserInfo()
			{
				ID              = data.TryGet("response.0.id")!,
				Email           = ctx.Params.TryGet("#email"),
				FirstName       = data.TryGet("response.0.first_name"),
				LastName        = data.TryGet("response.0.last_name"),
				AvatarURL       = data.TryGet("response.0.photo_max_orig"),
			};

			return u;
		}
	}
}
