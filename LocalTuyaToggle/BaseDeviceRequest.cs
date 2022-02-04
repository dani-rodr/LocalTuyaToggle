using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace LocalTuyaToggle
{
    public class BaseDeviceRequest
	{
		private readonly string _secret;
		private readonly string _token;
		private readonly Method _method;
		private readonly string _resource;

		protected readonly string _clientId;

		public BaseDeviceRequest(string clientId, string secret, string token, Method method, string resource)
		{
			_clientId = clientId;
			_secret = secret;
			_token = token;
			_method = method;
			_resource = resource;
		}

		protected async Task<T> RequestCommandAsync<T>(string body = "") where T : class
		{
			var request = CreateRequest(body);
			var domain = "https://openapi.tuyaus.com";
			var client = new RestClient($"{domain}{_resource}");

			var response = await client.ExecuteAsync(request);
			return await JsonSerializer.DeserializeAsync<T>(response.Content.ToStream());
		}

		private RestRequest CreateRequest(string body)
		{
			var timestamp = Helper.TimeStamp;
			var sign = CreateSign(body, timestamp);

            var request = new RestRequest
            {
                Method = _method
            };

			FillRequestHeader(request, body, timestamp, sign);
			return request;
		}

		protected virtual void FillRequestHeader(RestRequest request, string body, string timestamp, string sign)
        {
			request.AddHeader("client_id", _clientId);
			request.AddHeader("access_token", _token);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");
			if (!string.IsNullOrEmpty(body))
			{
				request.AddStringBody(body, DataFormat.Json);
			}
		}

		private string CreateSign(string body, string timestamp)
        {
			var message = _clientId + _token + timestamp + GetStringToSign(body);
			var sign = Helper.Encrypt(message, _secret);
			return sign;
		}

		private string GetStringToSign(string body)
		{
			var httpMethod = (_method == Method.Post) ? "POST" : "GET";
			string contentSha256 = "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855";
			if (!string.IsNullOrEmpty(body))
            {
				contentSha256 = Helper.Sha256(body);
			}
			var stringToSign = httpMethod + "\n" + contentSha256 + "\n\n" + _resource;
			return stringToSign;
		}
	}
}
