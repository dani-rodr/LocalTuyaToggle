using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocalTuyaToggle
{
    public class ServiceRequestStatus
    {
		private readonly string _clientId;
		private readonly string _secret;
		private readonly string _deviceId;
		private readonly string _token;
		public ServiceRequestStatus(string clientId, string secret, string token, string deviceId)
		{
			_clientId = clientId;
			_secret = secret;
			_deviceId = deviceId;
			_token = token;
		}

		public async Task<string> RequestService()
		{
			var timestamp = Helper.TimeStamp;
			//var command = "{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}";
			var command = "";
			var message = _clientId + _token + timestamp + GetStringToSign(command);
			var sign = Helper.Encrypt(message, _secret);

			HttpResponseMessage response;
			using (var httpClient = new HttpClient())
			{
				using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://openapi.tuyaus.com/v1.0/devices/82315003c44f33f81519/status"))
				{
					request.Headers.TryAddWithoutValidation("client_id", _clientId);
					request.Headers.TryAddWithoutValidation("access_token", _token);
					request.Headers.TryAddWithoutValidation("sign", sign);
					request.Headers.TryAddWithoutValidation("t", timestamp);
					request.Headers.TryAddWithoutValidation("sign_method", "HMAC-SHA256");

					response = await httpClient.SendAsync(request);
				}
			}
			var result = await response.Content.ReadAsStringAsync();
			Console.WriteLine(result);
			return result;
		}

		private string GetStringToSign(string command)
		{
			var httpMethod = "GET";
			string contentSHA256 = Helper.Sha256(command);
			var url = $"/v1.0/devices/{_deviceId}/status";
			var stringToSign = httpMethod + "\n" +
							   contentSHA256 + "\n" +
											   "\n" +
							   url;
			return stringToSign;
		}
	}
}
