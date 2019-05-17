namespace WebApi.Model
{
	/// <summary>
	/// 名片消息
	/// </summary>
	public class ShardCardModel : BaseModel
	{
		public string user
		{
			get;
			set;
		}

		public string wxid
		{
			get;
			set;
		}

		public string title
		{
			get;
			set;
		}
	}
}
