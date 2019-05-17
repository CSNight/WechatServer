using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Utils
{
	public class ffmpegUtils
	{
		private static string url = Directory.GetCurrentDirectory();

		private static ffmpegUtils instance;

		private static object _lock = new object();

		private static int maxNum = 10;

		private ffmpegUtils()
		{
			Init();
		}

		public static ffmpegUtils GetInstance()
		{
			if (instance == null)
			{
				lock (_lock)
				{
					if (instance == null)
					{
						instance = new ffmpegUtils();
					}
				}
			}
			return instance;
		}

		/// <summary>
		///             删除中间件pcm文件
		/// </summary>
		private static void Init()
		{
			if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\cache\\"))
			{
				Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\cache\\");
			}
			string[] fileSystemEntries = Directory.GetFileSystemEntries(Directory.GetCurrentDirectory() + "\\cache\\");
			foreach (string text in fileSystemEntries)
			{
				if (File.Exists(text))
				{
					try
					{
						FileInfo fileInfo = new FileInfo(text);
						if (fileInfo.Extension.ToLower() == ".pcm")
						{
							if (fileInfo.Attributes.ToString().IndexOf("ReadOnly") != -1)
							{
								fileInfo.Attributes = FileAttributes.Normal;
							}
							File.Delete(text);
						}
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message.ToString());
					}
				}
			}
		}

		/// <summary>
		/// amr 转换  mp3 格式
		/// </summary>
		/// <param name="amrFile">amr文件所在路径</param>
		/// <returns></returns>
		public bool ConvertAmrToMp3(string amrFile, string savePath)
		{
			string text = amrFile.Substring(amrFile.LastIndexOf("\\"));
			string text2 = text.Substring(1, text.IndexOf(".") - 1);
			string str = Guid.NewGuid().ToString();
			string copyFile = "silk_v3_encoder_" + str;
			string text3 = url + "\\ffmpeg\\silk_v3_decoder.exe";
			string cmdStr = "\"" + text3 + "\"  \"" + amrFile + "\"  \"" + url + "\\cache\\" + text2 + ".pcm\" -quiet";
			string strCommand = "\"" + url + "\\ffmpeg\\ffmpeg.exe\" -y -f s16le -ar 24000 -ac 1 -i  \"" + url + "\\cache\\" + text2 + ".pcm\"  \"" + savePath + "\"";
			int indexNum = 1;
			try
			{
				bool b = FormatConvert(cmdStr);
				string pcmdir = url + "\\cache\\" + text2 + ".pcm";
				return ContinueTryConvert(amrFile, pcmdir, b, strCommand, savePath, text2, indexNum, copyFile, savePath);
			}
			catch (Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// MP3 转换为 amr格式
		/// </summary>
		/// <param name="mp3File"></param>
		/// <returns></returns>
		public bool ConvertMp3ToAmr(string mp3File, string savePath)
		{
			string text = (mp3File.LastIndexOf("\\") == -1) ? mp3File : mp3File.Substring(mp3File.LastIndexOf("\\"));
			string text2 = text.Substring(1, text.IndexOf(".") - 1);
			string str = Guid.NewGuid().ToString();
			string copyFile = "silk_v3_encoder_" + str;
			string text3 = url + "\\ffmpeg\\silk_v3_encoder.exe";
			string cmdStr = "\"" + url + "\\ffmpeg\\ffmpeg.exe\" -i \"" + mp3File + "\" -f s16le -acodec pcm_s16le -ac 1 -ar 24000 \"" + url + "\\cache\\" + text2 + ".pcm\"";
			string strCommand = "\"" + text3 + "\"  \"" + url + "\\cache\\" + text2 + ".pcm\"  \"" + savePath + "\"  -tencent";
			int indexNum = 1;
			try
			{
				bool b = FormatConvert(cmdStr);
				string pcmdir = url + "\\cache\\" + text2 + ".pcm";
				return ContinueTryConvert(mp3File, pcmdir, b, strCommand, savePath, text2, indexNum, copyFile, savePath);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private bool ContinueTryConvert(string originFile, string pcmdir, bool b, string strCommand, string filePath, string finalName, int indexNum, string copyFile, string savePath)
		{
			if (File.Exists(pcmdir))
			{
				if (b)
				{
					if (MyUtils.IsFileInUse(pcmdir))
					{
						Thread.Sleep(100);
						return ContinueTryConvert(originFile, pcmdir, b, strCommand, filePath, finalName, indexNum, copyFile, savePath);
					}
					FormatConvert(strCommand);
					Task.Factory.StartNew(delegate
					{
						Thread.Sleep(10000);
						try
						{
							File.Delete(pcmdir);
						}
						catch (Exception)
						{
						}
						try
						{
							string text = url + "\\software\\" + copyFile + ".exe";
							if (File.Exists(text))
							{
								new FileInfo(text).Attributes = FileAttributes.Normal;
								File.Delete(text);
							}
						}
						catch (Exception)
						{
						}
					});
					indexNum = 1;
					return ContinueSecondTryConvert(indexNum, filePath, strCommand);
				}
				return false;
			}
			indexNum++;
			if (indexNum <= maxNum)
			{
				Thread.Sleep(200);
				return ContinueTryConvert(originFile, pcmdir, b, strCommand, filePath, finalName, indexNum, copyFile, savePath);
			}
			return false;
		}

		private bool ContinueSecondTryConvert(int indexNum, string finaldir, string strCommand)
		{
			if (!File.Exists(finaldir))
			{
				indexNum++;
				if (indexNum <= maxNum)
				{
					Thread.Sleep(100 * indexNum);
					return ContinueSecondTryConvert(indexNum, finaldir, strCommand);
				}
				FormatConvert(strCommand);
				Thread.Sleep(1000);
				return File.Exists(finaldir);
			}
			return true;
		}

		/// <summary>
		///             解码/转码-格式
		///             @param silk 源silk文件,需要绝对路径! 例:F:\zhuanma\vg2ub41omgipvrmur1fnssd3tq.silk
		///             @param pcm 目标pcm文件,需要绝对路径  例:F:\zhuanma\vg2ub41omgipvrmur1fnssd3tq.pcm
		///             @return
		/// </summary>
		/// <param name="cmdStr"></param>
		/// <returns></returns>
		private static bool FormatConvert(string cmdStr)
		{
			bool result = true;
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardInput = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.RedirectStandardError = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					process.StandardInput.WriteLine(cmdStr + "&exit");
					process.StandardInput.AutoFlush = true;
					return result;
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
