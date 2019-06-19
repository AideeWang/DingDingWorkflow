using DingTalkApi.Common;
using DingTalkApi.Models;
using DingTalkApi.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DingTalkApi.Controllers
{
    public class DingDingCallbackController : ApiController
    {
        CallbackService callbackserivce = new CallbackService();

        /// <summary>
        /// 注册回调接口
        /// </summary>
        [HttpGet]
        public string RegisterCallbackApi()
        {
            try
            {
               return callbackserivce.ReigsterCallbackApi();
            }
            catch (Exception ex )
            {
                LogHelper.Debug(ex.ToJson());
                return new BaseDDModel { errcode=001,errmsg="注册回调接口失败"}.ToJson();
               
            }
           
        }

        public string GetCallbackFailure()
        {
            try
            {
                return callbackserivce.GetCallbackFailure();
            }
            catch (Exception ex)
            {
                
                LogHelper.Debug(ex.ToJson());
                return new BaseDDModel { errcode=001,errmsg="注册回调接口失败"}.ToJson();
            }
        }


        /// <summary>
        /// 钉钉审批完成回调
        /// </summary>
        public void EndApprovalCallback()
        {
            callbackserivce.EndApprovalCallback();
        }


    }
}
