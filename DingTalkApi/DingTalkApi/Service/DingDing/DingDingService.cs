using DingTalkApi.Common;
using DingTalkApi.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Top.Api.Util;

namespace DingTalkApi.Service
{
    public class DingDingService
    {
        private DingDingHelper _dingDingHelper = new DingDingHelper();
        public CacheHelper _cacheHelper = new CacheHelper();
        public DingDingService()
        {
            corpid = AppSettings.Get(Constants.CORPID);
            corpsecret = AppSettings.Get(Constants.CORPSECRET);
            agentid = AppSettings.Get(Constants.AGENTID);
            appkey = AppSettings.Get(Constants.APPKEY);
            appsecret = AppSettings.Get(Constants.APPSERCRET);
            url = AppSettings.Get(Constants.URL);
        }
        public string corpid { get; set; }
        public string corpsecret { get; set; }
        public string agentid { get; set; }

        public string appkey { get; set; }

        public string appsecret { get; set; }

        public string access_token { get; set; }

        public string url { get; set; }

        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public string GetAccessToken()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(Constants.APPKEY, appkey);//appkey
            dic.Add(Constants.APPSERCRET, appsecret);//appsecret
            string accesstoken = _dingDingHelper.GetAccessToken(dic);
            _cacheHelper.Add(Constants.ACCESS_TOKEN, accesstoken);
            return accesstoken;
        }

