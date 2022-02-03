using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using RestSharp;

namespace LocalTuyaToggle
{
	public class ServiceRequest
	{
		private readonly string _clientId;
		private readonly string _secret;
		private readonly string _deviceId;
		private readonly string _token;
		public ServiceRequest(string clientId, string secret, string token, string deviceId)
		{
			_clientId = clientId;
			_secret = secret;
			_deviceId = deviceId;
			_token = token;
		}

		public async Task<string> RequestService(string value)
		{
			var timestamp = Helper.TimeStamp;
			var command = "{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}";
			var message = _clientId + _token + timestamp + GetStringToSign(command);
			var sign = Helper.Encrypt(message, _secret);

			HttpResponseMessage response;
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://openapi.tuyaus.com/v1.0/devices/82315003c44f33f81519/commands"))
				{
					request.Headers.TryAddWithoutValidation("client_id", _clientId);
					request.Headers.TryAddWithoutValidation("access_token", _token);
					request.Headers.TryAddWithoutValidation("sign", sign);
					request.Headers.TryAddWithoutValidation("t", timestamp);
					request.Headers.TryAddWithoutValidation("sign_method", "HMAC-SHA256");
					request.Content = new StringContent(command);
					request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

					response = await httpClient.SendAsync(request);
				}
			}
			return await response.Content.ReadAsStringAsync();
		}

		private string GetStringToSign(string command)
		{
			var httpMethod = "POST";
			string contentSHA256 = Helper.Sha256(command);
			var url = $"/v1.0/devices/{_deviceId}/commands";
			var stringToSign = httpMethod + "\n" +
							   contentSHA256 + "\n" +
											   "\n" +
							   url;
			return stringToSign;
		}
	}
}
