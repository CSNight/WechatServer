namespace WebApi.Model
{
	public class ScanLoginModel : BaseModel
	{
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
	}
}
