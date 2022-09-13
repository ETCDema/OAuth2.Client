using System.Text;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	/// <summary>
	/// Тесты Google клиента
	/// </summary>
	public class GoogleClientTests : ClientTests<GoogleClientTests.GoogleService>
	{
		public class GoogleService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.Google(new Options
				(
					"google-client-id",
					"google-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/google/",
					new TestMessageHandler()
						.Add("[POST]https://accounts.google.com/o/oauth2/token",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=google-client-id&client_secret=google-client-secret&code=code-from-Google&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fgoogle%2F",
							"application/json",
							Encoding.UTF8.GetBytes(@"{
								""access_token"": ""google-access-token"",
								""expires_in"": 3599,
								""scope"": ""https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email openid"",
								""token_type"": ""Bearer"",
								""id_token"": ""google-id-token""
							}"))
						.Add("[GET]https://www.googleapis.com/oauth2/v1/userinfo",
							"Headers: [ Accept: application/json\nAuthorization: Bearer google-access-token\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes(@"{
								""id"": ""user-id"",
								""email"": ""email@service"",
								""verified_email"": true,
								""name"": ""FName LName"",
								""given_name"": ""FName"",
								""family_name"": ""LName"",
								""picture"": ""avatar-url"",
								""locale"": ""ru""
							}"))
				));
			}
		}

		public GoogleClientTests(GoogleService service)
			: base(service)
		{
		}

		protected override string ExpectedLoginURI		=> "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=google-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fgoogle%2f";
	}
}