using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Straaw.Framework.Logging
{
	public class LoggerScope : IDisposable
	{
		internal LoggerScope(Logger logger, LogEvent logEvent)
		{
			_logger = logger;
			_logEvent = logEvent;
		}
		public void Dispose()
		{
			var logEvent = new LogEvent(_logEvent, Logger.Stopwatch.ElapsedMilliseconds);
			_logger.Log(logEvent);
		}

		private readonly Logger _logger;
		private readonly LogEvent _logEvent;
	}
}
