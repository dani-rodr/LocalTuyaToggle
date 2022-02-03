using System.Threading.Tasks;
using RestSharp;
using System.Text.Json;

namespace LocalTuyaToggle
{
    public class TokenRequest
	{
		private readonly string _clientId;
		private readonly string _secret;
		public string Token { get; private set; }
		public TokenRequest(string clientId, string secret)
		{
			_clientId = clientId;
			_secret = secret;
		}

		public async Task<Response> RequestToken()
		{
			var timestamp = Helper.TimeStamp;
			var message = _clientId + timestamp + GetStringToSign();
			var sign = Helper.Encrypt(message, _secret);
			var request = CreateRequest(sign, timestamp);
			var client = new RestClient("https://openapi.tuyaus.com/v1.0/token?grant_type=1");
			var response = await client.ExecuteAsync(request);

			return await JsonSerializer.DeserializeAsync<Response>(response.Content.ToStream());
		}

		private RestRequest CreateRequest(string sign, string timestamp)
		{
			var request = new RestRequest();
			request.AddHeader("client_id", _clientId);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");
			request.AddHeader("nonce", "");
			request.AddHeader("stringToSign", "");

			return request;
		}

		private string GetStringToSign()
		{
			var httpMethod = "GET";
			var contentSHA256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855"; // sha256 of empty body
			var url = "/v1.0/token?grant_type=1";
			var stringToSign = httpMethod + "\n" +
							   contentSHA256 + "\n" +
											   "\n" +
							   url;
			return stringToSign;
		}

	}
}
