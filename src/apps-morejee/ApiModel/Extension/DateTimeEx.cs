using System;

namespace ApiModel.Extension
{
    public static class DateTimeEx
    {
        /// <summary>
        /// 当下时间对应的Unix时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static long ReferUnixTimestampFromDateTime(this DateTime date)
        {
            long unixTimestamp = date.Ticks - new DateTime(1970, 1, 1).Ticks;
            unixTimestamp /= TimeSpan.TicksPerSecond;
            return unixTimestamp;
        }
    }
}
