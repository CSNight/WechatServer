namespace WebApi.Model
{
	public class UserLoginModel : BaseModel
	{
		/// <summary>
		/// 账号
		/// </summary>
		public string username
		{
			get;
			set;
		}

		/// <summary>
		/// 密码
		/// </summary>
		public string password
		{
			get;
			set;
		}

		/// <summary>
		/// 62数据 未做hex和base64解码 注意传本api获取的62
		/// </summary>
		public string str62
		{
			get;
			set;
		}

		/// <summary>
		/// 设备名称
		/// </summary>
		public string devicename
		{
			get;
			set;
		}

		/// <summary>
		/// 代理类型，1为http代理，2为socks4，3为socks5(需要用户名和密码)
		/// </summary>
		public int proxytype
		{
			get;
			set;
		}

		/// <summary>
		/// &gt;http代理服务器，格式192.168.1.1:8888 ,如不需要代理可不传
		/// </summary>
		public string proxy
		{
			get;
			set;
		}

		/// <summary>
		/// 代理账号，如代理不需要可为空 ,
		/// </summary>
		public string proxyname
		{
			get;
			set;
		}

		/// <summary>
		/// 代理密码，如代理不需要可为空 ,
		/// </summary>
		public string proxypwd
		{
			get;
			set;
		}

		/// <summary>
		/// 是否重置，如果传true则重置之前进程，如果传false 则复用之前进程（仅针对同一uuid进程做对比）
		/// </summary>
		public bool isreset
		{
			get;
			set;
		}
	}
}
