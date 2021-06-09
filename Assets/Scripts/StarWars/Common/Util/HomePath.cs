
/**
 * @file HomePath.cs
 * @brief 获取游戏工作目录
 *
 * @author 
 * @version 
 * @date 
 */

using System;
using System.IO;
using System.Reflection;

namespace StarWars
{
    /// <summary>
    /// 获取游戏工作目录
    /// </summary>
    public class HomePath
    {
        public static string CurHomePath
        {
            get { return m_HomePath; }
            set { m_HomePath = value; }
        }
        public static void InitHomePath()
        {
            string tmpPath = Assembly.GetExecutingAssembly().Location;
            m_HomePath = tmpPath.Substring(0, tmpPath.LastIndexOfAny(new char[] { '\\', '/' }));
        }
        public static string GetAbsolutePath(string path)
        {
            return Path.Combine(m_HomePath, path);
        }

        private static string m_HomePath = "";
    }
}

