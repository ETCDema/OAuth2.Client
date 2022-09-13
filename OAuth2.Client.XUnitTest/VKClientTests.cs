using System.Text;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

namespace OAuth2.Client.XUnitTest
{
	public class VKClientTests : ClientTests<VKClientTests.VKService>
	{
		public class VKService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.VK(new Options
				(
					"vk-client-id",
					"vk-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/vk/",
					new TestMessageHandler()
						.Add("[GET]https://oauth.vk.com/access_token?grant_type=authorization_code&client_id=vk-client-id&client_secret=vk-client-secret&code=code-from-VK&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fvk%2f",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""access_token"":""vk-access-token"",""expires_in"":86400,""user_id"":""user-id"",""email"":""email@service""}"))
						.Add("[POST]https://api.vk.com/method/users.get",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: v=5.131&user_ids=user-id&fields=first_name%2Clast_name%2Chas_photo%2Cphoto_max_orig&access_token=vk-access-token",
							"application/json",
							Encoding.UTF8.GetBytes(@"{""response"":[{""id"":""user-id"",""first_name"":""FName"",""last_name"":""LName"",""can_access_closed"":true,""is_closed"":false,""photo_max_orig"":""avatar-url"",""has_photo"":1}]}"))
				));
			}
		}

		public VKClientTests(VKService service)
			: base(service)
		{
		}

		protected override string ExpectedLoginURI		=> "https://oauth.vk.com/authorize?response_type=code&client_id=vk-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fvk%2f";
	}
}
