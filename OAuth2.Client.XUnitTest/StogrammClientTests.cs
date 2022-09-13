using System.Text;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	public class StogrammClientTests : ClientTests<StogrammClientTests.StogrammService>
	{
		public class StogrammService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.Stogramm(new Options
				(
					"stogramm-client-id",
					"stogramm-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/stogramm/",
					new TestMessageHandler()
						.Add("[POST]https://stogramm.xyz/oauth/token",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=stogramm-client-id&client_secret=stogramm-client-secret&code=code-from-Stogramm&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fstogramm%2F",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""access_token"":""stogramm-access-token"",""token_type"":""Bearer"",""scope"":""read: accounts"",""created_at"":1651230000}"))
						.Add("[GET]https://stogramm.xyz/api/v1/accounts/verify_credentials",
							"Headers: [ Accept: application/json\nAuthorization: Bearer stogramm-access-token\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""id"":""user-id"",""username"":""username"",""acct"":""username"",""display_name"":""FName LName"",""locked"":true,""bot"":false,""discoverable"":false,""group"":false,""created_at"":""2022-01-01T00:00:00.000Z"",""note"":"""",""url"":""https://stogramm.xyz/@username"",""avatar"":""avatar-url"",""avatar_static"":""avatar-url"",""header"":""https://stogramm.xyz/headers/original/missing.png"",""header_static"":""https://stogramm.xyz/headers/original/missing.png"",""followers_count"":0,""following_count"":0,""statuses_count"":0,""last_status_at"":null,""source"":{""privacy"":""private"",""sensitive"":false,""language"":null,""note"":"""",""fields"":[],""follow_requests_count"":0},""emojis"":[],""fields"":[]}"))
				));
			}
		}

		public StogrammClientTests(StogrammService service)
			: base(service)
		{
		}

		protected override string ExpectedLoginURI		=> "https://stogramm.xyz/oauth/authorize?response_type=code&client_id=stogramm-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fstogramm%2f";

		protected override string? Email				=> null;
	}
}
