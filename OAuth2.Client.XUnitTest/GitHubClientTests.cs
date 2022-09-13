using System;
using System.Text;

using OAuth2.Client.Models;
using OAuth2.Client.XUnitTest.Core;

using RestSharp;

namespace OAuth2.Client.XUnitTest
{
	/// <summary>
	/// Тесты GitHub клиента
	/// </summary>
	public class GitHubClientTests : ClientTests<GitHubClientTests.GithubService>
	{
		public class GithubService : Service
		{
			protected override OAuth2Based<UserInfo> CreateClient()
			{
				return new For.GitHub(new Options(
					"github-client-id",
					"github-client-secret",
					string.Empty,
					"https://test.host/oauth2/by/github/",
					new TestMessageHandler()
						.Add("[POST]https://github.com/login/oauth/access_token",
							"Headers: [ Accept: application/json\nUser-Agent: RestSharp/107\n ], Body: grant_type=authorization_code&client_id=github-client-id&client_secret=github-client-secret&code=code-from-GitHub&redirect_uri=https%3A%2F%2Ftest.host%2Foauth2%2Fby%2Fgithub%2F",
							"application/json",
							Encoding.UTF8.GetBytes("{\"access_token\":\"github-access-token\",\"token_type\":\"bearer\",\"scope\":\"read: user, user: email\"}"))
						.Add("[GET]https://api.github.com/user",
							"Headers: [ Accept: application/json\nAuthorization: Bearer github-access-token\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes("{\"login\":\"GHLogin\",\"id\":\"user-id\",\"node_id\":\"node-id\",\"avatar_url\":\"avatar-url\",\"gravatar_id\":\"\",\"url\":\"https://api.github.com/users/GHLogin\",\"html_url\":\"https://github.com/GHLogin\",\"followers_url\":\"https://api.github.com/users/GHLogin/followers\",\"following_url\":\"https://api.github.com/users/GHLogin/following{/other_user}\",\"gists_url\":\"https://api.github.com/users/GHLogin/gists{/gist_id}\",\"starred_url\":\"https://api.github.com/users/GHLogin/starred{/owner}{/repo}\",\"subscriptions_url\":\"https://api.github.com/users/GHLogin/subscriptions\",\"organizations_url\":\"https://api.github.com/users/GHLogin/orgs\",\"repos_url\":\"https://api.github.com/users/GHLogin/repos\",\"events_url\":\"https://api.github.com/users/GHLogin/events{/privacy}\",\"received_events_url\":\"https://api.github.com/users/GHLogin/received_events\",\"type\":\"User\",\"site_admin\":false,\"name\":\"FName LName\",\"company\":null,\"blog\":\"\",\"location\":\"Russia\",\"email\":null,\"hireable\":null,\"bio\":null,\"twitter_username\":null,\"public_repos\":2,\"public_gists\":0,\"followers\":0,\"following\":0,\"created_at\":\"2016-05-15T10:40:37Z\",\"updated_at\":\"2022-01-14T16:18:36Z\",\"private_gists\":1,\"total_private_repos\":1,\"owned_private_repos\":1,\"disk_usage\":179,\"collaborators\":0,\"two_factor_authentication\":true,\"plan\":{\"name\":\"free\",\"space\":100500,\"collaborators\":0,\"private_repos\":10000}}"))
						.Add("[GET]https://api.github.com/user/emails",
							"Headers: [ Accept: application/json\nAuthorization: Bearer github-access-token\nUser-Agent: RestSharp/107\n ]",
							"application/json",
							Encoding.UTF8.GetBytes("[{\"email\":\"email@service\",\"primary\":true,\"verified\":true,\"visibility\":\"public\"}]"))
				));
			}
		}

		public GitHubClientTests(GithubService service)
			: base(service)
		{
		}

		protected override string ExpectedLoginURI		=> "https://github.com/login/oauth/authorize?response_type=code&client_id=github-client-id&redirect_uri=https%3a%2f%2ftest.host%2foauth2%2fby%2fgithub%2f";
	}
}
