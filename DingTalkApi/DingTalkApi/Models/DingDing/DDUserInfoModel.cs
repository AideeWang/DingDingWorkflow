using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DingTalkApi.Models
{
    public class DDUserInfoModel: BaseDDModel
    {
        /// <summary>
        /// 员工在企业内的UserID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 手机设备号,由钉钉在安装时随机产生
        /// </summary>
        public string deviceId { get; set; }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool is_sys { get; set; }

        /// <summary>
        /// 级别，三种取值。0:非管理员 1：普通管理员 2：超级管理员
        /// </summary>
        public string sys_level { get; set; }
    }
}
