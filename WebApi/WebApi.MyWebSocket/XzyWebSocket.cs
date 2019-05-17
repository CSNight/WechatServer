using Fleck;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using WebApi.Model;
using WebApi.Utils;
using WebApi.WeChat;

namespace WebApi.MyWebSocket
{
	public class XzyWebSocket
	{
		/// <summary>
		/// websocket 连接池
		/// </summary>
		public static Dictionary<string, DicSocket> _dicSockets = new Dictionary<string, DicSocket>();

		/// <summary>
		/// 初始化socket服务
		/// </summary>
		public static void Init()
		{
			new WebSocketServer("ws://0.0.0.0:" + ConfigurationManager.AppSettings["WebSocketHost"]).Start(delegate(IWebSocketConnection socket)
			{
				socket.OnOpen = delegate
				{
					string baseUrl = "";
					MyUtils.ParseUrl(socket.ConnectionInfo.Path, out baseUrl, out NameValueCollection nvc);
					string a = nvc["action"];
					string key = nvc["uuid"];
					string devicename = nvc["devicename"];
					string a2 = nvc["isreset"];
					string proxy = nvc["proxy"];
					string proxyname = nvc["proxyname"];
					string proxypwd = nvc["proxypwd"];
					string text = nvc["proxytype"];
					if (text == "")
					{
						text = "1";
					}
					ScanLoginModel model = new ScanLoginModel
					{
						devicename = devicename,
						proxy = proxy,
						proxyname = proxyname,
						proxypwd = proxypwd,
						proxytype = text.ConvertToInt32()
					};
					if (a == "scan")
					{
						if (_dicSockets.ContainsKey(key) && a2 == "false")
						{
							_dicSockets[key].socket = socket;
							_dicSockets[key].weChatThread._socket = socket;
							_dicSockets[key].weChatThread.SocketIsConnect = true;
						}
						else
						{
							XzyWeChatThread xzyWeChatThread = new XzyWeChatThread(socket, model);
							DicSocket value = new DicSocket
							{
								socket = socket,
								weChatThread = xzyWeChatThread
							};
							_dicSockets.Remove(key);
							_dicSockets.Add(key, value);
							xzyWeChatThread.SocketIsConnect = true;
						}
					}
					else if (a == "62")
					{
						string username = nvc["username"];
						string password = nvc["password"];
						string str = nvc["str62"];
						string proxy2 = nvc["proxy"];
						string proxyname2 = nvc["proxyname"];
						string proxypwd2 = nvc["proxypwd"];
						string s = nvc["proxytype"];
						UserLoginModel model2 = new UserLoginModel
						{
							username = username,
							password = password,
							str62 = str,
							proxy = proxy2,
							proxyname = proxyname2,
							proxypwd = proxypwd2,
							proxytype = s.ConvertToInt32()
						};
						if (_dicSockets.ContainsKey(key) && a2 == "false")
						{
							_dicSockets[key].socket = socket;
							_dicSockets[key].weChatThread._socket = socket;
							_dicSockets[key].weChatThread.SocketIsConnect = true;
						}
						else
						{
							XzyWeChatThread xzyWeChatThread2 = new XzyWeChatThread(socket, model2);
							DicSocket value2 = new DicSocket
							{
								socket = socket,
								weChatThread = xzyWeChatThread2
							};
							_dicSockets.Remove(key);
							_dicSockets.Add(key, value2);
							xzyWeChatThread2.SocketIsConnect = true;
						}
					}
				};
				socket.OnClose = delegate
				{
					try
					{
						(from p in _dicSockets
							where p.Value.socket == socket
							select p).ToList().FirstOrDefault().Value.weChatThread.SocketIsConnect = false;
					}
					catch (Exception)
					{
					}
				};
				socket.OnMessage = delegate
				{
				};
			});
		}
	}
}
