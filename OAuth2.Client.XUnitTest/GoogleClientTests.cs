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
				{
					ClientID            = "google-client-id",
					ClientSecret        = "google-client-secret",
					RedirectURI         = "https://test.host/oauth2/by/google/",
					TestHandler			= new TestMessageHandler()
											.Add("[POST]https://accounts.google.com/o/oauth2/token",
												"Headers: [ Accept: application/json\r\nUser-Agent: RestSharp/107\r\n ], Body: grant_type=authorization_code&client_id=google-client-id&client_secret=google-client-secret&code=code-from-Google&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fgoogle%2F",
												"application/json",
												"ewogICJhY2Nlc3NfdG9rZW4iOiAiZ29vZ2xlLWFjY2Vzcy10b2tlbiIsCiAgImV4cGlyZXNfaW4iOiAzNTk5LAogICJzY29wZSI6ICJodHRwczovL3d3dy5nb29nbGVhcGlzLmNvbS9hdXRoL3VzZXJpbmZvLnByb2ZpbGUgaHR0cHM6Ly93d3cuZ29vZ2xlYXBpcy5jb20vYXV0aC91c2VyaW5mby5lbWFpbCBvcGVuaWQiLAogICJ0b2tlbl90eXBlIjogIkJlYXJlciIsCiAgImlkX3Rva2VuIjogImdvb2dsZS1pZC10b2tlbiIKfQ==")
											.Add("[GET]https://www.googleapis.com/oauth2/v1/userinfo",
												"Headers: [ Accept: application/json\r\nAuthorization: Bearer google-access-token\r\nUser-Agent: RestSharp/107\r\n ]",
												"application/json",
												"ewogICJpZCI6ICJ1c2VyLWlkIiwKICAiZW1haWwiOiAiZW1haWxAc2VydmljZSIsCiAgInZlcmlmaWVkX2VtYWlsIjogdHJ1ZSwKICAibmFtZSI6ICJGTmFtZSBMTmFtZSIsCiAgImdpdmVuX25hbWUiOiAiRk5hbWUiLAogICJmYW1pbHlfbmFtZSI6ICJMTmFtZSIsCiAgInBpY3R1cmUiOiAiYXZhdGFyLXVybCIsCiAgImxvY2FsZSI6ICJydSIKfQ=="),
				});
			}
		}

		public GoogleClientTests(GoogleService service)
			: base(service)
		{
		}

		protected override string ExpectedLoginURI		=> "https://accounts.google.com/o/oauth2/auth?response_type=code&client_id=google-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fgoogle%2f";
	}
}