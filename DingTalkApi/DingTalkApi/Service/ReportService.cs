using DingTalkApi.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkApi.Service
{
    public class ReportService
    {

        private DingDingService _dingdingService = new DingDingService();
        private DingDingHelper _dingDingHelper = new DingDingHelper(); 

        public string GetReportListByUserId(string StartTime,string EndTime,string TemplateName,  string UserId)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("start_time", Convert.ToDateTime(StartTime).ConvertDateTimeInt());
            dic.Add("end_time", Convert.ToDateTime(EndTime).ConvertDateTimeInt());
            dic.Add("template_name", TemplateName);
            dic.Add("userid", UserId);
            dic.Add("cursor", 0);
            dic.Add("size", 10);
            string result =_dingDingHelper.CommonFunctions(dic,Constants.REPORT_LIST+ "?" + _dingdingService.ConvertDictionaryToString(_dingdingService.GetAccessTokenToDicObject()));
            JArray jarray = new JArray();
            JObject dyReport = result.ToJObject();
            DingTalk.Api.Response.CorpReportListResponse.PageVoDomain pvd = dyReport["result"].ToJson().ToObject<DingTalk.Api.Response.CorpReportListResponse.PageVoDomain>();
            pvd.DataList.ForEach(a => {
                a.Contents.ForEach(b => {
                    jarray.Add(new JObject {"name",b.Key });
                });
            });
            return result;
        }

    }
}