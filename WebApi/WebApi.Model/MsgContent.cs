namespace WebApi.Model
{
	/// <summary>
	/// Tcp消息
	/// </summary>
	public class MsgContent
	{
		public string content
		{
			get;
			set;
		}

		public bool Img
		{
			get;
			set;
		}

		public string ImgPath
		{
			get;
			set;
		}

		public string towxid
		{
			get;
			set;
		}
	}
}
