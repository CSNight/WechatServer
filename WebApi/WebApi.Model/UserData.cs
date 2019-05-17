using Newtonsoft.Json;

namespace WebApi.Model
{
	public class UserData
	{
		[JsonProperty("email")]
		public string Email
		{
			get;
			set;
		}

		[JsonProperty("external")]
		public string External
		{
			get;
			set;
		}

		[JsonProperty("long_link_server")]
		public string LongLinkServer
		{
			get;
			set;
		}

		[JsonProperty("message")]
		public string Message
		{
			get;
			set;
		}

		[JsonProperty("nick_name")]
		public string NickName
		{
			get;
			set;
		}

		[JsonProperty("phone_number")]
		public string PhoneNumber
		{
			get;
			set;
		}

		[JsonProperty("qq")]
		public long Qq
		{
			get;
			set;
		}

		[JsonProperty("short_link_server")]
		public string ShortLinkServer
		{
			get;
			set;
		}

		[JsonProperty("status")]
		public int Status
		{
			get;
			set;
		}

		[JsonProperty("user_name")]
		public string UserName
		{
			get;
			set;
		}

		[JsonProperty("uin")]
		public long Uin
		{
			get;
			set;
		}

		public string HeadImg
		{
			get;
			set;
		}
	}
}
