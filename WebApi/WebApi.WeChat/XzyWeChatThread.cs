using Fleck;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Model;
using WebApi.MyWebSocket;
using WebApi.Utils;
using WeChat.Core;

namespace WebApi.WeChat
{
	/// <summary>
	/// 微信操作线程
	/// </summary>
	public class XzyWeChatThread
	{
		public IWebSocketConnection _socket;

		private bool WxFlag;

		public int pointerWxUser;

		public int pushStr;

		public int result;

		public int msgPtr;

		public int callBackMsg;

		private string name = "changtuiqie-ipad";

		private int proxytype = 1;

		private string proxy = "";

		private string proxyname = "";

		private string proxypwd = "";

		public int wxMeId;

		public string bankerWxid = "";

		public string cheshouWxid = "";

		public string groupId = "";

		/// <summary>
		/// socket 链接状态
		/// </summary>
		public bool SocketIsConnect;

		/// <summary>
		/// 登录二维码
		/// </summary>
		public string ScanQrCode = "";

		/// <summary>
		/// 微信对象
		/// </summary>
		public UserData userData = new UserData();

		private WxUser wxUser = new WxUser();

		private Dictionary<string, string> dicRedPack;

		private Dictionary<string, string> dicReadContent;

		public List<Contact> wxContacts = new List<Contact>();

		public List<Contact> wxGroups = new List<Contact>();

		public List<Contact> wxGzhs = new List<Contact>();

		private Random R = new Random();

		public object wx_objMsg = new object();

		public object wx_groupObj = new object();

		public int redPack;

		public object obj = new object();

		private List<WxGroup> wxGroup
		{
			get;
			set;
		}

		private string Mac
		{
			get
			{
				int minValue = 0;
				int maxValue = 16;
				Random random = new Random();
				return string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x"), random.Next(minValue, maxValue).ToString("x")).ToUpper();
			}
		}

		private string UUID => RandomStr(8) + "-" + RandomStr(4) + "-" + RandomL(4).ToString("X") + "-" + RandomL(4).ToString("X") + "-" + RandomL(12).ToString("X");

		public XzyWxApis.DllcallBack msgCallBack
		{
			get;
			set;
		}

		public static int TimeStamp => Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds - 180.0);

		public Dictionary<string, string> dic_redpack
		{
			get;
			set;
		}

