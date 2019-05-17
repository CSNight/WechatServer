namespace WebApi.Model
{
	/// <summary>
	/// 发送图片消息实体类
	/// </summary>
	public class SendMediaModel : BaseModel
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
		public string base64
		{
			get;
			set;
		}
	}
}
