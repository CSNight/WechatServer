using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;

namespace WebApi.Utils
{
	public class Utilities
	{
		public static double GetTimestamp => (DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;

		/// <summary>
		/// 单个汉字转换为%AB%CD格式,如"我=%CE%D2"
		/// </summary>
		/// <param name="Source"></param>
		/// <returns></returns>
		public static string GB2Unicode(string source)
		{
			string text = "";
			byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(source);
			for (int i = 0; i < bytes.Length; i++)
			{
				string str;
				if ((bytes[i] >= 48 && bytes[i] <= 57) || (bytes[i] >= 65 && bytes[i] <= 90) || (bytes[i] >= 97 && bytes[i] <= 122))
				{
					char c = (char)bytes[i];
					str = c.ToString();
				}
				else
				{
					str = "%" + bytes[i].ToString("X");
				}
				text += str;
			}
			return text;
		}

		/// <summary>
		/// 编码部分默认转为大写字母,字符编码采用默认值
		/// </summary>
		/// <param name="Source">http://www.123.com</param>
		/// <returns>http%3A%2F%2Fwww.123.com</returns>
		public static string UTF8(string Source, bool toUpper = true)
		{
			if (toUpper)
			{
				StringBuilder stringBuilder = new StringBuilder();
				for (int i = 0; i < Source.Length; i++)
				{
					string text = Source[i].ToString();
					string text2 = HttpUtility.UrlEncode(text, Encoding.Default);
					if (text == text2)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append(text2.ToUpper());
					}
				}
				return stringBuilder.ToString();
			}
			return HttpUtility.UrlEncode(Source, Encoding.Default);
		}

		public static string URL(string Source)
		{
			return HttpUtility.UrlEncode(Source);
		}

		public static string EncodeStr(string Source)
		{
			return HttpUtility.UrlEncodeUnicode(Source);
		}

		public static string DecodeStr(string Source)
		{
			string text = Source;
			if (Source.Contains("&#"))
			{
				text = "";
				if (Source.EndsWith(";"))
				{
					Source = Source.Substring(0, Source.Length - 1);
				}
				string[] array = Regex.Replace(Source, "&#", "").Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string text2 = int.Parse(array[i]).ToString("x4");
					text = text + "%u" + text2.Substring(text2.Length - 4);
				}
			}
			return HttpUtility.UrlDecode(Regex.Unescape(text));
		}

		public static string MD5(string Source)
		{
			return FormsAuthentication.HashPasswordForStoringInConfigFile(Source, "MD5");
		}

		public static string GetMD5(string sDataIn)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = Encoding.UTF8.GetBytes(sDataIn);
			byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
			mD5CryptoServiceProvider.Clear();
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i].ToString("X").PadLeft(2, '0');
			}
			return text.ToLower();
		}

		/// <summary>
		/// 输入byte[]数组或16进制文本文件的全路径文本名
		/// </summary>
		/// <param name="ByteOrPath">字节数组或全路径文件名</param>
		/// <returns>Image图像</returns>
		public static Image ImageFromByte(object ByteOrPath)
		{
			try
			{
				byte[] buffer;
				if (ByteOrPath.GetType().Name == "String")
				{
					string input = File.ReadAllText(ByteOrPath.ToString());
					input = Regex.Replace(input, "\\s+|\r\n", "");
					byte[] array = new byte[input.Length / 2];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = Convert.ToByte("0x" + input.Substring(i * 2, 2), 16);
					}
					buffer = array;
				}
				else
				{
					buffer = (byte[])ByteOrPath;
				}
				MemoryStream obj = new MemoryStream(buffer)
				{
					Position = 0L
				};
				Image result = Image.FromStream(obj);
				obj.Close();
				return result;
			}
			catch
			{
				return null;
			}
		}

		public static Image ImageFromBase64(string PathOrStr)
		{
			try
			{
				string s = (!PathOrStr.Contains("\\")) ? PathOrStr : File.ReadAllText(PathOrStr);
				MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(s));
				Bitmap result = new Bitmap(memoryStream);
				memoryStream.Close();
				return result;
			}
			catch
			{
				return null;
			}
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

		public static string StripHTML(string stringToStrip)
		{
			stringToStrip = Regex.Replace(stringToStrip, "</p(?:\\s*)>(?:\\s*)<p(?:\\s*)>", "\n\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			stringToStrip = Regex.Replace(stringToStrip, "", "\n", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			stringToStrip = Regex.Replace(stringToStrip, "\"", "''", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			stringToStrip = StripHtmlXmlTags(stringToStrip);
			return stringToStrip;
		}

		private static string StripHtmlXmlTags(string content)
		{
			return Regex.Replace(content, "<[^>]+>", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		}

		public static string GetMidStr(string Source, string StartStr, string EndStr)
		{
			try
			{
				int num = Source.IndexOf(StartStr, 0) + StartStr.Length;
				int num2 = Source.IndexOf(EndStr, num);
				return Source.Substring(num, num2 - num);
			}
			catch
			{
				return "";
			}
		}

		public static string GetMidStr(string Source, string EndStr)
		{
			try
			{
				int num = 0;
				for (int num2 = Source.Length - 1; num2 >= 0; num2--)
				{
					if (Source.Substring(num2, 1) == EndStr)
					{
						num = num2;
						break;
					}
				}
				return Source.Substring(0, num + 1);
			}
			catch
			{
				return "";
			}
		}

		public static string GetTime()
		{
			DateTime dateTime = new DateTime(1970, 1, 1);
			return ((DateTime.UtcNow.Ticks - dateTime.Ticks) / 10000).ToString();
		}

		public static string OperationTime(string BaseTime, string AddTime, string AddType)
		{
			DateTime dateTime = Convert.ToDateTime(BaseTime);
			DateTime dateTime2 = DateTime.Now;
			switch (AddType)
			{
			case "年":
				dateTime2 = dateTime.AddYears(int.Parse(AddTime));
				break;
			case "月":
				dateTime2 = dateTime.AddMonths(int.Parse(AddTime));
				break;
			case "日":
				dateTime2 = dateTime.AddDays(double.Parse(AddTime));
				break;
			case "时":
				dateTime2 = dateTime.AddHours(double.Parse(AddTime));
				break;
			case "分":
				dateTime2 = dateTime.AddMinutes(double.Parse(AddTime));
				break;
			case "秒":
				dateTime2 = dateTime.AddSeconds(double.Parse(AddTime));
				break;
			case "毫":
				dateTime2 = dateTime.AddMilliseconds(double.Parse(AddTime));
				break;
			}
			return dateTime2.ToString();
		}

		public static string strToNum(string Month)
		{
			string result = "";
			string[] array = new string[12]
			{
				"Jan",
				"Feb",
				"Mar",
				"Apr",
				"May",
				"Jun",
				"Jul",
				"Aug",
				"Sep",
				"Oct",
				"Nov",
				"Dec"
			};
			string[] array2 = new string[12]
			{
				"01",
				"02",
				"03",
				"04",
				"05",
				"06",
				"07",
				"08",
				"09",
				"10",
				"11",
				"12"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (Month == array[i])
				{
					result = array2[i];
					break;
				}
			}
			return result;
		}
	}
}
