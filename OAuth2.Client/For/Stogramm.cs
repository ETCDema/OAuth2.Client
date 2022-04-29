using OAuth2.Client.Models;

using RestSharp;

namespace OAuth2.Client.For
{
	public class Stogramm : Stogramm<UserInfo>
	{
		public Stogramm(Options opt)
			: base(opt)
		{
		}
	}

	public class Stogramm<TUserInfo> : MastodonBased<TUserInfo>
		where TUserInfo : UserInfo, new()
	{
		private RestClient? _client;

		public Stogramm(Options opt)
			: base(opt)
		{
		}

		public override string Name => "Stogramm";

		protected override RestClient NewAccessCodeClient()
		{
			return _client ??= new RestClient(NewOptions("https://stogramm.xyz"));
		}
	}
}
