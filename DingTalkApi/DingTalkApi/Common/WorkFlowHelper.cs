using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Data;
using System.Xml;
using SourceCode.Workflow.Client;
using SourceCode.Workflow.Management;
using System.Collections.Generic;
using System.Reflection;
using Com.DAO;

namespace DingTalkApi.Common
{
    /// <summary>
    /// WorkFlowHelper 的摘要说明。

    /// </summary>
    public class WorkFlowHelper
    {
        public WorkFlowHelper()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 流程类型 WF_APPLY_TYPE
        /// <summary>
        /// 组织单元新建审批
        /// </summary>
        public const string WF_APPLY_TYPE_DEPT_ADD = "组织单元新建审批";

        /// <summary>
        /// 组织单元调整审批
        /// </summary>
        public const string WF_APPLY_TYPE_DEPT_ADJUST = "组织单元调整审批";

        /// <summary>
        /// 组织单元撤销审批
        /// </summary>
        public const string WF_APPLY_TYPE_DEPT_CANCLE = "组织单元撤销审批";


        /// <summary>
        /// 组织单元调整审批
        /// </summary>
        public const string WF_APPLY_TYPE_DEPT_ModName = "组织单元更名审批";

        /// <summary>
        /// 职员入职审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_JOIN = "职员入职审批";

        /// <summary>
        /// 职员转正审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_PASS = "职员转正审批";

        /// <summary>
        /// 职员异动审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_ADJUST = "职员异动审批";

        /// <summary>
        /// 职员工资方案变更审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_SPCHANGE = "职员工资方案变更审批";


        /// <summary>
        /// 职员离职审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_DIMISSION = "职员离职审批";

        /// <summary>
        /// 实习生续签审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_INTERN_RENEWAL = "实习生续签审批";

        /// <summary>
        /// 职员合同续签审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_RENEWAL = "职员续签审批";


        /// <summary>
        /// 行车津贴审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_MILEAGE = "行车津贴审批";

        /// <summary>
        /// 行车津贴撤销审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_MILEAGE_CANCEL = "行车津贴撤销审批";

        /// <summary>
        /// 收入证明审批
        /// </summary>
        public const string WF_APPLY_TYPE_EMP_INCOME = "收入证明审批";


        /// <summary>
        /// 招聘
        /// </summary>
        public const string WF_APPLY_TYPE_ERCRUIT = "职员招聘审批";


        /// <summary>
        /// 编制申请审批
        /// </summary>
        public const string WF_APPLY_TYPE_POS_ADD = "编制申请审批";
        #endregion

        #region 成交报告当前进程状态常量定义

        public const string CONTRACT_ALREADY_REPORT = "IsReportAchievement";//已上数

        #endregion 成交报告当前进程状态常量定义


        #region 审批状态常量

        //WF_INSTANCE_STATE
        //其他状态增加类似的常量
        public const string WF_INSTANCE_PRESUBMIT_STATE = "待提交";
        public const string WF_INSTANCE_APPROVING_STATE = "审批中";
        public const string WF_INSTANCE_COMPLETED_STATE = "已完成";
        #endregion

