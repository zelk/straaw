using System;
using System.Collections.Generic;

namespace Straaw.Framework.Logging
{
	public sealed class LogManager : IDisposable
	{
		public LogManager(string appName)
		{
			_loggers = new Dictionary<Type, Logger>();
			_logWriters = new List<LogWriter>();
			_logWritersPerLogger = new Dictionary<Logger, List<LogWriter>>();

			AppName = appName;
		}

		public Logger G(Type loggingType)
		{
			return GetLogger(loggingType);
		}

		public Logger GetLogger(Type loggingType)
		{
			lock(_loggers)
			{
				Logger cachedLogger;
				if (_loggers.TryGetValue(loggingType, out cachedLogger))
				{
					return cachedLogger;
				}
			}

			return CreateLogger(loggingType);
		}

		public void AddLogWriter(LogWriter logWriter)
		{
			if (logWriter == null)
				throw new ArgumentNullException("LogWriter must not be null");

			lock (_logWriters)
			{
				if (_logWriters.Contains(logWriter))
					throw new ArgumentException("LogWriter has already been added");

				lock (_loggers)
				{
					if (_loggers.Count != 0)
						throw new ArgumentOutOfRangeException("LogWriter cannot be added once the first Logger has been created.");
				}

				_logWriters.Add(logWriter);
			}
		}

		internal void RecalculateLogLevels(LogWriter logWriter)
		{
			lock(_logWritersPerLogger)
			{
				foreach (var logger in _logWritersPerLogger.Keys)
				{
					List<LogWriter> logWriters;
					if (!_logWritersPerLogger.TryGetValue(logger, out logWriters))
					{
						throw new Exception("Unable to find dictionary key - mutli threading issue?");
					}
					if (logWriter.GetLogLevel(logger) == LogLevel.None)
					{
						if (logWriters.Contains(logWriter))
						{
							logWriters.Remove(logWriter);
						}
					}
					else
					{
						if (!logWriters.Contains(logWriter))
						{
							logWriters.Add(logWriter);
						}
					}
				}
			}

			foreach (var logger in _loggers.Values)
			{
				logger.SetMinimumLogLevel(GetMinimumLogLevel(logger));
			}
		}

		internal void Log(LogEvent logEvent)
		{
			lock(_logWritersPerLogger)
			{
				List<LogWriter> logWriters;
				if (!_logWritersPerLogger.TryGetValue(logEvent.Logger, out logWriters))
				{
					throw new NullReferenceException("Logger is missing from LogManager.");
				}

				foreach (var logWriter in logWriters)
				{
					logWriter.Log(logEvent);
				}
			}
		}

		internal string AppName { get; private set; }

		private LogLevel GetMinimumLogLevel(Logger logger)
		{
			var minimumLogLevel = LogLevel.None;

			lock(_logWritersPerLogger)
			{
				List<LogWriter> logWriters;
				if (_logWritersPerLogger.TryGetValue(logger, out logWriters))
				{
					foreach (var logWriter in logWriters)
					{
						var logLevel = logWriter.GetLogLevel(logger);
						if (logLevel.Ordinal <= minimumLogLevel.Ordinal)
						{
							minimumLogLevel = logLevel;
						}
					}
				}
			}

			return minimumLogLevel;
		}

		private Logger CreateLogger(Type loggingType)
		{
			var newLogger =  new Logger(this, loggingType);

			lock (_logWriters)
			{
				foreach (var logWriter in _logWriters)
				{
					var logWriterLogLevel = logWriter.GetLogLevel(newLogger);
					if (logWriterLogLevel != LogLevel.None)
					{
						AddToLogWritersPerLogger(newLogger, logWriter, logWriterLogLevel);
					}
				}
			}
			newLogger.SetMinimumLogLevel(GetMinimumLogLevel(newLogger));

			lock (_loggers)
			{
				_loggers.Add(loggingType, newLogger);
			}

			return newLogger;
		}

		private void AddToLogWritersPerLogger(Logger logger, LogWriter logWriter, LogLevel logLevel)
		{
			lock(_logWritersPerLogger)
			{
				List<LogWriter> logWriters;
				if (!_logWritersPerLogger.ContainsKey(logger))
				{
					logWriters = new List<LogWriter>(1);
					_logWritersPerLogger.Add(logger, logWriters);
				}
				else
				{
					if (!_logWritersPerLogger.TryGetValue(logger, out logWriters))
					{
						throw new Exception("Unable to find dictionary key - mutli threading issue?");
					}
				}
				logWriters.Add(logWriter);
			}
		}

		public void Dispose()
		{
			foreach (LogWriter logWriter in _logWriters)
			{
				logWriter.Dispose();
			}
		}

		private readonly Dictionary<Type, Logger> _loggers;
		private readonly List<LogWriter> _logWriters;
		private readonly Dictionary<Logger, List<LogWriter>> _logWritersPerLogger;
	}
}
