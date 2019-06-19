using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DingTalkApi.Models
{
    public class Department
    {
        public string Id { get; set; }
        public string DeptName { get; set; }
        public string parentid { get; set; }
    }
}