using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DingTalkApi.Common
{
    public class AppSettings
    {
        public static string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) { return key; }
            return ConfigurationManager.AppSettings[key].ToString();
        }
    }
}