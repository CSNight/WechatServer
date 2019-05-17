using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace WebApi.Utils
{
	public static class MyUtils
	{
		public static bool IsFileInUse(string fileName)
		{
			bool result = true;
			FileStream fileStream = null;
			try
			{
				fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
				result = false;
				return result;
			}
			catch (Exception)
			{
				try
				{
					Process[] processes = Process.GetProcesses();
					foreach (Process process in processes)
					{
						if (process.MainModule.FileName == fileName)
						{
							process.Kill();
						}
					}
					return result;
				}
				catch (Exception)
				{
					return result;
				}
			}
			finally
			{
				fileStream?.Close();
			}
		}

		public static Bitmap Base64StringToImage(string basestr)
		{
			Bitmap result = null;
			try
			{
				MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(basestr));
				Bitmap bitmap = new Bitmap(memoryStream);
				memoryStream.Close();
				result = bitmap;
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Base64StringToImage 转换失败\nException：" + ex.Message);
				return result;
			}
		}

		public static string ImageToBase64String(this Image image)
		{
			return Convert.ToBase64String(image.ImageToBytes());
		}

		/// <summary>
		/// 分析 url 字符串中的参数信息
		/// </summary>
		/// <param name="url">输入的 URL</param>
		/// <param name="baseUrl">输出 URL 的基础部分</param>
		/// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
		public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			nvc = new NameValueCollection();
			baseUrl = "";
			if (url == "")
			{
				return;
			}
			int num = url.IndexOf('?');
			if (num == -1)
			{
				baseUrl = url;
				return;
			}
			baseUrl = url.Substring(0, num);
			if (num == url.Length - 1)
			{
				baseUrl = url;
				return;
			}
			string input = url.Substring(num + 1);
			foreach (Match item in new Regex("(^|&)?(\\w+)=([^&]+)(&|$)?", RegexOptions.Compiled).Matches(input))
			{
				nvc.Add(item.Result("$2").ToLower(), item.Result("$3"));
			}
		}
	}
}
