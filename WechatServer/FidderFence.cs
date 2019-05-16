using Fiddler;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using NLog;

namespace WechatServer
{
    class FidderFence
    {
        private List<Session> oAllSessions = new List<Session>();
        private string LocalServer = ConfigurationManager.AppSettings["LocalServer"].ToString();
        private string FidderHost = ConfigurationManager.AppSettings["ProxyHost"].ToString();
        private ushort FidderPort = ushort.Parse(ConfigurationManager.AppSettings["ProxyPort"].ToString());
        public static NLog.Logger logger = LogManager.GetLogger("session");
        public void StartFidderFence()
        {
            ProxySettings.SetProxy(FidderHost + ":" + FidderPort);
            FiddlerCoreStartupSettingsBuilder settingsBuilder = new FiddlerCoreStartupSettingsBuilder();
            FiddlerCoreStartupSettings coreStartupSettings= settingsBuilder.ListenOnPort(FidderPort).AllowRemoteClients().Build();
            FiddlerApplication.Startup(coreStartupSettings);
            //在请求前拦截
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
        }

        private void FiddlerApplication_BeforeRequest(Session oSession)
        {
            logger.Info(oSession.fullUrl);
            if (oSession.fullUrl.Contains("http://www.wechattools.com/api/AjaxManager.ashx"))
            {
                oSession.bBufferResponse = true;
                Monitor.Enter(oAllSessions);
                oAllSessions.Add(oSession);
                Monitor.Exit(oAllSessions);
                oSession.fullUrl = oSession.fullUrl.Replace("http://www.wechattools.com/api/AjaxManager.ashx", LocalServer);
            }
            if (oSession.fullUrl.Contains("http://www.wechattools.com/Api/Auth.ashx"))
            {
                oSession.bBufferResponse = true;
                Monitor.Enter(oAllSessions);
                oAllSessions.Add(oSession);
                Monitor.Exit(oAllSessions);
                oSession.fullUrl = oSession.fullUrl.Replace("http://www.wechattools.com/Api/Auth.ashx", LocalServer);
            }
            if (oSession.fullUrl.Contains("http://www.wechattools.com/api/IpPort.ashx"))
            {
                oSession.bBufferResponse = true;
                Monitor.Enter(oAllSessions);
                oAllSessions.Add(oSession);
                Monitor.Exit(oAllSessions);
                oSession.fullUrl = oSession.fullUrl.Replace("http://www.wechattools.com/api/IpPort.ashx", LocalServer);
            }
        }
    }
}
