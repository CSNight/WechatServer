using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using WebApi.Model;
using WebApi.MyWebSocket;
using WebApi.Utils;

namespace WebApi.Controllers
{
	/// <summary>
	/// 系统模块
	/// </summary>
	[RoutePrefix("api/system")]
	[Error]
	public class SystemController : ApiController
	{
		/// <summary>
		/// 设置设备登录key
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("setDeviceKey")]
		public IHttpActionResult SysDeviceKey(DeviceKeyModel model)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (ConfigurationManager.AppSettings["AdminPassword"].ConvertToString() == model.password)
				{
					App.DeviceKey = model.devicekey;
					apiServerMsg.Success = true;
					apiServerMsg.Context = "设置成功";
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "管理员密码不正确，请检查webconfig配置";
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
		/// 查询所有在线微信
		/// </summary>
		/// <param name="password">管理员密码,配置在webconfig中</param>
		/// <returns></returns>
		[HttpGet]
		[Route("getallonline")]
		public IHttpActionResult SysGetAllOnline(string password)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (ConfigurationManager.AppSettings["AdminPassword"].ConvertToString() == password)
				{
					List<OnlineWxModel> list = new List<OnlineWxModel>();
					foreach (KeyValuePair<string, DicSocket> dicSocket in XzyWebSocket._dicSockets)
					{
						OnlineWxModel onlineWxModel = new OnlineWxModel();
						onlineWxModel.uuid = dicSocket.Key;
						if (!dicSocket.Value.weChatThread.IsNull())
						{
							onlineWxModel.wxid = dicSocket.Value.weChatThread.userData.UserName.ConvertToString();
							onlineWxModel.nickname = dicSocket.Value.weChatThread.userData.NickName.ConvertToString();
							onlineWxModel.headimg = dicSocket.Value.weChatThread.userData.HeadImg.ConvertToString();
							onlineWxModel.contactcount = dicSocket.Value.weChatThread.wxContacts.Count.ConvertToString().ConvertToInt32();
							onlineWxModel.groupcount = dicSocket.Value.weChatThread.wxGroups.Count.ConvertToString().ConvertToInt32();
							onlineWxModel.gzhcount = dicSocket.Value.weChatThread.wxGzhs.Count.ConvertToString().ConvertToInt32();
						}
						list.Add(onlineWxModel);
					}
					apiServerMsg.Success = true;
					apiServerMsg.Context = JsonConvert.SerializeObject(list);
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "管理员密码不正确，请检查webconfig配置";
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
		/// 注销所有在线微信
		/// </summary>
		/// <param name="password"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("logoutall")]
		public IHttpActionResult SysLogOutAll(string password)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (ConfigurationManager.AppSettings["AdminPassword"].ConvertToString() == password)
				{
					foreach (KeyValuePair<string, DicSocket> dicSocket in XzyWebSocket._dicSockets)
					{
						dicSocket.Value.weChatThread.Wx_Logout();
					}
					apiServerMsg.Success = true;
					apiServerMsg.Context = "全部下线完成";
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "管理员密码不正确，请检查webconfig配置";
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
		/// 注销单个进程
		/// </summary>
		/// <param name="uuid"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("logout")]
		public IHttpActionResult SysLogOutAll(string uuid, string password)
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			try
			{
				if (ConfigurationManager.AppSettings["AdminPassword"].ConvertToString() == password)
				{
					XzyWebSocket._dicSockets[uuid].weChatThread.Wx_Logout();
					apiServerMsg.Success = true;
					apiServerMsg.Context = "注销成功";
					return Ok(apiServerMsg);
				}
				apiServerMsg.Success = false;
				apiServerMsg.Context = "管理员密码不正确，请检查webconfig配置";
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
