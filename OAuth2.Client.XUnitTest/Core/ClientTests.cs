#if NET4
using Microsoft.AspNetCore.Http.Internal;
#else
using Microsoft.AspNetCore.Http;
#endif

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

using OAuth2.Client.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

using Xunit;


namespace OAuth2.Client.XUnitTest.Core
{
	/// <summary>
	/// Set of client base tests
	/// 
	/// Набор базовых тестов OAuth2 клиента
	/// </summary>
	/// <typeparam name="TService">
	/// Shared service class
	/// 
	/// Сервисный singleton класс для тестов
	/// </typeparam>
	public abstract class ClientTests<TService> : IClassFixture<TService>
		where TService : ClientTests<TService>.Service
	{
		/// <summary>
		/// Shared service base class
		/// 
		/// Базовый сервисный singleton класс для тестов
		/// </summary>
		public abstract class Service
		{
			public Service()
			{
				Client          = CreateClient();
				Assert.NotNull(Client.Options.TestHandler);
			}

			/// <summary>
			/// Экземпляр клиента, используемый в тестах
			/// </summary>
			public OAuth2Based<UserInfo> Client { get; }

			/// <summary>
			/// Метод создания экземпляра клиента
			/// </summary>
			/// <returns>Экземпляр клиента для тестов</returns>
			protected abstract OAuth2Based<UserInfo> CreateClient();
		}

		/// <summary>
		/// Конструктор
		/// </summary>
		/// <param name="svc">Сервисный singleton объект для тестов</param>
		public ClientTests(TService svc)
		{
			Client              = svc.Client;
		}

		/// <summary>
		/// Экземпляр клиента для тестов
		/// </summary>
		protected OAuth2Based<UserInfo> Client	{ get; }

		/// <summary>
		/// Получить URL для авторизации на OAuth2 сервисе
		/// </summary>
		[Fact]
		public async Task GetLoginURI()
		{
			var url             = await Client.GetLoginURIAsync(GetState());
#if FIX_REST_ENCODING
			Assert.Equal(ExpectedLoginURI.Replace("%3a", ":"), url);
#else
			Assert.Equal(ExpectedLoginURI, url);
#endif
		}

		protected virtual string? GetState()
		{
			return null;
		}

		/// <summary>
		/// Получить информацию о пользователе
		/// </summary>
		[Fact]
		public async Task GetUserInfo()
		{
			var client          = Client;
			var query           = QueryHelpers.ParseQuery("code=code-from-"+client.Name);
			InitReturnQuery(query);
			var callbackData    = new QueryCollection(query);
			var userInfo        = await client.GetUserInfoAsync(callbackData);

			Assert.Equal(client.Name,		userInfo.ProviderName);
			Assert.Equal("user-id",			userInfo.ID);
			Assert.Equal(Email,				userInfo.Email);
			Assert.Equal("FName",			userInfo.FirstName);
			Assert.Equal("LName",			userInfo.LastName);
			Assert.Equal(AvatarURL,			userInfo.AvatarURL);
		}

		protected virtual void InitReturnQuery(Dictionary<string, StringValues> query)
		{
		}

		/// <summary>
		/// Ожидаемое значение URL для авторизации на OAuth2 сервисе
		/// </summary>
		protected abstract string ExpectedLoginURI { get; }

		/// <summary>
		/// Ожидаемое значение Email, данные могут отличаться от сервиса к сервису
		/// </summary>
		protected virtual string? Email					=> "email@service";

		/// <summary>
		/// Ожидаемое значение URL аватара, данные могут отличаться от сервиса к сервису
		/// </summary>
		protected virtual string? AvatarURL				=> "avatar-url";
	}
}
