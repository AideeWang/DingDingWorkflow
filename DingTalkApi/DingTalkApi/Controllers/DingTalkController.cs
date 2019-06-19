using DingTalkApi.Common;
using DingTalkApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DingTalkApi.Controllers
{
    public class DingTalkController : Controller
    {
        DingDingService seriver = new DingDingService();
        // GET: DingTalk
        public ActionResult Index()
        {
            ViewBag.AccessToken = seriver.GetAccessToken();
            ViewBag.JsApiTicket = seriver.GetJsApiTicket();
            ViewBag.Signature = seriver.GetSignature();
            ViewBag.NonceStr = seriver._cacheHelper.Get(Constants.NONCESTR);
            ViewBag.CorpId = seriver.corpid;
            ViewBag.CorpSecret = seriver.corpsecret;
            ViewBag.AgentId = seriver.agentid;
            ViewBag.TimeStamp = seriver._cacheHelper.Get(Constants.TIMESTAMP);
            ViewBag.Url = seriver.url;
            return View();
        }

        [HttpGet]
        public ActionResult GetSignature(string jsapi_ticket,string noncestr,string timeStamp,string url)
        {
            return Json(seriver.GetSignature(jsapi_ticket,noncestr, timeStamp,url), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDingDingUserInfo(string code)
        {
            return Json(seriver.GetDingDingUserID(code), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetUserInfo(string dingDingUserID)
        {
            return Json(seriver.GetUserInfo(dingDingUserID), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetDeptUserInfoList(string dingDingDeptID)
        {
            return Json(seriver.GetDeptUserInfoList(dingDingDeptID), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult SendMessage(string userid,string cid, string message)
        {
           
            return Json(seriver.SendMessage(userid,cid, message), JsonRequestBehavior.AllowGet);
        }
    }
}