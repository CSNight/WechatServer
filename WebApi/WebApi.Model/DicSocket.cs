using Fleck;
using System;
using WebApi.WeChat;

namespace WebApi.Model
{
	public class DicSocket
	{
		public IWebSocketConnection socket;

		public XzyWeChatThread weChatThread;

		public DateTime dateTime;
	}
}
