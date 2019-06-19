using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DingTalkApi.Models
{
    public enum LeaveType
    {
        
        /// <summary>
        /// 年假
        /// </summary>
        [Description("年假")]
        AnnualLeave,
        /// <summary>
        /// 事假
        /// </summary>
        [Description("事假")]
        CompassionateLeave,
        /// <summary>
        /// 病假
        /// </summary>
        [Description("病假")]
        SickLeave,
        /// <summary>
        /// 调休
        /// </summary>
        [Description("调休")]
        BreakOff,
        /// <summary>
        /// 产假
        /// </summary>
        [Description("产假")]
        MaternityLeave,
        /// <summary>
        /// 陪产假
        /// </summary>
        [Description("陪产假")]
        PaternityLeave,
        /// <summary>
        /// 婚假
        /// </summary>
        [Description("婚假")]
        MarriageLeave,
        /// <summary>
        /// 例假
        /// </summary>
        [Description("例假")]
        OfficialHoliday,
        /// <summary>
        /// 丧假
        /// </summary>
        [Description("丧假")]
        FuneralLeave

    }
}