using DingTalkApi.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Top.Api.Util;

namespace DingTalkApi.Service
{
    public class CallbackService
    {
        public CallbackService()
        {
            token = AppSettings.Get(Constants.TOKEN);
            aes_key = AppSettings.Get(Constants.AES_KEY);
            url = AppSettings.Get(Constants.REGISTERURL);
            corpid = AppSettings.Get(Constants.CORPID);
        }

        private string token { get; set; }
        private string aes_key { get; set; }
        private string url { get; set; }
        private string corpid { get; set; }
        private DingDingHelper _dingDingHelper = new DingDingHelper();
        private DingDingService _dingdingSeriver = new DingDingService();

        public CacheHelper _cacheHelper = new CacheHelper();
        /// <summary>
        /// 钉钉加密解密
        /// </summary>
        /// <param name="Msignature"></param>
        /// <param name="Mtimestamp"></param>
        /// <param name="Mnonce"></param>
        /// <param name="MencryptStr"></param>
        public string DingTalkCrypto(string Msignature, string Mtimestamp, string Mnonce, string MencryptStr)
        {
            DingTalkCrypt dingTalk = new DingTalkCrypt(token, aes_key, corpid);
            string PlainText = string.Empty;
            dingTalk.DecryptMsg(Msignature, Mtimestamp, Mnonce, MencryptStr, ref PlainText);
            Hashtable tb = (Hashtable)JsonConvert.DeserializeObject(PlainText, typeof(Hashtable));
            string EventType = tb["EventType"].ToString();
            //switch (EventType)
            //{
            //    default:
            //        break;
            //}

            CallbackEvent(tb);
            string timestamp = TopUtils.GetCurrentTimeMillis().ToString();
            string encrypt = "";
            string signature = "";
            string res = "success";//传递的消息体明文
            dingTalk = new DingTalkCrypt(token, aes_key, corpid);
            dingTalk.EncryptMsg(res, timestamp, Mnonce, ref encrypt, ref signature);
            Hashtable jsonMap = new Hashtable
                {
                    {"msg_signature", signature},
                    {"encrypt", encrypt},
                    {"timeStamp", timestamp},
                    {"nonce", Mnonce}
                };
            string result = JsonConvert.SerializeObject(jsonMap);
            LogHelper.Log("ReceiveCallbackSerivce result: " + result);
            return result;
        }



        /// <summary>
        /// 注册回调事件
        /// </summary>
        /// <returns></returns>
        public string ReigsterCallbackApi()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Constants.CALL_BACK_TAG, new string[]{Constants.BPMS_INSTANCE_CHANGE,Constants.BPMS_TASK_CHANGE});
            dic.Add(Constants.TOKEN,token);
            dic.Add(Constants.AES_KEY, aes_key);
            dic.Add(Constants.URL, url);
            LogHelper.Log("accesstoken: " + _cacheHelper.Get(Constants.ACCESS_TOKEN));
            string response = _dingDingHelper.CommonFunctions(dic, _dingDingHelper.CommonFunctions(dic, Constants.REGISTER_CALL_BACK + "?" + _dingdingSeriver.ConvertDictionaryToString(_dingdingSeriver.GetAccessTokenToDicObject())));
            LogHelper.Log(response);
            return response;
           
        }

        /// <summary>
        /// 回调事件
        /// </summary>
        /// <param name="EventType"></param>
        /// <returns></returns>
        public string CallbackEvent(Hashtable tb)
        {
            string EventType = tb["EventType"].ToString();
            switch (EventType)
            {
                case Constants.CHECK_URL:
                    //LogHelper.Log();
                    break;
                case Constants.BPMS_INSTANCE_CHANGE:
                    string WorkflowUrl = tb["url"].ToString();
                    string WorkflowTitle = tb["title"].ToString();
                    string ProcessInstanceId = tb["processInstanceId"].ToString();
                    string StaffId = tb["staffId"].ToString();
                    string CreateTime=tb["createTime"].ToString();

                    //Dictionary<string, object> dic = new Dictionary<string, object>();
                    //dic.Add("msgtype", "link");
                    //Dictionary<string, object> dicLink = new Dictionary<string, object>();
                    //dicLink.Add("messageUrl",WorkflowUrl);
                    //dicLink.Add("title",WorkflowTitle);
                    //dicLink.Add("picUrl","http://b-ssl.duitang.com/uploads/item/201707/22/20170722134146_aQxXy.thumb.700_0.jpeg");
                    //dicLink.Add("text","流程发起的提醒");

                    //dic.Add("link", dicLink);
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("title", "请假类型");
                    dic.Add("content", "ssss");
                    List<Dictionary<string,string>> dicList=new List<Dictionary<string,string>>();
                    dicList.Add(dic);

                    string result = _dingdingSeriver.ToDoItems("172849403829072399", CreateTime, WorkflowTitle, WorkflowUrl, dicList);
                    LogHelper.Log("流程发起的提醒 result ：" + result);
                    break;
            }
            return "";
        }

        public string GetCallbackFailure()
        {
            Dictionary<string, string> dic = _dingdingSeriver.GetAccessTokenToDicObject();
            string response = _dingDingHelper.CommonFunctions(dic, Constants.GET_CALL_BACK_FAILED_RESULT);
            return response;
        }

        public void EndApprovalCallback()
        {
            
        }

    }
}