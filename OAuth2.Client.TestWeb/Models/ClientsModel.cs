using OAuth2.Client.Models;

namespace OAuth2.Client.TestWeb.Models
{
	public class ClientsModel
	{
		public ClientsModel(IEnumerable<IClient> clients)
		{
			Clients				= new List<IClient>(clients);
			Log                 = new List<string>();
		}

		public IList<IClient> Clients	{ get; set; }

		public IClient? Current			{ get; set; }

		public string? StateValue		{ get; set; }

		public string? Error			{ get; set; }

		public UserInfo? User			{ get; set; }

		public IList<string> Log		{ get; }
	}
}