        public string GetAccessTokenByCorp()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(Constants.CORPID, corpid);
            dic.Add(Constants.CORPSECRET, corpsecret);
            string accesstoken = _dingDingHelper.GetAccessToken(dic);
            _cacheHelper.Add(Constants.ACCESS_TOKEN, accesstoken);
            return accesstoken;
        }

        public string GetJsApiTicket()
        {
            string ticket = _dingDingHelper.GetJsApiTicket(GetAccessTokenToDic());
            _cacheHelper.Add(Constants.JSAPI_TICKET, ticket);
            return ticket;
        }

        public string GetDingDingUserID(string code)
        {
            Dictionary<string, string> dic = GetAccessTokenToDic();
            dic.Add(Constants.CODE, code);
            return _dingDingHelper.GetDingDingUserID(dic);
        }

        public string GetUserInfo(string dingDingUserID)
        {
            return _dingDingHelper.GetUserInfo(dingDingUserID).ToJson();
        }

        public string GetDeptUserInfoList(string dingDingDeptID)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(Constants.ACCESS_TOKEN ,GetAccessToken());
            dic.Add(Constants.DEPARTMENT_ID, dingDingDeptID);
            dic.Add(Constants.OFFSET, "0");
            dic.Add(Constants.SIZE, "10");
            return _dingDingHelper.GetDeptUserInfoList(dic);
        }

        public string GetSignature()
        {
            //String plain = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket, nonceStr, timeStamp, url);
            string randomStr = DingTalkSignatureUtil.GetRandomStr(10);
            string timeStamp = TopUtils.GetCurrentTimeMillis().ToString();

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Constants.JSAPI_TICKET, _cacheHelper.Get(Constants.JSAPI_TICKET).ToString());
            dic.Add(Constants.NONCESTR, randomStr);
            dic.Add(Constants.TIMESTAMP, timeStamp);
            dic.Add(Constants.URL, AppSettings.Get(Constants.URL));

            _cacheHelper.Add(Constants.NONCESTR, randomStr);
            _cacheHelper.Add(Constants.TIMESTAMP, timeStamp);

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ConvertDictionaryToString(dic));
                byte[] digest = SHA1.Create().ComputeHash(bytes);
                string digestBytesString = BitConverter.ToString(digest).Replace("-", "");
                _cacheHelper.Add(Constants.SIGNATURE, digestBytesString.ToLower());
                return digestBytesString.ToLower();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public string GetSignature(string jsapi_ticket,string noncestr, string timeStamp,string url)
        {
            //String plain = string.Format("jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}", ticket, nonceStr, timeStamp, url);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Constants.JSAPI_TICKET, jsapi_ticket);
            dic.Add(Constants.NONCESTR, noncestr);
            dic.Add(Constants.TIMESTAMP, timeStamp);
            dic.Add(Constants.URL, url);

            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ConvertDictionaryToString(dic));
                byte[] digest = SHA1.Create().ComputeHash(bytes);
                string digestBytesString = BitConverter.ToString(digest).Replace("-", "");
                _cacheHelper.Add(Constants.SIGNATURE, digestBytesString.ToLower());
                return digestBytesString.ToLower();
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public string ConvertDictionaryToString(Dictionary<string, object> dic)
        {
            return string.Join("&", dic.Select(o => o.Key + "=" + o.Value));
        }

        public string ConvertDictionaryToString(Dictionary<string, string> dic)
        {
            return string.Join("&", dic.Select(o => o.Key + "=" + o.Value));
        }

        public Dictionary<string, string> GetAccessTokenToDic()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string AccessToken = string.Empty;
            if (_cacheHelper.Get(Constants.ACCESS_TOKEN) != null)
            {
                AccessToken = _cacheHelper.Get(Constants.ACCESS_TOKEN).ToString();
            }
            else
            {
                AccessToken = GetAccessToken();
            }

            dic.Add(Constants.ACCESS_TOKEN, AccessToken);
            return dic;
        }
        public Dictionary<string, string> GetAccessTokenToDicObject()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string AccessToken=string.Empty;
            if (_cacheHelper.Get(Constants.ACCESS_TOKEN)!=null){
               AccessToken = _cacheHelper.Get(Constants.ACCESS_TOKEN).ToString();
            }else{
                AccessToken =GetAccessTokenByCorp();
            }

            dic.Add(Constants.ACCESS_TOKEN, AccessToken);
            return dic;
        }

        /// <summary>
        /// 发起个人会话
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="cid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string SendMessage(string userid,string cid ,string message)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add(Constants.SENDER, userid);
            dic.Add(Constants.CID, cid);
            dic.Add(Constants.MSG, message);
            return  _dingDingHelper.CommonFunctions(dic, Constants.SEND_TO_CONVERSATION+"?"+ ConvertDictionaryToString(GetAccessTokenToDicObject()), Constants.POST);
        }

        public string GetDeptInfoByUserId(string UserId)
        {
            Dictionary<string, string> dic = GetAccessTokenToDicObject();
            dic.Add(Constants.USERID, UserId);
            string UserInfoToString = _dingDingHelper.GetDingDingUserInfo(dic);
            dynamic UserInfoToObject = UserInfoToString.ToJObject();
            return GetDeptInfoByDeptId(UserInfoToObject.department);
        }

        public string GetDeptInfoByDeptId(JArray DeptIds)
        {
            
            Dictionary<string,string> deptDic =new Dictionary<string,string>();
            List<Dictionary<string, string>> deptListDic = new List<Dictionary<string, string>>();
            foreach(string DeptId in DeptIds)
            {
               Dictionary<string, string> dic = GetAccessTokenToDic();
               dic.Add(Constants.ID, DeptId);
               dynamic DeptInfoToObject = _dingDingHelper.GetDeptInfoByDeptId(dic).ToJObject();
               deptDic.Add(Constants.ID,DeptInfoToObject.id.ToString());
               deptDic.Add(Constants.NAME, DeptInfoToObject.name.ToString());
               deptListDic.Add(deptDic);
            }
            return deptListDic.ToJson();
        }


        public string ToDoItems(string StartUserId, string ToDoDate, string Title, string url, List<Dictionary<string, string>> dicList)
        {
            
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("userid", "172849403829072399");
            dic.Add("create_time", ToDoDate);
            dic.Add("title", Title);
            dic.Add("url", url);
            dic.Add("formItemList", dicList);
            string result = _dingDingHelper.CommonFunctions(dic, Constants.WORKCORD_ADD + "?" + ConvertDictionaryToString(GetAccessTokenToDicObject()));
            return result;
        }

    }
}