using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;
using WebApi.Utils;

namespace WebApi.Controllers
{
	/// <summary>
	/// 消息模块
	/// </summary>
	[RoutePrefix("api/msg")]
	[Error]
	public class MsgController : ApiController
	{
		/// <summary>
		/// 发送文字消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendtext")]
		public IHttpActionResult SendText(SendTextModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SendMsg(model.wxid, model.text, model.atlist);
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
		/// 发送语音消息 mp3格式
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendvoice")]
		public IHttpActionResult SendVoice(SendMediaModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string text = AppDomain.CurrentDomain.BaseDirectory + "files";
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					byte[] array = Convert.FromBase64String(model.base64.Replace("data:audio/mp3;base64,", ""));
					string text2 = text + "\\" + Guid.NewGuid() + ".mp3";
					string text3 = text2.Replace(".mp3", ".silk");
					FileStream fileStream = File.Create(text2, array.Length);
					fileStream.Write(array, 0, array.Length);
					fileStream.Flush();
					fileStream.Close();
					if (ffmpegUtils.GetInstance().ConvertMp3ToAmr(text2, text3))
					{
						while (MyUtils.IsFileInUse(text3))
						{
							Thread.Sleep(100);
						}
					}
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SendVoice(model.wxid, text3, 1);
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
		///  发送语音消息 silk格式
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendvoicesilk")]
		public IHttpActionResult SendVoiceSilk(SendMediaModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string text = AppDomain.CurrentDomain.BaseDirectory + "files";
					if (!Directory.Exists(text))
					{
						Directory.CreateDirectory(text);
					}
					byte[] array = Convert.FromBase64String(model.base64.Replace("data:application/octet-stream;base64,", ""));
					string text2 = text + "\\" + Guid.NewGuid() + ".silk";
					FileStream fileStream = File.Create(text2, array.Length);
					fileStream.Write(array, 0, array.Length);
					fileStream.Flush();
					fileStream.Close();
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SendVoice(model.wxid, text2, 1);
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
		/// 发送图片消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendimg")]
		public IHttpActionResult SendImg(SendMediaModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					Image imageFromBase = ConvertUtils.GetImageFromBase64(model.base64);
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SendImg(model.wxid, imageFromBase);
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
		/// 发送链接消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendapp")]
		public IHttpActionResult SendApp(SendAppModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = App.AppMsgXml.Replace("$appid$", model.appid).Replace("$sdkver$", model.sdkver).Replace("$title$", model.title)
						.Replace("$des$", model.des)
						.Replace("$url$", model.url)
						.Replace("$thumburl$", model.thumburl);
					string context2 = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_SendAppMsg(model.wxid, context);
					apiServerMsg.Success = true;
					apiServerMsg.Context = context2;
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
		/// 发送名片消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendshardcard")]
		public IHttpActionResult SendShareCard(ShardCardModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_EShareCard(model.user, model.wxid, model.title);
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
		/// 群发消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("sendmass")]
		public IHttpActionResult SendMass(SendMassModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_MassMessage(JsonConvert.SerializeObject(model.wxids), model.text);
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
		/// 获取图片消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getimg")]
		public IHttpActionResult GetMsgImg(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetMsgImage(JsonConvert.SerializeObject(model.msg));
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
		/// 获取语音消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getvoice")]
		public IHttpActionResult GetMsgVoice(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetMsgVoice(JsonConvert.SerializeObject(model.msg));
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
		/// 获取视频消息
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getvideo")]
		public IHttpActionResult GetMsgVideo(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_GetMsgVideo(JsonConvert.SerializeObject(model.msg));
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
		/// 抢红包 ，sub_type == 49    CDATA[1002]
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("getreadpack")]
		public IHttpActionResult GetReadPack(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					if (model.msg.content.IndexOf("CDATA[1002]") != -1)
					{
						Dictionary<string, packitme> context = XzyWebSocket._dicSockets[model.uuid].weChatThread.RedpackOK(JsonConvert.SerializeObject(model.msg), model.msg.Timestamp);
						apiServerMsg.Success = true;
						apiServerMsg.Context = context;
						return Ok(apiServerMsg);
					}
					apiServerMsg.Success = false;
					apiServerMsg.Context = "消息错误";
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
		/// 接受转账 ，sub_type == 49   CDATA[微信转账]
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("gettransfer")]
		public IHttpActionResult GetTransfer(MessageModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					if (model.msg.content.IndexOf("CDATA[微信转账]") != -1)
					{
						string context = XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_WXTransferOperation(JsonConvert.SerializeObject(model.msg));
						apiServerMsg.Success = true;
						apiServerMsg.Context = context;
						return Ok(apiServerMsg);
					}
					apiServerMsg.Success = false;
					apiServerMsg.Context = "消息错误";
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
		/// 接受群邀请，sub_type == 49  ,传msg.content 包含 “加入群聊”
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("intogroup")]
		public IHttpActionResult IntoGroup(IntoGroupModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (XzyWebSocket._dicSockets.ContainsKey(model.uuid))
				{
					if (model.content.IndexOf("加入群聊") != -1)
					{
						string midStr = Utilities.GetMidStr(model.content, "<url><![CDATA[", "]]>");
						XzyWebSocket._dicSockets[model.uuid].weChatThread.Wx_IntoGroup(midStr);
						apiServerMsg.Success = true;
						apiServerMsg.Context = "调用成功";
						return Ok(apiServerMsg);
					}
					apiServerMsg.Success = false;
					apiServerMsg.Context = "参数错误，请检查消息是否为进群消息";
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
