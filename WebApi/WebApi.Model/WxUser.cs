namespace WebApi.Model
{
	public class WxUser
	{
		/// <summary>
		/// 句柄
		/// </summary>
		public object hander
		{
			get;
			set;
		}

		/// <summary>
		/// 微信ID
		/// </summary>
		public string wxid
		{
			get;
			set;
		}

		/// <summary>
		/// 状态
		/// </summary>
		public int type
		{
			get;
			set;
		}

		/// <summary>
		/// 功能
		/// </summary>
		public int code
		{
			get;
			set;
		}

		/// <summary>
		/// 名称
		/// </summary>
		public string name
		{
			get;
			set;
		}

		/// <summary>
		/// 头像
		/// </summary>
		public string headurl
		{
			get;
			set;
		}

		/// <summary>
		/// 头像
		/// </summary>
		public int status
		{
			get;
			set;
		}
	}
}
