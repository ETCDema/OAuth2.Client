using System.Security.Cryptography;
using System.Text;

#if MVC5
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Threading;
#else
using Microsoft.AspNetCore.Http;
#endif

using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client
{
	/// <summary>
	/// For OAuth2.1 based clients
	/// </summary>
	/// <typeparam name="TUserInfo">Тип модели пользоввателя</typeparam>
	public abstract class OAuth2_1Based<TUserInfo> : OAuth2Based<TUserInfo>
		where TUserInfo : IUserInfo
	{
		private readonly byte[] _codeBase;

		public OAuth2_1Based(Options opt)
			: base(opt)
		{
			_codeBase           = Encoding.ASCII.GetBytes($"[{opt.ClientID}]{opt.ClientSecret}");
		}

		/// <inheritdoc cref="OAuth2Based{TUserInfo}.GetLoginURIAsync"/>
		/// <param name="codeVerifier">Код для проверки запросов</param>
		public virtual Task<string> GetLoginURIAsync(string codeVerifier, string? state = null, string? hint = null, string? redirectURI = null, CancellationToken cancellationToken = default)
		{
			return Task.FromResult(GetLoginURI(codeVerifier, state, hint, redirectURI));
		}

		/// <inheritdoc cref="OAuth2Based{TUserInfo}.GetLoginURI"/>
		/// <param name="codeVerifier">Код для проверки запросов</param>
		public virtual string GetLoginURI(string codeVerifier, string? state = null, string? hint = null, string? redirectURI = null)
		{
			if (string.IsNullOrWhiteSpace(codeVerifier)) throw new ArgumentException("Required codeVerifier value", nameof(codeVerifier));

			var req             = new RestRequest()
									.AddParameter("response_type",  "code")
									.AddParameter("client_id",      Options.ClientID)
									.AddParameter("redirect_uri",   string.IsNullOrEmpty(redirectURI) ? Options.RedirectURI : redirectURI);

			if (!String.IsNullOrEmpty(Options.Scope))
				req.AddParameter("scope", Options.Scope);

			if (!String.IsNullOrEmpty(state))
				req.AddParameter("state", state);

			var codeVerifierArr = Encoding.ASCII.GetBytes(codeVerifier);

			using var hash      = SHA256.Create();
			var codeChallenge   = hash.ComputeHash(codeVerifierArr);
			var codeChallengeB64= Base64URL.Encode(codeChallenge);

			req.AddParameter("code_challenge", codeChallengeB64)
			   .AddParameter("code_challenge_method", "S256");

			InitLoginURIRequest(req, state, hint);

			return AccessCodeClient.BuildUri(req).ToString();
		}

		/// <inheritdoc/>
		/// <remarks>Только для совместимости! Нужно использовать <see cref="GetLoginURI(string, string?, string?, string?)"/></remarks>
		public override string GetLoginURI(string? state, string? hint = null, string? redirectURI = null)
		{
			return GetLoginURI(BuildCodeVerifier(state!), state, hint, redirectURI);
		}

		/// <inheritdoc cref="OAuth2Based{TUserInfo}.GetUserInfoAsync"/>
		/// <param name="codeVerifier">Код для проверки запросов</param>
#if MVC5
		public Task<TUserInfo> GetUserInfoAsync(string codeVerifier, NameValueCollection parameters, CancellationToken cancellationToken = default)
#else
		public Task<TUserInfo> GetUserInfoAsync(string codeVerifier, IQueryCollection parameters, CancellationToken cancellationToken = default)
#endif
		{
			// Контекст для получения данных
			var ctx             = CheckErrorAndSetState(new Ctx	// Проверяем, если ошибки и забираем переданное в GetLoginURIAsync значение state
			{
				GrantType       = "authorization_code",
				Params          = new TokensData(parameters)			// Приводим параметры к нужному виду
									.Add("code_verifier", codeVerifier)	// Добавим переданный код
			});

			return GetUserInfoAsync(ctx, cancellationToken);
		}

		/// <inheritdoc/>
		/// <remarks>Добавляет в контекст code_verifier на основе state, если такого в контексте еще нет.</remarks>
		protected override Ctx CheckErrorAndSetState(Ctx ctx)
		{
			ctx					= base.CheckErrorAndSetState(ctx);
			if (string.IsNullOrEmpty(ctx.Params.TryGet("code_verifier")))
				ctx.Params.Add("code_verifier", BuildCodeVerifier(ctx.Params.Get("state")));
			return ctx;
		}

		/// <inheritdoc/>
		protected override Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			ctx.Request.Parameters.RemoveParameter("client_secret");
			ctx.Request.AddParameter("code_verifier", ctx.Params.Get("code_verifier"));
			return base.QueryAccessTokenAsync(ctx, cancellationToken);
		}

		/// <summary>
		/// Генерация кода проверки запросов на основе ClientSecret и state.
		/// Менее безопасно, чем генерация случайных данных для каждого запроса, но нужна для совместимости со старым кодом.
		/// </summary>
		/// <param name="state">Дополнительные данные для генерации кода</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		protected virtual string BuildCodeVerifier(string state)
		{
			if (string.IsNullOrWhiteSpace(state)) throw new Exception("Required state value");

			var stateBytes      = Encoding.UTF8.GetBytes(state);
			var stateSize       = stateBytes.Length;
			var codeSize        = _codeBase.Length;
			var codeVerifier    = new byte[codeSize];

			if (codeSize<=stateSize)
			{
				for (int i = 0; i < codeSize; i++)
				{
					codeVerifier[i]     = (byte)(_codeBase[i] ^ stateBytes[i]);
				}
			} else
			{
				for (int i = 0; i < codeSize; i++)
				{
					codeVerifier[i]     = (byte)(_codeBase[i] ^ stateBytes[i % stateSize]);
				}
			}

			return Base64URL.Encode(codeVerifier);
		}
	}
}
