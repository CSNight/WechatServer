using Newtonsoft.Json;
using System;
using System.Threading;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;
using WebApi.WeChat;

namespace WebApi.Controllers
{
	/// <summary>
	/// 用户模块
	/// </summary>
	[RoutePrefix("api/user")]
	[Error]
	public class UserController : ApiController
	{
		/// <summary>
		/// 获取登录二维码，（如果需要消息回调用同样的uuid 创建websocket isreset传false）
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("loginscan")]
		public IHttpActionResult UserLoginScan(ScanLoginModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				MySocket socket = new MySocket();
				XzyWeChatThread xzyWeChatThread = new XzyWeChatThread(socket, model);
				DicSocket value = new DicSocket
				{
					socket = socket,
					weChatThread = xzyWeChatThread,
					dateTime = DateTime.Now
				};
				XzyWebSocket._dicSockets.Remove(model.uuid);
				XzyWebSocket._dicSockets.Add(model.uuid, value);
				while (xzyWeChatThread.ScanQrCode == "")
				{
					Thread.Sleep(200);
				}
				apiServerMsg.Success = true;
				apiServerMsg.Context = xzyWeChatThread.ScanQrCode;
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 账号密码+62登录，登录后使用相同的UUID链接websocket也可以再次接收消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("login62")]
		public IHttpActionResult UserLogin62(UserLoginModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				MySocket socket = new MySocket();
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid) && !model.isreset)
				{
					XzyWebSocket._dicSockets[model.uuid].socket = socket;
					XzyWebSocket._dicSockets[model.uuid].weChatThread._socket = socket;
				}
				else
				{
					XzyWeChatThread weChatThread = new XzyWeChatThread(socket, model);
					DicSocket value = new DicSocket
					{
						socket = socket,
						weChatThread = weChatThread,
						dateTime = DateTime.Now
					};
					XzyWebSocket._dicSockets.Remove(model.uuid);
					XzyWebSocket._dicSockets.Add(model.uuid, value);
				}
				apiServerMsg.Success = true;
				apiServerMsg.Context = "调用成功，如未登陆成功可能账号受限，请使用websocket登陆查看详细原因";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 设置头像
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("setheadimg")]
		public IHttpActionResult SetHeadImg(UserSetHeadImg model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SetHeadImage(model.base64);
					apiServerMsg.Success = true;
					apiServerMsg.Context = context;
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "不存在该websocket连接";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 设置wxid
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("setwxid")]
		public IHttpActionResult UserSetWxid(UserSetWxidModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SetWeChatID(model.wxid);
					apiServerMsg.Success = true;
					apiServerMsg.Context = context;
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "不存在该websocket连接";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 设置个人信息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("setuserinfo")]
		public IHttpActionResult UserSetUserInfo(UserSetUserInfoModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SetUserInfo(model.nickname, model.sign, model.sex, model.country, model.provincia, model.city);
					apiServerMsg.Success = true;
					apiServerMsg.Context = context;
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "不存在该websocket连接";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 获取个人信息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get")]
		public IHttpActionResult GetInfo(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					apiServerMsg.Success = true;
					apiServerMsg.Context = JsonConvert.SerializeObject(XzyWebSocket._dicSockets[model.uuid].weChatThread.userData);
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "不存在该websocket连接";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}

		/// <summary>
		/// 微信注销
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("logout")]
		public IHttpActionResult LogOut(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_Logout();
					XzyWebSocket._dicSockets[model.uuid].weChatThread = null;
					XzyWebSocket._dicSockets.Remove(model.uuid);
					apiServerMsg.Success = true;
					apiServerMsg.Context = context;
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "不存在该websocket连接";
				return Ok(apiServerMsg);
			}
			catch (Exception ex)
			{
				apiServerMsg.Success = false;
				apiServerMsg.ErrContext = ex.Message;
				return Ok(apiServerMsg);
			}
		}
	}
}
