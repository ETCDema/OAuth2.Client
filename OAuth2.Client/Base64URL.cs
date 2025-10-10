using System.Security.Cryptography;
using System.Text;

namespace OAuth2.Client
{
	public static class Base64URL
	{
		public static string Encode(byte[] data)
		{
			using var encoder   = new ToBase64Transform();
			var ascii			= encoder.TransformFinalBlock(data, 0, data.Length);
			var last            = ascii.Length-1;
			while (ascii[last]=='=') last--;
			for (var i = 0; i<=last; i++)
			{
				switch (ascii[i])
				{
					case 43: ascii[i]	= 45; continue;	// + -> -
					case 47: ascii[i]	= 95; continue;	// / -> _
				}
			}
			return Encoding.ASCII.GetString(ascii, 0, last+1);
		}
	}
}
