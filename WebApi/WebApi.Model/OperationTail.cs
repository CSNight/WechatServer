using Newtonsoft.Json;

namespace WebApi.Model
{
	public class OperationTail
	{
		[JsonProperty("enable")]
		public int Enable
		{
			get;
			set;
		}
	}
}
