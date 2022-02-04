using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace LocalTuyaToggle
{
    public class CommandRequest : BaseDeviceRequest
	{
		public CommandRequest(string clientId, string secret, string deviceId)
							: base(clientId, secret, Method.Post, $"/v1.0/devices/{deviceId}/commands")
		{
		}

		public async Task<bool> TurnOnAsync(string token)
		{
			var value = "true";
			return await CreateRequestAsync(value, token);
		}

		public async Task<bool> TurnOffAsync(string token)
		{
			var value = "false";
			return await CreateRequestAsync(value, token);
		}

		private async Task<bool> CreateRequestAsync(string value, string token)
        {
			var command = "{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}";
			var response = await SendRequest<CommandResponse>(command, token);
			return response.success;
		}
	}

	public class CommandResponse
	{
		public bool result { get; set; }
		public bool success { get; set; }
		public long t { get; set; }
	}
}
