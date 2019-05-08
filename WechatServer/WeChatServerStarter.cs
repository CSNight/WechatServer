using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.Threading;
using WebApi;

namespace WechatServer
{
    class WeChatServerStarter
    {
        private string WebApiAddr = ConfigurationManager.AppSettings["WebApiAddr"].ToString();
        public void StartUpApi()
        {
            try
            {
                StartOptions opt = new StartOptions();
                opt.Urls.Add(WebApiAddr);
                using (WebApp.Start<Startup>(opt))
                {
                    Console.WriteLine("开启服务");
                    Thread.Sleep(-1);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
