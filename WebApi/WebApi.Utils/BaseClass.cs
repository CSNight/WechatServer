using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace WebApi.Utils
{
	public static class BaseClass
	{
		/// <summary>
		/// 判断是否为空，为空返回true
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNull(this object data)
		{
			if (data == null)
			{
				return true;
			}
			if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 判断是否为空，为空返回true
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static bool IsNull(string data)
		{
			if (data == null)
			{
				return true;
			}
			if (data.GetType() == typeof(string) && string.IsNullOrEmpty(data.ToString().Trim()))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// 将obj类型转换为string
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static string ConvertToString(this object s)
		{
			if (s == null)
			{
				return "";
			}
			return Convert.ToString(s);
		}

		/// <summary>
		/// 将字符串转int32
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static int ConvertToInt32(this string s)
		{
			int result = 0;
			if (s == null)
			{
				return 0;
			}
			int.TryParse(s, out result);
			return result;
		}

		/// <summary>
		/// 将字符串转double
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static double ConvertToDouble(this object s)
		{
			double result = 0.0;
			if (s == null)
			{
				return 0.0;
			}
			double.TryParse(s.ToString(), out result);
			return result;
		}

		/// <summary>
		/// 转换为datetime类型
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime ConvertToDateTime(this string s)
		{
			DateTime result = default(DateTime);
			if (s == null || s == "")
			{
				return DateTime.Now;
			}
			DateTime.TryParse(s, out result);
			return result;
		}

		/// <summary>
		/// 转换为datetime类型的格式字符串
		/// </summary>
		/// <param name="s">要转换的对象</param>
		/// <param name="y">格式化字符串</param>
		/// <returns></returns>
		public static string ConvertToDateTime(this string s, string y)
		{
			DateTime result = default(DateTime);
			DateTime.TryParse(s, out result);
			return result.ToString(y);
		}

		/// <summary>
		/// 将字符串转换成decimal
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static decimal ConvertToDecimal(this object s)
		{
			decimal result = default(decimal);
			if (s == null || s == "")
			{
				return decimal.Zero;
			}
			decimal.TryParse(s.ToString(), out result);
			return result;
		}

		public static string ReplaceHtml(this string s)
		{
			return s.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&")
				.Replace("&quot;", "\"");
		}

		public static string ImageToBase64String(this Image image)
		{
			return Convert.ToBase64String(image.ImageToBytes());
		}

		public static Image Base64StringToImage(this string base64)
		{
			return Convert.FromBase64String(base64).BytesToImage();
		}

		public static byte[] ImageToBytes(this Image image)
		{
			ImageFormat rawFormat = image.RawFormat;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, ImageFormat.Png);
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Seek(0L, SeekOrigin.Begin);
				memoryStream.Read(array, 0, array.Length);
				return array;
			}
		}

		public static Image BytesToImage(this byte[] buffer)
		{
			return Image.FromStream(new MemoryStream(buffer));
		}

		public static string Utf8ToAnsi(this string str)
		{
			return Encoding.Default.GetString(Encoding.UTF8.GetBytes(str));
		}
	}
}
