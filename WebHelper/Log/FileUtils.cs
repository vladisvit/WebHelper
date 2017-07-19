using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace WebHelper.Log
{
    public class FileUtils
    {
        public static readonly object lockObject = new object();

        public static void CreateDirectoryIfNotExists(string path)
        {
            // double-checked locking optimization
            if (!Directory.Exists(path))
            {
                lock (lockObject)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
        }

        public static StreamWriter GetOrCreateFile(string path)
        {
            //double-checked locking optimization
            if (!File.Exists(path))
            {
                lock (lockObject)
                {
                    if (!File.Exists(path))
                    {
                        return File.CreateText(path);
                    }
                }
            }

            return File.AppendText(path);
        }
    }
}
