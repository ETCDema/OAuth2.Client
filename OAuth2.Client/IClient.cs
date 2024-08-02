using Microsoft.AspNetCore.Http;

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
		/// Получить URI на страницу авторизации сервиса
		/// </summary>
		/// <param name="state">Дополнительные данные, возвращаемые сервисом</param>
		/// <param name="cancellationToken"></param>
		/// <returns>URI на страницу авторизации сервиса</returns>
		Task<string> GetLoginURIAsync(string? state = null, string? hint = null, CancellationToken cancellationToken = default);

		/// <summary>
		/// Получить информацию о пользователе по данным, полученными со страницы сервиса авторизации.
		/// Обычно передается code, но могут быть варианты, поэтому требуются все параметры запроса.
		/// </summary>
		/// <param name="parameters">Параметры запроса</param>
		/// <param name="cancellationToken"></param>
		/// <returns>Информация о пользователе, выполнившем вход через сервис авторизации.</returns>
		Task<IUserInfo> GetUserInfoAsync(IQueryCollection parameters, CancellationToken cancellationToken = default);
	}
}
