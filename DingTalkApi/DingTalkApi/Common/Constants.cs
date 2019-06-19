using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DingTalkApi.Common
{
    public sealed class Constants
    {
        public const string SEPARATOR = "&";
        public const string SPOT = ".";
        public const string HMACSHA1 = "HMACSHA1";
        public const string ENCODING_UTF8 = "UTF-8";

        public const string GET = "GET";
        public const string POST = "POST";

        #region 配置文件地址
        public const string APPSETTING = "appsettings.json";
        #endregion

        #region DINGDING 
        public const string DINGTALK = "dingtalk";
        public const string CORPID = "corpid";
        public const string CORPSECRET = "corpsecret";
        public const string AGENTID = "agentid";
        public const string APPKEY = "appkey";
        public const string APPSERCRET = "appsecret";
        public const string ACCESS_TOKEN= "access_token";
        public const string ID = "id";



        public const string JSAPI_TICKET="jsapi_ticket";
        public const string NONCESTR = "noncestr";
        public const string TIMESTAMP= "timestamp";
        public const string URL = "url";
        public const string SIGNATURE= "signature";
        public const string CODE = "code";

        public const string DEPARTMENT_ID = "department_id";
        public const string OFFSET = "offset";
        public const string SIZE = "size";

        public const string CID = "cid";
        public const string MSG = "msg";
        public const string SENDER = "sender";

        //API
        public const string GETTOKEN = "gettoken";
        public const string GET_JSAPI_TICKET = "get_jsapi_ticket";
        public const string GETUSERINFO = "user/getuserinfo";
        public const string SIMPLELIST = "user/simplelist";
        public const string SEND_TO_CONVERSATION = "message/send_to_conversation";//发起个人会话
        public const string USERGET = "user/get";
        public const string DEPARTMENTGET="department/get";
        public const string WORKCORD_ADD="topapi/workrecord/add";

        //workflow
        public const string CREATEPROCESS = "topapi/processinstance/create";
        public const string GETPROCESS="topapi/processinstance/get";
        public const string LISTBYUSERID="topapi/process/listbyuserid";
        public const string LISTIDS="topapi/processinstance/listids";

        public const string PROCESS_CODE = "process_code";
        public const string ORIGINATOR_USER_ID="originator_user_id";
        public const string DEPT_ID="dept_id";
        public const string APPROVERS="approvers";
        public const string APPROVERS_V2="approvers_v2";
        public const string USER_IDS="user_ids";
        public const string TASK_ACTION_TYPE="task_action_type";
        public const string CC_LIST1 = "cc_list";
        public const string CC_POSITION="cc_position";
        public const string FORM_COMPONENT_VALUES="form_component_values";
        public const string NAME = "name";
        public const string VALUE = "value";
        public const string EXT_VALUE="ext_value";
        public const string PROCESS_INSTANCE_ID="process_instance_id";
        public const string START_TIME="start_time";
        public const string END_TIME="end_time";
        public const string USERID_LIST="userid_list";

        //callback
        public const string REGISTER_CALL_BACK="call_back/register_call_back";
        public const string CALL_BACK_TAG="call_back_tag";
        public const string TOKEN = "token";
        public const string AES_KEY = "aes_key";
        public const string BPMS_INSTANCE_CHANGE="bpms_instance_change";//审批实例开始，结束 事件类型名称
        public const string BPMS_TASK_CHANGE="bpms_task_change";
        public const string CHECK_URL="check_url";

        public const string SUCCESS="success";
        public const string REGISTERURL="registerurl";
        public const string GET_CALL_BACK_FAILED_RESULT="get_call_back_failed_result";

        //Form
        public const string LEAVETYPE = "请假类型";
        public const string STRAT_END = "[\"开始时间\",\"结束时间\"]";
        public const string STARTDATE = "开始时间";
        public const string ENDDATE = "结束时间";
        public const string REASONLEAVE = "请假事由";

        //user
        public const string USERID = "userid";

        //report
        public const string REPORT_LIST="topapi/report/list";

        #endregion

        #region CCHR
        public const string CCHECONNECTIONSTRING = "CCHRConnectionString";
        #endregion
    }
}
