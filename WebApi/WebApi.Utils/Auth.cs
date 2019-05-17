using System;
using System.Configuration;
using System.IO;
using System.Net;

namespace WebApi.Utils
{
	public class Auth
	{
		public static void Init()
		{
			try
			{
				string str = FingerPrint.Value();
				string str2 = ConfigurationSettings.AppSettings["AuthKey"].ConvertToString();
				new StreamReader(((HttpWebResponse)((HttpWebRequest)WebRequest.Create("http://www.wechattools.com/Api/Auth.ashx?authCode=" + str2 + "&computerCode=" + str)).GetResponse()).GetResponseStream()).ReadToEnd();
			}
			catch (Exception)
			{
			}
		}
	}
}
