using Microsoft.AspNetCore.Http;

using OAuth2.Client.Models;

namespace OAuth2
{
	public interface IClient
	{
		string Name				{ get; }

		Task<string> GetLoginURIAsync(string? state = null, CancellationToken cancellationToken = default);

		Task<UserInfo> GetUserInfoAsync(IQueryCollection parameters, CancellationToken cancellationToken = default);
	}
}
