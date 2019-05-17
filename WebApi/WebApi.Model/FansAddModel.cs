namespace WebApi.Model
{
	/// <summary>
	/// 添加好友
	/// </summary>
	public class FansAddModel : BaseModel
	{
		public string v1
		{
			get;
			set;
		}

		public string v2
		{
			get;
			set;
		}

		/// <summary>
		/// 1   -通过QQ好友添加--可以
		/// 2   -通过搜索邮箱--可加但无提示
		/// 3   -通过微信号搜索--可以
		/// 5   -通过朋友验证消息-可加但无提示
		/// 7   -通过朋友验证消息(可回复)-可加但无提示
		/// 12  -来自QQ好友--可以
		/// 13  -通过手机通讯录添加--可以
		/// 14  -通过群来源--no
		/// 15  -通过搜索手机号--可以
		/// 16  -通过朋友验证消息-可加但无提示
		/// 17  -通过名片分享--no
		/// 18  -通过附近的人--可以(貌似只需要v1就够了)
		/// 22  -通过摇一摇打招呼方式--可以
		/// 25  -通过漂流瓶---no
		/// 30  -通过二维码方式--可以
		/// </summary>
		public int type
		{
			get;
			set;
		}

		/// <summary>
		/// 打招呼语句
		/// </summary>
		public string hellotext
		{
			get;
			set;
		}
	}
}
