using Newtonsoft.Json;
using System;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;
using WebApi.Util;

namespace WebApi.Controllers
{
	/// <summary>
	/// 二次登陆模块
	/// </summary>
	[RoutePrefix("api/autologin")]
	[Error]
	public class AutoLoginController : ApiController
	{
		/// <summary>
		/// 获取62数据（未做base64和 hex解码）
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get62")]
		public IHttpActionResult Get62(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					WxDat wxDat = JsonConvert.DeserializeObject<WxDat>(XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GenerateWxDat());
					apiServerMsg.Success = true;
					apiServerMsg.Context = wxDat.data;
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
		/// 获取62数据 ，base62和 hex解码， 数据内容微62xxxxxx
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("get62hex")]
		public IHttpActionResult Get62Hex(BaseModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					WxDat wxDat = JsonConvert.DeserializeObject<WxDat>(XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GenerateWxDat());
					apiServerMsg.Success = true;
					apiServerMsg.Context = Convert62.eStrToHex(wxDat.data);
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
