using Fiddler;
using System;
using System.Runtime.InteropServices;

namespace WechatServer
{
    class Program
    {
        public delegate bool ControlCtrlDelegate(int CtrlType);
        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ControlCtrlDelegate HandlerRoutine, bool Add);

        static ControlCtrlDelegate newDelegate = new ControlCtrlDelegate(HandlerRoutine);

        public static bool HandlerRoutine(int CtrlType)
        {
            switch (CtrlType)
            {
                case 0:
                    Console.WriteLine("0工具被强制关闭"); //Ctrl+C关闭  
                    //相关代码执行
                    ProxySettings.UnsetProxy();
                    FiddlerApplication.Shutdown();
                    break;
                case 2:
                    Console.WriteLine("2工具被强制关闭");//按控制台关闭按钮关闭 
                    //相关代码执行 
                    ProxySettings.UnsetProxy();
                    FiddlerApplication.Shutdown();
                    break;
            }
            return true;
        }
        static void Main(string[] args)
        {
            try
            {
                bool bRet = SetConsoleCtrlHandler(newDelegate, true);
                FidderFence fidderFence = new FidderFence();
                WeChatServerStarter serverStarter = new WeChatServerStarter();
                fidderFence.StartFidderFence();
                serverStarter.StartUpApi();
            }
            catch(Exception ex)
            {
                ProxySettings.UnsetProxy();
                FiddlerApplication.Shutdown();
            }
        }
    }
}
