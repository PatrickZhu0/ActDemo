
/**
 * @file LogSystem.cs
 * @brief 日志系统，考虑使用Log4Net，UWA等暂时用LogSystem.Debug代替，控制台输出
 * @author 
 * @version 
 * @date 
 */

using System;

namespace StarWars
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum Log_Type
    {
        LT_Debug,
        LT_Info,
        LT_Warn,
        LT_Error,
        LT_Assert,
    }
    public delegate void LogSystemOutputDelegation(Log_Type type, string msg);

    /// <summary>
    /// 日志系统
    /// </summary>
    public class LogSystem
    {
        public static LogSystemOutputDelegation OnOutput;
        public static void Debug(string format, params object[] args)
        {
            string str = string.Format("[Debug]:" + format, args);
            Output(Log_Type.LT_Debug, str);
        }
        public static void Info(string format, params object[] args)
        {
            string str = string.Format("[Info]:" + format, args);
            Output(Log_Type.LT_Info, str);
        }
        public static void Warn(string format, params object[] args)
        {
            string str = string.Format("[Warn]:" + format, args);
            Output(Log_Type.LT_Warn, str);
        }
        public static void Error(string format, params object[] args)
        {
            string str = string.Format("[Error]:" + format, args);
            Output(Log_Type.LT_Error, str);
        }
        public static void Assert(bool check, string format, params object[] args)
        {
            if (!check)
            {
                string str = string.Format("[Assert]:" + format, args);
                Output(Log_Type.LT_Assert, str);
            }
        }

        private static void Output(Log_Type type, string msg)
        {
            if (null != OnOutput)
            {
                OnOutput(type, msg);
            }
            switch (type)
            {
                case Log_Type.LT_Debug:
                    UnityEngine.Debug.Log(msg);
                    break;
                case Log_Type.LT_Error:
                    UnityEngine.Debug.LogError(msg);
                    break;
                case Log_Type.LT_Warn:
                    UnityEngine.Debug.LogWarning(msg);
                    break;
                default:
                    UnityEngine.Debug.Log(msg);
                    break;
            }
        }
    }
}
