using System;

namespace Straaw.Framework.Logging.Ios
{
	public class IosLogWriter : LogWriter
	{
		public IosLogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null)
			: base(logManager, logFormatter)
		{
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			Console.WriteLine(textLine);
		}
	}
}

