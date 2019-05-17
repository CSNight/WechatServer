namespace WebApi.Model
{
	public class SnsComment : SnsModel
	{
		/// <summary>
		/// 评论内容
		/// </summary>
		public string context
		{
			get;
			set;
		}

		/// <summary>
		/// 回复id
		/// </summary>
		public int replyid
		{
			get;
			set;
		}
	}
}
