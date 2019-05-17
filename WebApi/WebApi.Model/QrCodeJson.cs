using Newtonsoft.Json;

namespace WebApi.Model
{
	public class QrCodeJson
	{
		[JsonProperty("device_type")]
		public string DeviceType
		{
			get;
			set;
		}

		[JsonProperty("expired_time")]
		public int ExpiredTime
		{
			get;
			set;
		}

		[JsonProperty("head_url")]
		public string HeadUrl
		{
			get;
			set;
		}

		[JsonProperty("nick_name")]
		public string NickName
		{
			get;
			set;
		}

		[JsonProperty("password")]
		public string Password
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

		[JsonProperty("user_name")]
		public string UserName
		{
			get;
			set;
		}
	}
}
