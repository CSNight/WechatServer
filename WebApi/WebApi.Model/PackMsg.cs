using System.Collections.Generic;

namespace WebApi.Model
{
	public class PackMsg
	{
		public string fromuser
		{
			get;
			set;
		}

		/// <summary>
		/// 红包key
		/// </summary>
		public string key
		{
			get;
			set;
		}

		/// <summary>
		/// 消息内容
		/// </summary>
		public string msg
		{
			get;
			set;
		}

		/// <summary>
		/// 第几页
		/// </summary>
		public int page
		{
			get;
			set;
		}

		/// <summary>
		/// 是否读完
		/// </summary>
		public bool ok
		{
			get;
			set;
		}

		/// <summary>
		/// 红包数据
		/// </summary>
		public Dictionary<string, packitme> packitme
		{
			get;
			set;
		}

		public int Timestamp
		{
			get;
			set;
		}
	}
}
