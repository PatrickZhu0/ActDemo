/**
 * @file Helper.cs
 * @brief 帮助类
 *
 * @author 
 * @version 
 * @date
 */

using System;
using System.Text;
using System.Diagnostics;

namespace StarWars
{
    public sealed class Helper
    {
        public static string BinToHex(byte[] bytes)
        {
            return BinToHex(bytes, 0);
        }
        public static string BinToHex(byte[] bytes, int start)
        {
            return BinToHex(bytes, start, bytes.Length - start);
        }
        public static string BinToHex(byte[] bytes, int start, int count)
        {
            if (start < 0 || count <= 0 || start + count > bytes.Length)
                return "";
            StringBuilder sb = new StringBuilder(count * 4);
            for (int ix = 0; ix < count; ++ix)
            {
                sb.AppendFormat("{0,2:X2}", bytes[ix + start]);
                if ((ix + 1) % 16 == 0)
                    sb.AppendLine();
                else
                    sb.Append(' ');
            }
            return sb.ToString();
        }

        public static bool StringIsNullOrEmpty(string str)
        {
            if (str == null || str == "")
                return true;
            return false;
        }

        public static void LogCallStack()
        {
            StackTrace trace = new StackTrace();
            //LogSystem.Debug("LogCallStack:\n{0}\n", trace.ToString());
        }

        // 防君子不防小人的简单加密
        public static void Xor(byte[] buffer, byte[] xor)
        {
            int len = buffer.Length;
            int xlen = xor.Length;
            int xi = 0;
            for (int i = 0; i < len; ++i)
            {
                buffer[i] = (byte)(buffer[i] ^ xor[xi]);
                xi = (xi + 1) % xlen;
            }
        }

        public sealed class Random
        {
            static public int Next()
            {
                return Instance.Next(100);
            }
            static public int Next(int max)
            {
                return Instance.Next(max);
            }
            static public int Next(int min, int max)
            {
                return Instance.Next(min, max);
            }
            static public float NextFloat()
            {
                return (float)Instance.NextDouble();
            }

            static private System.Random Instance
            {
                get
                {
                    if (null == rand)
                    {
                        rand = new System.Random();
                    }
                    return rand;
                }
            }
            [ThreadStatic]
            static private System.Random rand = null;
        }
    }
}

