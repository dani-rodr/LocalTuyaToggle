using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

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

		public async Task<DeviceStatus> RequestDeviceStatus()
		{
			var timestamp = Helper.TimeStamp;
			var message = _clientId + _token + timestamp + GetStringToSign("");
			var sign = Helper.Encrypt(message, _secret);
			var request = CreateRequest(sign, timestamp);
			var client = new RestClient("https://openapi.tuyaus.com/v1.0/devices/82315003c44f33f81519/status");
			var response = await client.ExecuteAsync(request);

			return await JsonSerializer.DeserializeAsync<DeviceStatus>(response.Content.ToStream());
		}

		private RestRequest CreateRequest(string sign, string timestamp)
		{
			var request = new RestRequest();
			request.AddHeader("client_id", _clientId);
			request.AddHeader("access_token", _token);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");

			return request;
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
