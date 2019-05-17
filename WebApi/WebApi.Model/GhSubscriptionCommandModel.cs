namespace WebApi.Model
{
	public class GhSubscriptionCommandModel : GhModel
	{
		/// <summary>
		/// 公众号uin
		/// </summary>
		public string uin
		{
			get;
			set;
		}

		/// <summary>
		/// 公众号key
		/// </summary>
		public string key
		{
			get;
			set;
		}
	}
}
