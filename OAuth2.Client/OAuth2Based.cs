using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

using Microsoft.AspNetCore.Http;

using OAuth2.Client.Models;

using RestSharp;
using RestSharp.Authenticators.OAuth2;

namespace OAuth2.Client
{
	/// <summary>
	/// For OAuth2 based clients
	/// </summary>
	public abstract class OAuth2Based<TUserInfo> : IClient
		where TUserInfo : UserInfo
	{
		protected static readonly string ACCESSTOKENKEY	= "access_token";
		protected static readonly string REFRESHTOKENKEY= "refresh_token";
		protected static readonly string EXPIRESKEY		= "expires_in";
		protected static readonly string TOKENTYPEKEY	= "token_type";
		protected static readonly string USERAGENT      = "RestSharp/107";

		/// <summary>Настройки клиента</summary>
		protected internal readonly Options Options;

		/// <summary>Транспортный клиент для получения кода</summary>
		protected readonly RestClient AccessCodeClient;

		/// <summary>Транспортный клиент для получения токена доступа</summary>
		protected readonly RestClient AccessTokenClient;

		/// <summary>Транспортный клиент для получения данных пользователя</summary>
		protected readonly RestClient UserInfoClient;

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="opt"></param>
		public OAuth2Based(Options opt)
		{
			Options             = opt;

			AccessCodeClient	= NewAccessCodeClient();
			AccessTokenClient   = NewAccessTokenClient();
			UserInfoClient		= NewUserInfoClient();
		}

		/// <summary>
		/// Имя сервиса
		/// </summary>
		public abstract string Name		{ get; }

		/// <summary>
		/// Создать транспортный клиент для получения кода
		/// </summary>
		/// <returns></returns>
		protected abstract RestClient NewAccessCodeClient();

		/// <summary>
		/// Создать транспортный клиент для получения токена доступа
		/// </summary>
		/// <returns></returns>
		protected abstract RestClient NewAccessTokenClient();

		/// <summary>
		/// Создать транспортный клиент для получения данных пользователя
		/// </summary>
		/// <returns></returns>
		protected abstract RestClient NewUserInfoClient();

		/// <summary>
		/// Создать экземпляр настроек RestClient с указанным baseUrl
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		protected virtual RestClientOptions NewOptions(string baseUrl)
		{
			if (string.IsNullOrEmpty(baseUrl)) throw new ArgumentNullException(nameof(baseUrl));

			return new RestClientOptions
			{
				UserAgent				= USERAGENT,
				BaseUrl					= new Uri(baseUrl),
				ConfigureMessageHandler	= Options.TestHandler!=null
										? h => Options.TestHandler
										: null
			};
		}

		/// <summary>
		/// Получить URI на страницу авторизации сервиса
		/// </summary>
		/// <param name="state">Дополнительные данные, возвращаемые сервисом</param>
		/// <param name="cancellationToken"></param>
		/// <returns>URI на страницу авторизации сервиса</returns>
		public virtual Task<string> GetLoginURIAsync(string? state = null, CancellationToken cancellationToken = default)
		{
			var req				= new RestRequest()
									.AddParameter("response_type",	"code")
									.AddParameter("client_id",		Options.ClientID)
									.AddParameter("redirect_uri",	Options.RedirectURI);

			if (!String.IsNullOrEmpty(Options.Scope))
				req.AddParameter("scope", Options.Scope);

			if (!String.IsNullOrEmpty(state))
				req.AddParameter("state", state);

			InitLoginURIRequest(req, state);

			return Task.FromResult(AccessCodeClient.BuildUri(req).ToString());
		}

		/// <summary>
		/// Инициализировать запрос для входа на страницу авторизации сервиса
		/// </summary>
		/// <param name="request">Запрос для инициализации</param>
		protected abstract void InitLoginURIRequest(RestRequest request, string? state);

		/// <summary>
		/// Получить информацию о пользователе по данным, полученными со страницы сервиса авторизации.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса.
		/// </summary>
		/// <param name="parameters">Параметры запроса</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
		public Task<TUserInfo> GetUserInfoAsync(IQueryCollection parameters, CancellationToken cancellationToken = default)
		{
			// Контекст для получения данных
			var ctx 			= CheckErrorAndSetState(new Ctx	// Проверяем, если ошибки и забираем переданное в GetLoginURIAsync значение state
			{
				GrantType       = "authorization_code",
				Params          = new TokensData(parameters)    // Приводим параметры к нужному виду
			});

			return GetUserInfoAsync(ctx, cancellationToken);
		}

		/// <summary>
		/// Получить информацию о пользователе по данным, полученными со страницы сервиса авторизации.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса.
		/// 
		/// Работает аналогично <see cref="GetUserInfoAsync(IQueryCollection, CancellationToken)"/>
		/// Основное отличие - отслеживание запроса/ответа, для использования в тестах.
		/// </summary>
		/// <param name="parameters">Параметры запроса</param>
		/// <param name="onReq">Обработчик запроса/ответа (Request.URL, Request.Headers+Request.Body, Response.Content)</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
		internal virtual Task<TUserInfo> GetUserInfoAsync(IQueryCollection parameters, Action<string, string?, string?> onReq, CancellationToken cancellationToken = default)
		{
			// Контекст для получения данных
			var ctx 			= CheckErrorAndSetState(new _dumpCtx(onReq)	// Проверяем, если ошибки и забираем переданное в GetLoginURIAsync значение state
			{
				GrantType       = "authorization_code",
				Params          = new TokensData(parameters)    // Приводим параметры к нужному виду
			});

			return GetUserInfoAsync(ctx, cancellationToken);
		}

		/// <summary>
		/// Реализация получения информации о пользователе по данным, полученными со страницы сервиса авторизации.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса и доступны они через ctx.Params.
		/// </summary>
		/// <param name="ctx">Контекст</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
		protected virtual async Task<TUserInfo> GetUserInfoAsync(Ctx ctx, CancellationToken cancellationToken)
		{
			// Запрос токена доступа, проверка
			await _queryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);

			// Запрос информации о пользователе
			ctx.Client          = UserInfoClient;
			ctx.Request         = new RestRequest();
			ctx.Request.AddHeader("Accept", "application/json");
			ctx.Response		= null;
			ctx.Content         = null;
			ctx.RawContent		= null;
			await QueryUserInfoAsync(ctx, cancellationToken).ConfigureAwait(false);

			// Раскладка данных в модель
			var result          = ParseUserInfo(ctx);
			result.ProviderName = Name;

			return result;
		}

