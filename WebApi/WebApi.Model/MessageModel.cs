namespace WebApi.Model
{
	/// <summary>
	/// 消息文件实体类
	/// </summary>
	public class MessageModel : BaseModel
	{
		/// <summary>
		/// 微信返回的msg结果 传入JSON字符串
		/// </summary>
		public Msg msg
		{
			get;
			set;
		}
	}
}