        /// <summary>
        /// 获取K2连接
        /// </summary>
        /// <returns></returns>
        public static Connection GetK2Connection()
        {
            try
            {
                Connection oK2Connection = new Connection();
                string K2Server = System.Configuration.ConfigurationManager.AppSettings["PlanServer"];

                string mngUserID = System.Configuration.ConfigurationManager.AppSettings["MngUserID"].ToString();
                string mngUserPasswrod = Cryptography.DESDecryption(System.Configuration.ConfigurationManager.AppSettings["MngUserPassword"].ToString());
                string connectionString = "[;];Authentication=Windows;Domain=centaline;User=" + mngUserID + ";Password=" + mngUserPasswrod + "";

                oK2Connection.Open(K2Server, connectionString);
                return oK2Connection;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 得到K2连接串

        /// </summary>
        /// <returns></returns>
        public static string GetK2ManagementServerConnectionString()
        {
            SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder connectionString;
            connectionString = new SourceCode.Hosting.Client.BaseAPI.SCConnectionStringBuilder();
            connectionString.Authenticate = true;
            connectionString.WindowsDomain = "CENTALINE";
            connectionString.SecurityLabelName = "K2";
            connectionString.UserID = System.Configuration.ConfigurationManager.AppSettings["MngUserID"].ToString();
            connectionString.Password = Cryptography.DESDecryption(System.Configuration.ConfigurationManager.AppSettings["MngUserPassword"].ToString());
            connectionString.Host = System.Configuration.ConfigurationManager.AppSettings["PlanServer"];
            connectionString.Integrated = true;
            connectionString.IsPrimaryLogin = true;
            connectionString.Port = 5555;

            return connectionString.ToString();
        }

        /// <summary>
        /// 通过管理员找到对应WorkListItem的状态

        /// </summary>
        /// <param name="processFullName"></param>
        /// <param name="activityName"></param>
        /// <param name="destination"></param>
        /// <returns></returns>      
        public static SourceCode.Workflow.Management.WorklistItem.WorklistStatus GetWorkListItemStatus(string processFullName, string activityName, string destination)
        {
            SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter
                worklistCriteriaFilter = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
            worklistCriteriaFilter.REGULAR_FILTER(WorklistFields.ProcessFullName,
                                                    SourceCode.Workflow.Management.Criteria.Comparison.Equals,
                                                    processFullName,
                                                    SourceCode.Workflow.Management.Criteria.RegularFilter.FilterCondition.AND);
            worklistCriteriaFilter.AddRegularFilter(WorklistFields.ActionName,
                                                    SourceCode.Workflow.Management.Criteria.Comparison.Equals,
                                                    activityName);
            worklistCriteriaFilter.AddRegularFilter(WorklistFields.Destination,
                                                    SourceCode.Workflow.Management.Criteria.Comparison.Like,
                                                    destination);

            WorkflowManagementServer wms = new WorkflowManagementServer();
            wms.Open(GetK2ManagementServerConnectionString());


            SourceCode.Workflow.Management.WorklistItems wlis = wms.GetWorklistItems(worklistCriteriaFilter);
            SourceCode.Workflow.Management.WorklistItem.WorklistStatus ret = SourceCode.Workflow.Management.WorklistItem.WorklistStatus.Completed;
            foreach (SourceCode.Workflow.Management.WorklistItem wli in wlis)
            {
                ret = wli.Status;
            }
            return ret;
        }


        /// <summary>
        /// 根据流程类型名称查询类型ID  add luliang 2013-05-24
        /// </summary>
        /// <param name="applyTypeName"></param>
        /// <returns></returns>
        private static string GetApplyTypeID(string applyTypeName)
        {
            var dataManager = new DataManager();
            try
            {
                var sqlStr = string.Format("select ApplyTypeID from WF_ApplyType where ApplyType = '{0}'", applyTypeName);
                if (dataManager.IsClosed)
                {
                    dataManager.OpenWithConfig();
                }
                object applyId = dataManager.SelectScalar(sqlStr);
                return applyId == null ? "" : Convert.ToString(applyId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                if (!dataManager.IsClosed)
                    dataManager.Close();
            }
        }


        /// <summary>
        /// 根据流程类型名称查询类型ID  add luliang 2013-10-09
        /// </summary>
        /// <param name="applyTypeName"></param>
        /// <returns></returns>
        private static string GetApplyTypeIDWorkflow(DataManager dataManager, string applyTypeName)
        {

            try
            {
                var sqlStr = string.Format("select ApplyTypeID from WF_ApplyType where ApplyType = '{0}'", applyTypeName);
                if (dataManager.IsClosed)
                {
                    dataManager.OpenWithConfig();
                }
                object applyId = dataManager.SelectScalar(sqlStr);
                return applyId == null ? "" : Convert.ToString(applyId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }





        public static bool StartProcessInstance(string strApplyID, string strApplyTypeID, string strBizDataID,
                string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName,
                string strApplyEmpID, string strApplyTypeName, string strPositionID, string strRoleName,
                string strJoinDeptKind, string strJoinEmpName, string strJoinDeptFullName, string strJoinDeptID,
                string JobLevel, string strValidate, string strBizShortDeptName, string topAuditorBySalaryPlan)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                switch (strApplyTypeName)
                {
                    case WorkFlowHelper.WF_APPLY_TYPE_EMP_JOIN:
                        #region 职员入职
                        //公有
                        oNewProcessInstance.DataFields["Originator"].Value = originator;
                        oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                        oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                        oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                        oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                        oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                        oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                        oNewProcessInstance.DataFields["ValidedDate"].Value = Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd");//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        

                        //私有
                        oNewProcessInstance.DataFields["BizDeptID"].Value = strJoinDeptID;
                        oNewProcessInstance.DataFields["BizDeptName"].Value = strJoinDeptFullName;
                        oNewProcessInstance.DataFields["DeptKind"].Value = strJoinDeptKind;
                        oNewProcessInstance.DataFields["BizEmpID"].Value = "";
                        oNewProcessInstance.DataFields["BizEmpName"].Value = strJoinEmpName;
                        oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strPositionID;//初始化，用它找上级

                        oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                        oNewProcessInstance.DataFields["JobLevel"].Value = JobLevel;

                        //20131206新增
                        oNewProcessInstance.DataFields["BizShortDeptName"].Value = strBizShortDeptName;

                        //20180122新增  by zhangww  用于判定是否逐级审批到指定人员，目前是陆成
                        oNewProcessInstance.DataFields["TopAuditorBySalaryPlan"].Value = topAuditorBySalaryPlan;

                        //20150413 add ,西区招聘专员发起的入职申请不需要逐级审批，直接进入西区人事经理步骤.
                        if (strRoleName == "西区招聘专员" || strRoleName == "东区招聘专员" || strRoleName == "北区招聘专员")
                        {
                            oNewProcessInstance.DataFields["IsDeptAduit"].Value = "0";//IsDeptAduit=0表示不需要逐级审批
                        }
                        else
                        {
                            oNewProcessInstance.DataFields["IsDeptAduit"].Value = "1";
                        }

                        //20171115 add 获取区域人事从配置表设置 zhangww
                        if (oNewProcessInstance.DataFields["IsDeptAduit"].Value.ToString() == "0")
                        {
                            oNewProcessInstance.DataFields["HrAuditor"].Value = "0";//HrAuditor=0 不需要逐级审批的根据部门获取区域人事账号
                        }
                        else
                        {
                            oNewProcessInstance.DataFields["HrAuditor"].Value = "1";//HrAuditor=1表示其他情况，获取默认人事账号
                        }
                        //SystemConfig sysconfig = new SystemConfig();
                        //List<SystemConfigEntity> list = new List<SystemConfigEntity>();
                        //list = sysconfig.GetList("DeptHrAuditor");
                        //foreach (var item in list)
                        //{
                        //    if (strJoinDeptFullName.Contains(item.PramaKey))//如果是指定部门，也根据部门获取人事账号
                        //    {
                        //        oNewProcessInstance.DataFields["HrAuditor"].Value = "0";
                        //    }
                        //}

                        folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + ")";

                        #endregion
                        break;
                }
                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;

                oK2Connection.StartProcessInstance(oNewProcessInstance, false);

                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static bool StartProcessInstance_DeptAdjust(string strDeptName, string strApplyID, string strApplyTypeID,
            string strBizDataID, string strCurrentLoginName, string strCurrentEmpName,
            string strApplyDeptID, string strApplyDeptName, string strApplyEmpID,
            string strApplyTypeName, string strPositionID, string strRoleName, string strJoinDeptKind,
            string strJoinEmpName, string strOldDeptFullName, string strJoinDeptID, string strValidate,
            string strOldJobLevel, string strOldDeptID, string strOldDeptKindID, string strOldPositionID,
            string strPreDeptFullName, string strNewShortDeptName)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                //switch (strApplyTypeName)
                //{
                //    case WorkFlowHelper.WF_APPLY_TYPE_EMP_ADJUST:
                #region 职员群调
                //公有
                oNewProcessInstance.DataFields["Originator"].Value = originator;
                oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                oNewProcessInstance.DataFields["ValidedDate"].Value = Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd");//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        

                //私有
                oNewProcessInstance.DataFields["CurBizDeptID"].Value = strJoinDeptID;
                oNewProcessInstance.DataFields["PreBizDeptID"].Value = strOldDeptID;

                oNewProcessInstance.DataFields["CurBizDeptName"].Value = strDeptName;
                oNewProcessInstance.DataFields["PreBizDeptName"].Value = strOldDeptFullName;
                oNewProcessInstance.DataFields["PreBizDeptFullName"].Value = strPreDeptFullName;

                oNewProcessInstance.DataFields["BizShortDeptName"].Value = strNewShortDeptName;
                oNewProcessInstance.DataFields["DeptKind"].Value = strOldDeptKindID;

                oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strOldPositionID;//原部门职位id
                oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;//调入部门职位id

                //if (strPreDeptFullName.Contains("上海宝原"))
                //{
                //    if (strPreDeptFullName.Contains("事业部"))
                //    {
                //        if (strOldJobLevel.Substring(0, 1) == "D")
                //        {
                //            oNewProcessInstance.DataFields["joblevel"].Value = 1;
                //        }
                //        else
                //        {
                //            oNewProcessInstance.DataFields["joblevel"].Value = Commons.GetZJ(strOldJobLevel);
                //        }
                //    }
                //    else
                //    {
                //        oNewProcessInstance.DataFields["joblevel"].Value = Commons.GetZJ(strOldJobLevel);
                //    }

                //}
                //else
                //{
                //    oNewProcessInstance.DataFields["joblevel"].Value = strOldJobLevel;

                //}
                folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strOldDeptFullName + " / " + strDeptName + ")";
                #endregion

                //}
                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;

                oK2Connection.StartProcessInstance(oNewProcessInstance, false);

                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static bool StartProcessInstance_OriganizationCreate(string strDeptName, string strApplyID, string strApplyTypeID, string strBizDataID, string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName, string strApplyEmpID, string strApplyTypeName, string strRoleName, string strValidate, string strBizDeptID, string strDeptKindID, string strBizDeptFullName)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;

                #region 组织单元新建
                //公有
                oNewProcessInstance.DataFields["Originator"].Value = originator;
                oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                oNewProcessInstance.DataFields["ValidedDate"].Value = Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd");//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        

                //私有
                oNewProcessInstance.DataFields["BizDeptID"].Value = strBizDeptID;
                oNewProcessInstance.DataFields["BizDeptName"].Value = strBizDeptFullName;
                oNewProcessInstance.DataFields["DeptKind"].Value = "";

                folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](新建部门为：" + strDeptName + ")";
                #endregion

                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;

                oK2Connection.StartProcessInstance(oNewProcessInstance, false);

                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static bool StartProcessInstance_OrignaizationCancel(string strDeptName, string strApplyID, string strApplyTypeID, string strBizDataID, string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName, string strApplyEmpID, string strApplyTypeName, string strRoleName, string strValidate, string strBizDeptID, string strBizDeptFullName, string strK2FolderName)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);

            try
            {
                //Create New Process Instance 
                oNewProcessInstance = oK2Connection.CreateProcessInstance(strK2FolderName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称

                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                string[] jobLevel = { };

                #region 组织单元撤销
                //公有
                oNewProcessInstance.DataFields["Originator"].Value = originator;
                oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                oNewProcessInstance.DataFields["ValidedDate"].Value = Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd");//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号   

                //私有
                oNewProcessInstance.DataFields["BizDeptID"].Value = strBizDeptID;
                oNewProcessInstance.DataFields["BizDeptName"].Value = strBizDeptFullName;
                //oNewProcessInstance.DataFields["DeptKind"].Value = "";

                folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](撤销部门为：" + strDeptName + ")";
                #endregion

                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;

                //Plan\Start new Process Instance 
                oK2Connection.StartProcessInstance(oNewProcessInstance, false);

                //更改K2的发起人为当前用户                
                //UpdateProcessOriginator(oNewProcessInstance.ID.ToString(), originator);

                //Close the Connection 
                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }






        public static bool StartProcessInstance_AdjustType(string empno, string strApplyID, string strApplyTypeID, string strBizDataID,
            string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName,
            string strApplyEmpID, string strApplyTypeName, string strPositionID, string strRoleName,
            string strJoinDeptKind, string strJoinEmpName, string strOldDeptFullName, string strJoinDeptID,
            string JobLevel, string strValidate, string strAdjustTypeID, string strOldPositionID, string strOldJobLevel,
            string strOldDeptID, string strOldDeptKindID,
            string strPreTopAuditorPositionID,
            string AdjustSmallType,
            string JoinCompanyDate,
            string JoinIntDate,
            string strDomainAccount,
            string strNewDeptFullName, string strSalaryPlan, string TopAuditorBySalaryPlan, string IsSameDeptType, string IsSamePositionType)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                switch (strApplyTypeName)
                {
                    case WorkFlowHelper.WF_APPLY_TYPE_EMP_ADJUST:

                        #region 职员异动
                        //公有
                        oNewProcessInstance.DataFields["Originator"].Value = originator;
                        oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                        oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                        oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                        oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                        oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                        oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                        oNewProcessInstance.DataFields["ValidedDate"].Value = strValidate != "" ? Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd") : "";//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        

                        //私有
                        oNewProcessInstance.DataFields["NewBizDeptName"].Value = strNewDeptFullName;
                        oNewProcessInstance.DataFields["BizDeptID"].Value = strJoinDeptID;
                        oNewProcessInstance.DataFields["BizDeptName"].Value = strOldDeptFullName;
                        oNewProcessInstance.DataFields["DeptKind"].Value = strOldDeptKindID;

                        //2014-05-22
                        oNewProcessInstance.DataFields["BizDomainAccount"].Value = strDomainAccount;
                        //2012-07-05 add
                        oNewProcessInstance.DataFields["CurDeptKind"].Value = strJoinDeptKind;
                        oNewProcessInstance.DataFields["BizEmpID"].Value = "";
                        oNewProcessInstance.DataFields["BizEmpName"].Value = strJoinEmpName;
                        oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strOldPositionID;//初始化，用它找上级
                        oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strOldPositionID;
                        oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                        oNewProcessInstance.DataFields["JobLevel"].Value = strOldJobLevel;//*-s;
                        oNewProcessInstance.DataFields["JobLevelOut"].Value = JobLevel;

                        //oNewProcessInstance.DataFields["BizShortDeptName"].Value = strNewDeptFullName;//此列可不要，暂没删除
                        oNewProcessInstance.DataFields["AdjustSmallType"].Value = AdjustSmallType;
                        oNewProcessInstance.DataFields["JoinCompanyDate"].Value = JoinCompanyDate;
                        oNewProcessInstance.DataFields["JoinIntDate"].Value = JoinIntDate;//此列可不要，暂没删除
                        oNewProcessInstance.DataFields["AdjustTypeID"].Value = strAdjustTypeID; //"1_2_3"

                        //add by zhangww 20180122
                        oNewProcessInstance.DataFields["SalaryPlan"].Value = strSalaryPlan;
                        oNewProcessInstance.DataFields["TopAuditorBySalaryPlan"].Value = TopAuditorBySalaryPlan;
                        oNewProcessInstance.DataFields["IsSameDeptType"].Value = IsSameDeptType;
                        oNewProcessInstance.DataFields["IsSamePositionType"].Value = IsSamePositionType;

                        //2011-02-10 modify 无论是晋升，降职还是转职，如果后面职位的职级高，则按后面的职级
                        if ((strAdjustTypeID.IndexOf("1") >= 0 && strAdjustTypeID.IndexOf("11") < 0 && strAdjustTypeID.IndexOf("10") < 0) || strAdjustTypeID.IndexOf("2") >= 0 || strAdjustTypeID.IndexOf("3") >= 0)//兼职按兼职的流程走
                        //if (bizEmpEntity.AdjustTypeID != "4" && bizEmpEntity.AdjustTypeID != "5") //就是兼职也会按照转职=3的流程走，没有发布流程前用这个(已调整但没发布)
                        {
                            oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strOldPositionID;

                            int preJobLevel = int.Parse(string.IsNullOrEmpty(strOldJobLevel) ? "0" : strOldJobLevel);
                            int curJobLevel = int.Parse(string.IsNullOrEmpty(JobLevel) ? "0" : JobLevel);

                            if (strOldDeptID == strJoinDeptID)
                            {
                                if (strAdjustTypeID.IndexOf("3") >= 0)
                                    oNewProcessInstance.DataFields["AdjustTypeID"].Value = "0";
                            }
                            else
                            {
                                oNewProcessInstance.DataFields["AdjustTypeID"].Value = "3";
                                //AdjustSmallType 用于检查顶级职位，
                                /*如果AdjustSmallType强制转换为调动，则选择降职，晋升等类别时，下面的规则会有问题.
                                 * 1-3级 
                                 * 签至（资深/高级）区域经理（调职流程）
                                   签至（高级）区域营业董事（晋升、降职、调薪流程）
                                 * 比如：2级的晋升，本应签到营业董事，但因为改为调动，则签到区经就结束了部门内的审批了.应该不对
                                 * 周一再确认
                                 * */
                                //oNewProcessInstance.DataFields["AdjustSmallType"].Value = "调动";//不管选择了什么类别，都要这样。
                            }
                        }

                        folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + " / " + empno + ")";
                        #endregion
                        break;
                }
                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;
                oK2Connection.StartProcessInstance(oNewProcessInstance, false);
                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static bool StartProcessInstance_AdjustType(string empno, string strApplyID, string strApplyTypeID, string strBizDataID,
    string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName,
    string strApplyEmpID, string strApplyTypeName, string strPositionID, string strRoleName,
    string strJoinDeptKind, string strJoinEmpName, string strOldDeptFullName, string strJoinDeptID,
    string JobLevel, string strValidate, string strAdjustTypeID, string strOldPositionID, string strOldJobLevel,
    string strOldDeptID, string strOldDeptKindID,
    string strPreTopAuditorPositionID,
    string AdjustSmallType,
    string JoinCompanyDate,
    string JoinIntDate,
    string strDomainAccount,
    string strNewDeptFullName, string strSalaryPlan, string TopAuditorBySalaryPlan)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                switch (strApplyTypeName)
                {
                    case WorkFlowHelper.WF_APPLY_TYPE_EMP_ADJUST:

                        #region 职员异动
                        //公有
                        oNewProcessInstance.DataFields["Originator"].Value = originator;
                        oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                        oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                        oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                        oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                        oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                        oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                        oNewProcessInstance.DataFields["ValidedDate"].Value = strValidate != "" ? Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd") : "";//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        

                        //私有
                        oNewProcessInstance.DataFields["NewBizDeptName"].Value = strNewDeptFullName;
                        oNewProcessInstance.DataFields["BizDeptID"].Value = strJoinDeptID;
                        oNewProcessInstance.DataFields["BizDeptName"].Value = strOldDeptFullName;
                        oNewProcessInstance.DataFields["DeptKind"].Value = strOldDeptKindID;

                        //2014-05-22
                        oNewProcessInstance.DataFields["BizDomainAccount"].Value = strDomainAccount;
                        //2012-07-05 add
                        oNewProcessInstance.DataFields["CurDeptKind"].Value = strJoinDeptKind;
                        oNewProcessInstance.DataFields["BizEmpID"].Value = "";
                        oNewProcessInstance.DataFields["BizEmpName"].Value = strJoinEmpName;
                        oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strOldPositionID;//初始化，用它找上级
                        oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strOldPositionID;
                        oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                        oNewProcessInstance.DataFields["JobLevel"].Value = strOldJobLevel;//*-s;
                        oNewProcessInstance.DataFields["JobLevelOut"].Value = JobLevel;

                        //oNewProcessInstance.DataFields["BizShortDeptName"].Value = strNewDeptFullName;//此列可不要，暂没删除
                        oNewProcessInstance.DataFields["AdjustSmallType"].Value = AdjustSmallType;
                        oNewProcessInstance.DataFields["JoinCompanyDate"].Value = JoinCompanyDate;
                        oNewProcessInstance.DataFields["JoinIntDate"].Value = JoinIntDate;//此列可不要，暂没删除
                        oNewProcessInstance.DataFields["AdjustTypeID"].Value = strAdjustTypeID; //"1_2_3"

                        //add by zhangww 20180122
                        oNewProcessInstance.DataFields["SalaryPlan"].Value = strSalaryPlan;
                        oNewProcessInstance.DataFields["TopAuditorBySalaryPlan"].Value = TopAuditorBySalaryPlan;

                        //2011-02-10 modify 无论是晋升，降职还是转职，如果后面职位的职级高，则按后面的职级
                        if ((strAdjustTypeID.IndexOf("1") >= 0 && strAdjustTypeID.IndexOf("11") < 0 && strAdjustTypeID.IndexOf("10") < 0) || strAdjustTypeID.IndexOf("2") >= 0 || strAdjustTypeID.IndexOf("3") >= 0)//兼职按兼职的流程走
                        //if (bizEmpEntity.AdjustTypeID != "4" && bizEmpEntity.AdjustTypeID != "5") //就是兼职也会按照转职=3的流程走，没有发布流程前用这个(已调整但没发布)
                        {
                            oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strOldPositionID;

                            int preJobLevel = int.Parse(string.IsNullOrEmpty(strOldJobLevel) ? "0" : strOldJobLevel);
                            int curJobLevel = int.Parse(string.IsNullOrEmpty(JobLevel) ? "0" : JobLevel);

                            if (strOldDeptID == strJoinDeptID)
                            {
                                if (strAdjustTypeID.IndexOf("3") >= 0)
                                    oNewProcessInstance.DataFields["AdjustTypeID"].Value = "0";
                            }
                            else
                            {
                                oNewProcessInstance.DataFields["AdjustTypeID"].Value = "3";
                                //AdjustSmallType 用于检查顶级职位，
                                /*如果AdjustSmallType强制转换为调动，则选择降职，晋升等类别时，下面的规则会有问题.
                                 * 1-3级 
                                 * 签至（资深/高级）区域经理（调职流程）
                                   签至（高级）区域营业董事（晋升、降职、调薪流程）
                                 * 比如：2级的晋升，本应签到营业董事，但因为改为调动，则签到区经就结束了部门内的审批了.应该不对
                                 * 周一再确认
                                 * */
                                //oNewProcessInstance.DataFields["AdjustSmallType"].Value = "调动";//不管选择了什么类别，都要这样。
                            }
                        }

                        folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + " / " + empno + ")";
                        #endregion
                        break;
                }
                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;
                oK2Connection.StartProcessInstance(oNewProcessInstance, false);
                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static bool StartProcessInstance_SalaryPlanChange(string empno, string jobLevel, string strApplyID, string strApplyTypeID, string strBizDataID,
           string strCurrentLoginName, string strCurrentEmpName, string strApplyDeptID, string strApplyDeptName,string deptKind,
          string strApplyTypeName, string strPositionID,
          string strJoinEmpName,
          string strValidate, string strSalaryPlanType, string auditType)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;

                #region 职员工资方案变更
                //公有
                oNewProcessInstance.DataFields["Originator"].Value = originator;
                oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;
                oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strPositionID;//初始化，用它找上级
                oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                oNewProcessInstance.DataFields["ValidedDate"].Value = strValidate != "" ? Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd") : "";//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        
                oNewProcessInstance.DataFields["JobLevel"].Value = jobLevel;//*-s;
                oNewProcessInstance.DataFields["DeptKind"].Value = deptKind;
                //oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                //add by zhangww 20180122
                oNewProcessInstance.DataFields["SalaryPlanType"].Value = strSalaryPlanType;
                oNewProcessInstance.DataFields["AuditType"].Value = auditType;//审批类型，1：直接到人事审批，2：逐级到区董，3：逐级到陆总


                folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + " / " + empno + ")";
                #endregion


                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;
                oK2Connection.StartProcessInstance(oNewProcessInstance, false);
                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        /// <summary>
        /// 启动预付款流程
        /// </summary>
        /// <param name="empno"></param>
        /// <param name="jobLevel"></param>
        /// <param name="strApplyID"></param>
        /// <param name="strApplyTypeID"></param>
        /// <param name="strBizDataID"></param>
        /// <param name="strCurrentLoginName"></param>
        /// <param name="strCurrentEmpName"></param>
        /// <param name="strApplyDeptID"></param>
        /// <param name="strApplyDeptName"></param>
        /// <param name="deptKind"></param>
        /// <param name="strApplyTypeName"></param>
        /// <param name="strPositionID"></param>
        /// <param name="strJoinEmpName"></param>
        /// <param name="strValidate"></param> 
        /// <param name="WorkflowModular">流程模块(发起预付佣金流程;暂停预付佣金流程)</param>
        /// <returns></returns>
        public static bool StartProcessInstance_PrepayApply(string empno,//工号
            string jobLevel,//职级
            string strApplyID,//申请id
            string strApplyTypeID, //申请类型id
            string strBizDataID, 
            string strCurrentLoginName, //当前登录人账号
            string strCurrentEmpName, //当前登录人姓名
            string strApplyDeptID,  //申请部门ID
            string strApplyDeptName, //申请部门名称
            string deptKind,   //部门类型
            string strApplyTypeName, //申请类型名称
            string strPositionID, //职位id
            string strJoinEmpName, //职位名称
            string strValidate,  //生效日期       
            string WorkflowModular,
            string applyType
            )
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;

      
                //公有
                oNewProcessInstance.DataFields["Originator"].Value = originator;//
                oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;
                oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strPositionID;//初始化，用它找上级
                oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                oNewProcessInstance.DataFields["ValidedDate"].Value = strValidate != "" ? Convert.ToDateTime(strValidate).ToString("yyyy-MM-dd") : "";//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        
                oNewProcessInstance.DataFields["JobLevel"].Value = jobLevel;//*-s;
                oNewProcessInstance.DataFields["DeptKind"].Value = deptKind;
                oNewProcessInstance.DataFields["ApplyType"].Value = applyType;
        
                oNewProcessInstance.DataFields["WorkflowModular"].Value = WorkflowModular;
          

                folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + " / " + empno + ")";
           


                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;
                oK2Connection.StartProcessInstance(oNewProcessInstance, false);
                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }


        public static bool StartProcessInstance_Dimission(string strApplyID,
            string strApplyTypeID,
            string strBizDataID,
            string strCurrentLoginName,
            string strCurrentEmpName,
            string strApplyDeptID,
           string strApplyDeptName, string strApplyEmpID, string strApplyTypeName,
           string strPositionID, string strRoleName, string strJoinDeptKind, string strJoinEmpName,
           string strJoinDeptFullName, string strJoinDeptID, string JobLevel, string strLastAdjustDate,
           string strAdjustTypeID, string strOldPositionID, string strOldJobLevel, string strOldDeptID,
          string strBizShortDeptName, string AdjustSmallType, string strEmpNo, string strPrePositionID, string flowType)
        {
            SourceCode.Workflow.Client.ProcessInstance oNewProcessInstance;
            Connection oK2Connection = GetK2Connection();

            //取K2流程名

            string k2WorkFlowName = GetK2ApplyTypeNameByType(strApplyTypeID);
            try
            {
                oNewProcessInstance = oK2Connection.CreateProcessInstance(k2WorkFlowName);

                //设置K2 datafield
                string originatorName = ""; //流程发起人姓名

                string originator = "";
                string folio = "";  //流程标题名称
                //日志记录用
                originatorName = strCurrentEmpName;
                originator = strCurrentLoginName;
                switch (strApplyTypeName)
                {
                    case WorkFlowHelper.WF_APPLY_TYPE_EMP_DIMISSION:
                        #region 职员离职
                        //公有

                        oNewProcessInstance.DataFields["Originator"].Value = originator;
                        oNewProcessInstance.DataFields["OriginatorName"].Value = originatorName;
                        oNewProcessInstance.DataFields["OriginatorMail"].Value = strCurrentLoginName + "@centaline.com.cn";
                        oNewProcessInstance.DataFields["OriginatorDeptID"].Value = strApplyDeptID;
                        oNewProcessInstance.DataFields["OriginatorDeptName"].Value = strApplyDeptName;

                        oNewProcessInstance.DataFields["ApplyID"].Value = strApplyID;
                        oNewProcessInstance.DataFields["BizDataID"].Value = strBizDataID;
                        //	离职流程：1）员工离职当月发起离职流程，生效日期（即离职日期）=最后工作日+1，流程需当月结束（若至月底最后1天，离职审批流程没有推送完毕，系统则强制将此离职流程推送至结束环节）
                        //  2）员工离职次月发起离职流程，生效日期=次月1日


                        oNewProcessInstance.DataFields["ValidedDate"].Value = Convert.ToDateTime(strLastAdjustDate).ToString("yyyy-MM-dd");//20111031 add,在流程审批完成后，如果审批完日期不在生效日期当月，而是跨月，则生效日期自动修改为审批完月份的1号                        


                        //私有
                        oNewProcessInstance.DataFields["BizDeptID"].Value = strJoinDeptID;
                        oNewProcessInstance.DataFields["BizDeptName"].Value = strJoinDeptFullName;
                        oNewProcessInstance.DataFields["DeptKind"].Value = strJoinDeptKind;


                        oNewProcessInstance.DataFields["BizEmpID"].Value = "";
                        oNewProcessInstance.DataFields["BizEmpName"].Value = strJoinEmpName;

                        oNewProcessInstance.DataFields["CurAuditorPositionID"].Value = strOldPositionID;//初始化，用它找上级


                        oNewProcessInstance.DataFields["BizEmpPositionID"].Value = strPositionID;
                        oNewProcessInstance.DataFields["PreBizEmpPositionID"].Value = strPrePositionID;

                        oNewProcessInstance.DataFields["JobLevel"].Value = strOldJobLevel;

                        //20131206新增
                        oNewProcessInstance.DataFields["BizShortDeptName"].Value = strBizShortDeptName;
                        oNewProcessInstance.DataFields["AdjustSmallType"].Value = AdjustSmallType;

                        oNewProcessInstance.DataFields["FlowType"].Value = flowType;//20171123新增
                        folio = "[" + strApplyDeptName + " " + originatorName + "]提交的" + strApplyTypeName + "](" + strJoinEmpName + " / " + strEmpNo + ")";
                        #endregion
                        break;
                }
                //Set the Process Folio 
                oNewProcessInstance.Folio = folio;

                oK2Connection.StartProcessInstance(oNewProcessInstance, false);

                oK2Connection.Close();

                //将ApplyID的状态设为[已提交]
                //WorkFlowHelper.UpdateWorkFlowInstance(strApplyID, 1, oNewProcessInstance.ID, folio, 0, WorkFlowHelper.WF_INSTANCE_APPROVING_STATE);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                oK2Connection.Close();
            }

        }

        public static string GetK2ApplyTypeNameByType(string strApplyTypeID)
        {
            string strk2workflowname = "";
            DataManager dataManager = new Com.DAO.DataManager();
            try
            {
                if (dataManager.IsClosed == true)
                {
                    dataManager.OpenWithConfig();
                }
                string sql = "";

                sql = "select K2WorkflowName from WF_ApplyType where ApplyTypeID='" + strApplyTypeID + "'";

                object obj = dataManager.SelectScalar(sql);
                if (obj != null && obj.ToString().Trim() != "")
                {
                    strk2workflowname = obj.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (dataManager != null && dataManager.IsClosed)
                {
                    dataManager.Close();
                }
            }
            return strk2workflowname;// "深圳职员入职审批";
        }

        public static string GetK2ApplyTypeNameByType(DataManager dataManager, string strApplyTypeID)
        {
            string strk2workflowname = "";
            try
            {
                if (dataManager.IsClosed == true)
                {
                    dataManager.OpenWithConfig();
                }
                string sql = "";

                sql = "select K2WorkflowName from WF_ApplyType where ApplyTypeID='" + strApplyTypeID + "'";

                object obj = dataManager.SelectScalar(sql);
                if (obj != null && obj.ToString().Trim() != "")
                {
                    strk2workflowname = obj.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
            }
            //finally
            //{
            //    if (dataManager != null && dataManager.IsClosed)
            //    {
            //        dataManager.Close();
            //    }
            //}
            return strk2workflowname;// "深圳职员入职审批";
        }












        /// <summary>
        /// 根据ApplyType名称获取ApplyTypeID
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="applyType"></param>
        /// <returns></returns>
        public static string GetApplyTypeID(DataManager dataManager, string applyType)
        {
            return GetApplyTypeFieldValueByType(dataManager, applyType, "ApplyTypeID"); ;
        }

        public static string GetApplyTypeFieldValueByType(DataManager dataManager, string applyType, string field)
        {
            string rtn = "";
            string sql = String.Format("select * from CFG_WFApplyType where applyType = '{0}'", applyType);
            SqlDataReader rs = dataManager.selectReader(sql);
            if (rs.Read())
            {
                rtn = rs[field].ToString();
            }
            rs.Close();
            return rtn;
        }




        #region ===StartNewProcess===



        #endregion

        #region 从K2后台数据库更改发起人
        private static void UpdateProcessOriginator(string procInstID, string originator)
        {
            if (int.Parse(procInstID) < 1)
            {
                throw new Exception("流程实例ID有误码!");
            }

            DataManager k2DataManager = new DataManager();
            string k2DBConnString = System.Configuration.ConfigurationManager.AppSettings["K2DBConnectionString"];
            string k2LogDBConnString = System.Configuration.ConfigurationManager.AppSettings["K2LogDBConnectionString"];
            try
            {
                string sql = String.Format(" Update _ProcInst Set Originator = 'CENTALINE\\{0}'  WHERE [ID] = {1}  ", originator, procInstID);

                k2DataManager.Open(k2DBConnString);
                k2DataManager.Execute(sql);

                k2DataManager.Close();
                k2DataManager.Open(k2LogDBConnString);
                k2DataManager.Execute(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                k2DataManager.Close();
            }
        }
        #endregion

        /// <summary>
        /// 设置成交报告标志
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="contractID"></param>
        /// <param name="flagName"></param>
        public static void EnableContractFlagByName(DataManager dataManager, string contractID, string flagName)
        {
            try
            {
                string sql = String.Format(@"UPDATE CT_Contract
					SET {1} = 1 
					FROM CT_Contract 
					WHERE ContractID = '{0}'", contractID, flagName);

                dataManager.Execute(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







        /// <summary>
        /// 检查bizDataID是否已存在applyType的流程实例

        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="bizDataID"></param>
        /// <param name="applyType"></param>
        /// <returns></returns>
        public static bool IsWFInstanceExist(DataManager dataManager, string bizDataID, string applyType)
        {
            string sql = String.Format(@"SELECT *
				FROM WF_ApplyInstance INNER JOIN
					CFG_WFApplyType ON 
					WF_ApplyInstance.ApplyTypeID = CFG_WFApplyType.ApplyTypeID
				WHERE (WF_ApplyInstance.BizDataID = N'{0}') AND (CFG_WFApplyType.ApplyType = N'{1}')", bizDataID, applyType);

            SqlDataReader rs = dataManager.selectReader(sql);
            bool rtn = rs.Read();
            rs.Close();
            return rtn;
        }


        /// <summary>
        /// 根据BizDataID获取ApplyID
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="bizDataID"></param>
        /// <returns></returns>3
        public static string GetApplyIDByBizDataID(DataManager dataManager, string bizDataID)
        {
            BusinessEntiry be = new BusinessEntiry(dataManager, "WF_ApplyInstance", "ApplyID");
            if (be.Open(String.Format("BizDataID='{0}'", bizDataID)))
            {
                return be.GetPropertyValue("ApplyID");
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取用户帐号短名称(去掉"centaline\\")
        /// </summary>
        /// <param name="adUserID">用户帐号</param>
        /// <returns></returns>
        public static string GetShortADUserID(string adUserID)
        {
            try
            {
                string result = adUserID;
                if (adUserID.Trim() != "")
                {
                    string delimStr = "\\";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] split = null;
                    split = adUserID.Split(delimiter);
                    if (split.Length > 1)
                    {
                        result = split[1];
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region 取得当前审批步骤的名称

        public static string GetCurrentActivityName(Connection oK2Connection, string sn)
        {
            string retValue = "";
            SourceCode.Workflow.Client.WorklistItem wli = oK2Connection.OpenWorklistItem(sn);
            if (wli != null)
            {
                retValue = wli.ActivityInstanceDestination.Name;
            }
            return retValue;
        }

        public static string GetCurrentActivityName(SourceCode.Workflow.Client.WorklistItem wli)
        {
            string retValue = "";
            if (wli != null)
            {
                retValue = wli.ActivityInstanceDestination.Name;
            }
            return retValue;
        }

        public static string GetCurrentActivityDisplayName(Connection oK2Connection, string sn)
        {
            string retValue = "";
            SourceCode.Workflow.Client.WorklistItem wli = oK2Connection.OpenWorklistItem(sn);
            if (wli != null)
            {
                retValue = wli.ActivityInstanceDestination.Description;
            }
            return retValue;
        }

        public static string GetCurrentActivityDisplayName(SourceCode.Workflow.Client.WorklistItem wli)
        {
            string retValue = "";
            if (wli != null)
            {
                retValue = wli.ActivityInstanceDestination.Description;
            }
            return retValue;
        }
        #endregion 取得当前审批步骤的名称

        #region 检查WorkListItem的操作

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oK2Connection"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        /*
        public static bool CheckWorkListItem(Connection oK2Connection, string sn)
        {
            try
            {
                SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP",false);
                return true;
            }
            catch
            {
                return false;
            }
        }
        */

        public static bool CheckWorkListItem(Connection oK2Connection, string sn, string managedUser, string sharedUser)
        {

            try
            {
                SourceCode.Workflow.Client.WorklistItem worklistItem = GetWorkListItemBySN(oK2Connection, sn, managedUser, sharedUser);
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADUserID">域账号</param>
        /// <param name="sn"></param>
        /// <returns>0代表打开待办事项失败</returns>
        public static bool CheckWorkListItem(DataManager dataManager, string ADUserID, string sn, string shareUser)
        {
            try
            {
                string sql = string.Format(" EXEC PR_WF_CheckWorkListItem '{0}','{1}','{2}'", ADUserID, sn, shareUser);
                object obj = dataManager.SelectScalar(sql);
                if (obj != null)
                {
                    if (obj.ToString() == "1")
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (dataManager != null && dataManager.IsOpen)
                    dataManager.Close();
            }
        }
        #endregion

        #region 取得流程待办事项
        public static SourceCode.Workflow.Client.WorklistItem GetWorkListItemBySN1(Connection oK2Connection, string sn)
        {
            SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                return worklistItem;
            }
            else
            {
                return null;
            }
        }
        public static SourceCode.Workflow.Client.WorklistItem GetWorkListItemBySN(Connection oK2Connection, string sn, string managedUser, string sharedUser)
        {
            SourceCode.Workflow.Client.WorklistItem worklistItem = null;
            try
            {
                //正常的

                if ((string.IsNullOrEmpty(sharedUser)) && (managedUser == string.Empty))
                {
                    worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
                }

                if ((!string.IsNullOrEmpty(sharedUser)) && (managedUser == string.Empty))
                {

                    worklistItem = oK2Connection.OpenSharedWorklistItem(sharedUser, managedUser, sn);

                }

                if ((string.IsNullOrEmpty(sharedUser)) && (managedUser != string.Empty))
                {
                    worklistItem = oK2Connection.OpenManagedWorklistItem(managedUser, sn);
                }

                // 检查是否为委托的

                if ((!string.IsNullOrEmpty(sharedUser)) && (managedUser != string.Empty))
                {
                    worklistItem = oK2Connection.OpenSharedWorklistItem(sharedUser, managedUser, sn);
                }
            }
            catch
            {
                worklistItem = null;
            }

            return worklistItem;

        }
        protected static SourceCode.Workflow.Client.WorklistItem GetWorkListItemBySN2(Connection oK2Connection, string sn, string managedUser, string sharedUser, bool bAlloc, bool bIgnoreStatus)
        {
            SourceCode.Workflow.Client.WorklistItem worklistItem = null;
            try
            {
                //正常的

                if ((string.IsNullOrEmpty(sharedUser)) && (managedUser == string.Empty))
                {
                    worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP", bAlloc, bIgnoreStatus);
                }

                if ((!string.IsNullOrEmpty(sharedUser)) && (managedUser == string.Empty))
                {

                    worklistItem = oK2Connection.OpenSharedWorklistItem(sharedUser, managedUser, sn, "ASP", bAlloc, bIgnoreStatus);

                }

                if ((string.IsNullOrEmpty(sharedUser)) && (managedUser != string.Empty))
                {
                    worklistItem = oK2Connection.OpenManagedWorklistItem(managedUser, sn, "ASP", bAlloc, bIgnoreStatus);
                }

                // 检查是否为委托的

                if ((!string.IsNullOrEmpty(sharedUser)) && (managedUser != string.Empty))
                {
                    worklistItem = oK2Connection.OpenSharedWorklistItem(sharedUser, managedUser, sn, "ASP", bAlloc, bIgnoreStatus);
                }
            }
            catch
            {
                worklistItem = null;
            }

            return worklistItem;

        }
        #endregion

        #region 取得流程实例的DataField
        public static string GetInstanceDataFields(Connection oK2Connection, string sn, string fieldName, bool isXMLField)
        {
            string retValue = "";
            SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    retValue = worklistItem.ProcessInstance.XmlFields[fieldName].Value.ToString();
                }
                else
                {
                    retValue = worklistItem.ProcessInstance.DataFields[fieldName].Value.ToString();
                }
            }

            return retValue;
        }
        public static string GetInstanceDataFields(Connection oK2Connection, string sn, string fieldName, bool isXMLField, string managedUser, string sharedUser)
        {
            string retValue = "";

            SourceCode.Workflow.Client.WorklistItem worklistItem = GetWorkListItemBySN(oK2Connection, sn, managedUser, sharedUser);
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    retValue = worklistItem.ProcessInstance.XmlFields[fieldName].Value.ToString();
                }
                else
                {
                    retValue = worklistItem.ProcessInstance.DataFields[fieldName].Value.ToString();
                }
            }

            return retValue;
        }
        public static string GetInstanceDataFields(SourceCode.Workflow.Client.WorklistItem worklistItem, string fieldName, bool isXMLField)
        {
            string retValue = "";
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    retValue = worklistItem.ProcessInstance.XmlFields[fieldName].Value.ToString();
                }
                else
                {
                    retValue = worklistItem.ProcessInstance.DataFields[fieldName].Value.ToString();
                }
            }

            return retValue;
        }

        public static DataSet GetInstanceDataFields(DataManager dataManager, string sn)
        {
            DataSet ds = null;
            try
            {
                string sql = string.Format("EXEC PR_WF_GetProcInstData '{0}'", sn);
                ds = dataManager.SelectDataSet(sql);
            }
            catch (Exception ex)
            {
            }
            return ds;
        }
        #endregion 取得流程实例的DataField

        #region 设置流程实例的DataField
        public static void SetInstanceDataFields(Connection oK2Connection, string sn, string fieldName, string fieldValue, bool isXMLField)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            //转给流程管理者

            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    worklistItem.ProcessInstance.XmlFields[fieldName].Value = fieldValue;

                }
                else
                {
                    worklistItem.ProcessInstance.DataFields[fieldName].Value = fieldValue;

                }

                //更新实例的DataField值

                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }
        }
        public static void SetInstanceDataFields(Connection oK2Connection, string sn, string fieldName, string fieldValue, bool isXMLField, string managedUser, string sharedUser)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = GetWorkListItemBySN(oK2Connection, sn, managedUser, sharedUser);
            //转给流程管理者

            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    worklistItem.ProcessInstance.XmlFields[fieldName].Value = fieldValue;

                }
                else
                {
                    worklistItem.ProcessInstance.DataFields[fieldName].Value = fieldValue;

                }

                //更新实例的DataField值

                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }

        }


        // 一欠更新多个dataField
        //
        public static void SetInstanceDataFields(Connection oK2Connection, string sn, List<DataFieldItemEntity> dataFieldItemEntitys)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            //转给流程管理者


            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");

            if (worklistItem != null)
            {
                foreach (DataFieldItemEntity entity in dataFieldItemEntitys)
                {
                    if (entity.IsXMLField)
                    {
                        worklistItem.ProcessInstance.XmlFields[entity.FieldName].Value = entity.FieldValue;
                    }
                    else
                    {
                        worklistItem.ProcessInstance.DataFields[entity.FieldName].Value = entity.FieldValue;
                    }
                }

                //更新实例的DataField值

                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }
        }

        // 一欠更新多个dataField
        //
        public static void SetInstanceDataFields(Connection oK2Connection, string sn, List<DataFieldItemEntity> dataFieldItemEntitys, string managedUser, string sharedUser)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = GetWorkListItemBySN(oK2Connection, sn, managedUser, sharedUser); // oK2Connection.OpenWorklistItem(sn, "ASP");
            //转给流程管理者


            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");

            if (worklistItem != null)
            {
                foreach (DataFieldItemEntity entity in dataFieldItemEntitys)
                {
                    if (entity.IsXMLField)
                    {
                        worklistItem.ProcessInstance.XmlFields[entity.FieldName].Value = entity.FieldValue;
                    }
                    else
                    {
                        worklistItem.ProcessInstance.DataFields[entity.FieldName].Value = entity.FieldValue;
                    }
                }

                //更新实例的DataField值

                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }
        }

        public static void SetInstanceDataFields(SourceCode.Workflow.Client.WorklistItem worklistItem, string fieldName, string fieldValue, bool isXMLField)
        {
            if (worklistItem != null)
            {
                if (isXMLField)
                {
                    worklistItem.ProcessInstance.XmlFields[fieldName].Value = fieldValue;
                }
                else
                {
                    worklistItem.ProcessInstance.DataFields[fieldName].Value = fieldValue;
                }
            }
        }

        #endregion 设置流程实例的DataField

        #region 设置流程实例的folio
        public static void SetInstanceFolio(Connection oK2Connection, string sn, string folio)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            //转给流程管理者


            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                worklistItem.ProcessInstance.Folio = folio;
                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }
        }
        public static void SetInstanceFolio(Connection oK2Connection, string sn, string folio, string managedUser, string sharedUser)
        {
            string userName = oK2Connection.User.Name;
            SourceCode.Workflow.Client.WorklistItem worklistItem = GetWorkListItemBySN(oK2Connection, sn, managedUser, sharedUser); //oK2Connection.OpenWorklistItem(sn, "ASP");
            //转给流程管理者


            oK2Connection.RevertUser();
            worklistItem.Redirect(oK2Connection.User.Name);

            worklistItem = oK2Connection.OpenWorklistItem(sn, "ASP");
            if (worklistItem != null)
            {
                worklistItem.ProcessInstance.Folio = folio;
                try
                {
                    worklistItem.ProcessInstance.Update();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //再转回当前审批人
                    worklistItem.Redirect(userName);
                    oK2Connection.ImpersonateUser(userName);
                }
            }
        }
        #endregion 设置流程实例的folio

        #region 完成当前的WorkListItem
        public static void FinishWorklistItem(SourceCode.Workflow.Client.WorklistItem wli)
        {
            foreach (SourceCode.Workflow.Client.Action action in wli.Actions)
            {
                if (action.Name == "Task Completed")
                {
                    action.Execute();
                }
            }
        }
        #endregion

        #region 窗口方法
        /// <summary>
        /// 当前审批步骤完成
        /// </summary>
        /// <param name="page"></param>
        public static void CloseApprovalWindow(System.Web.UI.Page page)
        {
            string returnValue = "true";
            page.Response.Write("<script language='javascript'>window.returnValue=\"" + returnValue + "\";</script>");
            page.Response.Write("<script language='javascript'>window.close();</script>");
            page.Response.End();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public static void ajaxCloseApprovalWindow(System.Web.UI.Control control, string returnValue)
        {
            //string returnValue = "true";
            AjaxSetWindowReturnValue(control, returnValue);
            AjaxWindowClose(control);
        }

        /// <summary>
        /// 显示信息并闭窗口
        /// </summary>
        /// <param name="page"></param>
        /// <param name="message"></param>
        public static void ShowWarningMessage(System.Web.UI.Page page, string message)
        {
            string script = "<script language='javascript'>alert('" + message + "');window.returnValue=\"true\";window.close();</script>";
            page.Response.Write(script);
            page.Response.End();
        }

        /// <summary>
        /// 关闭使用了Ajax技术的窗口
        /// </summary>
        /// <param name="control">所属对象，一般用this或this.Page或UpdatePanel的ID</param>
        public static void AjaxWindowClose(System.Web.UI.Control control)
        {
            System.Web.UI.ScriptManager.RegisterStartupScript(control, control.GetType(), "Close", "window.close();", true);

        }

        /// <summary>
        /// 从使用了Ajax技术的模态窗口返回值

        /// </summary>
        /// <param name="control">所属对象，一般为this或this.Page</param>
        /// <param name="returnValue">返回值</param>
        public static void AjaxSetWindowReturnValue(System.Web.UI.Control control, string returnValue)
        {
            string script = "window.returnValue=\"" + returnValue + "\";";
            System.Web.UI.ScriptManager.RegisterStartupScript(control, control.GetType(), "ReturnValue", script, true);

        }

        #endregion




        #region 获取用户代办事项
        /// <summary>
        /// 获取当前用户的WorkListItems
        /// </summary>
        /// <param name="oK2Connection"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static DataSet GetCurrentUserWorkListItem(Connection oK2Connection, string k2folder, string currentUser)
        {
            oK2Connection.ImpersonateUser(currentUser);

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListID", typeof(string));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(string));
            ds.Tables[0].Columns.Add("RegionName", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("ProcessName", typeof(string));
            ds.Tables[0].Columns.Add("StartDate", typeof(string));
            ds.Tables[0].Columns.Add("CurActivityName", typeof(string));
            ds.Tables[0].Columns.Add("URL", typeof(string));
            ds.Tables[0].Columns.Add("SN", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));
            ds.Tables[0].Columns.Add("OpenBy", typeof(string));
            ds.Tables[0].Columns.Add("Type", typeof(string));

            //需要考虑是否有委托事项

            SourceCode.Workflow.Client.WorklistCriteria criteria = new SourceCode.Workflow.Client.WorklistCriteria();
            criteria.Platform = "ASP";
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.And, SourceCode.Workflow.Client.WCField.ProcessFolder, SourceCode.Workflow.Client.WCCompare.Equal, k2folder);
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.And, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Me", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Me);
            //criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.Or, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Other", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Other);
            Worklist worklist = oK2Connection.OpenWorklist(criteria);

            foreach (SourceCode.Workflow.Client.WorklistItem worklistItem in worklist)
            {
                ////忽略状态为Allocated的WorkListItem
                //if (worklistItem.Status == WorklistStatus.Allocated)
                //{
                //    continue;
                //}

                //如果当前待办事项不为指定的K2文件夹，则跳过
                //if (worklistItem.ProcessInstance.Folder != k2folder)
                //{
                //    continue;
                //}

                string folio = worklistItem.ProcessInstance.Folio;
                DataRow dr = ds.Tables[0].NewRow();
                dr["WorkListID"] = worklistItem.ID;
                dr["ProcInstID"] = worklistItem.ProcessInstance.ID.ToString();
                dr["Folio"] = folio;
                dr["RegionName"] = folio.Substring(1, folio.IndexOf("]") - 1);
                dr["ProcessName"] = worklistItem.ProcessInstance.Name;
                //dr["StartDate"] = worklistItem.ProcessInstance.StartDate.ToString("yyyy-MM-dd");
                //dr["StartDate"] = worklistItem.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                dr["StartDate"] = worklistItem.ActivityInstanceDestination.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                dr["CurActivityName"] = worklistItem.ActivityInstanceDestination.Description;
                dr["URL"] = worklistItem.Data;
                dr["SN"] = worklistItem.SerialNumber;
                dr["Status"] = worklistItem.Status;
                dr["OpenBy"] = worklistItem.AllocatedUser == "" ? "" : worklistItem.AllocatedUser.Replace(@"K2:CENTALINE\", "");
                dr["Type"] = "";
                ds.Tables[0].Rows.Add(dr);

            }

            return ds;

        }
        /// <summary>
        /// 获取当前用户的WorkListItems
        /// </summary>
        /// <param name="oK2Connection"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static DataSet GetCurrentUserWorkListItem(Connection oK2Connection, string currentUser)
        {
            oK2Connection.ImpersonateUser(currentUser);

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListID", typeof(string));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("ProcessName", typeof(string));
            ds.Tables[0].Columns.Add("StartDate", typeof(string));
            ds.Tables[0].Columns.Add("CurActivityName", typeof(string));
            ds.Tables[0].Columns.Add("URL", typeof(string));
            ds.Tables[0].Columns.Add("SN", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));
            ds.Tables[0].Columns.Add("OpenBy", typeof(string));

            //需要考虑是否有委托事项

            SourceCode.Workflow.Client.WorklistCriteria criteria = new SourceCode.Workflow.Client.WorklistCriteria();
            criteria.Platform = "ASP";
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.Or, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Me", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Me);
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.Or, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Other", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Other);
            Worklist worklist = oK2Connection.OpenWorklist(criteria);

            foreach (SourceCode.Workflow.Client.WorklistItem worklistItem in worklist)
            {
                ////忽略状态为Allocated的WorkListItem
                if (worklistItem.Status == WorklistStatus.Allocated)
                {
                    continue;
                }

                oK2Connection.OpenWorklistItem(worklistItem.SerialNumber, "ASP", false);
                if (worklistItem != null)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["WorkListID"] = worklistItem.ID;
                    dr["ProcInstID"] = worklistItem.ProcessInstance.ID.ToString();
                    dr["Folio"] = worklistItem.ProcessInstance.Folio;
                    dr["ProcessName"] = worklistItem.ProcessInstance.Name;
                    dr["StartDate"] = worklistItem.ProcessInstance.StartDate.ToString();
                    dr["CurActivityName"] = worklistItem.ActivityInstanceDestination.Description;
                    dr["URL"] = worklistItem.Data;
                    dr["SN"] = worklistItem.SerialNumber;
                    dr["Status"] = worklistItem.Status;
                    dr["OpenBy"] = worklistItem.AllocatedUser;
                    ds.Tables[0].Rows.Add(dr);
                }
            }
            return ds;
        }
        /// <summary>
        /// 取得当前（有委托）的待办事宜
        /// </summary>
        /// <param name="oK2Connection"></param>
        /// <param name="currentUser"></param>
        /// <param name="dsShared"></param>
        /// <returns></returns>
        public static DataSet GetCurrentUserWorkListItem(Connection oK2Connection, string k2Folder, string currentUser, DataSet dsShared)
        {
            oK2Connection.ImpersonateUser(currentUser);

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListID", typeof(string));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(string));
            ds.Tables[0].Columns.Add("RegionName", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("ProcessName", typeof(string));
            ds.Tables[0].Columns.Add("StartDate", typeof(string));
            ds.Tables[0].Columns.Add("CurActivityName", typeof(string));
            ds.Tables[0].Columns.Add("URL", typeof(string));
            ds.Tables[0].Columns.Add("SN", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));
            ds.Tables[0].Columns.Add("OpenBy", typeof(string));
            ds.Tables[0].Columns.Add("Type", typeof(string));

            //需要考虑是否有委托事项

            SourceCode.Workflow.Client.WorklistCriteria criteria = new SourceCode.Workflow.Client.WorklistCriteria();
            criteria.Platform = "ASP";

            //criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.Or, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Me", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Me);
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.And, SourceCode.Workflow.Client.WCField.ProcessFolder, SourceCode.Workflow.Client.WCCompare.Equal, k2Folder);
            criteria.AddFilterField(SourceCode.Workflow.Client.WCLogical.And, SourceCode.Workflow.Client.WCField.WorklistItemOwner, "Other", SourceCode.Workflow.Client.WCCompare.Equal, SourceCode.Workflow.Client.WCWorklistItemOwner.Other);
            Worklist worklist = oK2Connection.OpenWorklist(criteria);

