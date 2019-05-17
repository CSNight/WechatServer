namespace WebApi.Model
{
	/// <summary>
	/// 发送图片消息实体类
	/// </summary>
	public class SendAppModel : BaseModel
	{
		/// <summary>
		/// 微信ID
		/// </summary>
		public string wxid
		{
			get;
			set;
		}

		/// <summary>
		/// wx0f76313b15c62c93
		/// </summary>
		public string appid
		{
			get;
			set;
		}

		/// <summary>
		/// 0
		/// </summary>
		public string sdkver
		{
			get;
			set;
		}

		/// <summary>
		/// 标题
		/// </summary>
		public string title
		{
			get;
			set;
		}

		/// <summary>
		/// 描述
		/// </summary>
		public string des
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

		/// <summary>
		/// 图片地址
		/// </summary>
		public string thumburl
		{
			get;
			set;
		}
	}
}
