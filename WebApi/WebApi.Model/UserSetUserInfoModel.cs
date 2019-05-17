namespace WebApi.Model
{
	public class UserSetUserInfoModel : BaseModel
	{
		/// <summary>
		/// 昵称
		/// </summary>
		public string nickname
		{
			get;
			set;
		}

		/// <summary>
		/// 签名
		/// </summary>
		public string sign
		{
			get;
			set;
		}

		/// <summary>
		/// 性别 0/1
		/// </summary>
		public int sex
		{
			get;
			set;
		}

		/// <summary>
		/// 国籍 CN
		/// </summary>
		public string country
		{
			get;
			set;
		}

		/// <summary>
		/// 省份 guangdong
		/// </summary>
		public string provincia
		{
			get;
			set;
		}

		/// <summary>
		/// 市 guangzhou
		/// </summary>
		public string city
		{
			get;
			set;
		}
	}
}