		private string RandomStr(int n)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < n; i++)
			{
				list.Add(R.Next(0, 9));
			}
			return string.Join("", list);
		}

		private long RandomL(int n)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < n; i++)
			{
				list.Add(R.Next(0, 9));
			}
			return Convert.ToInt64(string.Join("", list));
		}

		/// <summary>
		/// 初始化，并且使用二维码登录。并且初始化一些计时器，但实际上这些计时器没有什么用。这些计时器，应该是为了实现类似signalr的功能
		/// </summary>
		public XzyWeChatThread(IWebSocketConnection socket, ScanLoginModel model)
		{
			if (model.devicename != "")
			{
				name = model.devicename;
			}
			if (model.proxy != "")
			{
				proxy = model.proxy;
				proxyname = model.proxyname;
				proxypwd = model.proxypwd;
				proxytype = model.proxytype;
			}
			_socket = socket;
			dicRedPack = new Dictionary<string, string>();
			dicReadContent = new Dictionary<string, string>();
			Task.Factory.StartNew(delegate
			{
				Init();
			});
			msgCallBack = (XzyWxApis.DllcallBack)Delegate.Combine(msgCallBack, new XzyWxApis.DllcallBack(Wx_MsgCallBack));
		}

		/// <summary>
		/// 使用62数据 账号密码进行初始化
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="str62"></param>
		/// <param name="devicename"></param>
		public XzyWeChatThread(IWebSocketConnection socket, UserLoginModel model)
		{
			if (model.devicename != "")
			{
				name = model.devicename;
			}
			if (model.proxy != "")
			{
				proxy = model.proxy;
				proxyname = model.proxyname;
				proxypwd = model.proxypwd;
				proxytype = model.proxytype;
			}
			_socket = socket;
			dicRedPack = new Dictionary<string, string>();
			dicReadContent = new Dictionary<string, string>();
			Task.Factory.StartNew(delegate
			{
				Init62(model.str62, model.username, model.password);
			});
			msgCallBack = (XzyWxApis.DllcallBack)Delegate.Combine(msgCallBack, new XzyWxApis.DllcallBack(Wx_MsgCallBack));
		}

		private void WebSocketSend(string msg)
		{
			try
			{
				if (!_socket.IsNull() && SocketIsConnect)
				{
					_socket.Send(msg);
				}
			}
			catch (Exception)
			{
			}
		}

		private void WebSocketSendLog(string msg)
		{
			try
			{
				if (!_socket.IsNull() && SocketIsConnect)
				{
					SocketModel socketModel = new SocketModel();
					socketModel.action = "log";
					socketModel.context = msg;
					_socket.Send(JsonConvert.SerializeObject(socketModel));
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 登录
		/// </summary>
		[HandleProcessCorruptedStateExceptions]
		public unsafe void Init()
		{
            try
			{
				try
				{
					fixed (int* ptr = &pointerWxUser)
					{
						int* intPtr = ptr;
						fixed (int* value = &pushStr)
						{
                            
							string uUID = UUID;
							string mac = Mac;
							XzyAuth.GetIpPort(ConfigurationManager.AppSettings["IpCode"]);
                            string msg = XzyAuth.Init(ConfigurationManager.AppSettings["AuthKey"]);
							WebSocketSendLog(msg);
							XzyWxApis.WXInitialize(device_type: string.Format(App.DeviceKey, UUID, Mac), objects: (int)intPtr, device_name: name, device_uuid: UUID);
							if (proxy != "")
							{
								Wx_SetProxyInfo(proxy, proxytype, proxyname, proxypwd);
							}
							XzyWxApis.WXSetRecvMsgCallBack(pointerWxUser, msgCallBack);
							XzyWxApis.WXGetQRCode(pointerWxUser, (IntPtr)(void*)value);
							WxQrCode wxQrCode = JsonConvert.DeserializeObject<WxQrCode>(Marshal.PtrToStringAnsi(new IntPtr(Convert.ToInt32(pushStr))));
							ScanQrCode = wxQrCode.QrCodeStr;
							SocketModel value2 = new SocketModel
							{
								action = "qrcode",
								context = wxQrCode.QrCodeStr
							};
							WebSocketSend(JsonConvert.SerializeObject(value2));
							Wx_ReleaseEX(ref pushStr);
							QrCodeJson qrCodeJson = null;
							while (!WxFlag)
							{
								Thread.Sleep(1000);
								XzyWxApis.WXCheckQRCode(pointerWxUser, (IntPtr)(void*)value);
								object obj = MarshalNativeToManaged((IntPtr)pushStr);
								if (obj != null)
								{
									qrCodeJson = JsonConvert.DeserializeObject<QrCodeJson>(obj.ConvertToString());
									Wx_ReleaseEX(ref pushStr);
									bool flag = false;
									switch (qrCodeJson.Status)
									{
									case 0:
										WebSocketSendLog("请扫描二维码");
										break;
									case 1:
										WebSocketSendLog("请点在手机上点确认");
										break;
									case 2:
										WebSocketSendLog("正在登录中");
										flag = true;
										break;
									case 3:
										WebSocketSendLog("已过期");
										break;
									case 4:
										WebSocketSendLog("取消操作了");
										flag = true;
										break;
									case -2007:
										WebSocketSendLog("已过期");
										return;
									}
									if (flag)
									{
										break;
									}
								}
							}
							if (qrCodeJson.Status == 2)
							{
								string userName = qrCodeJson.UserName;
								wxUser.wxid = qrCodeJson.UserName;
								wxUser.name = qrCodeJson.NickName;
								string password = qrCodeJson.Password;
								XzyWxApis.WXQRCodeLogin(pointerWxUser, userName, password, (IntPtr)(void*)value);
								string value3 = MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
								Wx_ReleaseEX(ref pushStr);
								userData = JsonConvert.DeserializeObject<UserData>(value3);
								userData.HeadImg = qrCodeJson.HeadUrl;
								if (userData.Status != -301)
								{
									goto IL_039a;
								}
								XzyWxApis.WXQRCodeLogin(pointerWxUser, userName, password, (IntPtr)(void*)value);
								value3 = MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
								Wx_ReleaseEX(ref pushStr);
								userData = JsonConvert.DeserializeObject<UserData>(value3);
								userData.HeadImg = qrCodeJson.HeadUrl;
								if (userData.Status != 0)
								{
									WebSocketSendLog("登录失败：" + userData.Message);
									goto IL_039a;
								}
								XzyWxApis.WXHeartBeat(pointerWxUser, (IntPtr)(void*)value);
								MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
								Wx_ReleaseEX(ref pushStr);
								WebSocketSendLog("登录成功");
								Wx_GetContacts();
							}
							goto end_IL_0012;
							IL_039a:
							if (userData.Status == 0)
							{
								XzyWxApis.WXHeartBeat(pointerWxUser, (IntPtr)(void*)value);
								MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
								Wx_ReleaseEX(ref pushStr);
								WebSocketSendLog("登录成功");
								Wx_GetContacts();
							}
							else
							{
								WebSocketSendLog("登录失败：" + userData.Message);
							}
							end_IL_0012:;
						}
					}
				}
				finally
				{
				}
			}
			catch (Exception)
			{
			}
		}

		/// <summary>
		/// 初始化62数据
		/// </summary>
		/// <param name="str16"></param>
		/// <param name="WxUsername"></param>
		/// <param name="wxpassword"></param>
		public unsafe void Init62(string str16, string WxUsername, string wxpassword)
		{
			try
			{
				try
				{
					fixed (int* ptr = &pointerWxUser)
					{
						int* intPtr = ptr;
						fixed (int* value = &pushStr)
						{
							XzyAuth.GetIpPort(ConfigurationManager.AppSettings["IpCode"]);
							XzyAuth.Init(ConfigurationSettings.AppSettings["AuthKey"].ConvertToString());
							string uUID = UUID;
							string mac = Mac;
							XzyWxApis.WXInitialize(device_type: $"<softtype><k3>11.0.1</k3><k9>iPad</k9><k10>2</k10><k19>58BF17B5-2D8E-4BFB-A97E-38F1226F13F8</k19><k20>{UUID}</k20><k21>neihe_5GHz</k21><k22>(null)</k22><k24>{Mac}</k24><k33>\\345\\276\\256\\344\\277\\241</k33><k47>1</k47><k50>1</k50><k51>com.tencent.xin</k51><k54>iPad4,4</k54></softtype>", objects: (int)intPtr, device_name: name, device_uuid: UUID);
							if (proxy != "")
							{
								Wx_SetProxyInfo(proxy, proxytype, proxyname, proxypwd);
							}
							XzyWxApis.WXSetRecvMsgCallBack(pointerWxUser, msgCallBack);
							byte[] array = Convert.FromBase64String(str16);
							XzyWxApis.WXLoadWxDat(pointerWxUser, array, array.Length, (IntPtr)(void*)value);
							if (string.IsNullOrEmpty(MarshalNativeToManaged((IntPtr)pushStr).ConvertToString()))
							{
								WebSocketSendLog("登陆失败，重新登录");
							}
							Wx_ReleaseEX(ref pushStr);
							XzyWxApis.WXUserLogin(pointerWxUser, WxUsername, wxpassword, (IntPtr)(void*)value);
							string value2 = MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
							Wx_ReleaseEX(ref pushStr);
							userData = JsonConvert.DeserializeObject<UserData>(value2);
							if (userData.Status != -301)
							{
								goto IL_0262;
							}
							XzyWxApis.WXUserLogin(pointerWxUser, WxUsername, wxpassword, (IntPtr)(void*)value);
							value2 = MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
							Wx_ReleaseEX(ref pushStr);
							WebSocketSendLog("微信重定向");
							userData = JsonConvert.DeserializeObject<UserData>(value2);
							wxUser.wxid = userData.UserName;
							wxUser.name = userData.NickName;
							if (userData.Status != 0)
							{
								WebSocketSendLog("登录失败");
								goto IL_0262;
							}
							WebSocketSendLog("登录成功");
							XzyWxApis.WXHeartBeat(pointerWxUser, (IntPtr)(void*)value);
							MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
							Wx_ReleaseEX(ref pushStr);
							Task.Factory.StartNew(delegate
							{
								Wx_GetContacts();
							});
							Wx_ReleaseEX(ref pushStr);
							goto end_IL_0012;
							IL_0262:
							if (userData.Status == 0)
							{
								WebSocketSendLog("登录成功");
								XzyWxApis.WXHeartBeat(pointerWxUser, (IntPtr)(void*)value);
								MarshalNativeToManaged((IntPtr)pushStr).ConvertToString();
								Wx_ReleaseEX(ref pushStr);
								wxUser.wxid = userData.UserName;
								wxUser.name = userData.NickName;
								Task.Factory.StartNew(delegate
								{
									Wx_GetContacts();
								});
								Wx_ReleaseEX(ref pushStr);
							}
							else
							{
								WebSocketSendLog("登录失败");
							}
							end_IL_0012:;
						}
					}
				}
				finally
				{
				}
			}
			catch (Exception ex)
			{
				LogServer.Error(ex.Message);
			}
		}

		/// <summary>
		/// 微信返回消息解码
		/// </summary>
		/// <param name="pNativeData"></param>
		/// <returns></returns>
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			try
			{
				if ((int)pNativeData <= 0)
				{
					return null;
				}
				List<byte> list = new List<byte>();
				int num = 0;
				while (true)
				{
					byte b = Marshal.ReadByte(pNativeData, num);
					if (b == 0)
					{
						break;
					}
					list.Add(b);
					num++;
				}
				return Encoding.UTF8.GetString(list.ToArray(), 0, list.Count);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// 打印消息
		/// </summary>
		/// <param name="msg"></param>
		private void ShowMessage(string msg)
		{
		}

		/// <summary>
		/// 获取视频消息
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetMsgVideo(string msg)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetMsgVideo(pointerWxUser, msg, (IntPtr)(void*)value);
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 获取语音消息
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetMsgVoice(string msg)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetMsgVoice(pointerWxUser, msg, (IntPtr)(void*)value);
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 获取图片消息
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="content"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetMsgImage(string msg)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetMsgImage(pointerWxUser, msg, (IntPtr)(void*)value);
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 发消息 -文字
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="content"></param>
		/// <param name="atlist"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendMsg(string wxid, string content, List<string> atlist)
		{
			content = content.Replace(" ", "\r\n");
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						if (atlist.Count > 0)
						{
							XzyWxApis.WXSendMsg(pointerWxUser, wxid, content, JsonConvert.SerializeObject(atlist), (IntPtr)(void*)value);
						}
						else
						{
							XzyWxApis.WXSendMsg(pointerWxUser, wxid, content, null, (IntPtr)(void*)value);
						}
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 群发消息
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_MassMessage(string wxid, string content)
		{
			content = content.Replace(" ", "\r\n");
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXMassMessage(pointerWxUser, wxid, content, (IntPtr)(void*)value);
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 发消息 - 图片
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="imgpath"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe void Wx_SendImg(string wxid, string imgpath)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						byte[] array = ConvertUtils.ImageToBytes(Image.FromStream(WebRequest.Create(imgpath).GetResponse().GetResponseStream()));
						if (array.Length != 0)
						{
							XzyWxApis.WXSendImage(pointerWxUser, wxid, array, array.Length, (IntPtr)(void*)value);
							MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
		}

		/// <summary>
		/// 发消息 - 图片
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="imgpath"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendImg(string wxid, Image _image)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						byte[] array = ConvertUtils.ImageToBytes(_image);
						XzyWxApis.WXSendImage(pointerWxUser, wxid, array, array.Length, (IntPtr)(void*)value);
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
		}

		/// <summary>
		/// 发语音 - silk
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="imgpath"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendVoice(string wxid, string silkpath, int time)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						FileStream fileStream = new FileStream(silkpath, FileMode.Open, FileAccess.Read);
						byte[] array = new byte[fileStream.Length];
						fileStream.Read(array, 0, array.Length);
						fileStream.Close();
						if (array.Length != 0)
						{
							XzyWxApis.WXSendVoice(pointerWxUser, wxid, array, array.Length, time * 1000, (IntPtr)(void*)value);
							text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 分享名片
		/// </summary>
		/// <param name="user"></param>
		/// <param name="wxid"></param>
		/// <param name="title"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ShareCard(string user, string wxid, string title)
		{
			string text;
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXShareCard(pointerWxUser, user, wxid, title.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return "";
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_EShareCard(string user, string wxid, string title)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.EShareCarde(pointerWxUser, user, wxid, title);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch
					{
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 微信消息 - 回调
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe void Wx_MsgCallBack(int a, IntPtr b)
		{
      
            
            if (b.ConvertToString().ConvertToInt32() == -1)
			{
				string uuid = "";
				lock (XzyWebSocket._dicSockets)
				{
					try
					{
						uuid = (from p in XzyWebSocket._dicSockets
							where p.Value.weChatThread.wxUser == wxUser
							select p).ToList().FirstOrDefault().Key;
						if (uuid.ConvertToString() != "")
						{
							WebSocketSendLog("您的账号" + uuid + "已下线");
							msgCallBack = (XzyWxApis.DllcallBack)Delegate.Remove(msgCallBack, new XzyWxApis.DllcallBack(Wx_MsgCallBack));
							Task.Factory.StartNew(delegate
							{
								Thread.Sleep(10000);
								XzyWebSocket._dicSockets[uuid].weChatThread = null;
								XzyWebSocket._dicSockets.Remove(uuid);
							});
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			else
			{
				fixed (int* ptr = &pointerWxUser)
				{
					fixed (int* value = &callBackMsg)
					{
						try
						{
							XzyWxApis.WXSyncMessage(pointerWxUser, (IntPtr)(void*)value);
							if (callBackMsg == 0)
							{
								return;
							}
							string value2 = MarshalNativeToManaged((IntPtr)callBackMsg).ConvertToString();
							new List<BackWxMsg>();
							Wx_ReleaseEX(ref callBackMsg);
							foreach (WxTtsMsg item in JsonConvert.DeserializeObject<List<WxTtsMsg>>(value2))
							{
								int msgType = item.MsgType;
								string content = item.Content;
								int subType = item.SubType;
								string msgId = item.MsgId;
								if (item.Timestamp >= TimeStamp && Wx_SetMsgKey(msgId))
								{
									try
									{
										item.mywxid = userData.UserName;
										ApiClient.Instance.Post(ConfigurationManager.AppSettings["MsgCallBackUrl"], item);
									}
									catch
									{
									}
									SocketModel value3 = new SocketModel
									{
										action = "msgcallback",
										context = JsonConvert.SerializeObject(item)
									};
									WebSocketSend(JsonConvert.SerializeObject(value3));
								}
							}
						}
						catch (Exception ex2)
						{
							LogServer.Error(ex2.Message);
						}
					}
				}
			}
		}

		/// <summary>
		/// 设置消息。用于判断消息是否被处理过。若未处理过，则返回true，已经处理过的，返回false。
		/// </summary>
		/// <param name="msgid"></param>
		/// <returns></returns>
		public bool Wx_SetMsgKey(string msgid)
		{
			lock (wx_objMsg)
			{
				try
				{
					if (dicReadContent.Count > 5000)
					{
						dicReadContent = new Dictionary<string, string>();
					}
					if (!dicReadContent.ContainsKey(msgid))
					{
						dicReadContent.Add(msgid, msgid);
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// 取群成员
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe List<WxMember> Wx_GetGroupMember(string groupId)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetChatRoomMember(pointerWxUser, groupId, (IntPtr)(void*)value);
						string value2 = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						List<Member> list = JsonConvert.DeserializeObject<List<Member>>(JsonConvert.DeserializeObject<GroupMember>(value2).Member);
						List<WxMember> list2 = new List<WxMember>();
						if (list != null && list.Count > 0)
						{
							foreach (Member item in list)
							{
								WxMember wxMember = new WxMember();
								wxMember.userid = wxUser.wxid;
								wxMember.groupid = groupId;
								wxMember.nickname = item.NickName;
								wxMember.wxid = item.UserName;
								list2.Add(wxMember);
							}
							return list2;
						}
						return null;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return null;
					}
				}
			}
		}

		/// <summary>
		/// 进群
		/// </summary>
		/// <param name="url"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe void Wx_IntoGroup(string url)
		{
			if (url != "")
			{
				fixed (int* ptr2 = &pointerWxUser)
				{
					fixed (int* ptr = &msgPtr)
					{
						try
						{
							XzyWxApis.WXGetRequestToken(pointerWxUser, "", url, (IntPtr)(void*)ptr);
							if (ptr == null)
							{
								return;
							}
							string text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
							if (text == "")
							{
								return;
							}
							EnterGroupJson enterGroupJson = JsonConvert.DeserializeObject<EnterGroupJson>(text);
							string fullUrl = enterGroupJson.FullUrl;
							Utilities.GetMidStr(enterGroupJson.FullUrl + "||||", "ticket=", "||||");
							Http_Helper http_Helper = new Http_Helper();
							string pResponsetext = "";
							http_Helper.GetResponse_WX(ref pResponsetext, fullUrl, "POST", "", fullUrl, 30000);
							Wx_GetContacts();
						}
						catch (Exception ex)
						{
							LogServer.Error(ex.Message);
						}
					}
				}
			}
		}

		/// <summary>
		/// 更新群成员
		/// </summary>
		/// <param name="groupId"></param>
		/// <returns></returns>
		public bool Wx_SetGroup(string groupId)
		{
			lock (wx_groupObj)
			{
				try
				{
					List<WxGroup> list = new List<WxGroup>();
					for (int i = 0; i < this.wxGroup.Count; i++)
					{
						WxGroup wxGroup = this.wxGroup[i];
						if (groupId == wxGroup.groupid)
						{
							wxGroup.member = Wx_GetGroupMember(this.wxGroup[i].groupid);
						}
						if (wxGroup.member != null)
						{
							list.Add(wxGroup);
						}
					}
					this.wxGroup = list;
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// 创建群
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_CreateChatRoom(string users)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXCreateChatRoom(pointerWxUser, users, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						var anon = JsonConvert.DeserializeAnonymousType(text, new
						{
							user_name = ""
						});
						if (!string.IsNullOrEmpty(anon.user_name))
						{
							text = anon.user_name;
						}
						text.Contains("@chatroom");
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 退群
		/// </summary>
		/// <param name="groupid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_QuitChatRoom(string groupid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXQuitChatRoom(pointerWxUser, groupid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 修改群名称
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetChatroomName(string groupid, string content)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetChatroomName(pointerWxUser, groupid, content.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 修改群名称
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESetChatroomName(string groupid, string content)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESetChatroomName(pointerWxUser, groupid, content);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 修改群公告
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetChatroomAnnouncement(string groupid, string content)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetChatroomAnnouncement(pointerWxUser, groupid, content.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 修改群公告
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESetChatroomAnnouncement(string groupid, string content)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESetChatroomAnnouncement(pointerWxUser, groupid, content);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取群、好友二维码
		/// </summary>
		/// <param name="userid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetUserQRCode(string userid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetUserQRCode(pointerWxUser, userid, 0, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取群成员资料
		/// </summary>
		/// <param name="groupid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetChatRoomMember(string groupid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetChatRoomMember(pointerWxUser, groupid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 添加群成员
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_AddChatRoomMember(string groupid, string user)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXAddChatRoomMember(pointerWxUser, groupid, user, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_InviteChatRoomMember(string groupid, string user)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXInviteChatRoomMember(pointerWxUser, groupid, user, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 删除群成员
		/// </summary>
		/// <param name="groupid"></param>
		/// <param name="user"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_DeleteChatRoomMember(string groupid, string user)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXDeleteChatRoomMember(pointerWxUser, groupid, user, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 朋友圈评论
		/// </summary>
		/// <param name="snsid"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsComment(string snsid, string content, int replyid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsComment(pointerWxUser, wxUser.wxid, snsid, content.Utf8ToAnsi(), replyid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESnsComment(string snsid, string content, int replyid)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESnsComment(pointerWxUser, wxUser.wxid, snsid, content, replyid);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 同步朋友圈
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsSync(string key)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsSync(pointerWxUser, key, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 删除评论、点赞
		/// </summary>
		/// <param name="snsid"></param>
		/// <param name="cid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsObjectOpDeleteComment(string snsid, int cid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsObjectOp(pointerWxUser, snsid, 4, cid, 3, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取朋友圈消息详情
		/// </summary>
		/// <param name="snsid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsObjectDetail(string snsid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsObjectDetail(pointerWxUser, snsid, (IntPtr)(void*)value);
						object obj = MarshalNativeToManaged((IntPtr)msgPtr);
						if (obj != null)
						{
							text = obj.ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 查看指定用户朋友圈
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="snsid">获取到的最后一次的id，第一次调用设置为空</param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsUserPage(string wxid, string snsid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsUserPage(pointerWxUser, wxid, snsid, (IntPtr)(void*)value);
						object obj = MarshalNativeToManaged((IntPtr)msgPtr);
						if (obj != null)
						{
							text = obj.ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 发朋友圈
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="imagelist">图片数组</param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendMoment(string content, List<string> imagelist)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					if (!imagelist.IsNull())
					{
						try
						{
							string text = "";
							foreach (string item in imagelist)
							{
								string input = new Regex("data:image/(.*);base64,").Replace(item, "");
								input = new Regex("data:video/(.*);base64,").Replace(input, "");
								SnsUpload snsUpload = JsonConvert.DeserializeObject<SnsUpload>(Wx_SnsUpload(input));
								text += string.Format(App.PYQContentImage, snsUpload.big_url, snsUpload.small_url, snsUpload.size, 100, 100);
							}
							string content2 = string.Format(App.PYQContent, wxUser.wxid, content.Utf8ToAnsi(), text);
							XzyWxApis.WXSendMoments(pointerWxUser, content2, (IntPtr)(void*)value);
							content2 = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
							return content2;
						}
						catch (Exception ex)
						{
							LogServer.Error(ex.Message);
							return "";
						}
					}
					return "参数不正确";
				}
			}
		}

		/// <summary>
		/// 发朋友圈
		/// </summary>
		/// <param name="content">内容</param>
		/// <param name="imagelist">图片数组</param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESendMoment(string content, List<string> imagelist)
		{
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					if (!imagelist.IsNull())
					{
						try
						{
							string text = "";
							foreach (string item in imagelist)
							{
								string input = new Regex("data:image/(.*);base64,").Replace(item, "");
								input = new Regex("data:video/(.*);base64,").Replace(input, "");
								SnsUpload snsUpload = JsonConvert.DeserializeObject<SnsUpload>(Wx_SnsUpload(input));
								text += string.Format(App.PYQContentImage, snsUpload.big_url, snsUpload.small_url, snsUpload.size, 100, 100);
								Thread.Sleep(1000);
							}
							string xml = string.Format(App.EPYQContent, wxUser.wxid, text);
							msgPtr = EUtils.ESendSNSImage(pointerWxUser, xml, content);
							string text2 = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
							return text2;
						}
						catch (Exception ex)
						{
							LogServer.Error(ex.Message);
							return "";
						}
					}
					return "参数错误";
				}
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESendLink(string title, string text, string url)
		{
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					string text2 = "";
					try
					{
						string xml = string.Format(App.SnsLink, wxUser.wxid, url);
						int hande = EUtils.ESendSNSLink(pointerWxUser, xml, title, text);
						text2 = MarshalNativeToManaged((IntPtr)hande).ConvertToString();
						Wx_ReleaseEX(ref hande);
						return text2;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return text2;
					}
				}
			}
		}

		/// <summary>
		/// 发送文字朋友圈
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendMoment(string content)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSendMoments(pointerWxUser, content.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return text;
					}
				}
			}
		}

		/// <summary>
		/// 发送文字朋友圈
		/// </summary>
		/// <param name="content"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESendMoment(string content)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESendSNS(pointerWxUser, content);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
						return text;
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
						return text;
					}
				}
			}
		}

		/// <summary>
		/// 查看朋友圈 ID第一次传空
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsTimeline(string id)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSnsTimeline(pointerWxUser, id, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 朋友圈图片上传
		/// </summary>
		/// <param name="base64"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SnsUpload(string base64)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						byte[] array = Convert.FromBase64String(base64);
						if (array.Length != 0)
						{
							XzyWxApis.WXSnsUpload(pointerWxUser, array, array.Length, (IntPtr)(void*)value);
							text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 设置头像
		/// </summary>
		/// <param name="base64"></param>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetHeadImage(string base64)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						byte[] array = Convert.FromBase64String(base64);
						if (array.Length != 0)
						{
							XzyWxApis.WXSetHeadImage(pointerWxUser, array, array.Length, (IntPtr)(void*)value);
							text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
							Wx_ReleaseEX(ref msgPtr);
						}
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取通讯录
		/// </summary>
		[HandleProcessCorruptedStateExceptions]
		public unsafe void Wx_GetContacts()
		{
			if (wxGroup == null || wxGroup.Count <= 0)
			{
				wxGroup = new List<WxGroup>();
				new Dictionary<string, string>();
				fixed (int* ptr = &pointerWxUser)
				{
					fixed (int* value = &msgPtr)
					{
						while (true)
						{
							try
							{
								Thread.Sleep(200);
								XzyWxApis.WXSyncContact(pointerWxUser, (IntPtr)(void*)value);
								if (msgPtr != 0)
								{
									object obj = MarshalNativeToManaged((IntPtr)msgPtr);
									Wx_ReleaseEX(ref msgPtr);
									if (obj != null)
									{
										List<Contact> list = JsonConvert.DeserializeObject<List<Contact>>(obj.ConvertToString());
										result = 0;
										int num = 0;
										foreach (Contact item in list)
										{
											num = item.Continue;
											if (num == 0)
											{
												break;
											}
											if (!((object)item.UserName).IsNull())
											{
												if (item.UserName.IndexOf("@chatroom") == -1 && item.UserName.IndexOf("gh_") == -1)
												{
													wxContacts.Add(item);
													SocketModel value2 = new SocketModel
													{
														action = "getcontact",
														context = JsonConvert.SerializeObject(item)
													};
													WebSocketSend(JsonConvert.SerializeObject(value2));
												}
												else if (item.UserName.IndexOf("@chatroom") != -1)
												{
													wxGroups.Add(item);
													SocketModel value3 = new SocketModel
													{
														action = "getgroup",
														context = JsonConvert.SerializeObject(item)
													};
													WebSocketSend(JsonConvert.SerializeObject(value3));
												}
												else if (item.UserName.IndexOf("gh_") != -1)
												{
													wxGzhs.Add(item);
													SocketModel value4 = new SocketModel
													{
														action = "getgzh",
														context = JsonConvert.SerializeObject(item)
													};
													WebSocketSend(JsonConvert.SerializeObject(value4));
												}
											}
										}
										if (num == 0)
										{
											break;
										}
									}
								}
							}
							catch (Exception ex)
							{
								LogServer.Error(ex.Message);
							}
						}
						XzyWxApis.WXSyncReset(pointerWxUser);
					}
				}
			}
		}

		/// <summary>
		/// 接受好友请求
		/// </summary>
		/// <param name="stranger"></param>
		/// <param name="ticket"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_AcceptUser(string stranger, string ticket)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXAcceptUser(pointerWxUser, stranger, ticket, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 注销
		/// </summary>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_Logout()
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXLogout(pointerWxUser, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 二维码登录
		/// </summary>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_QRCodeLogin(string username, string password)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXQRCodeLogin(pointerWxUser, username, password, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取好友详情
		/// </summary>
		/// <param name="wxid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetContact(string wxid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetContact(pointerWxUser, wxid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 设置用户备注
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetUserRemark(string wxid, string context)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetUserRemark(pointerWxUser, wxid, context.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESetUserRemark(string wxid, string context)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESetUserRemark(pointerWxUser, wxid, context);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 删除好友
		/// </summary>
		/// <param name="wxid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_DeleteUser(string wxid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXDeleteUser(pointerWxUser, wxid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取登录token
		/// </summary>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetLoginToken()
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetLoginToken(pointerWxUser, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 设置微信id
		/// </summary>
		/// <param name="wxid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetWeChatID(string wxid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetWeChatID(pointerWxUser, wxid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取本地二维码信息
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_QRCodeDecode(string path)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXQRCodeDecode(pointerWxUser, path, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取其他设备登陆请求
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ExtDeviceLoginGet(string url)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXExtDeviceLoginGet(pointerWxUser, url, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 确认其他设备登陆请求
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ExtDeviceLoginOK(string url)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXExtDeviceLoginOK(pointerWxUser, url, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 设置用户资料
		/// </summary>
		/// <param name="nick_name"></param>
		/// <param name="unsigned"></param>
		/// <param name="sex"></param>
		/// <param name="country"></param>
		/// <param name="provincia"></param>
		/// <param name="city"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetUserInfo(string nick_name, string unsigned, int sex, string country, string provincia, string city)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetUserInfo(pointerWxUser, nick_name, unsigned, sex, country, provincia, city, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取62数据
		/// </summary>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GenerateWxDat()
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGenerateWxDat(pointerWxUser, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 断线重连
		/// </summary>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_AutoLogin(string token)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXAutoLogin(pointerWxUser, token, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetPeopleNearby(float lat, float lng)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetPeopleNearby(pointerWxUser, lat, lng, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void Wx_SetProxyInfo(string proxy, int proxytype, string proxyname, string proxypwd)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetProxyInfo(pointerWxUser, proxy, proxytype, proxyname, proxypwd, (IntPtr)(void*)value);
						MarshalNativeToManaged((IntPtr)msgPtr);
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
		}

		/// <summary>
		/// 搜索用户信息
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SearchContact(string user)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSearchContact(pointerWxUser, user, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 添加好友
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="type"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_AddUser(string v1, string v2, int type, string context)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXAddUser(pointerWxUser, v1, v2, type, context.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_EAddUser(string v1, string v2, int type, string context)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.EAddUser(pointerWxUser, v1, v2, type, context);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 附近的人打招呼
		/// </summary>
		/// <param name="v1"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESayHello(string v1, string context)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESayHello(pointerWxUser, v1, context);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 公众号搜索
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_WebSearch(string search)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXWebSearch(pointerWxUser, search.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_EWebSearch(string search)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.EWebSearch(pointerWxUser, search);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取公众号菜单
		/// </summary>
		/// <param name="gzhid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string GetSubscriptionInfo(string gzhid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetSubscriptionInfo(pointerWxUser, gzhid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 执行公众号菜单
		/// </summary>
		/// <param name="wxid">公众号用户名gh* 开头的</param>
		/// <param name="uin">通过WXGetSubscriptionInfo获取</param>
		/// <param name="key">通过WXGetSubscriptionInfo获取</param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SubscriptionCommand(string wxid, uint uin, string key)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSubscriptionCommand(pointerWxUser, wxid, uin, key, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 阅读链接
		/// </summary>
		/// <param name="url"></param>
		/// <param name="uin"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_RequestUrl(string url, string uin, string key)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXRequestUrl(pointerWxUser, url, key, uin, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 获取访问链接token
		/// </summary>
		/// <param name="ghid"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetRequestToken(string ghid, string url)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetRequestToken(pointerWxUser, ghid, url, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 后期所有标签
		/// </summary>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_GetContactLabelList()
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXGetContactLabelList(pointerWxUser, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 设置用户标签
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="labelid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SetContactLabel(string wxid, string labelid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSetContactLabel(pointerWxUser, wxid, labelid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 创建标签
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_AddContactLabel(string context)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXAddContactLabel(pointerWxUser, context.Utf8ToAnsi(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 创建标签
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_EAddContactLabel(string context)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.EAddContactLabel(pointerWxUser, context);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 创建标签
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_DeleteContactLabel(string labelid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXDeleteContactLabel(pointerWxUser, labelid, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 同步收藏
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_FavSync(string key)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXFavSync(pointerWxUser, key, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 添加收藏
		/// </summary>
		/// <param name="fav_object"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_FavAddItem(string fav_object)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXFavAddItem(pointerWxUser, fav_object, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 查看收藏
		/// </summary>
		/// <param name="favid"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_FavGetItem(string favid)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXFavGetItem(pointerWxUser, favid.ConvertToInt32(), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_FavDeleteItem(int id)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXFavDeleteItem(pointerWxUser, id, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// 发送链接消息
		/// </summary>
		/// <param name="wxid"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_SendAppMsg(string wxid, string context)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXSendAppMsg(pointerWxUser, wxid, Encoding.Default.GetString(Encoding.UTF8.GetBytes(context)), (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_ESendAppMsg(string wxid, string appid, string sdkver, string title, string des, string url, string thumburl)
		{
			string text = "";
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						msgPtr = EUtils.ESendAppMsg(pointerWxUser, wxid, appid, sdkver, title, des, url, thumburl);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		/// <summary>
		/// token登录
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_LoginRequest(string token, string str62)
		{
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						byte[] array = Convert.FromBase64String(str62);
						XzyWxApis.WXLoadWxDat(pointerWxUser, array, array.Length, (IntPtr)(void*)value);
						MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			string text;
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* value2 = &msgPtr)
				{
					XzyWxApis.WXLoginRequest(pointerWxUser, token, (IntPtr)(void*)value2);
					text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
					Wx_ReleaseEX(ref msgPtr);
				}
			}
			return text;
		}

		/// <summary>
		/// 接受转账
		/// </summary>
		/// <param name="msg"></param>
		/// <returns></returns>
		[HandleProcessCorruptedStateExceptions]
		public unsafe string Wx_WXTransferOperation(string msg)
		{
			string text = "";
			fixed (int* ptr = &pointerWxUser)
			{
				fixed (int* value = &msgPtr)
				{
					try
					{
						XzyWxApis.WXTransferOperation(pointerWxUser, msg, (IntPtr)(void*)value);
						text = MarshalNativeToManaged((IntPtr)msgPtr).ConvertToString();
						Wx_ReleaseEX(ref msgPtr);
					}
					catch (Exception ex)
					{
						LogServer.Error(ex.Message);
					}
				}
			}
			return text;
		}

		public unsafe Dictionary<string, packitme> RedpackOK(string json, int Timestamp)
		{
			fixed (int* ptr2 = &pointerWxUser)
			{
				fixed (int* ptr = &msgPtr)
				{
					try
					{
						XzyWxApis.WXReceiveRedPacket(pointerWxUser, json, (IntPtr)(void*)ptr);
						if (ptr == null)
						{
							return null;
						}
						string text = MarshalNativeToManaged((IntPtr)msgPtr).ToString();
						if (text == null)
						{
							return null;
						}
						Wx_ReleaseEX(ref msgPtr);
						WXReceiveRedPacketJson wXReceiveRedPacketJson = JsonConvert.DeserializeObject<WXReceiveRedPacketJson>(text);
						text = wXReceiveRedPacketJson.Key;
						string sendUserName = JsonConvert.DeserializeObject<RedPacketJson>(wXReceiveRedPacketJson.External).SendUserName;
						if (!SET_redpack_Key(text, json))
						{
							return null;
						}
						CallBackRedPack(ok: false, -2, "收到新红包", text, sendUserName, Timestamp);
						XzyWxApis.WXOpenRedPacket(pointerWxUser, json, text, (IntPtr)(void*)ptr);
						if (ptr == null)
						{
							return null;
						}
						MarshalNativeToManaged((IntPtr)msgPtr).ToString();
						Wx_ReleaseEX(ref msgPtr);
						double getTimestamp = Utilities.GetTimestamp;
						XzyWxApis.WXQueryRedPacket(pointerWxUser, json, 0, (IntPtr)(void*)ptr);
						if (ptr == null)
						{
							return null;
						}
						string value = MarshalNativeToManaged((IntPtr)msgPtr).ToString();
						Wx_ReleaseEX(ref msgPtr);
						ReadPackJson readPackJson = JsonConvert.DeserializeObject<ReadPackJson>(value);
						Dictionary<string, packitme> dic = new Dictionary<string, packitme>();
						if (readPackJson.External != "")
						{
							ReadPackItem readPackItem = JsonConvert.DeserializeObject<ReadPackItem>(readPackJson.External);
							if (readPackItem.HeadTitle != null)
							{
								int recNum = readPackItem.RecNum;
								int totalNum = readPackItem.TotalNum;
								if (recNum == totalNum || readPackItem.HeadTitle.IndexOf("被抢光") != -1)
								{
									CallBackRedPack(ok: false, -1, "红包被抢光,读包中", text, sendUserName, Timestamp);
									int num2;
									do
									{
										int num = Convert.ToInt32(Convert.ToDouble(totalNum) / 11.0);
										if (totalNum % 11 > 0)
										{
											num++;
										}
										dic = new Dictionary<string, packitme>();
										num2 = 0;
										List<packitme> list = new List<packitme>();
										for (int i = 0; i < num + num; i++)
										{
											XzyWxApis.WXQueryRedPacket(pointerWxUser, json, i, (IntPtr)(void*)ptr);
											if (ptr == null)
											{
												return null;
											}
											string value2 = MarshalNativeToManaged((IntPtr)msgPtr).ToString();
											Wx_ReleaseEX(ref msgPtr);
											ReadPackItem readPackItem2 = JsonConvert.DeserializeObject<ReadPackItem>(JsonConvert.DeserializeObject<ReadPackJson>(value2).External);
											CallBackRedPack(ok: false, i + 1, $"读红包第{i + 1}页", text, sendUserName, Timestamp);
											object[] record = readPackItem2.Record;
											for (int j = 0; j < record.Length; j++)
											{
												packitme packitme = JsonConvert.DeserializeObject<packitme>(record[j].ToString());
												if (!dic.ContainsKey(packitme.UserName))
												{
													packitme.xh = num2;
													dic.Add(packitme.UserName, packitme);
													num2++;
													list.Add(packitme);
												}
											}
										}
									}
									while (num2 != totalNum);
									CallBackRedPack(ok: false, 0, "读包完毕", text, sendUserName, Timestamp, dic);
									return dic;
								}
								if (Utilities.GetTimestamp - getTimestamp > 60000.0)
								{
									CallBackRedPack(ok: false, -3, "红包超时", text, sendUserName, Timestamp, dic);
									return null;
								}
							}
						}
						return null;
					}
					catch
					{
						return null;
					}
				}
			}
		}

		public bool SET_redpack_Key(string key, string json)
		{
			lock (this.obj)
			{
				try
				{
					if (dic_redpack == null)
					{
						dic_redpack = new Dictionary<string, string>();
					}
					if (!dic_redpack.ContainsKey(key))
					{
						dic_redpack.Add(key, json);
						return true;
					}
				}
				catch
				{
					return false;
				}
			}
			return false;
		}

		/// <summary>
		/// 收到新红包时的回调处理
		/// </summary>
		/// <param name="ok"></param>
		/// <param name="page"></param>
		/// <param name="msg"></param>
		/// <param name="key"></param>
		/// <param name="fromuser"></param>
		/// <param name="Timestamp"></param>
		/// <param name="dic"></param>
		[HandleProcessCorruptedStateExceptions]
		public void CallBackRedPack(bool ok, int page, string msg, string key, string fromuser, int Timestamp, Dictionary<string, packitme> dic = null)
		{
			PackMsg packMsg = new PackMsg();
			packMsg.msg = msg;
			packMsg.key = key;
			packMsg.fromuser = fromuser;
			packMsg.Timestamp = Timestamp;
			if (ok)
			{
				packMsg.ok = true;
				packMsg.packitme = dic;
			}
			else
			{
				packMsg.ok = false;
				packMsg.page = page;
			}
		}

		/// <summary>
		/// 释放内存
		/// </summary>
		/// <param name="hande"></param>
		[HandleProcessCorruptedStateExceptions]
		public void Wx_ReleaseEX(ref int hande)
		{
			try
			{
				XzyWxApis.WXRelease((IntPtr)hande);
				hande = 0;
			}
			catch (Exception ex)
			{
				LogServer.Error(ex.Message);
			}
		}
	}
}
