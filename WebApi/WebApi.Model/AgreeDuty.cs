using Newtonsoft.Json;

namespace WebApi.Model
{
	public class AgreeDuty
	{
		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("service_protocol_wording")]
		public string ServiceProtocolWording
		{
			get;
			set;
		}

		[JsonProperty("service_protocol_url")]
		public string ServiceProtocolUrl
		{
			get;
			set;
		}

		[JsonProperty("button_wording")]
		public string ButtonWording
		{
			get;
			set;
		}

		[JsonProperty("delay_expired_time")]
		public int DelayExpiredTime
		{
			get;
			set;
		}

		[JsonProperty("agreed_flag")]
		public int AgreedFlag
		{
			get;
			set;
		}
	}
}
