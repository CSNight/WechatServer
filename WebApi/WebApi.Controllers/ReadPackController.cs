using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;

namespace WebApi.Controllers
{
	/// <summary>
	/// 红包模块
	/// </summary>
	[RoutePrefix("api/readpack")]
	[Error]
	public class ReadPackController : ApiController
	{
		/// <summary>
		/// 抢红包
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getpack")]
		public IHttpActionResult GetPack(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					Dictionary<string, packitme> context = XzyWebSocket._dicSockets[model.uuid].weChatThread.RedpackOK(JsonConvert.SerializeObject(model.msg), model.msg.Timestamp);
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
