using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for id.vk.ru with base UserInfo model
	/// </summary>
	public class VKID : VKID<UserInfo>
	{
		public VKID(VKIDOptions opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for id.vk.ru
	/// </summary>
	/// <remarks>
	/// Login hint not supported.
	/// </remarks>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class VKID<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient? _client;
		private readonly bool _usePublicInfo;

		/// <inheritdoc/>
		public VKID(VKIDOptions opt)
			: base(opt)
		{
			_usePublicInfo      = opt.UsePublicInfo;
		}

		/// <inheritdoc/>
		public override string Name => "VKID";

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://id.vk.ru"));
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
		protected override void InitLoginURIRequest(RestRequest request, string? state, string? hint)
		{
			var codeVerifier    = Encoding.ASCII.GetBytes(_getCodeVerifier(state!));

			using var hash      = SHA256.Create();
			var codeChallenge   = hash.ComputeHash(codeVerifier);
			var codeChallengeB64= _toBase64url(codeChallenge);

			request.Resource    = "/authorize";
			request.AddParameter("code_challenge", codeChallengeB64)
				   .AddParameter("code_challenge_method", "S256");
		}

		private static string _toBase64url(byte[] data)
		{
			return Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
		}

		private string _getCodeVerifier(string state)
		{
			return string.IsNullOrEmpty(state)
				 ? throw new Exception("Required state value")
				 : _toBase64url(Encoding.UTF8.GetBytes(state));
		}

		/// <inheritdoc/>
		protected override async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			var codeVerifier    = _getCodeVerifier(ctx.Params.Get("state"));

			ctx.Request.Resource= "/oauth2/auth";
			ctx.Request.Parameters.RemoveParameter("client_secret");
			ctx.Request.AddParameter("device_id", ctx.Params.Get("device_id"));
			ctx.Request.AddParameter("code_verifier", codeVerifier);

			await base.QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
			var data            = ctx.Content;
			if (data!=null)
			{
				// Идентификатор пользователя и его почта придут вместе с access token - сохраним их
				ctx.Params.Add("#user-id", data.Get("user_id"));
				ctx.Params.Add("id_token", data.Get("id_token"));
			}
		}

		/// <inheritdoc/>
		protected override async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Resource= _usePublicInfo ? "/oauth2/public_info" : "/oauth2/user_info";
			ctx.Request.Method  = Method.Post;
			ctx.Request.AddParameter("client_id",	 Options.ClientID);

			if (_usePublicInfo)
			{
				ctx.Request.AddParameter("id_token", ctx.Params.Get("id_token"));
				await base.ExecuteAndVerifyAsync(ctx, false, cancellationToken).ConfigureAwait(false);
			} else
			{
				await base.QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);
			}
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

			var avatarUrl       = data.TryGet("user.avatar");
			if (!string.IsNullOrEmpty(avatarUrl))
				avatarUrl       = new Regex("&cs=\\d+[^&]+").Replace(avatarUrl, "");

			var u               = new TUserInfo()
			{
				ID              = data.TryGet("user.user_id") ?? data.Get("#user-id"),
				Email           = data.TryGet("user.email"),
				FirstName       = data.TryGet("user.first_name"),
				LastName        = data.TryGet("user.last_name"),
				AvatarURL       = avatarUrl,
			};

			return u;
		}
	}
}
