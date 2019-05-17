using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http.Filters;
using WebApi.Model;

namespace WebApi
{
	public class ErrorAttribute : ExceptionFilterAttribute
	{
		public override void OnException(HttpActionExecutedContext actionExecutedContext)
		{
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
			httpResponseMessage.Content = new StringContent(JsonConvert.SerializeObject(new ApiServerMsg
			{
				Success = true,
				ErrContext = "服务崩啦，异常信息：" + actionExecutedContext.Exception.Message + " 详情查看日志"
			}), Encoding.UTF8, "application/json");
			actionExecutedContext.Response = httpResponseMessage;
			Exception exception = actionExecutedContext.Exception;
			LogServer.Error(exception.Message + "--" + exception.Source + "--" + exception.StackTrace);
			base.OnException(actionExecutedContext);
		}
	}
}
