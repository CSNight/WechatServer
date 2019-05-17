using System.Collections.Generic;

namespace WebApi.Model
{
	public class SnsSendImageTextModel : SnsSendTextModel
	{
		/// <summary>
		/// 朋友圈图片 base64 数组 不超过9张
		/// </summary>
		public List<string> base64list
		{
			get;
			set;
		}
	}
}
