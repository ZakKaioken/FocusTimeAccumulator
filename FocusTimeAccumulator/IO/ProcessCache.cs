using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator.IO
{
    internal class ProcessCache
    {
        private static Dictionary<string, Process> processCache = new Dictionary<string, Process>();

        /// <summary>
        /// Store a process in the cache, if the storage is succesful return TRUE.
        /// </summary>
        public static bool Cache(string appName, Process proc)
        {
            if (!processCache.ContainsKey(appName) && proc != null)
            {
                processCache.Add(appName, proc);
                return true;
            }

            return false;
        }

        public static string GetProductName(string appName)
        {
            string result = "NULL";
            
            if (processCache.ContainsKey(appName))
            {
                if (processCache[appName].HasExited)
                {
                    return "NULL";
                }

                result = processCache[appName].MainModule?.FileVersionInfo.ProductName ?? "NULL";
            }

            return result;
        }

        public static string GetProductDescription(string appName)
        {
            string result = "NULL";

            if (processCache.ContainsKey(appName))
            {
                if (processCache[appName].HasExited)
                {
                    return "NULL";
                }

                result = processCache[appName].MainModule?.FileVersionInfo.FileDescription ?? "NULL";
            }

            return result;
        }
    }
}