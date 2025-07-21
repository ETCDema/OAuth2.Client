#if MVC5
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;
#else
using Microsoft.AspNetCore.Http;
#endif

using OAuth2.Client.Models;

namespace OAuth2
{
	/// <summary>
	/// API клиента OAuth2 сервиса
	/// </summary>
	public interface IClient
	{
		/// <summary>Название сервиса</summary>
		string Name				{ get; }

		/// <summary>
		/// Получить URI на страницу авторизации сервиса асинхронно
		/// </summary>
		/// <param name="state">Дополнительные данные, возвращаемые сервисом</param>
		/// <param name="cancellationToken"></param>
		/// <returns>URI на страницу авторизации сервиса</returns>
		Task<string> GetLoginURIAsync(string? state = null, string? hint = null, string? redirectURI = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Получить URI на страницу авторизации сервиса синхронно
		/// </summary>
		/// <param name="state">Дополнительные данные, возвращаемые сервисом</param>
		/// <returns>URI на страницу авторизации сервиса</returns>
		string GetLoginURI(string? state = null, string? hint = null, string? redirectURI = null);

		/// <summary>
		/// Получить информацию о пользователе по данным, полученными со страницы сервиса авторизации асинхронно.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса.
		/// </summary>
		/// <param name="parameters">Параметры запроса</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
#if MVC5
		Task<IUserInfo> GetUserInfoAsync(NameValueCollection parameters, CancellationToken cancellationToken = default);
#else
		Task<IUserInfo> GetUserInfoAsync(IQueryCollection parameters, CancellationToken cancellationToken = default);
#endif

		/// <summary>
		/// Получить информацию о пользователе по данным, полученными со страницы сервиса авторизации синхронно.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса.
		/// </summary>
		/// <param name="parameters">Параметры запроса</param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
#if MVC5
		IUserInfo GetUserInfo(NameValueCollection parameters);
#else
		IUserInfo GetUserInfo(IQueryCollection parameters);
#endif
	}
}
