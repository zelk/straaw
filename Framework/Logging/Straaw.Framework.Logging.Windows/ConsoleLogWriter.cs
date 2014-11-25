using System;
using Straaw.Framework.Logging;

namespace Straaw.Framework.Logging.Windows
{
	public class ConsoleLogWriter : LogWriter
	{
		public ConsoleLogWriter(LogManager logManager) : base(logManager)
		{
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			Console.WriteLine(textLine);
		}

	}
}
