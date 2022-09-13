using System;
using System.Text;

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
				(
					"ms-live-client-id",
					"ms-live-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/microsoftlive/",
					new TestMessageHandler()
						.Add("[POST]https://login.live.com/oauth20_token.srf",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=ms-live-client-id&client_secret=ms-live-client-secret&code=code-from-MicrosoftLive&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fmicrosoftlive%2F",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""token_type"":""bearer"",""expires_in"":3600,""scope"":""User.Read profile email openid"",""access_token"":""microsoft-live-access-token"",""id_token"":""microsoft-live-token-id""}"))
						.Add("[GET]https://graph.microsoft.com/v1.0/me",
							"Headers: [ Accept: application/json\nAuthorization: Bearer microsoft-live-access-token\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""@odata.context"":""https://graph.microsoft.com/v1.0/$metadata#users/$entity"",""displayName"":""FName LName"",""surname"":""LName"",""givenName"":""FName"",""id"":""user-id"",""userPrincipalName"":""email@service"",""businessPhones"":[],""jobTitle"":null,""mail"":null,""mobilePhone"":null,""officeLocation"":null,""preferredLanguage"":null}"))
						.Add("[GET]https://graph.microsoft.com/v1.0/me/photo/$value",
							"Headers: [ Authorization: Bearer microsoft-live-access-token\nAccept: application/json, text/json, text/x-json, text/javascript, application/xml, text/xml\nUser-Agent: RestSharp/107\n ]",
							"image/jpeg",
							Convert.FromBase64String("YXZhdGFyLWRhdGE="))
				));
			}
		}

		public MicrosoftLiveClientTests(MicrosoftLiveService service)
			: base(service)
		{
		}

		protected override string AvatarURL				=> "data:image/jpeg;base64,YXZhdGFyLWRhdGE=";

		protected override string ExpectedLoginURI		=> "https://login.live.com/oauth20_authorize.srf?response_type=code&client_id=ms-live-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fmicrosoftlive%2f";
	}
}