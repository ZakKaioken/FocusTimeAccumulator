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
            string now = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");

            string path = Directory.GetCurrentDirectory() + "\\CrashDump-" + now + ".txt";
            File.WriteAllText(path, ParamMessage);
        }
    }
}
