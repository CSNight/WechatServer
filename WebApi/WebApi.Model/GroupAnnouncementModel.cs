namespace WebApi.Model
{
	/// <summary>
	/// 群信息
	/// </summary>
	public class GroupAnnouncementModel : GroupModel
	{
		/// <summary>
		/// 群公告
		/// </summary>
		public string context
		{
			get;
			set;
		}
	}
}
