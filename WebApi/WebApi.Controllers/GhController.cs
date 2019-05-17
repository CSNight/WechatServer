using System;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;

namespace WebApi.Controllers
{
	/// <summary>
	/// 公众号模块
	/// </summary>
	[RoutePrefix("api/gh")]
	[Error]
	public class GhController : ApiController
	{
		/// <summary>
		/// 获取公众号内容
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get")]
		public IHttpActionResult GhGet(GhModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string subscriptionInfo = XzyWebSocket._dicSockets[model.uuid].weChatThread.GetSubscriptionInfo(model.ghid);
					apiServerMsg.Success = true;
					apiServerMsg.Context = subscriptionInfo;
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
		/// 关注公众号
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("follow")]
		public IHttpActionResult GhFlower(GhModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_AddUser(model.ghid, "", 0, "");
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
		/// 搜索公众号
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("search")]
		public IHttpActionResult GhSearch(GhSearchModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_WebSearch(model.name);
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
		/// 执行公众号菜单
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("subscriptioncommand")]
		public IHttpActionResult SubscriptionCommand(GhSubscriptionCommandModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SubscriptionCommand(model.ghid, uint.Parse(model.uin), model.key);
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
		/// 阅读链接
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("requesturl")]
		public IHttpActionResult RequestUrl(GhRequestUrl model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_RequestUrl(model.url, model.uin, model.key);
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
		/// 获取阅读链接token  （uin,key）
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getrquesttoken")]
		public IHttpActionResult GetRequestToken(GhRequestTokenModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetRequestToken(model.ghid, model.url);
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
