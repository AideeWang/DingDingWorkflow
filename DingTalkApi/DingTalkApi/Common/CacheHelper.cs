
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.SessionState;

namespace DingTalkApi.Common
{
    /// <summary>
    /// Session 帮助类
    /// </summary>
    public class CacheHelper
    {
        private ObjCacheProvider<object> obj = new ObjCacheProvider<object>();
        private DateTime dt { get; set; }
        public CacheHelper()
        {
            dt = DateTime.Now.AddHours(1);
        }
        public void Add(string key,object value)
        {

            obj.Create(key, value, dt);
        }

        public object Get(string key)
        {
            return obj.GetCache(key);
        }

    }
}
