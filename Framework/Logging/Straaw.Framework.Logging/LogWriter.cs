using System;
using System.Collections.Generic;

namespace Straaw.Framework.Logging
{
	public abstract class LogWriter : IDisposable
	{
		protected LogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null)
		{
			_logManager = logManager;
			_logLevelsPerPartialName = new Dictionary<string, LogLevel>();
			_logLevelsPerType = new Dictionary<Type, LogLevel>();
			_loggerLogLevelCache = new Dictionary<Logger, LogLevel>();

			if (logFormatter != null)
			{
				_logFormatter = logFormatter;
			}
			else
			{
				_logFormatter = (logEvent) =>
				{
					var className = logEvent.Logger.LoggingTypeName;
					if (className.Length > 20)
					{
						className = className.Substring(0, 20);
					}

					var memberName = logEvent.MemberName;
					if (memberName.Length > 20)
					{
						memberName = memberName.Substring(0, 20);
					}

					var message = logEvent.Message;
					if (logEvent.ScopeTime.HasValue)
					{
						message = string.Format("{0} exit after {1} ms", message, logEvent.ScopeTime.Value);
					}

					return string.Format
					(
						DefaultFormatString,
						DateTime.Now,
						logEvent.AppName,
						logEvent.FileName,
						logEvent.LineNumber,
						className,
						memberName,
						logEvent.LogLevel.Code,
						logEvent.LogLevel.ShortName,
						logEvent.LogLevel.LongName,
						message
					);
				};
			}
		}

		public void SetLogLevel(string partialClassName, LogLevel logLevel)
		{
			lock (_logLevelsPerPartialName)
			{
				if (partialClassName == null)
					partialClassName = string.Empty;

				_logLevelsPerPartialName.Remove(partialClassName);

				if (logLevel != null && logLevel != LogLevel.None)
				{
					_logLevelsPerPartialName.Add(partialClassName, logLevel);
				}
			}

			lock(_loggerLogLevelCache)
			{
				_loggerLogLevelCache.Clear();
			}

			_logManager.RecalculateLogLevels(this);
		}

		public void SetLogLevel(Type type, LogLevel logLevel)
		{
			lock(_logLevelsPerType)
			{
				if (type == null)
					throw new ArgumentNullException();

				_logLevelsPerType.Remove(type);

				if (logLevel != null && logLevel != LogLevel.None)
				{
					_logLevelsPerType.Add(type, logLevel);
				}
			}

			lock(_loggerLogLevelCache)
			{
				_loggerLogLevelCache.Clear();
			}

			_logManager.RecalculateLogLevels(this);
		}

		internal LogLevel GetLogLevel(Logger logger)
		{
			LogLevel logLevel;

			lock(_loggerLogLevelCache)
			{
				if (_loggerLogLevelCache.TryGetValue(logger, out logLevel))
				{
					return logLevel;
				}
			}

			logLevel = LogLevel.None;

			lock (_logLevelsPerType)
			{
				foreach (var type in _logLevelsPerType.Keys)
				{
					if (logger.LoggingType != type)
					{
						continue;
					}

					LogLevel newLogLevel;
					if (!_logLevelsPerType.TryGetValue(type, out newLogLevel))
					{
						throw new Exception("Unable to find dictionarty key - mutli threading issue?");
					}

					if (newLogLevel.Ordinal < logLevel.Ordinal)
					{
						logLevel = newLogLevel;
					}
				}
			}

			lock (_logLevelsPerPartialName)
			{
				var loggingFullName = logger.LoggingType.FullName;
				foreach (var partialName in _logLevelsPerPartialName.Keys)
				{
					if (!loggingFullName.StartsWith(partialName))
					{
						continue;
					}

					LogLevel newLogLevel;
					if (!_logLevelsPerPartialName.TryGetValue(partialName, out newLogLevel))
					{
						throw new Exception("Unable to find dictionarty key - multi threading issue?");
					}

					if (newLogLevel.Ordinal < logLevel.Ordinal)
					{
						logLevel = newLogLevel;
					}
				}
			}

			_loggerLogLevelCache.Add(logger, logLevel);

			return logLevel;
		}

		internal void Log(LogEvent logEvent)
		{
			if (GetLogLevel(logEvent.Logger).Ordinal <= logEvent.LogLevel.Ordinal)
			{
				WriteLine(_logFormatter(logEvent), logEvent);
			}
		}

		protected abstract void WriteLine(string textLine, LogEvent logEvent);

//		private const string DefaultFormatString = "{0,-23:yyyy-MM-dd HH:mm:ss.FFF} {7} {1} {4,20}.{5,-20} line {3,4}: {9}";
		private const string DefaultFormatString = "{7} {1} {4,20}.{5,-20} line {3,4}: {9}";

		private readonly LogManager _logManager;
		private readonly Dictionary <string, LogLevel> _logLevelsPerPartialName;
		private readonly Dictionary <Type, LogLevel> _logLevelsPerType;
		private readonly Dictionary <Logger, LogLevel> _loggerLogLevelCache;
		private readonly Func<LogEvent, string> _logFormatter;

		public abstract void Dispose();
	}
}
