using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Program;
namespace FocusTimeAccumulator.IO
{
	internal class MessageBuilder
	{
		public static string BuildMessage(string message, DateTime time, string process, string title, TimeSpan span, TimeSpan? totalTime = null) {
			StringBuilder sb = new( message );
			sb.Replace( @"{time}", time.ToString(settings.timeStampFormat) );
			sb.Replace( @"{process}", process ).Replace( @"{title}", title );
			sb.Replace( @"{span}", span.ToString( ) );
			sb.Replace( @"{totalTime}", totalTime.ToString( ) );

			return sb.ToString();
		}
	}
}
