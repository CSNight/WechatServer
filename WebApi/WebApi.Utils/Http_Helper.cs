using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Utils
{
	public class Http_Helper
	{
		/// <summary>
		/// 枚举类型
		/// </summary>
		public enum InternetFlags
		{
			INTERNET_FLAG_RESTRICTED_ZONE = 0x10,
			INTERNET_COOKIE_HTTPONLY = 0x2000,
			INTERNET_COOKIE_THIRD_PARTY = 0x20000
		}

		public CookieContainer CookieContainers = new CookieContainer();

		public string IE7 = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET4.0C; .NET4.0E)";

		public string IE8 = "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0)";

		private RequestCachePolicy rcp = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

		public bool isUserAgentSet;

		public string Cookies
		{
			get;
			set;
		}

		public Http_Helper()
		{
			ServicePointManager.DefaultConnectionLimit = 1024;
		}

		[DllImport("wininet.dll", CharSet = CharSet.Auto)]
		public static extern bool DeleteUrlCacheEntry(string lpszUrlName);

		[DllImport("wininet.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
		[SecurityCritical]
		[SuppressUnmanagedCodeSecurity]
		internal static extern bool InternetGetCookieExW([In] string Url, [In] string cookieName, [Out] StringBuilder cookieData, [In] [Out] ref uint pchCookieData, uint flags, IntPtr reserved);

		public HttpStatusCode GetResponse(ref string pResponsetext, string pUrl, string pMethod, string pData, string pReferer, int pTime = 10000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod.ToUpper();
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = IE8;
				pRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
				pRequest.Timeout = pTime;
				pRequest.ReadWriteTimeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() != "GET" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				HttpWebResponse obj = (HttpWebResponse)pRequest.GetResponse();
				StreamReader streamReader = new StreamReader(obj.GetResponseStream(), encoding);
				pResponsetext = streamReader.ReadToEnd();
				pRequest.Abort();
				obj.Close();
				return obj.StatusCode;
			}
			catch (WebException ex)
			{
				try
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
					if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
					{
						using (Stream stream = httpWebResponse.GetResponseStream())
						{
							using (StreamReader streamReader2 = new StreamReader(stream))
							{
								Console.WriteLine(pResponsetext = streamReader2.ReadToEnd());
							}
						}
					}
				}
				catch
				{
					return HttpStatusCode.ExpectationFailed;
				}
				return HttpStatusCode.ExpectationFailed;
			}
		}

		public HttpStatusCode GetResponse_WX(ref string pResponsetext, string pUrl, string pMethod, string pData, string pReferer, int pTime = 10000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod.ToUpper();
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 MicroMessenger/6.5.2.501 NetType/WIFI WindowsWechat QBCore/3.43.556.400 QQBrowser/9.0.2524.400";
				pRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
				pRequest.Timeout = pTime;
				pRequest.ReadWriteTimeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() != "GET" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				HttpWebResponse obj = (HttpWebResponse)pRequest.GetResponse();
				StreamReader streamReader = new StreamReader(obj.GetResponseStream(), encoding);
				pResponsetext = streamReader.ReadToEnd();
				pRequest.Abort();
				obj.Close();
				return obj.StatusCode;
			}
			catch (WebException ex)
			{
				try
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
					if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
					{
						using (Stream stream = httpWebResponse.GetResponseStream())
						{
							using (StreamReader streamReader2 = new StreamReader(stream))
							{
								Console.WriteLine(pResponsetext = streamReader2.ReadToEnd());
							}
						}
					}
				}
				catch
				{
					return HttpStatusCode.ExpectationFailed;
				}
				return HttpStatusCode.ExpectationFailed;
			}
		}

		public HttpStatusCode GetResponse1(ref string pResponsetext, string pUrl, string pMethod, string pData, string pReferer, int pTime = 10000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod.ToUpper();
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = IE8;
				pRequest.Accept = "application/json, text/javascript, */*; q=0.01";
				pRequest.Timeout = pTime;
				pRequest.ReadWriteTimeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() == "POST" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/json; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				HttpWebResponse obj = (HttpWebResponse)pRequest.GetResponse();
				StreamReader streamReader = new StreamReader(obj.GetResponseStream(), encoding);
				pResponsetext = streamReader.ReadToEnd();
				pRequest.Abort();
				obj.Close();
				return obj.StatusCode;
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
				{
					using (Stream stream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader2 = new StreamReader(stream))
						{
							Console.WriteLine(pResponsetext = streamReader2.ReadToEnd());
						}
					}
				}
				return HttpStatusCode.ExpectationFailed;
			}
		}

		public HttpStatusCode GetResponse2(ref string pResponsetext, string pUrl, string pMethod, string pData, string pReferer, int pTime = 10000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod.ToUpper();
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = IE8;
				pRequest.Accept = "application/json, text/javascript, */*; q=0.01";
				pRequest.Timeout = pTime;
				pRequest.ReadWriteTimeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() == "POST" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				HttpWebResponse obj = (HttpWebResponse)pRequest.GetResponse();
				StreamReader streamReader = new StreamReader(obj.GetResponseStream(), encoding);
				pResponsetext = streamReader.ReadToEnd();
				pRequest.Abort();
				obj.Close();
				return obj.StatusCode;
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
				{
					using (Stream stream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader2 = new StreamReader(stream))
						{
							Console.WriteLine(pResponsetext = streamReader2.ReadToEnd());
						}
					}
				}
				return HttpStatusCode.ExpectationFailed;
			}
		}

		public HttpStatusCode GetResponse3(ref string pResponsetext, string pUrl, string pMethod, string pData, string pReferer, int pTime = 10000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod.ToUpper();
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
				pRequest.Accept = "text/plain, */*; q=0.01";
				pRequest.Timeout = pTime;
				pRequest.ReadWriteTimeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() != "GET" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				HttpWebResponse obj = (HttpWebResponse)pRequest.GetResponse();
				StreamReader streamReader = new StreamReader(obj.GetResponseStream(), encoding);
				pResponsetext = streamReader.ReadToEnd();
				pRequest.Abort();
				obj.Close();
				return obj.StatusCode;
			}
			catch (WebException ex)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
				if (httpWebResponse.StatusCode == HttpStatusCode.BadRequest)
				{
					using (Stream stream = httpWebResponse.GetResponseStream())
					{
						using (StreamReader streamReader2 = new StreamReader(stream))
						{
							Console.WriteLine(pResponsetext = streamReader2.ReadToEnd());
						}
					}
				}
				return HttpStatusCode.ExpectationFailed;
			}
		}

		public Stream GetResponseImage(string pUrl, string pReferer = "", string pMethod = "GET", string pData = "", int pTime = 6000, string pEncoding = "UTF-8", bool pIsPTHander = true)
		{
			Set_Cookies(pUrl);
			try
			{
				DeleteUrlCacheEntry(pUrl);
				HttpWebRequest pRequest = (HttpWebRequest)WebRequest.Create(pUrl);
				pRequest.KeepAlive = true;
				pRequest.Method = pMethod;
				pRequest.AllowAutoRedirect = true;
				pRequest.CookieContainer = CookieContainers;
				pRequest.Referer = pReferer;
				pRequest.UserAgent = IE8;
				pRequest.Accept = "*/*";
				pRequest.Timeout = pTime;
				pRequest.CachePolicy = rcp;
				pRequest.Headers.Add("x-requested-with", "XMLHttpRequest");
				pRequest.ProtocolVersion = HttpVersion.Version10;
				pRequest.KeepAlive = false;
				if (pIsPTHander)
				{
					PTRequest(ref pRequest);
				}
				Encoding encoding = Encoding.GetEncoding(pEncoding);
				if (pMethod.ToUpper() == "POST" && pData != null)
				{
					byte[] bytes = encoding.GetBytes(pData);
					pRequest.ContentLength = bytes.Length;
					pRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
					Stream requestStream = pRequest.GetRequestStream();
					requestStream.Write(bytes, 0, bytes.Length);
					requestStream.Close();
				}
				ServicePointManager.ServerCertificateValidationCallback = (RemoteCertificateValidationCallback)Delegate.Combine(ServicePointManager.ServerCertificateValidationCallback, (RemoteCertificateValidationCallback)((object se, X509Certificate cert, X509Chain chain, SslPolicyErrors sslerror) => true));
				return pRequest.GetResponse().GetResponseStream();
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 提取string Cookies
		/// </summary>
		/// <param name="pUrl"></param>
		/// <returns></returns>
		public string GetCookieInternal(string pUrl)
		{
			string result = "";
			uint pchCookieData = 0u;
			string url = pUrl.ToString();
			uint flags = 8192u;
			if (InternetGetCookieExW(url, null, null, ref pchCookieData, flags, IntPtr.Zero))
			{
				pchCookieData++;
				StringBuilder stringBuilder = new StringBuilder((int)pchCookieData);
				if (InternetGetCookieExW(url, null, stringBuilder, ref pchCookieData, flags, IntPtr.Zero))
				{
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		/// <summary>
		/// 提取string Cookies
		/// </summary>
		/// <param name="pUrl"></param>
		/// <param name="pCookie"></param>
		/// <returns></returns>
		public string GetHttpHelperCookieString(Uri pUrl, CookieCollection pCookie = null)
		{
			List<string> list = new List<string>();
			if (pCookie == null)
			{
				pCookie = CookieContainers.GetCookies(pUrl);
			}
			foreach (Cookie item in pCookie)
			{
				string name = item.Name;
				string value = item.Value;
				list.Add(name + "=" + value);
			}
			return string.Join("; ", list);
		}

		/// <summary>
		/// 提取string Cookies
		/// </summary>
		/// <param name="pUrl"></param>
		/// <param name="pCookie"></param>
		/// <returns></returns>
		public string GetHttpHelperCookieString(string pUrl, CookieCollection pCookie = null)
		{
			List<string> list = new List<string>();
			if (pCookie == null)
			{
				pCookie = CookieContainers.GetCookies(new Uri(pUrl));
			}
			foreach (Cookie item in pCookie)
			{
				string name = item.Name;
				string value = item.Value;
				list.Add(name + "=" + value);
			}
			return string.Join("; ", list);
		}

		public Dictionary<string, string> GetHttpHelperCookie(Uri pUrl)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (Cookie cooky in CookieContainers.GetCookies(pUrl))
			{
				string name = cooky.Name;
				string text = dictionary[name] = cooky.Value;
			}
			return dictionary;
		}

		/// <summary>
		/// 删除Heard
		/// </summary>
		/// <param name="pRequest"></param>
		public void PTRequest(ref HttpWebRequest pRequest)
		{
			pRequest.Headers.Remove("x-requested-with");
		}

		/// <summary>
		/// 设置COOKIES
		/// </summary>
		/// <param name="url"></param>
		public void Set_Cookies(string url)
		{
			if (Cookies == null)
			{
				return;
			}
			string[] array = Cookies.Split(';');
			CookieContainer cookieContainer = new CookieContainer();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.IndexOf("expires") <= 0)
				{
					cookieContainer.SetCookies(new Uri(url), text);
				}
			}
			CookieContainers = cookieContainer;
		}

		public static string LiteCookies(string Cookies)
		{
			try
			{
				string text = "";
				Cookies = Cookies.Replace("HttpOnly", "").Replace(";", "; ");
				Match match = new Regex("(?<=,|^)(?<cookie>[^ ]+=[\\s|\"]?(?![\"]?deleted[\"]?)[^;]+)[\"]?;").Match(Cookies);
				while (match.Success)
				{
					text = GetCleanCookie(text, match.Groups["cookie"].Value);
					match = match.NextMatch();
				}
				return text;
			}
			catch
			{
				return "";
			}
		}

		private static string GetCleanCookie(string source, string inStr)
		{
			if (source != "" && inStr != "")
			{
				bool flag = false;
				string[] array = source.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Split('=')[0] == inStr.Split('=')[0])
					{
						source = source.Replace(array[i], inStr);
						flag = true;
					}
				}
				if (!flag)
				{
					source = source + ";" + inStr;
				}
				return source;
			}
			if (inStr != "")
			{
				return inStr;
			}
			if (source != "")
			{
				return source;
			}
			return "";
		}

		public static string MergerCookies(string OldCookie, string NewCookie)
		{
			if (!string.IsNullOrEmpty(OldCookie) && !string.IsNullOrEmpty(NewCookie))
			{
				if (OldCookie == NewCookie)
				{
					return OldCookie;
				}
				List<string> list = new List<string>(OldCookie.Split(';'));
				List<string> list2 = new List<string>(NewCookie.Split(';'));
				foreach (string item in list2)
				{
					foreach (string item2 in list)
					{
						if (item2 == item || item2.Split('=')[0] == item.Split('=')[0])
						{
							list.Remove(item2);
							break;
						}
					}
				}
				List<string> list3 = new List<string>(list);
				list3.AddRange(list2);
				return string.Join(";", list3.ToArray());
			}
			if (!string.IsNullOrEmpty(OldCookie))
			{
				return OldCookie;
			}
			if (!string.IsNullOrEmpty(NewCookie))
			{
				return NewCookie;
			}
			return "";
		}

		public byte[] StreamToBytes(Stream stream)
		{
			byte[] array = new byte[stream.Length];
			stream.Read(array, 0, array.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return array;
		}
	}
}
