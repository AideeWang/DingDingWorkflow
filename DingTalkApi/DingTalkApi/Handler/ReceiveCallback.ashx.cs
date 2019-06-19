
using DingTalkApi.Common;
using DingTalkApi.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DingTalkApi.Handler
{
    /// <summary>
    /// ReceiveCallback 的摘要说明
    /// </summary>
    public class ReceiveCallback : IHttpHandler
    {
        CallbackService callbackSerivce = new CallbackService();
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                #region 获取回调URL里面的参数
                //url中的签名
                string msgSignature = context.Request["signature"];
                //url中的时间戳
                string timeStamp = context.Request["timestamp"];
                //url中的随机字符串
                string nonce = context.Request["nonce"];
                //post数据包数据中的加密数据
                string encryptStr = GetPostParam(context);
                #endregion
                //string.IsNullOrEmpty(msgSignature)? AppSettings
                LogHelper.Debug(string.Format("signature: {0},timestamp:{1},nonce:{2},encryptStr:{3}", msgSignature, timeStamp,nonce,encryptStr));

                string result = callbackSerivce.DingTalkCrypto(msgSignature, timeStamp, nonce, encryptStr);
                context.Response.Write(result);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("ReceiveCallback：" + ex.ToJson());
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        private string GetPostParam(HttpContext context)
        {
            if ("POST" == context.Request.RequestType)
            {
                Stream sm = context.Request.InputStream;//获取post正文
                int len = (int)sm.Length;//post数据长度
                byte[] inputByts = new byte[len];//字节数据,用于存储post数据
                sm.Read(inputByts, 0, len);//将post数据写入byte数组中
                sm.Close();//关闭IO流

                //**********下面是把字节数组类型转换成字符串**********

                string data = Encoding.UTF8.GetString(inputByts);//转为String
                data = data.Replace("{\"encrypt\":\"", "").Replace("\"}", "");
                return data;
            }
            return "get方法";
        }
        
    }
}