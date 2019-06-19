using DingTalkApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DingTalkApi.Controllers
{
    public class DingReportController : ApiController
    {
        private ReportService reportService = new ReportService();
        public string GetReportListByUserId(string StartTime, string EndTime, string TemplateName, string UserId)
        {
            return reportService.GetReportListByUserId(StartTime, EndTime, TemplateName, UserId);
        }
    }
}
;