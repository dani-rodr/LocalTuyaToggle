using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;
using System.Linq;

namespace LocalTuyaToggle
{
    public class StatusRequest : BaseDeviceRequest
	{
		public StatusRequest(string clientId, string secret, string token, string deviceId)
								: base(clientId, secret, token, Method.Get, $"/v1.0/devices/{deviceId}/status")
		{
		}

		public async Task<bool> IsOnAsync()
		{
			var response = await RequestCommandAsync<StatusResponse>();
			if (!response.success)
            {
				return false;
            }
			var isOn = ((JsonElement)response.result.First().value).GetBoolean();
			return isOn;
		}
	}

	public class KeyValuePair
	{
		public string code { get; set; }
		public object value { get; set; }
	}

	public class StatusResponse
	{
		public IEnumerable<KeyValuePair> result { get; set; }
		public bool success { get; set; }
		public long t { get; set; }
	}
}