		/// <summary>
		/// Запрос токена доступа к информации пользователя.
		/// В случае успешного запроса будут заполнены свойства AccessToken, RefreshToken, TokenType, ExpiresAt
		/// </summary>
		/// <param name="ctx">Контекст</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		protected virtual async Task QueryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			await ExecuteAndVerifyAsync(ctx, false, cancellationToken).ConfigureAwait(false);

			var content         = ctx.Content ?? throw new Exception("ctx.Content==null");

			ctx.TokenType       = content.TryGet(TOKENTYPEKEY);
			ctx.AccessToken     = content.TryGet(ACCESSTOKENKEY);
			if (String.IsNullOrEmpty(ctx.AccessToken)) throw new Exception(ACCESSTOKENKEY+"==null");

			if (ctx.GrantType != "refresh_token")
				ctx.RefreshToken= content.TryGet(REFRESHTOKENKEY);

			if (Int32.TryParse(content.TryGet(EXPIRESKEY), out int expiresIn))
				ctx.ExpiresAt   = DateTime.Now.AddSeconds(expiresIn);
		}

		protected virtual async Task AddAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			if ("bearer".Equals(ctx.TokenType, StringComparison.OrdinalIgnoreCase))
				ctx.Request.AddHeader("Authorization", "Bearer " + ctx.AccessToken);
			else
				await new OAuth2UriQueryParameterAuthenticator(ctx.AccessToken!).Authenticate(ctx.Client, ctx.Request).ConfigureAwait(false);
		}

		/// <summary>
		/// Запрос информации о пользователе с использованием AccessToken
		/// </summary>
		/// <param name="ctx">Контекст</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		protected virtual async Task QueryUserInfoAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			await AddAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
			await ExecuteAndVerifyAsync(ctx, false, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Раскладка полученных данных в модель, обычно данные берутся из ctx.Content
		/// </summary>
		/// <param name="ctx">Контекст</param>
		/// <returns>Заполненная модель информации о пользователе</returns>
		protected abstract TUserInfo ParseUserInfo(Ctx ctx);

		/// <summary>
		/// Проверить пришедшие данные на наличие ошибки и вернуть значение state, переданное в GetLoginURIAsync
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		protected virtual Ctx CheckErrorAndSetState(Ctx ctx)
		{
			var data            = ctx.Params;
			var error			= data.TryGet("error");
			if (!string.IsNullOrEmpty(error)) throw new Exception(error);
			ctx.State			= data.TryGet("state");
			return ctx;
		}

		/// <summary>
		/// Начальная инициализация запроса AccessToken и запуск его получения
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="parameters"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task _queryAccessTokenAsync(Ctx ctx, CancellationToken cancellationToken = default)
		{
			if (ctx.Params==null) throw new NullReferenceException("ctx.Params==null");

			ctx.Client			= AccessTokenClient;
			var request			= ctx.Request = new RestRequest();
			request.Method		= Method.Post;
			request.AddHeader("Accept", "application/json");
			request.AddParameter("grant_type",		ctx.GrantType)
					.AddParameter("client_id",		Options.ClientID)
					.AddParameter("client_secret",	Options.ClientSecret);

			if (ctx.GrantType == "refresh_token")
				request	.AddParameter("refresh_token",	ctx.Params.Get("refresh_token"));
			else
				request	.AddParameter("code",			ctx.Params.Get("code"))
						.AddParameter("redirect_uri",	Options.RedirectURI);

			await QueryAccessTokenAsync(ctx, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Выполнить запрос ctx.Request, проверить на наличие результата и заполнить ctx.Response, ctx.RawContent и ctx.Content
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		protected virtual async Task ExecuteAndVerifyAsync(Ctx ctx, bool mayEmpty, CancellationToken cancellationToken = default)
		{
			var response        = await ctx.Client.ExecuteAsync(ctx.Request, cancellationToken).ConfigureAwait(false);
			if (response.ErrorException!=null)
				throw response.ErrorException;

			if (!mayEmpty && (response.RawBytes==null || response.RawBytes.Length==0) || (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created))
				throw new Exception("Invalid response: "+response.StatusCode+" -> "+response.Content);

			ctx.Response        = response;
			if (IsTextContent(response.ContentType))
			{
				ctx.RawContent  = response.Content;
				ctx.Content     = ParseTokenResponse(ctx.RawContent);

				if (ctx.Content!=null)
				{
					var err     = ctx.Content.TryGet("error_description") ??
								  ctx.Content.TryGet("error") ?? 
								  ctx.Content.TryGet("error.error_msg");
					if (!string.IsNullOrEmpty(err)) throw new Exception(err);
				}
			} else if (response.RawBytes!=null)
			{
				var encoded     = Convert.ToBase64String(response.RawBytes);
				var ct          = response.ContentType;

				if (ct=="image/jpeg" || ct=="image/png")
					encoded		= "data:"+ct+";base64,"+encoded;

				ctx.RawContent  = encoded;
				ctx.Content     = null;
			} else
			{
				ctx.RawContent  = null;
				ctx.Content     = null;
			}
		}

		protected virtual bool IsTextContent(string? ct)
		{
			return ct!=null && (ct.StartsWith("text/") || ct.EndsWith("/json"));
		}

		/// <summary>
		/// Разобрать данные из строки в объект
		/// </summary>
		/// <param name="content">Данные в строке, может быть как JSON так и FormData</param>
		/// <returns></returns>
		protected virtual TokensData? ParseTokenResponse(string? content)
		{
			if (String.IsNullOrEmpty(content)) return null;

			try
			{
				// response can be sent in JSON format
				return new TokensData(JsonDocument.Parse(content).RootElement);
			} catch (JsonException)
			{
				// or it can be in "query string" format (param1=val1&param2=val2)
				return new TokensData(HttpUtility.ParseQueryString(content));
			}
		}

		/// <summary>Контекст запроса данных</summary>
		protected class Ctx
		{
#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

			/// <summary>Клиент для выполнения запроса</summary>
			public RestClient Client;

			/// <summary>Запрос</summary>
			public virtual RestRequest Request			{ get; set; }

			/// <summary>Параметры для callback</summary>
			public TokensData Params;

#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

			/// <summary>Ответ</summary>
			public virtual RestResponse? Response		{ get; set; }

			/// <summary>Необработанный ответ</summary>
			public string? RawContent;

			/// <summary>Ответ, разложенный в структуру</summary>
			public TokensData? Content;

			/// <summary>Тип доступа</summary>
			public string? GrantType;

			/// <summary>Токен доступа</summary>
			public string? AccessToken;

			/// <summary>Токен для обновления токена доступа</summary>
			public string? RefreshToken;

			/// <summary>Тип токена</summary>
			public string? TokenType;

			/// <summary>Срок жизни токена</summary>
			public DateTime? ExpiresAt;

			/// <summary>Значение, переданное в GetLoginURIAsync</summary>
			public string? State;
		}

		/// <summary>
		/// Контекст запроса данных с дампом запросов
		/// </summary>
		private class _dumpCtx : Ctx
		{
			private readonly Action<string, string?, string?> _onReq;

			public _dumpCtx(Action<string, string?, string?> onReq)
				: base()
			{
				_onReq			= onReq;
			}

			public override RestRequest Request
			{
				get => base.Request;
				set
				{
					base.Request= value;
					value.OnBeforeRequest = req =>
					{
						var reqTxt		= "Headers: [ "+req.Headers.ToString()+"User-Agent: "+USERAGENT+"\n ]";
						if (req.Content!=null)
							reqTxt		+= ", Body: "+req.Content.ReadAsStringAsync().Result;

						_onReq(">>>", string.Concat("[", req.Method, "]", req.RequestUri), reqTxt);

						return ValueTask.CompletedTask;
					};
				}
			}

			public override RestResponse? Response
			{
				get => base.Response;
				set
				{
					base.Response		= value;
					if (value!=null)
					{
						var contentType	= value.ContentType;
						var content     = OAuth2Based<TUserInfo>._dumpCtx._toString(contentType, value.RawBytes);

						_onReq("<<<", contentType, content);
					}
				}
			}

			private static string? _toString(string contentType, byte[]? raw)
			{
				if (raw==null || raw.Length==0) return null;

				if (contentType!=null && contentType.ToLower().Contains("application/json"))
					return Encoding.UTF8.GetString(raw);
				else
					return Convert.ToBase64String(raw);
			}
		}


		async Task<UserInfo> IClient.GetUserInfoAsync(IQueryCollection parameters, CancellationToken cancellationToken)
		{
			return await GetUserInfoAsync(parameters, cancellationToken);
		}
	}
}
