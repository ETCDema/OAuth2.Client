using Xunit;

namespace OAuth2.Client.XUnitTest
{
	public class Base64URLTests
	{
		[Fact]
		public void Encode()
		{
			var input           = new byte[256];
			for (int i = 0; i < input.Length; i++) input[i] = (byte)i;

			var result			= Base64URL.Encode(input);
			Assert.Equal("AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGxwdHh8gISIjJCUmJygpK"+
						 "issLS4vMDEyMzQ1Njc4OTo7PD0-P0BBQkNERUZHSElKS0xNTk9QUVJTVF"+
						 "VWV1hZWltcXV5fYGFiY2RlZmdoaWprbG1ub3BxcnN0dXZ3eHl6e3x9fn-"+
						 "AgYKDhIWGh4iJiouMjY6PkJGSk5SVlpeYmZqbnJ2en6ChoqOkpaanqKmq"+
						 "q6ytrq-wsbKztLW2t7i5uru8vb6_wMHCw8TFxsfIycrLzM3Oz9DR0tPU1"+
						 "dbX2Nna29zd3t_g4eLj5OXm5-jp6uvs7e7v8PHy8_T19vf4-fr7_P3-_w", result);
		}

		[Fact]
		public void EncodeShort()
		{
			var input           = new byte[7];
			for (int i = 0; i < input.Length; i++) input[i] = (byte)i;

			var result          = Base64URL.Encode(input);
			Assert.Equal("AAECAwQFBg", result);
		}

		[Fact]
		public void EncodeLong()
		{
			var input           = new byte[256];
			for (int i = 0; i < input.Length; i++) input[i] = 255;

			var result          = Base64URL.Encode(input);
			Assert.Equal("_________________________________________________________"+
						 "_________________________________________________________"+
						 "_________________________________________________________"+
						 "_________________________________________________________"+
						 "_________________________________________________________"+
						 "________________________________________________________w", result);
		}

		[Fact]
		public void EncodeZero()
		{
			var input           = new byte[256];
			for (int i = 0; i < input.Length; i++) input[i] = 0;

			var result          = Base64URL.Encode(input);
			Assert.Equal("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
						 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
						 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
						 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
						 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA"+
						 "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA", result);
		}
	}
}
