using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;

namespace WebApi.Controllers
{
	/// <summary>
	/// 好友模块
	/// </summary>
	[RoutePrefix("api/contact")]
	[Error]
	public class ContactController : ApiController
	{
		/// <summary>
		/// 获取好友详情
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get")]
		public IHttpActionResult ContactGet(ContactModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetContact(model.wxid);
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
		/// 设置好友备注
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("setremark")]
		public IHttpActionResult ContactSetRemark(ContactRemarkModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_ESetUserRemark(model.wxid, model.remark);
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
		/// 删除好友
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("delete")]
		public IHttpActionResult ContactDelete(ContactModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_DeleteUser(model.wxid);
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
		/// 通过好友请求
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("accept")]
		public IHttpActionResult ContactAccept(ContactAcceptModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_AcceptUser(model.stranger, model.ticket);
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
		/// 获取好友列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getcontact")]
		public IHttpActionResult GetContact(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					apiServerMsg.Success = true;
					apiServerMsg.Context = JsonConvert.SerializeObject(XzyWebSocket._dicSockets[model.uuid].weChatThread.wxContacts);
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
		/// 获取群组列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getgroup")]
		public IHttpActionResult GetGroup(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					apiServerMsg.Success = true;
					apiServerMsg.Context = JsonConvert.SerializeObject(XzyWebSocket._dicSockets[model.uuid].weChatThread.wxGroups);
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
		/// 获取公众号列表
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getgzh")]
		public IHttpActionResult GetGzh(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					apiServerMsg.Success = true;
					apiServerMsg.Context = JsonConvert.SerializeObject(XzyWebSocket._dicSockets[model.uuid].weChatThread.wxGzhs);
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
		/// 重新获取好友 公众号 群组列表，会通过websocket 回调，同步完成以后可以通过查询接口进行查询
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		[Route("synccontacts")]
		public IHttpActionResult SyncContacts(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					XzyWebSocket._dicSockets[model.uuid].weChatThread.wxGzhs = new List<Contact>();
					XzyWebSocket._dicSockets[model.uuid].weChatThread.wxContacts = new List<Contact>();
					XzyWebSocket._dicSockets[model.uuid].weChatThread.wxGroups = new List<Contact>();
					XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetContacts();
					apiServerMsg.Success = true;
					apiServerMsg.Context = "正在同步，同步完成以后可以通过查询接口进行查询";
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
