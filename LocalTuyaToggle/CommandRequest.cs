using System.Text.Json;
using System.Threading.Tasks;
using RestSharp;

namespace LocalTuyaToggle
{
    public class CommandRequest : BaseDeviceRequest
	{
		public CommandRequest(string clientId, string secret, string token, string deviceId)
							: base(clientId, secret, token, Method.Post, $"/v1.0/devices/{deviceId}/commands")
		{
		}

		public async Task<bool> TurnOnAsync()
		{
			var value = "true";
			return await CreateRequestAsync(value);
		}

		public async Task<bool> TurnOffAsnyc()
		{
			var value = "false";
			return await CreateRequestAsync(value);
		}

		private async Task<bool> CreateRequestAsync(string value)
        {
			var command = "{'commands':[{ 'code': 'switch_led', 'value': " + value + " }]}";
			var response = await RequestCommandAsync<CommandResponse>(command);
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
