using System;
using System.Diagnostics;

//TODO:实现日志记录类
namespace BambooCommon
{
    /// <summary>
    /// 系统日志记录类
    /// 待完善
    /// </summary>
    public class Logger
    {
        /************************ publi method ********************************/

        #region static LogError 记录错误日志
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="strErrMsg"></param>
        public static void LogError(string strErrMsg)
        {
            Console.WriteLine(string.Format("...........................Sorry,you app capture an error,detail:{0}", strErrMsg));
        }
        #endregion

        #region static LogError 记录错误日志
        /// <summary>
        /// 记录错误日志
        /// </summary>
        /// <param name="strErrMsg"></param>
        /// <param name="ex"></param>
        public static void LogError(string strErrMsg, Exception ex)
        {
            Console.WriteLine(string.Format("...........................Sorry,you app capture an error at \"{0}\",detail:{1}", strErrMsg, ex));
        }
        #endregion

        #region static LogDebug 记录调试日志
        /// <summary>
        /// 记录调试日志
        /// </summary>
        /// <param name="strDebug"></param>
        public static void LogDebug(string strDebug)
        {

        }
        #endregion

        #region static ConsoleTrace 控制台输出日志
        /// <summary>
        /// 控制台输出日志
        /// </summary>
        /// <param name="strTrace">日志信息</param>
        public static void ConsoleTrace(string strTrace)
        {

        }
        #endregion
    }
}
