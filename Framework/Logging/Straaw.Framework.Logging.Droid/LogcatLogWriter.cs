using System;
using Android.Util;

namespace Straaw.Framework.Logging.Droid
{
	public class LogcatLogWriter : LogWriter
	{
		public LogcatLogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null)
			: base(logManager, logFormatter)
		{
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			if (logEvent.LogLevel == LogLevel.FatalError)
			{
				Android.Util.Log.Error(logEvent.AppName, textLine);
				return;
			}
			if (logEvent.LogLevel == LogLevel.Error)
			{
				Android.Util.Log.Error(logEvent.AppName, textLine);
				return;
			}
			if (logEvent.LogLevel == LogLevel.Warning)
			{
				Android.Util.Log.Warn(logEvent.AppName, textLine);
				return;
			}
			if (logEvent.LogLevel == LogLevel.Info)
			{
				Android.Util.Log.Info(logEvent.AppName, textLine);
				return;
			}
			if (logEvent.LogLevel == LogLevel.Debug)
			{
				Android.Util.Log.Debug(logEvent.AppName, textLine);
				return;
			}
			if (logEvent.LogLevel == LogLevel.Verbose)
			{
				Android.Util.Log.Verbose(logEvent.AppName, textLine);
				return;
			}
		}
	}
}
