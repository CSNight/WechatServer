namespace WebApi.Model
{
	public class FansSearchModel : BaseModel
	{
		/// <summary>
		/// 搜索条件  QQ号 手机号 微信号
		/// </summary>
		public string search
		{
			get;
			set;
		}
	}
}
