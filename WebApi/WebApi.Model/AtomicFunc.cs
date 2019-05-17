using Newtonsoft.Json;

namespace WebApi.Model
{
	public class AtomicFunc
	{
		[JsonProperty("enable")]
		public int Enable
		{
			get;
			set;
		}
	}
}
