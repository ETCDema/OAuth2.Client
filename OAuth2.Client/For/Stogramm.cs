using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	/// <summary>
	/// OAuth2 client for stogramm.xyz with base UserInfo model
	/// </summary>
	public class Stogramm : Stogramm<UserInfo>
	{
		public Stogramm(Options opt)
			: base(opt)
		{
		}
	}

	/// <summary>
	/// OAuth2 client for stogramm.xyz
	/// </summary>
	/// <typeparam name="TUserInfo">Type of UserInfo model</typeparam>
	public class Stogramm<TUserInfo> : MastodonBased<TUserInfo>
		where TUserInfo : IUserInfo, new()
	{
		private RestClient? _client;

		/// <inheritdoc/>
		public Stogramm(Options opt)
			: base(opt)
		{
		}

		/// <inheritdoc/>
		public override string Name => "Stogramm";

		/// <inheritdoc/>
		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://stogramm.xyz"));
		}
	}
}
