using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace OAuth2.Client.XUnitTest.Core
{
	internal class TestMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, Func<string, HttpContent>> _data;

		public TestMessageHandler()
		{
			_data               = new();
		}

		public TestMessageHandler Add(string url, string expectedReq, string contentType, string content)
		{
			var bytes			= Convert.FromBase64String(content);

			_data.Add(url, (actualReq) =>
			{
				Assert.Equal(expectedReq, actualReq);

				return new ByteArrayContent(bytes)
				{
					Headers		=
					{
						{ "Content-Type", contentType }
					}
				};
			});

			return this;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var url             = string.Concat("[", request.Method, "]", request.RequestUri);
			var actualReq     	= _reqToString(request);

			var response        = new HttpResponseMessage()
			{
				RequestMessage	= request,
				Content         = _getContent(url, actualReq)
			};

			return Task.FromResult(response);
		}

		private static string _reqToString(HttpRequestMessage req)
		{
			var reqTxt			= "Headers: [ "+req.Headers.ToString().Replace("\r\n", "\n")+" ]";
			if (req.Content!=null)
				reqTxt			+= ", Body: "+req.Content.ReadAsStringAsync().Result;

			return reqTxt;
		}

		private HttpContent? _getContent(string url, string actualReq)
		{
			if (_data.TryGetValue(url, out var fx)) return fx(actualReq);

			Assert.True(false, "NotFound: "+url);
			return null;
		}
	}
}
