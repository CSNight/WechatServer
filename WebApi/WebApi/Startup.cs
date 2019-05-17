using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using WebApi.MyWebSocket;
using WebApi.Utils;

namespace WebApi
{
	public class Startup
	{
		public void Configuration(IAppBuilder appBuilder)
		{
			HttpConfiguration httpConfiguration = new HttpConfiguration();
			httpConfiguration.EnableCors(new EnableCorsAttribute("*", "*", "*"));
			httpConfiguration.MapHttpAttributeRoutes();
			httpConfiguration.Routes.MapHttpRoute("WeChatApi", "api/{controller}/{id}", new
			{
				id = RouteParameter.Optional
			});
			httpConfiguration.EnableSwagger(delegate(SwaggerDocsConfig c)
			{
				c.SingleApiVersion("v1", "WebAPI");
				c.IncludeXmlComments(GetXmlCommentsPath());
				c.ResolveConflictingActions((IEnumerable<ApiDescription> x) => x.First());
			}).EnableSwaggerUi();
			appBuilder.UseWebApi(httpConfiguration);
			//Auth.Init();
			XzyWebSocket.Init();
		}

		private static string GetXmlCommentsPath()
		{
			return AppDomain.CurrentDomain.BaseDirectory + "\\WebApi.XML";
		}
	}
}
