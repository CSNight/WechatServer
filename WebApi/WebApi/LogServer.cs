using NLog;

namespace WebApi
{
	public class LogServer
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static void Info(string Msg)
		{
			logger.Info(Msg);
		}

		public static void Error(string Msg)
		{
			logger.Error(Msg);
		}
	}
}
