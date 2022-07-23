using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusTimeAccumulator.IO
{
    internal class CrashDump
    {
        public static void Dump( Exception error )
        {
            string now = DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss");
			string path = Program.settings.errorLogPath.Replace( "{time}", now );
			SaveData.Save( path, error.ToString( ) );
		}
    }
}