            for (int i = 0; i < dsShared.Tables[0].Rows.Count; i++)
            {
                //如果当前日期不在委托期限内的，则跳过
                DateTime start, end;
                if (!string.IsNullOrEmpty(dsShared.Tables[0].Rows[i]["StartDate"].ToString()))
                    start = DateTime.Parse(dsShared.Tables[0].Rows[i]["StartDate"].ToString());
                else
                    start = DateTime.Parse("1901-01-01");

                if (string.IsNullOrEmpty(dsShared.Tables[0].Rows[i]["EndDate"].ToString()))
                {
                    end = DateTime.Parse("9999-12-31");
                }
                else
                {
                    end = DateTime.Parse(dsShared.Tables[0].Rows[i]["EndDate"].ToString());
                }

                DateTime now = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                if (now < start || now > end)
                {
                    continue;
                }

                foreach (SourceCode.Workflow.Client.WorklistItem worklistItem in worklist)
                {
                    ////忽略状态为Allocated的WorkListItem
                    //if (worklistItem.Status == WorklistStatus.Allocated)
                    //{
                    //    continue;
                    //}

                    //如果当前待办事项不为指定的K2文件夹，则跳过
                    //if (worklistItem.ProcessInstance.Folder != k2Folder)
                    //{
                    //    continue;
                    //}

                    if (worklistItem.AllocatedUser.Replace(@"K2:CENTALINE\", "").ToLower() != dsShared.Tables[0].Rows[i]["SharedUser"].ToString().ToLower())
                    {

                        continue;
                    }


                    string folio = worklistItem.ProcessInstance.Folio;
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["WorkListID"] = worklistItem.ID;
                    dr["ProcInstID"] = worklistItem.ProcessInstance.ID.ToString();
                    dr["Folio"] = folio;
                    dr["RegionName"] = folio.Substring(1, folio.IndexOf("]") - 1);
                    dr["ProcessName"] = worklistItem.ProcessInstance.Name;
                    //dr["StartDate"] = worklistItem.ProcessInstance.StartDate.ToString("yyyy-MM-dd");
                    dr["StartDate"] = worklistItem.ProcessInstance.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
                    dr["CurActivityName"] = worklistItem.ActivityInstanceDestination.Description;
                    dr["URL"] = worklistItem.Data + (worklistItem.AllocatedUser.ToLower() == oK2Connection.User.FQN.ToLower() ? "" : "&SharedUser=" + worklistItem.AllocatedUser.Replace(@"\", @"\\"));
                    dr["SN"] = worklistItem.SerialNumber;
                    dr["Status"] = worklistItem.Status;
                    dr["OpenBy"] = worklistItem.AllocatedUser.Replace(@"K2:CENTALINE\", "");
                    dr["Type"] = "委托";
                    ds.Tables[0].Rows.Add(dr);

                }
            }

            return ds;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static DataSet GetWorkListSharedADUser(DataManager dataManager, string currentUser)
        {
            string sql = "";
            DataSet ds = null;
            if (dataManager == null)
            {
                dataManager = new DataManager();
                dataManager.OpenWithConfig();
                sql = string.Format(@"Select ForwardADUserID,ADUserID As SharedUser,StartDate,EndDate From VW_WF_GetSharedADUser Where ForwardADUserID = '{0}'", currentUser);
                ds = dataManager.SelectDataSet(sql);
                if (dataManager.IsClosed == false)
                {
                    dataManager.Close();
                }
            }
            else
            {
                sql = string.Format(@"Select ForwardADUserID,ADUserID As SharedUser,StartDate,EndDate From VW_WF_GetSharedADUser Where ForwardADUserID = '{0}'", currentUser);
                ds = dataManager.SelectDataSet(sql);
            }
            return ds;
        }
        #endregion

        #region 删除流程实例
        /// <summary>
        /// 删除流程实例
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="procInstID"></param>
        /// <returns></returns>
        public static bool DeleteProcessInstance(string k2ServerConnectionString, int procInstID)
        {
            bool ret = false;
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                ret = wms.DeleteProcessInstances(procInstID, true);
            }
            catch
            {
                ret = false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();

            }
            return ret;
        }

        /// <summary>
        /// 删除流程实例，如果有错误，将返回错误信息
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="procInstID"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public static bool DeleteProcessInstance(string k2ServerConnectionString, int procInstID, out string errMsg)
        {
            bool ret = false;
            errMsg = "";
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                ret = wms.DeleteProcessInstances(procInstID, true);
            }
            catch (Exception ex)
            {
                ret = false;
                errMsg = ex.Message;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();

            }

            return ret;
        }
        #endregion

        #region 修复流程
        /// <summary>
        /// 修复某个流程实例的错误

        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="procInstID"></param>
        /// <returns></returns>
        public static bool RepairErrorProcessInstance(string k2ServerConnectionString, int procInstID, out string errMsg)
        {
            bool ret = false;
            errMsg = "";
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                ErrorProfiles oK2ErrorProfiles = wms.GetErrorProfiles();
                ErrorLogs oK2ErrorLogs = wms.GetErrorLogs(oK2ErrorProfiles[0].ID);

                foreach (ErrorLog errLog in oK2ErrorLogs)
                {

                    if (errLog.ProcInstID == procInstID)
                    {
                        string userName = System.Configuration.ConfigurationManager.AppSettings["MngUserID"].ToString();
                        ret = wms.RetryError(procInstID, errLog.ID, userName);
                        //ret = wms.RepairError(errLog.ID, errLog.CodeItem.CodeText, errLog.ProcInstID);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                ret = false;
                errMsg = ex.Message;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();

            }

            return ret;
        }
        #endregion

        #region 取得错误流程列表
        /// <summary>
        /// 取得错误流程列表
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="floderName"></param>
        /// <returns></returns>
        public static DataSet GetErrorProcessInstanceList(string k2ServerConnectionString, string floderName)
        {

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());

            ds.Tables[0].Columns.Add("ProcInstID", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("Source", typeof(string));
            ds.Tables[0].Columns.Add("StartDate", typeof(string));
            ds.Tables[0].Columns.Add("ErrorDate", typeof(string));
            ds.Tables[0].Columns.Add("Description", typeof(string));

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                ErrorProfiles oK2ErrorProfiles = wms.GetErrorProfiles();
                ErrorLogs oK2ErrorLogs = wms.GetErrorLogs(oK2ErrorProfiles[0].ID);

                foreach (ErrorLog errLog in oK2ErrorLogs)
                {
                    string[] foldername = errLog.ProcessName.Split('\\');
                    if (foldername[0] == floderName)
                    //if (errLog.ProcessName.IndexOf(floderName) >= 0)
                    {
                        DataRow dr = ds.Tables[0].NewRow();
                        dr["ProcInstID"] = errLog.ProcInstID.ToString();
                        dr["Source"] = errLog.ErrorItemName;
                        dr["Folio"] = errLog.Folio;
                        dr["ErrorDate"] = errLog.ErrorDate;
                        dr["Description"] = errLog.Description;
                        dr["StartDate"] = errLog.StartDate;

                        ds.Tables[0].Rows.Add(dr);
                    }
                }
            }
            catch
            {
                ds = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();

            }

            return ds;
        }
        #endregion

        #region 取得被委托的用户信息
        /// <summary>
        ///  取得设置了委托的用户信息
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <returns></returns>
        public static DataSet GetOutOfOfficeUsers(string k2ServerConnectionString)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("UserID", typeof(string));
            ds.Tables[0].Columns.Add("UserName", typeof(string));
            ds.Tables[0].Columns.Add("StartDate", typeof(string));
            ds.Tables[0].Columns.Add("EnDate", typeof(string));

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);

                SourceCode.Workflow.Management.OOF.Users OOFUsers = wms.GetUsers(SourceCode.Workflow.Management.ShareType.OOF);

                foreach (SourceCode.Workflow.Management.OOF.User OOFUser in OOFUsers)
                {
                    SourceCode.Workflow.Management.WorklistShares wlss = wms.GetCurrentSharingSettings(OOFUser.FQN, SourceCode.Workflow.Management.ShareType.OOF);
                    foreach (SourceCode.Workflow.Management.WorklistShare wls in wlss)
                    {
                        DataRow dr = ds.Tables[0].NewRow();
                        dr["UserID"] = OOFUser.ID.ToString();
                        dr["UserName"] = OOFUser.FQN;
                        dr["StartDate"] = wls.StartDate.ToString();
                        dr["EndDate"] = wls.EndDate.ToString();
                        ds.Tables[0].Rows.Add(dr);
                    }

                }
            }
            catch
            {
                ds = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
            return ds;
        }
        #endregion

        #region 设置用户外出
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool SetUserOutOfOffice(string k2ServerConnectionString, string userName)
        {
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);

                return wms.SetUserStatus(userName, SourceCode.Workflow.Management.UserStatuses.OOF);
            }
            catch
            {
                return false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
        }

