using System;
using System.Threading.Tasks;
using RestSharp;

namespace LocalTuyaToggle
{
    public class TokenRequest : BaseDeviceRequest
	{
		public TokenRequest(string clientId, string secret)
							: base(clientId, secret, Method.Get, "/v1.0/token?grant_type=1")
		{
		}

		public async Task<string> GetToken()
        {
			var response = await RequestCommandAsync<TokenResponse>();
			if (!response.success)
            {
				Console.WriteLine($"Failed Token Request msg:{response.msg}");
				return "";
            }

			return response.result.access_token;
        }

		protected override void FillRequestHeader(RestRequest request, string body, string timestamp, string sign, string token)
		{
			request.AddHeader("client_id", _clientId);
			request.AddHeader("sign", sign);
			request.AddHeader("t", timestamp);
			request.AddHeader("sign_method", "HMAC-SHA256");
            request.AddHeader("nonce", "");
            request.AddHeader("stringToSign", "");
        }
	}

	public class TokenResult
	{
		public string access_token { get; set; }
		public int expire_time { get; set; }
		public string refresh_token { get; set; }
		public string uid { get; set; }
	}

	public class TokenResponse
	{
		public int code { get; set; }
		public string msg { get; set; }
		public TokenResult result { get; set; }
		public bool success { get; set; }
		public long t { get; set; }
	}
}
