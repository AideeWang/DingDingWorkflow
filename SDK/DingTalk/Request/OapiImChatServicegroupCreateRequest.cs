using System;
using System.Collections.Generic;
using Top.Api.Util;
using Top.Api;
using Top.Api.DingTalk;

namespace DingTalk.Api.Request
{
    /// <summary>
    /// TOP API: dingtalk.oapi.im.chat.servicegroup.create
    /// </summary>
    public class OapiImChatServicegroupCreateRequest : BaseDingTalkRequest<DingTalk.Api.Response.OapiImChatServicegroupCreateResponse>
    {
        /// <summary>
        /// 群主在钉钉组织内的userid
        /// </summary>
        public string OwnerUserid { get; set; }

        /// <summary>
        /// 群标题
        /// </summary>
        public string Title { get; set; }

        #region IDingTalkRequest Members

        public override string GetApiName()
        {
            return "dingtalk.oapi.im.chat.servicegroup.create";
        }

        public override string GetApiCallType()
        {
            return DingTalkConstants.CALL_TYPE_OAPI;
        }

        public override IDictionary<string, string> GetParameters()
        {
            TopDictionary parameters = new TopDictionary();
            parameters.Add("owner_userid", this.OwnerUserid);
            parameters.Add("title", this.Title);
            if (this.otherParams != null)
            {
                parameters.AddAll(this.otherParams);
            }
            return parameters;
        }

        public override void Validate()
        {
            RequestValidator.ValidateRequired("owner_userid", this.OwnerUserid);
            RequestValidator.ValidateRequired("title", this.Title);
            RequestValidator.ValidateMaxLength("title", this.Title, 256);
        }

        #endregion
    }
}
