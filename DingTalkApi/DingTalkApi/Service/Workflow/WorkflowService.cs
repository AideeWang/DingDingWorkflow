using DingTalkApi.Common;
using DingTalkApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkApi.Service
{
    public class WorkflowService
    {

        public WorkflowService()
        {
            corpid = AppSettings.Get(Constants.CORPID);
            corpsecret = AppSettings.Get(Constants.CORPSECRET);
            process_code = AppSettings.Get(Constants.PROCESS_CODE);
        }

        private string corpid { get; set; }
        private string corpsecret { get; set; }

        private string process_code { get; set; }


        private DingDingHelper _dingDingHelper = new DingDingHelper();
        public CacheHelper _cacheHelper = new CacheHelper();

        private DingDingService _dingdingSeriver = new DingDingService();
        /// <summary>
        /// 获取AccessToken
        /// </summary>
        /// <returns></returns>
        public string GetCorpAccessToken()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            dic.Add(Constants.CORPID, corpid);//appkey
            dic.Add(Constants.CORPSECRET, corpsecret);//appsecret
            string accesstoken = _dingDingHelper.GetAccessToken(dic);
            _cacheHelper.Add(Constants.ACCESS_TOKEN, accesstoken);
            return accesstoken;
        }


        public string CteateProcessInstances(string LeaveType,string StartUserId,string StartDeptId,string StartDate,string EndDate,string ReasonLeave)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Constants.PROCESS_CODE, process_code);
            dic.Add(Constants.ORIGINATOR_USER_ID, StartUserId);
            dic.Add(Constants.DEPT_ID, StartDeptId);
            List<Dictionary<string,string>> dicFormList=new List<Dictionary<string,string>>();
           
            //请假类型
            Dictionary<string,string> dicLeaveType =new Dictionary<string,string>();
            dicLeaveType.Add(Constants.NAME,Constants.LEAVETYPE);
            dicLeaveType.Add(Constants.VALUE, LeaveType);
            dicFormList.Add(dicLeaveType);

            //开始时间 结束时间
            Dictionary<string,string> dicStartAndEndDateTime =new Dictionary<string,string>();
            dicStartAndEndDateTime.Add(Constants.NAME,Constants.STRAT_END);
            dicStartAndEndDateTime.Add(Constants.VALUE, (StartDate+","+EndDate).Split(',').ToJson());
            dicStartAndEndDateTime.Add(Constants.EXT_VALUE, (Convert.ToDateTime(EndDate).CompareTo(Convert.ToDateTime(StartDate))).ToString());
            dicFormList.Add(dicStartAndEndDateTime);

            //请假事由
            Dictionary<string, string> dicReasonLeave = new Dictionary<string, string>();
            dicReasonLeave.Add(Constants.NAME, Constants.REASONLEAVE);
            dicReasonLeave.Add(Constants.VALUE, ReasonLeave);
            dicFormList.Add(dicReasonLeave);

            dic.Add(Constants.FORM_COMPONENT_VALUES, dicFormList);
            string ss = dic.ToJson();
            string result = _dingDingHelper.CommonFunctions(dic, Constants.CREATEPROCESS + "?" + _dingdingSeriver.ConvertDictionaryToString(_dingdingSeriver.GetAccessTokenToDicObject()));
            return result;
        }

        /// <summary>
        /// 获取请假类型
        /// </summary>
        /// <returns></returns>
        public string GetLeaveTypeList()
        {
            return ConvertEnum.GetEnumNameList<LeaveType>().ToJson();
        }


        /// <summary>
        /// 获取单个流程实例
        /// </summary>
        /// <param name="ProcessId"></param>
        /// <returns></returns>
        public string GetProcessInfo(string ProcessId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add(Constants.PROCESS_INSTANCE_ID, ProcessId);
            string result = _dingDingHelper.CommonFunctions(dic, Constants.GETPROCESS + "?" + _dingdingSeriver.ConvertDictionaryToString(_dingdingSeriver.GetAccessTokenToDicObject()));
            return result;
        }

        public string GetProcessIdList(string UserIds,string StartDate,string EndDate)
        {
            Dictionary<string, object> dic= new Dictionary<string, object>();
            dic.Add(Constants.PROCESS_CODE, process_code);
            dic.Add(Constants.START_TIME, ConvertDateTime.ConvertDateTimeInt(Convert.ToDateTime(StartDate)));
            dic.Add(Constants.ENDDATE, ConvertDateTime.ConvertDateTimeInt(Convert.ToDateTime(EndDate)));
            dic.Add(Constants.USERID_LIST, UserIds);
            string result = _dingDingHelper.CommonFunctions(dic, Constants.LISTIDS + "?" + _dingdingSeriver.ConvertDictionaryToString(_dingdingSeriver.GetAccessTokenToDicObject()));
            return result;
        }
        //public string GetProcessTemplate(string)
    }
}