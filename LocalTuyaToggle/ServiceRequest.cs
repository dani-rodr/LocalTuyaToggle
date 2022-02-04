using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
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

		public async Task<CommandResponse> RequestCommand(string value)
		{
			var timestamp = Helper.TimeStamp;
			var command = "{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}";
			var message = _clientId + _token + timestamp + GetStringToSign(command);
			var sign = Helper.Encrypt(message, _secret);
			var request = CreateRequest(sign, timestamp, command);
			var client = new RestClient("https://openapi.tuyaus.com/v1.0/devices/82315003c44f33f81519/commands");
			var response = await client.ExecuteAsync(request);

			return await JsonSerializer.DeserializeAsync<CommandResponse>(response.Content.ToStream());
		}

		private RestRequest CreateRequest(string sign, string timestamp, string command)
		{
			var request = new RestRequest();
			request.Method = Method.Post;
			request.AddHeader("client_id", _clientId);
			request.AddHeader("access_token", _token);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");
			request.AddStringBody(command, DataFormat.Json);
			return request;
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
