using Microsoft.Owin.Hosting;
using System;
using System.Configuration;
using System.Threading;
using WebApi;
using WeChat.Core;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace WechatServer
{
    class WeChatServerStarter
    {

        private string WebApiAddr = ConfigurationManager.AppSettings["WebApiAddr"].ToString();
        [HandleProcessCorruptedStateExceptions, STAThread]
        public void StartUpApi()
        {
            try
            {
                StartOptions opt = new StartOptions();
                opt.Urls.Add(WebApiAddr);
                string host = ConfigurationManager.AppSettings["WebApiHost"];
                using (WebApp.Start<Startup>(opt))
                {
                    Console.WriteLine("开启服务...");
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