        #endregion 设置用户外出

        #region 取消用户外出
        /// <summary>
        /// 
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool SetUserUnOutOfOffice(string k2ServerConnectionString, string userName)
        {
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);

                return wms.SetUserStatus(userName, SourceCode.Workflow.Management.UserStatuses.Available);
            }
            catch
            {
                return false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
        }

        #endregion 设置用户外出

        #region 设置用户委托
        /// <summary>
        /// 设置用户A在某个时间段内将待办事项委托给用户B、C、D......
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="userName"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="dsForwardUser"></param>
        public static bool SetUserOutOfOffice(string k2ServerConnectionString, string userName, string startDate, string endDate, DataSet dsForwardUser, string status)
        {
            bool ret = true;
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);

                string k2Label = "K2:CENTALINE\\";
                SourceCode.Workflow.Management.WorklistShare worklistShare = new SourceCode.Workflow.Management.WorklistShare();
                //worklistShare.StartDate = DateTime.Parse(CentaUtil.Date8To10(startDate));
                //worklistShare.EndDate = DateTime.Parse(CentaUtil.Date8To10(endDate));
                if (!string.IsNullOrEmpty(startDate))
                    worklistShare.StartDate = DateTime.Parse(startDate);
                if (!string.IsNullOrEmpty(endDate))
                    worklistShare.EndDate = DateTime.Parse(endDate);
                SourceCode.Workflow.Management.WorkType workType = new SourceCode.Workflow.Management.WorkType("MyOOFWorkType");
                workType.WorklistCriteria.Platform = "ASP";

