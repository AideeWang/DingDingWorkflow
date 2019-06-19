using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]  
namespace DingTalkApi.Common
{
    public class LogHelper
    {
        public static readonly ILog _log = LogManager.GetLogger("log4net");

        public static void Log(string message)
        {
            _log.Info(message);
        }

        public static void Debug(string message)
        {
            _log.Debug(message);
        }

        public static void Fatal(string message)
        {
            _log.Fatal(message);
        }

        public static void Warn(string message)
        {
            _log.Warn(message);
        }
    }
}