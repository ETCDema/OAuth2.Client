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
				{
					ClientID            = "vk-client-id",
					ClientSecret        = "vk-client-secret",
					RedirectURI         = "https://test.host/oauth2/by/vk/",
					TestHandler         = new TestMessageHandler()
											.Add("[GET]https://oauth.vk.com/access_token?grant_type=authorization_code&client_id=vk-client-id&client_secret=vk-client-secret&code=code-from-VK&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fvk%2f",
												"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ]",
												"application/json",
												"eyJhY2Nlc3NfdG9rZW4iOiJ2ay1hY2Nlc3MtdG9rZW4iLCJleHBpcmVzX2luIjo4NjQwMCwidXNlcl9pZCI6InVzZXItaWQiLCJlbWFpbCI6ImVtYWlsQHNlcnZpY2UifQ==")
											.Add("[POST]https://api.vk.com/method/users.get",
												"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: v=5.131&user_ids=user-id&fields=first_name%2Clast_name%2Chas_photo%2Cphoto_max_orig&access_token=vk-access-token",
												"application/json",
												"eyJyZXNwb25zZSI6W3siaWQiOiJ1c2VyLWlkIiwiZmlyc3RfbmFtZSI6IkZOYW1lIiwibGFzdF9uYW1lIjoiTE5hbWUiLCJjYW5fYWNjZXNzX2Nsb3NlZCI6dHJ1ZSwiaXNfY2xvc2VkIjpmYWxzZSwicGhvdG9fbWF4X29yaWciOiJhdmF0YXItdXJsIiwiaGFzX3Bob3RvIjoxfV19")
				});
			}
		}

		public VKClientTests(VKService service)
			: base(service)
		{
		}


		protected override string ExpectedLoginURI => "https://oauth.vk.com/authorize?response_type=code&client_id=vk-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fvk%2f";
	}
}
