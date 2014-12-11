using System;

namespace Straaw.Framework.Logging
{
	public class LogEvent
	{
		public LogEvent(long timeStamp, Logger logger, Type loggingType, string memberName, string fileName, int lineNumber, LogLevel logLevel, string appName, string formatString, params object[] args)
		{
			TimeStamp = timeStamp;
			ScopeTime = null;
			Logger = logger;
			MemberName = memberName;
			FileName = fileName;
			LineNumber = lineNumber;
			LogLevel = logLevel;
			AppName = appName;
			Message = string.Format(formatString, args);
		}

		internal LogEvent(LogEvent sourceLogEvent, long endTimeStamp)
		{
			TimeStamp = endTimeStamp;
			ScopeTime = endTimeStamp - sourceLogEvent.TimeStamp;
			Logger = sourceLogEvent.Logger;
			MemberName = sourceLogEvent.MemberName;
			FileName = sourceLogEvent.FileName;
			LineNumber = sourceLogEvent.LineNumber;
			LogLevel = sourceLogEvent.LogLevel;
			AppName = sourceLogEvent.AppName;
			Message = sourceLogEvent.Message;
		}

		public long TimeStamp { get; private set; }
		public long? ScopeTime { get; private set; }
		public Logger Logger { get; private set; }
		public string MemberName { get; private set; }
		public string FileName { get; private set; }
		public int LineNumber { get; private set; }
		public LogLevel LogLevel { get; private set; }
		public string AppName { get; private set; }
		public string Message { get; private set; }
	}
}
