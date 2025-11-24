using System;

namespace OAuth2.Client
{
	public static class Base64URL
	{
		private const char _plus	= (char)43;
		private const char _minus	= (char)45;
		private const char _slash	= (char)47;
		private const char _us		= (char)95;

		public static string Encode(byte[] data)
		{
			var count			= data.Length / 3L * 4;
			count				+= (data.Length % 3 != 0) ? 4 : 0;
			var ascii           = new char[count];
			var last			= Convert.ToBase64CharArray(data, 0, data.Length, ascii, 0, Base64FormattingOptions.None)-1;

			while (ascii[last]=='=') last--;
			for (var i = 0; i<=last; i++)
			{
				switch (ascii[i])
				{
					case _plus:  ascii[i]	= _minus; continue;	// + -> -
					case _slash: ascii[i]	= _us; continue;	// / -> _
				}
			}
			return new string(ascii, 0, last+1);
		}
	}
}
