using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace WebApi.Utils
{
	public static class ConvertUtils
	{
		public static Bitmap GetImageFromBase64(string base64string)
		{
			return new Bitmap(new MemoryStream(Convert.FromBase64String(base64string)));
		}

		/// <summary>
		/// 图片转byte
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public static byte[] ImageToBytes(Image image)
		{
			ImageFormat rawFormat = image.RawFormat;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				if (rawFormat.Equals(ImageFormat.Jpeg))
				{
					image.Save(memoryStream, ImageFormat.Jpeg);
				}
				else if (rawFormat.Equals(ImageFormat.Png))
				{
					image.Save(memoryStream, ImageFormat.Png);
				}
				else if (rawFormat.Equals(ImageFormat.Bmp))
				{
					image.Save(memoryStream, ImageFormat.Bmp);
				}
				else if (rawFormat.Equals(ImageFormat.Gif))
				{
					image.Save(memoryStream, ImageFormat.Gif);
				}
				else if (rawFormat.Equals(ImageFormat.Icon))
				{
					image.Save(memoryStream, ImageFormat.Icon);
				}
				byte[] array = new byte[memoryStream.Length];
				memoryStream.Seek(0L, SeekOrigin.Begin);
				memoryStream.Read(array, 0, array.Length);
				return array;
			}
		}
	}
}
