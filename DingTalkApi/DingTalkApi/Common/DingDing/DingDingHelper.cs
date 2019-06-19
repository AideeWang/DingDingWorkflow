using DingTalkApi.Common;
using DingTalkApi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DingTalkApi.Common
{
    public class DingDingHelper
    {
        HttpHelper _httpHelper = new HttpHelper(AppSettings.Get(Constants.DINGTALK));
        SqlHelper _sqlHelper = new SqlHelper();
        public string GetAccessToken(Dictionary<string,string> dic)
        {
            try
            {
                string response = _httpHelper.Get(dic, Constants.GETTOKEN);
                AccessTokenModel oat = Newtonsoft.Json.JsonConvert.DeserializeObject<AccessTokenModel>(response);

                if (oat != null)
                {
                    if (oat.errcode == 0)
                    {
                        return oat.access_token;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        public string GetJsApiTicket(Dictionary<string,string> dic)
        {
            try
            {
                string response = _httpHelper.Get(dic, Constants.GET_JSAPI_TICKET);
                JsApiTicketModel model = Newtonsoft.Json.JsonConvert.DeserializeObject<JsApiTicketModel>(response);

                if (model != null)
                {
                    if (model.errcode == 0)
                    {
                        return model.ticket;
                    }
                    else
                    {
                        return model.errmsg;
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        public string GetDingDingUserID(Dictionary<string,string> dic)
        {
            try
            {
                return _httpHelper.Get(dic, Constants.GETUSERINFO);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataTable GetUserInfo(string dingDingUserID)
        {
            string sql = string.Format(@"SELECT 
                                         D.DingDingUserID,
	                                     D.DingDingUserName,
                                         D.DingDingPhone,
                                         E.EmpName,
                                         E.EmpID,
                                         E.EmpNo,
	                                     E.DomainAccount FROM dbo.DingDingUser D
                                         LEFT JOIN dbo.Employee E ON  D.EmpID =E.EmpID
                                         WHERE D.IsDelete = 0 AND D.DingDingUserID = '{0}'", dingDingUserID);

            return _sqlHelper.ExecuteDataTable(sql);
        }

        public string GetDeptUserInfoList(Dictionary<string, string> dic)
        {
            try
            {
                return _httpHelper.Get(dic, Constants.SIMPLELIST);
              

            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }


        public string CommonFunctions(Dictionary<string, string> dic,string apiName,string requestType)
        {
            try
            {
                if (requestType == "GET")
                {
                    return _httpHelper.Get(dic, apiName);
                }else
                {
                    return _httpHelper.Post(dic, apiName);
                }


            }
            catch (Exception ex)
            {
                throw;
            }
            return string.Empty;
        }

        public string CommonFunctions(Dictionary<string, object> dic, string apiName)
        {
            try
            {
                return _httpHelper.Post(dic, apiName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string CommonFunctions(Dictionary<string, string> dic, string apiName)
        {
            try
            {
                return _httpHelper.Get(dic, apiName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public string GetDingDingUserInfo(Dictionary<string, string> dic)
        {
            try
            {
                return _httpHelper.Get(dic, Constants.USERGET);
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string GetDeptInfoByDeptId(Dictionary<string, string> dic)
        {
            try
            {
                return _httpHelper.Get(dic, Constants.DEPARTMENTGET);
            }
            catch (Exception)
            {
                
                throw;
            }
        }
    }
}