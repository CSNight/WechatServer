using Newtonsoft.Json;

namespace WebApi.Model
{
	public class EnterGroupJson
	{
		[JsonProperty("full_url")]
		public string FullUrl
		{
			get;
			set;
		}

		[JsonProperty("info")]
		public string Info
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

		[JsonProperty("share_url")]
		public string ShareUrl
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
