using System;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;

namespace WebApi.Controllers
{
	/// <summary>
	/// 收藏模块
	/// </summary>
	[RoutePrefix("api/fav")]
	[Error]
	public class FavController : ApiController
	{
		/// <summary>
		/// 同步收藏
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sync")]
		public IHttpActionResult FavSync(FavModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_FavSync(model.favkey);
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
		/// 添加收藏
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("add")]
		public IHttpActionResult FavAdd(FavAddModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_FavAddItem(model.favObject);
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
		/// 查看收藏
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("select")]
		public IHttpActionResult FavSelect(FavSelectModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_FavGetItem(model.favid);
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
