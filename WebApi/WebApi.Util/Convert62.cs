using System;
using System.Runtime.InteropServices;

namespace WebApi.Util
{
	public class Convert62
	{
		/// <summary>
		/// 16进制转字符串
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[DllImport("e.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
		private static extern IntPtr EStrToHex(string context);

		/// <summary>
		/// 16进制转字符串
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static string eStrToHex(string context)
		{
			return Marshal.PtrToStringAnsi(EStrToHex(context)).Substring(0, 344) ?? "";
		}
	}
}
