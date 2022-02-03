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
			var message = _clientId + _token + timestamp + GetStringToSign(value);
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


					request.Content = new StringContent("{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}");
					request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

					response = await httpClient.SendAsync(request);
				}
			}
			return await response.Content.ReadAsStringAsync();
		}

		private RestRequest CreateRequest(string sign, string timestamp, string value)
		{
			var request = new RestRequest();
			request.Method = Method.Post;
			request.AddHeader("client_id", _clientId);
			request.AddHeader("access_token", _token);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");
			request.AddHeader("Content-Type", "application/json");
			var body = "{'commands':[{ 'code': 'switch_led', 'value': true }]}";
			request.AddParameter("application/json", body, ParameterType.RequestBody);
			return request;
		}

		private string GetStringToSign(string value)
		{
			var httpMethod = "POST";
			string contentSHA256;
			if (value == "true")
			{
				contentSHA256 = "9dc6865aa51302096e552fc45997d7e613b26919b6e566e12d52a9bf79b4164f";

			}
			else
			{
				contentSHA256 = "ccada8f71c88db6c7abb3f892c35153c8a11a9239418be258e6cfa689d83186b";
			}
			var url = $"/v1.0/devices/{_deviceId}/commands";
			var stringToSign = httpMethod + "\n" +
							   contentSHA256 + "\n" +
											   "\n" +
							   url;
			return stringToSign;
		}
	}
}