                //可以委托给多个用户

                workType.Destinations.Clear();
                for (int i = 0; i < dsForwardUser.Tables[0].Rows.Count; i++)
                {
                    workType.Destinations.Add(new SourceCode.Workflow.Management.Destination(k2Label + dsForwardUser.Tables[0].Rows[i]["ForwardADUserID"].ToString(), SourceCode.Workflow.Management.DestinationType.User));
                }

                /*WorkTypeException workTypeException = new WorkTypeException("MyOOFWorkTypeException");
                workTypeException.WorklistCriteria.Platform = "ASP";
                workTypeException.WorklistCriteria.AddFilterField(WCLogical.And, WCField.ProcessFullName, WCCompare.Equal, @"K2OOF\\K2OOFProcess");
                workTypeException.WorklistCriteria.AddFilterField(WCLogical.And, WCField.ActivityName, WCCompare.Equal, "Activity");
                workTypeException.Destinations.Add(new Destination(@"K2:K2WORKFLOW\ExceptionUser", DestinationType.User));
                workType.WorkTypeExceptions.Add(workTypeException);*/

                worklistShare.WorkTypes.Add(workType);
                ret = wms.UnShareAll(k2Label + userName);

                if (status == "OUT")
                {
                    ret = wms.ShareWorkList(k2Label + userName, worklistShare);
                    ret = ret && wms.SetUserStatus(k2Label + userName, SourceCode.Workflow.Management.UserStatuses.OOF);
                }
                if (status == "IN")
                {

                    ret = ret && wms.SetUserStatus(k2Label + userName, SourceCode.Workflow.Management.UserStatuses.Available);
                }
            }

            catch
            {
                ret = false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }

            return ret;
        }
        #endregion 设置用户委托

        #region 取得参与过流程审批人的姓名(审批结果为通过)
        public static string GetWorkflowApprovalEmpName(DataManager dataManager, string bizDataID, string applyType, string activityName)
        {
            string rtn = "";
            string sql = string.Format(@" Select Top 1 UserName From VW_WF_AuditHistory Where BizDataID='{0}'
                   And ApplyType = '{1}' And ActivityName='{2}'  And ApprovalResult Like '通过%'
                   Order by ApprovalDate Desc ", bizDataID, applyType, activityName);
            object obj = dataManager.SelectScalar(sql);
            if (obj != null)
            {
                rtn = obj.ToString();
            }
            return rtn;
        }

        #endregion 取得流程审批人

        #region 是否存在助理区域经理
        public static string CheckExistsAssistantRegionManager(DataManager dataManager, string deptID)
        {
            string rtn = "0";
            DataSet ds = dataManager.SelectDataSet(string.Format(@"Exec PR_GetUserInfoByRole '{0}','助理区域经理',0", deptID));
            if (ds.Tables[0].Rows.Count != 0)
            {
                rtn = "1";
            }
            else
            {
                rtn = "0";
            }
            return rtn;
        }
        #endregion 是否存在助理区域经理

        #region 绑定审批提交按钮的客户端校验事件
        public static void SetSubmitClientScript(System.Web.UI.WebControls.RadioButtonList rblComAuditResult, System.Web.UI.WebControls.Button btnSubmit)
        {
            string onclick = "";

            if (rblComAuditResult.Items.Count != 0) //如果有”通过”,”拒绝”,”补充资料”等操作选取项
            {

                if (rblComAuditResult.Items[0].Text == "通过")
                {
                    onclick += "; return confirm('确定进行[" + btnSubmit.Text + "]操作？');";
                }
                else
                {
                    onclick += "; if(CheckComAuditComment() && OnFormSubmit()){return confirm('确定进行['+getRadioButtonListSelectedText(document.getElementById('" + rblComAuditResult.ClientID + "'))+']操作？');}else return false;";
                }
            }
            else
            {
                onclick += "; if(OnFormSubmit()){return confirm('确定进行[" + btnSubmit.Text + "]操作？');}else return false;";
            }

            btnSubmit.Attributes.Add("onclick", onclick);
        }
        public static void SetSubmitClientScript(System.Web.UI.WebControls.RadioButtonList rblComAuditResult, System.Web.UI.WebControls.TextBox txtComAuditComment, System.Web.UI.WebControls.Button btnSubmit)
        {
            string onclick = "";

            if (rblComAuditResult.Items.Count != 0) //如果有”通过”,”拒绝”,”补充资料”等操作选取项
            {

                if (rblComAuditResult.Items.Count == 1 && rblComAuditResult.Items[0].Text.Trim() == "通过")
                {
                    onclick += "; return confirm('确定进行[" + btnSubmit.Text + "]操作？');";
                }
                else
                {
                    onclick += "; if(CheckComAuditComment('" + rblComAuditResult.ClientID + "','" + txtComAuditComment.ClientID + "') && OnFormSubmit()){return confirm('确定进行['+getRadioButtonListSelectedText(document.getElementById('" + rblComAuditResult.ClientID + "'))+']操作？');}else return false;";
                }
            }
            else
            {
                onclick += "; if(OnFormSubmit()){return confirm('确定进行[" + btnSubmit.Text + "]操作？');}else return false;";
            }

            btnSubmit.Attributes.Add("onclick", onclick);
        }
        #endregion

        /// <summary>
        /// 根据帐号获取用户的姓名

        /// </summary>
        /// <param name="adUserID"></param>
        /// <returns></returns>
        public static string GetEmpNameByADUserID(string adUserID)
        {
            string rtn = "";
            DataManager dm = new DataManager();
            try
            {
                dm.OpenWithConfig();
                BusinessEntiry emp = new BusinessEntiry(dm, "Employee", "empid");
                adUserID = adUserID.ToLower().Replace("centaline\\", "");

                if (emp.Open(String.Format("ADUserID='{0}' and delflag = 0", adUserID)))
                {
                    rtn = emp.GetPropertyValue("EmpName");
                }
                return rtn;
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (dm != null && !dm.IsClosed)
                    {
                        dm.Close();
                    }
                }
                catch
                {
                }
            }
        }

        #region 取得WorkList列表
        /// <summary>
        /// 取得某个工作流文件夹下的所有WorkListItem
        /// Added by Liuyj.ce on 2011.09.26
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="floderName"></param>
        /// <returns></returns>
        public static DataSet GetWorkList(string k2ServerConnectionString, string folderName)
        {

            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListItemID", typeof(int));
            ds.Tables[0].Columns.Add("ActInstDestID", typeof(int));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(int));
            ds.Tables[0].Columns.Add("ProcName", typeof(string));
            ds.Tables[0].Columns.Add("ActivityName", typeof(string));
            ds.Tables[0].Columns.Add("EventName", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("Destination", typeof(string));
            ds.Tables[0].Columns.Add("WorkListDate", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));

            SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter
                 wcf = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
            string filterString = folderName + "%";
            wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, filterString);

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                WorklistItems wls = wms.GetWorklistItems(wcf);

                foreach (SourceCode.Workflow.Management.WorklistItem wl in wls)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["WorkListItemID"] = wl.ID;
                    dr["ActInstDestID"] = wl.ActInstDestID;
                    dr["ProcInstID"] = wl.ProcInstID;
                    dr["ProcName"] = wl.ProcName;
                    dr["ActivityName"] = wl.ActivityName;
                    dr["EventName"] = wl.EventName;
                    dr["Folio"] = wl.Folio;
                    dr["Destination"] = wl.Destination;
                    dr["WorkListDate"] = wl.StartDate;
                    dr["Status"] = wl.Status;

                    ds.Tables[0].Rows.Add(dr);
                }
            }
            catch
            {
                ds = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
            return ds;
        }


        public static DataSet GetWorkList(string k2ServerConnectionString, string folderName, string type, string folio, string destination)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListItemID", typeof(int));
            ds.Tables[0].Columns.Add("ActInstDestID", typeof(int));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(int));
            ds.Tables[0].Columns.Add("ProcName", typeof(string));
            ds.Tables[0].Columns.Add("ActivityName", typeof(string));
            ds.Tables[0].Columns.Add("EventName", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("Destination", typeof(string));
            ds.Tables[0].Columns.Add("WorkListDate", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));

            SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter
                 wcf = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
            string filterString = folderName + "%";
            wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, filterString);
            if (type != "")
                wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + type + "%");

            wcf.AddRegularFilter(WorklistFields.Folio, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + folio + "%");

            wcf.AddRegularFilter(WorklistFields.Destination, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + destination + "%");

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                WorklistItems wls = wms.GetWorklistItems(wcf);

                foreach (SourceCode.Workflow.Management.WorklistItem wl in wls)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["WorkListItemID"] = wl.ID;
                    dr["ActInstDestID"] = wl.ActInstDestID;
                    dr["ProcInstID"] = wl.ProcInstID;
                    dr["ProcName"] = wl.ProcName;
                    dr["ActivityName"] = wl.ActivityName;
                    dr["EventName"] = wl.EventName;
                    dr["Folio"] = wl.Folio;
                    dr["Destination"] = wl.Destination;
                    dr["WorkListDate"] = wl.StartDate;
                    dr["Status"] = wl.Status;

                    ds.Tables[0].Rows.Add(dr);
                }
            }
            catch
            {
                ds = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
            return ds;
        }



        public static DataSet GetWorkList(string k2ServerConnectionString, string folderName, string type, string folio, string destination, bool checkDimission)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(new DataTable());
            ds.Tables[0].Columns.Add("WorkListItemID", typeof(int));
            ds.Tables[0].Columns.Add("ActInstDestID", typeof(int));
            ds.Tables[0].Columns.Add("ProcInstID", typeof(int));
            ds.Tables[0].Columns.Add("ProcName", typeof(string));
            ds.Tables[0].Columns.Add("ActivityName", typeof(string));
            ds.Tables[0].Columns.Add("EventName", typeof(string));
            ds.Tables[0].Columns.Add("Folio", typeof(string));
            ds.Tables[0].Columns.Add("Destination", typeof(string));
            ds.Tables[0].Columns.Add("WorkListDate", typeof(string));
            ds.Tables[0].Columns.Add("Status", typeof(string));

            SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter
                 wcf = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
            string filterString = folderName + "%";
            wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, filterString);
            if (type != "")
                wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + type + "%");

            wcf.AddRegularFilter(WorklistFields.Folio, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + folio + "%");

            wcf.AddRegularFilter(WorklistFields.Destination, SourceCode.Workflow.Management.Criteria.Comparison.Like, "%" + destination + "%");

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                WorklistItems wls = wms.GetWorklistItems(wcf);

                foreach (SourceCode.Workflow.Management.WorklistItem wl in wls)
                {
                    DataRow dr = ds.Tables[0].NewRow();
                    dr["WorkListItemID"] = wl.ID;
                    dr["ActInstDestID"] = wl.ActInstDestID;
                    dr["ProcInstID"] = wl.ProcInstID;
                    dr["ProcName"] = wl.ProcName;
                    dr["ActivityName"] = wl.ActivityName;
                    dr["EventName"] = wl.EventName;
                    dr["Folio"] = wl.Folio;
                    dr["Destination"] = wl.Destination;
                    dr["WorkListDate"] = wl.StartDate;
                    dr["Status"] = wl.Status;

                    if (checkDimission)
                    {
                        //CCHR.BusinessObject.Employee e = new Employee();
                        string empno = GetShortADUserID(dr["Destination"].ToString());
                        //if (e.CheckIsDimission(empno))
                            ds.Tables[0].Rows.Add(dr);
                    }
                    else
                        ds.Tables[0].Rows.Add(dr);
                }
            }
            catch
            {
                ds = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
            return ds;
        }

        //得到所有的待办事项
        public static DataTable GetWorkList(string k2ServerConnectionString, string folderName, bool checkDimission)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("WorkListItemID", typeof(int));
            dt.Columns.Add("ActInstDestID", typeof(int));
            dt.Columns.Add("ProcInstID", typeof(int));
            dt.Columns.Add("ProcName", typeof(string));
            dt.Columns.Add("ActivityName", typeof(string));
            dt.Columns.Add("EventName", typeof(string));
            dt.Columns.Add("Folio", typeof(string));
            dt.Columns.Add("Destination", typeof(string));
            dt.Columns.Add("WorkListDate", typeof(string));
            dt.Columns.Add("Status", typeof(string));

            SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter
                 wcf = new SourceCode.Workflow.Management.Criteria.WorklistCriteriaFilter();
            string filterString = folderName + "%";
            wcf.AddRegularFilter(WorklistFields.ProcessFullName, SourceCode.Workflow.Management.Criteria.Comparison.Like, filterString);

            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                WorklistItems wls = wms.GetWorklistItems(wcf);

                foreach (SourceCode.Workflow.Management.WorklistItem wl in wls)
                {
                    DataRow dr = dt.NewRow();
                    dr["WorkListItemID"] = wl.ID;
                    dr["ActInstDestID"] = wl.ActInstDestID;
                    dr["ProcInstID"] = wl.ProcInstID;
                    dr["ProcName"] = wl.ProcName;
                    dr["ActivityName"] = wl.ActivityName;
                    dr["EventName"] = wl.EventName;
                    dr["Folio"] = wl.Folio;
                    dr["Destination"] = wl.Destination;
                    dr["WorkListDate"] = wl.StartDate;
                    dr["Status"] = wl.Status;

                    if (checkDimission)
                    {
                        //CCHR.BusinessObject.Employee e = new Employee();
                        string empno = GetShortADUserID(dr["Destination"].ToString());
                       // if (e.CheckIsDimission(empno))
                            dt.Rows.Add(dr);
                    }
                }
            }
            catch
            {
                dt = null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
            return dt;
        }

        #endregion

        #region  转移待办事项
        /// <summary>
        /// 转移待事项
        /// added by liuyj.ce on 2011.09.26
        /// </summary>
        /// <param name="k2ServerConnectionString"></param>
        /// <param name="fromUserName"></param>
        /// <param name="toUserName"></param>
        /// <param name="procInstID"></param>
        /// <param name="actInstDestID"></param>
        /// <param name="worklistItemID"></param>
        /// <returns></returns>
        public static bool RedirectWorkListItem(string k2ServerConnectionString, string fromUserName, string toUserName, int procInstID, int actInstDestID, int worklistItemID)
        {
            bool rtn = false;
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(k2ServerConnectionString);
                rtn = wms.RedirectWorklistItem(fromUserName, toUserName, procInstID, actInstDestID, worklistItemID);

            }
            catch
            {
                rtn = false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();

            }
            return rtn;
        }
        /// <summary>
        /// 将离职人员的待办实现委托给离职交接人
        /// </summary>
        /// <param name="k2FolderName"></param>
        /// <param name="fromUserNmae"></param>
        /// <param name="toUserName"></param>
        public static void RedirectWorkListItem(string k2FolderName, string fromUserNmae, string toUserName)
        {
            DataManager dataManager = new DataManager();
            try
            {
                string K2ConnString = System.Configuration.ConfigurationManager.AppSettings["K2ConnectionString"];
                if (dataManager.IsClosed)
                {
                    dataManager.Open(K2ConnString);
                }
                string sql = string.Format(" EXEC PR_WF_RedirectUserWorklistItems '{0}','{1}','{2}','','' ", k2FolderName, fromUserNmae, toUserName);
                dataManager.Execute(sql);
            }
            finally
            {
                if (dataManager != null && dataManager.IsOpen)
                    dataManager.Close();
            }

        }
        #endregion 转移待办事项

        #region 流程步骤跳转
        public static bool GotoActivity(int procInstID, string toActName)
        {
            bool ret = false;
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(GetK2ManagementServerConnectionString());
                ret = wms.GotoActivity(procInstID, toActName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                ret = false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }

            return ret;
        }

        public static bool GotoActivity(int procInstID, string toActName,string remark)
        {
            bool ret = false;
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(GetK2ManagementServerConnectionString());
                ret = wms.GotoActivity(procInstID, toActName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                ret = false;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }

            return ret;
        }


        public static bool GotoActivityNew(int procInstID, string toActName)
        {
            DataManager dataManager = new DataManager();
            DataSet ds;
            bool result = false;
            try
            {
                string K2ConnString = System.Configuration.ConfigurationManager.AppSettings["K2ConnectionString"];
                if (dataManager.IsClosed)
                {
                    dataManager.Open(K2ConnString);
                }
                string sql = string.Format(" EXEC PR_WF_GetWorklistItemsByProcInstID '{0}'", procInstID);//获取K2流程信息
                ds = dataManager.SelectDataSet(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Connection oK2Connection = GetK2Connection();
                    oK2Connection.ImpersonateUser(ds.Tables[0].Rows[0]["OpenBy"].ToString());
                    string sn = ds.Tables[0].Rows[0]["SN"].ToString().Split('&')[0];
                    SourceCode.Workflow.Client.WorklistItem wi = GetWorkListItemBySN1(oK2Connection, sn);
                    wi.GotoActivity(toActName);
                    oK2Connection.Close();
                    result = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (dataManager != null && dataManager.IsOpen)
                    dataManager.Close();
            }
            return result;
        }
        #endregion 流程步骤跳转

        #region 获取k2流程实例
        public static ProcessInstances GetProcessInstances(int procInstID)
        {
            WorkflowManagementServer wms = new WorkflowManagementServer();
            try
            {
                wms.Open(GetK2ManagementServerConnectionString());
                return wms.GetProcessInstances(procInstID);
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                wms.Connection.Close();
                wms.Connection.Dispose();
            }
        }
        #endregion

    }


    [Serializable]
    public class DataFieldItemEntity
    {
        #region 构造函数

        public DataFieldItemEntity()
        {
        }
        public DataFieldItemEntity(string fieldName, string fieldValue, bool isXmlField = false)
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
            this.IsXMLField = isXmlField;
        }
        #endregion

        private string _FieldName;

        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }

        private string _FieldValue;

        public string FieldValue
        {
            get { return _FieldValue; }
            set { _FieldValue = value; }
        }

        private bool _IsXMLField;

        public bool IsXMLField
        {
            get { return _IsXMLField; }
            set { _IsXMLField = value; }
        }
    }
}
