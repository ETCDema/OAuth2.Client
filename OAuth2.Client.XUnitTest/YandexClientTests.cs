using System.Text;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	/// <summary>
	/// Тесты Yandex клиента
	/// </summary>
	public class YandexClientTests : ClientTests<YandexClientTests.YandexService>
	{
		public class YandexService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.Yandex(new Options
				{
					ClientID            = "yandex-client-id",
					ClientSecret        = "yandex-client-secret",
					RedirectURI         = "https://test.host/oauth2/by/yandex/",
					TestHandler         = new TestMessageHandler()
											.Add("[POST]https://oauth.yandex.ru/token",
												"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=yandex-client-id&client_secret=yandex-client-secret&code=code-from-Yandex&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fyandex%2F",
												"application/json",
												Encoding.UTF8.GetBytes(@"{""access_token"": ""yandex-access-token"", ""expires_in"": 28171587, ""refresh_token"": ""1:215357213:3248324:refresh_token"", ""token_type"": ""bearer""}"))
											.Add("[GET]https://login.yandex.ru/info",
												"Headers: [ Accept: application/json\nAuthorization: Bearer yandex-access-token\nUser-Agent: RestSharp/107\n ]",
												"application/json",
												Encoding.UTF8.GetBytes(@"{""id"": ""user-id"", ""login"": ""YLogin"", ""client_id"": ""yandex-client-id"", ""display_name"": ""FName LName"", ""real_name"": ""FName LName"", ""first_name"": ""FName"", ""last_name"": ""LName"", ""sex"": ""male"", ""default_email"": ""email@service"", ""emails"": [""email@service""], ""default_avatar_id"": ""avatar-id"", ""is_avatar_empty"": false, ""psuid"": ""1.AAXAAX.AAXAAXAAXAAXAAXAAX.AAXAAXAAXAAXAAX""}"))
				});
			}
		}

		public YandexClientTests(YandexService service)
			: base(service)
		{
		}

		protected override string AvatarURL			=> "https://avatars.yandex.net/get-yapic/avatar-id/islands-200";

		protected override string ExpectedLoginURI	=> "https://oauth.yandex.ru/authorize?response_type=code&client_id=yandex-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fyandex%2f";
	}
}