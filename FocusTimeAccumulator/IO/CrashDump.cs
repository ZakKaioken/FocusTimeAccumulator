using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator.IO
{
    internal class CrashDump
    {
        public static void Dump(string ParamMessage)
        {
            DateTime now = DateTime.Now;

            // Instad of using DateTime.Now.ToString(),
            // We must Build our own timestamp because \ and : are bad in file names.
            StringBuilder sb = new StringBuilder();
            sb.Append("D");
            sb.Append(now.Day);
            sb.Append("-");
            sb.Append(now.Month);
            sb.Append("-");
            sb.Append(now.Year);
            sb.Append("-T");
            sb.Append(now.Hour);
            sb.Append("-");
            sb.Append(now.Minute);
            sb.Append("-");
            sb.Append(now.Second);

            string path = Directory.GetCurrentDirectory() + "\\CrashDump-" + sb.ToString() + ".txt";
            File.WriteAllText(path, ParamMessage);
        }
    }
}
