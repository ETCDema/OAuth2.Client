using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Primitives;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	public class VKIDClientTests : ClientTests<VKIDClientTests.VKIDService>
	{
		public class VKIDService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.VKID(new VKIDOptions
				(
					"vk-client-id",
					"vk-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/vkid/",
					new TestMessageHandler()
						.Add("[POST]https://id.vk.ru/oauth2/auth",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=vk-client-id&code=code-from-VKID&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fvkid%2F&device_id=device-id-from-vk&code_verifier=U3RhdGUtYjM5NDI0YzYtYmVjNy00ODM2LWI1YWMtY2VlNjU1MzZlOTdj",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""access_token"": ""vk-access-token"",""id_token"": ""vk-id-token"",""token_type"": ""Bearer"",""expires_in"": 3600,""user_id"": 11223344,""state"": """",""scope"": ""vkid.personal_info""}"))
						.Add("[POST]https://id.vk.ru/oauth2/user_info",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: client_id=vk-client-id&access_token=vk-access-token",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""user"": {""user_id"": ""user-id"",""first_name"": ""FName"",""last_name"": ""LName"",""avatar"": ""avatar-url&cs=50x50"",""email"":""email@service"", ""sex"": 2,""verified"": false,""birthday"": ""12.12.1912""}}"))
				));
			}
		}

		public VKIDClientTests(VKIDService service)
			: base(service)
		{
		}

		protected override string? GetState()
		{
			return "State-b39424c6-bec7-4836-b5ac-cee65536e97c";
		}

		protected override void InitReturnQuery(Dictionary<string, StringValues> query)
		{
			query.Add("state", GetState());
			query.Add("device_id", "device-id-from-vk");
		}

		protected override string ExpectedLoginURI => "https://id.vk.ru/authorize?response_type=code&client_id=vk-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fvkid%2f&state=State-b39424c6-bec7-4836-b5ac-cee65536e97c&code_challenge=VFdrKRwhGT0mY7NJc6ppdEf1LNVg-pGNzuhtyD8gC0Q&code_challenge_method=S256";
	}
}
