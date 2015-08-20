using System;
using Straaw.Framework.Logging;

namespace Straaw.Framework.Logging.Windows
{
	public class ConsoleLogWriter : LogWriter
	{
		public ConsoleLogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null) : base(logManager, logFormatter)
		{
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
		    var fgColor = Console.ForegroundColor;

		    if(logEvent.LogLevel.Ordinal >= 4)
                Console.ForegroundColor = ConsoleColor.Red;
            else if (logEvent.LogLevel.Ordinal >= 3)
                Console.ForegroundColor = ConsoleColor.Yellow;

			Console.WriteLine(textLine);
		    Console.ForegroundColor = fgColor;
		}

		public override void Dispose()
		{
		}
	}
}
