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
												"Headers: [ Accept: application/json\r\nUser-Agent: RestSharp/107\r\n ], Body: grant_type=authorization_code&client_id=yandex-client-id&client_secret=yandex-client-secret&code=code-from-Yandex&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fyandex%2F",
												"application/json",
												"eyJhY2Nlc3NfdG9rZW4iOiAieWFuZGV4LWFjY2Vzcy10b2tlbiIsICJleHBpcmVzX2luIjogMjgxNzE1ODcsICJyZWZyZXNoX3Rva2VuIjogIjE6MjE1MzU3MjEzOjMyNDgzMjQ6cmVmcmVzaF90b2tlbiIsICJ0b2tlbl90eXBlIjogImJlYXJlciJ9")
											.Add("[GET]https://login.yandex.ru/info",
												"Headers: [ Accept: application/json\r\nAuthorization: Bearer yandex-access-token\r\nUser-Agent: RestSharp/107\r\n ]",
												"application/json",
												"eyJpZCI6ICJ1c2VyLWlkIiwgImxvZ2luIjogIllMb2dpbiIsICJjbGllbnRfaWQiOiAieWFuZGV4LWNsaWVudC1pZCIsICJkaXNwbGF5X25hbWUiOiAiRk5hbWUgTE5hbWUiLCAicmVhbF9uYW1lIjogIkZOYW1lIExOYW1lIiwgImZpcnN0X25hbWUiOiAiRk5hbWUiLCAibGFzdF9uYW1lIjogIkxOYW1lIiwgInNleCI6ICJtYWxlIiwgImRlZmF1bHRfZW1haWwiOiAiZW1haWxAc2VydmljZSIsICJlbWFpbHMiOiBbImVtYWlsQHNlcnZpY2UiXSwgImRlZmF1bHRfYXZhdGFyX2lkIjogImF2YXRhci1pZCIsICJpc19hdmF0YXJfZW1wdHkiOiBmYWxzZSwgInBzdWlkIjogIjEuQUFYQUFYLkFBWEFBWEFBWEFBWEFBWEFBWC5BQVhBQVhBQVhBQVhBQVgifQ==")
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