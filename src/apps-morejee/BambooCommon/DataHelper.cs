using System;
using System.Collections.Generic;
using System.Text;

namespace BambooCommon
{
    public class DataHelper
    {
        #region ParseDateTime 将字符串转为日期
        /// <summary>
        /// 将字符串转为日期
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                var time = DateTime.MinValue;
                var realTime = DateTime.TryParse(str, out time);
                if (realTime)
                    return time;
            }
            return DateTime.MinValue;
        }
        #endregion

        #region FormatDateTime 将日期格式化为文本信息
        /// <summary>
        /// 将日期格式化为文本信息
        /// </summary>
        /// <param name="time"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string FormatDateTime(DateTime? time, string format = "yyyy-MM-dd hh:mm:ss")
        {
            if (time != null)
            {
                try
                {
                    var dd = (DateTime)time;
                    return dd.ToString(format);
                }
                catch (Exception)
                { }
            }
            return string.Empty;
        }
        #endregion
    }
}
