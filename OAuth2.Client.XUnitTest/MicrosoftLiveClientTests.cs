using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	/// <summary>
	/// Тесты MicrosoftLive клиента
	/// </summary>
	public class MicrosoftLiveClientTests : ClientTests<MicrosoftLiveClientTests.MicrosoftLiveService>
	{
		public class MicrosoftLiveService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.MicrosoftLive(new Options
				{
					ClientID            = "ms-live-client-id",
					ClientSecret        = "ms-live-client-secret",
					RedirectURI         = "https://test.host/oauth2/by/microsoftlive/",
					TestHandler         = new TestMessageHandler()
											.Add("[POST]https://login.live.com/oauth20_token.srf",
												"Headers: [ Accept: application/json\r\nUser-Agent: RestSharp/107\r\n ], Body: grant_type=authorization_code&client_id=ms-live-client-id&client_secret=ms-live-client-secret&code=code-from-MicrosoftLive&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fmicrosoftlive%2F",
												"application/json",
												"eyJ0b2tlbl90eXBlIjoiYmVhcmVyIiwiZXhwaXJlc19pbiI6MzYwMCwic2NvcGUiOiJVc2VyLlJlYWQgcHJvZmlsZSBlbWFpbCBvcGVuaWQiLCJhY2Nlc3NfdG9rZW4iOiJtaWNyb3NvZnQtbGl2ZS1hY2Nlc3MtdG9rZW4iLCJpZF90b2tlbiI6Im1pY3Jvc29mdC1saXZlLXRva2VuLWlkIn0=")
											.Add("[GET]https://graph.microsoft.com/v1.0/me",
												"Headers: [ Accept: application/json\r\nAuthorization: Bearer microsoft-live-access-token\r\nUser-Agent: RestSharp/107\r\n ]",
												"application/json",
												"eyJAb2RhdGEuY29udGV4dCI6Imh0dHBzOi8vZ3JhcGgubWljcm9zb2Z0LmNvbS92MS4wLyRtZXRhZGF0YSN1c2Vycy8kZW50aXR5IiwiZGlzcGxheU5hbWUiOiJGTmFtZSBMTmFtZSIsInN1cm5hbWUiOiJMTmFtZSIsImdpdmVuTmFtZSI6IkZOYW1lIiwiaWQiOiJ1c2VyLWlkIiwidXNlclByaW5jaXBhbE5hbWUiOiJlbWFpbEBzZXJ2aWNlIiwiYnVzaW5lc3NQaG9uZXMiOltdLCJqb2JUaXRsZSI6bnVsbCwibWFpbCI6bnVsbCwibW9iaWxlUGhvbmUiOm51bGwsIm9mZmljZUxvY2F0aW9uIjpudWxsLCJwcmVmZXJyZWRMYW5ndWFnZSI6bnVsbH0=")
											.Add("[GET]https://graph.microsoft.com/v1.0/me/photo/$value",
												"Headers: [ Authorization: Bearer microsoft-live-access-token\r\nAccept: application/json, text/json, text/x-json, text/javascript, *+json, application/xml, text/xml, *+xml, *\r\nUser-Agent: RestSharp/107\r\n ]",
												"image/jpeg",
												"YXZhdGFyLWRhdGE=")
				});
			}
		}

		public MicrosoftLiveClientTests(MicrosoftLiveService service)
			: base(service)
		{
		}

		protected override string AvatarURL			=> "data:image/jpeg;base64,YXZhdGFyLWRhdGE=";

		protected override string ExpectedLoginURI	=> "https://login.live.com/oauth20_authorize.srf?response_type=code&client_id=ms-live-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fmicrosoftlive%2f";
	}
}