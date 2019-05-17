using System.Collections.Generic;

namespace WebApi.Model
{
	public class GroupCreatModel : BaseModel
	{
		/// <summary>
		/// 好友wxid ，不小于3人且包含自己
		/// </summary>
		public List<string> users
		{
			get;
			set;
		}
	}
}
