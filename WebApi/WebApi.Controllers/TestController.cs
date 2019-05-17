using System.Web.Http;
using WebApi.Model;

namespace WebApi.Controllers
{
	/// <summary>
	/// 系统模块
	/// </summary>
	[RoutePrefix("api/test")]
	[Error]
	public class TestController : ApiController
	{
		[HttpGet]
		[Route("testonline")]
		public IHttpActionResult TestOnline()
		{
			ApiServerMsg apiServerMsg = new ApiServerMsg();
			apiServerMsg.Success = true;
			apiServerMsg.Context = "服务正常";
			return Ok(apiServerMsg);
		}
	}
}
