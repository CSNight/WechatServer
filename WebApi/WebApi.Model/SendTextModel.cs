using System.Collections.Generic;

namespace WebApi.Model
{
	/// <summary>
	/// 发送文字消息实体类
	/// </summary>
	public class SendTextModel : BaseModel
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
		/// 消息内容
		/// </summary>
		public string text
		{
			get;
			set;
		}

		/// <summary>
		/// @好友列表  要@的微信id list
		/// </summary>
		public List<string> atlist
		{
			get;
			set;
		}
	}
}
