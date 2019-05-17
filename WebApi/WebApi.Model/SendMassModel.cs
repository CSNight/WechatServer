using System.Collections.Generic;

namespace WebApi.Model
{
	/// <summary>
	/// 发送文字消息实体类
	/// </summary>
	public class SendMassModel : BaseModel
	{
		/// <summary>
		/// 用户名json数组 ["AB1","AC2","AD3"]
		/// </summary>
		public List<string> wxids
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
	}
}
