namespace WebApi.Model
{
	public class SnsSendLink : SnsSendTextModel
	{
		/// <summary>
		/// 链接标题
		/// </summary>
		public string title
		{
			get;
			set;
		}

		/// <summary>
		/// 链接地址
		/// </summary>
		public string url
		{
			get;
			set;
		}
	}
}
