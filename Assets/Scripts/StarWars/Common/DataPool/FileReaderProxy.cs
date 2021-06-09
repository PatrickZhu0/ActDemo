using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace StarWars
{
    public delegate byte[] delegate_ReadFile(string path);

    public static class FileReaderProxy
    {
        private static delegate_ReadFile handlerReadFile;

        public static MemoryStream ReadFileAsMemoryStream(string filePath)
        {
            try
            {
                byte[] buffer = ReadFileAsArray(filePath);
                if (buffer == null)
                {
                    //LogSystem.Debug("Err ReadFileAsMemoryStream failed:{0}\n", filePath);
                    return null;
                }
                return new MemoryStream(buffer);
            }
            catch (Exception e)
            {
                //LogSystem.Debug("Exception:{0}\n", e.Message);
                Helper.LogCallStack();
                return null;
            }
        }

        public static byte[] ReadFileAsArray(string filePath)
        {
            byte[] buffer = null;
            try
            {
                if (handlerReadFile != null)
                {
                    buffer = handlerReadFile(filePath);
                }
                else
                {
                    //LogSystem.Debug("ReadFileByEngine handler have not register: {0}", filePath);
                }
            }
            catch (Exception e)
            {
                //LogSystem.Debug("Exception:{0}\n", e.Message);
                Helper.LogCallStack();
                return null;
            }
            return buffer;
        }

        public static bool Exists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void RegisterReadFileHandler(delegate_ReadFile handler)
        {
            handlerReadFile = handler;
        }

    }
}
