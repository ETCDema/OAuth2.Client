using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Xunit;

namespace OAuth2.Client.XUnitTest.Core
{
	/// <summary>
	/// Коллекция пар запрос - ответ для эмуляции общения с сервисами.
	/// </summary>
	internal class TestMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, Func<string, HttpContent>> _data;

		/// <summary>
		/// Конструктор
		/// </summary>
		public TestMessageHandler()
		{
			_data               = new();
		}

		/// <summary>
		/// Добавить ответ на запрос
		/// </summary>
		/// <param name="url">URL запроса, должен содержать метод и полный адрес: "[METHOD]https://service/path?param"</param>
		/// <param name="expectedReq">Ожидаемые данные запроса: заголовки + тело запроса. Если нашли данные по URL, но ожидаемые данные запроса не совпадают, то будет возвращена ошибка.</param>
		/// <param name="contentType">Тип содержимого ответа</param>
		/// <param name="content">Base64Encoded содержтмое ответа</param>
		/// <returns>Эта же коллекция для добавления другого запроса</returns>
		public TestMessageHandler Add(string url, string expectedReq, string contentType, string content)
		{
			var bytes			= Convert.FromBase64String(content);

			_data.Add(url, (actualReq) =>
			{
				// Сначала проверим, что текущие заголовки и тело запроса совпадает с ожидаемым
				Assert.Equal(expectedReq, actualReq);

				// Вернем настроенное содержимое
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

		/// <summary>
		/// "Выполнить" запрос к сервису и вернуть результат:
		/// 1. По паре "Метод"+"URL" найти данные
		/// 2. Проверить соответствие реальных и ожидаемых заголовков и тела запроса
		/// 3. Вернуть соответствующий контент
		/// </summary>
		/// <param name="request">Запрос</param>
		/// <param name="cancellationToken"></param>
		/// <returns>"Ответ" от сервиса</returns>
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

		/// <summary>
		/// Получить заголовки и тело запроса для проверки
		/// </summary>
		/// <param name="req">Запрос</param>
		/// <returns>Готовая к проверки строка с данными</returns>
		private static string _reqToString(HttpRequestMessage req)
		{
			var reqTxt			= "Headers: [ "+req.Headers.ToString().Replace("\r\n", "\n")+" ]";
			if (req.Content!=null)
				reqTxt			+= ", Body: "+req.Content.ReadAsStringAsync().Result;

			return reqTxt;
		}

		/// <summary>
		/// Получить содержимое для указанного URL
		/// </summary>
		/// <param name="url">URL в том виде, что передается в метод <see cref="TestMessageHandler.Add(string, string, string, string)"/></param>
		/// <param name="actualReq">Текущие заголовки и тело запроса, подготовленные для проверки</param>
		/// <returns>Соответствующее содержимое или null</returns>
		private HttpContent? _getContent(string url, string actualReq)
		{
			if (_data.TryGetValue(url, out var fx)) return fx(actualReq);

			Assert.True(false, "NotFound: "+url);
			return null;
		}
	}
}
