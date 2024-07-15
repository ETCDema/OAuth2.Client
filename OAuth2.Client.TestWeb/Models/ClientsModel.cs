using OAuth2.Client.Models;

namespace OAuth2.Client.TestWeb.Models
{
	/// <summary>
	/// Client view model
	/// </summary>
	public class ClientsModel
	{
		public ClientsModel(IEnumerable<IClient> clients)
		{
			Clients				= new List<IClient>(clients);
			Log                 = new List<string>();
		}

		/// <summary>All configured clients</summary>
		public IList<IClient> Clients	{ get; set; }

		/// <summary>Selected client</summary>
		public IClient? Current			{ get; set; }

		/// <summary>State data</summary>
		public string? StateValue		{ get; set; }

		/// <summary>Login hint value</summary>
		public string? LoginHint		{ get; set; }

		/// <summary>Error message</summary>
		public string? Error			{ get; set; }

		/// <summary>User info from client</summary>
		public UserInfo? User			{ get; set; }

		/// <summary>Log of requests</summary>
		public IList<string> Log		{ get; }
	}
}
