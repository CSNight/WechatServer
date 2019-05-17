using Newtonsoft.Json;

namespace WebApi.Model
{
	public class WxQrCode
	{
		[JsonProperty("qr_code")]
		public string QrCodeStr
		{
			get;
			set;
		}
	}
}
