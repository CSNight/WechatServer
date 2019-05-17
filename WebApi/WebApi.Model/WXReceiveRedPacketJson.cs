using Newtonsoft.Json;

namespace WebApi.Model
{
	public class WXReceiveRedPacketJson
	{
		[JsonProperty("external")]
		public string External
		{
			get;
			set;
		}

		[JsonProperty("key")]
		public string Key
		{
			get;
			set;
		}

		[JsonProperty("message")]
		public string Message
		{
			get;
			set;
		}

		[JsonProperty("status")]
		public int Status
		{
			get;
			set;
		}
	}
}
