using DingTalkApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalkApi.Controllers
{
    public class DingDingWorkflowController : Controller
    {
        WorkflowService workflowSeriver = new WorkflowService();
        DingDingService dingDingSeriver = new DingDingService();
        // GET: /DingDingWorkflow/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateProcessInstances(string Leavetype,string StartUserId, string StartDeptId, string StartDate, string EndDate, string ReasonLeave)
        {

            return Json(workflowSeriver.CteateProcessInstances(Leavetype,StartUserId, StartDeptId, StartDate, EndDate, ReasonLeave), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDeptInfByUserId(string UserId)
        {
           return Json( dingDingSeriver.GetDeptInfoByUserId(UserId),JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetLeaveTypeList()
        {
            return Json(workflowSeriver.GetLeaveTypeList(),JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProcessInfo(string ProcessId)
        {
            return Json(workflowSeriver.GetProcessInfo(ProcessId),JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetProcessIdList(string UserIds, string StartDate, string EndDate)
        {
            return Json(workflowSeriver.GetProcessIdList(UserIds, StartDate, EndDate), JsonRequestBehavior.AllowGet);
        }
	}
}