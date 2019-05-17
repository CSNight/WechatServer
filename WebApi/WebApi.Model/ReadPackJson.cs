using Newtonsoft.Json;

namespace WebApi.Model
{
	public class ReadPackJson
	{
		[JsonProperty("external")]
		public string External
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
