using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Top.Api;

namespace DingTalk.Api.Response
{
    /// <summary>
    /// OapiAttendanceGetleavestatusResponse.
    /// </summary>
    public class OapiAttendanceGetleavestatusResponse : DingTalkResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("errcode")]
        public long Errcode { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [XmlElement("errmsg")]
        public string Errmsg { get; set; }

        /// <summary>
        /// 业务结果
        /// </summary>
        [XmlElement("result")]
        public LeaveStatusVODomain Result { get; set; }

        /// <summary>
        /// 请求是否成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }

	/// <summary>
/// LeaveStatusVODomain Data Structure.
/// </summary>
[Serializable]

public class LeaveStatusVODomain : TopObject
{
	        /// <summary>
	        /// 假期时长*100，例如用户请假时长为1天，该值就等于100
	        /// </summary>
	        [XmlElement("duration_percent")]
	        public long DurationPercent { get; set; }
	
	        /// <summary>
	        /// 请假单位：“percent_day”表示天，“percent_hour”表示小时
	        /// </summary>
	        [XmlElement("duration_unit")]
	        public string DurationUnit { get; set; }
	
	        /// <summary>
	        /// 请假结束时间，时间戳
	        /// </summary>
	        [XmlElement("end_time")]
	        public long EndTime { get; set; }
	
	        /// <summary>
	        /// 请假开始时间，时间戳
	        /// </summary>
	        [XmlElement("start_time")]
	        public long StartTime { get; set; }
	
	        /// <summary>
	        /// 用户id
	        /// </summary>
	        [XmlElement("userid")]
	        public string Userid { get; set; }
}

    }
}
