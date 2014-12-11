using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Straaw.Framework.Logging
{
	// TODO: Implement a console logwriter with color support!!!
	// TODO: Implement LogWriters for App Annie/TestFlight/UdpSocket/...
	// TODO: Move the documentation to the WIKI

/*
	LogWriter
		Implemented per platform/log target to WriteLine to an output target.
		Has a Dictionary<Type, LogLevel> to target specific classes
		Has a Dictionary<string, LogLevel> to target namespaces - string.StartsWith and always recursive
		Takes a delegate for formatting the log text, which has a default implementation.
		Can be asked which LogLevel is needed for this LogWriter in relation to a certain Logger
		Keeps a cache of Dictionary<Logger, LogLevel>
		Clears its cache when loglevels are altered
	LogManager
		Implemented once (sealed)
		Keeps a list of LogWriters (must be created before the first Logger is created)
		Gets calls from Loggers and passes them on to the correct LogWriters
		Creates Loggers and passes them an appropriate LogLevel, based on interested LogWriters
		Keeps a list of cached Loggers
		Makes sure that each Logger has the correct LogLevel at all times in a thread safe manner.
	Logger
		Implemented once (sealed)
		Only mutable data is the LogLevel
		Gets a LogLevel to adhere to, which can get updated over time
		Implements all the public fast-opt-out logging methods
		Calls the LogManager when logging should be done
*/

	// TODO: Several logging features:

	// Remove DateTime.Now from default LogWriter and move TimeStamp into the LoggerScope class instead
	// Write unit tests for revealing threading issues
	// Look at log4j/log4net support for user session logging.	// Thread local stack of loggers or something...	// I might be optimizing if I change the LogLevel to be an enum.	// Garbage Collection might suffer less since I won't be changing	// any member references.	// Add thread locks so that I always write one fill line?	// I cannot have locks obviously as that would hurt performance	// too much but I guess that I could have some queue system or something.	// Add support for including and excluding timing scopes.	// Optimize by creating a LogEvent object and keep it around - one for each logger.	// But it would cause threading issues, no? I can't be sure that a logger is not	// used by several threads. So, that would mean that I would need a locking	// meachanism to reuse those LogEvent objects and that's not am optimization.	// I could have a pool of LogEvents to reuse.
	public sealed class Logger
	{
		public Type LoggingType { get { return _loggingType; }}
		public string LoggingTypeName { get { return _loggingTypeName; } }

		public void Verbose
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.Verbose;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public void Debug
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.Debug;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public void Info
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.Info;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public void Warning
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.Warning;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public void Error
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.Error;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public void Fatal
		(
			string formatString,
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			var logLevel = LogLevel.FatalError;
			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
		}

		public LoggerScope Scope
		(
			LogLevel logLevel,
			string formatString = "",
			object arg0 = null,
			object arg1 = null,
			object arg2 = null,
			object arg3 = null,
			object arg4 = null,
			object arg5 = null,
			object arg6 = null,
			object arg7 = null,
			object arg8 = null,
			object arg9 = null,
			string overflowGuard = UniqueString,
			[CallerMemberName] string memberName = "",
			[CallerFilePath] string sourceFilePath = "",
			[CallerLineNumber] int sourceLineNumber = 0
		)
		{
			if (overflowGuard != UniqueString)
				throw new OverflowException("Only 10 arguments are supported");

			if (_minimumLogLevel.Ordinal > logLevel.Ordinal)
				return null;

			var logEvent = new LogEvent
			(
				Stopwatch.ElapsedMilliseconds,
				this,
				_loggingType,
				memberName,
				sourceFilePath,
				sourceLineNumber,
				logLevel,
				_logManager.AppName,
				formatString,
				arg0,
				arg1,
				arg2,
				arg3,
				arg4,
				arg5,
				arg6,
				arg7,
				arg8,
				arg9
			);
			_logManager.Log(logEvent);
			return new LoggerScope(this, logEvent);
		}

		internal void Log(LogEvent logEvent)
		{
			if (_minimumLogLevel.Ordinal > logEvent.LogLevel.Ordinal)
				return;

			_logManager.Log(logEvent);
		}

		static Logger()
		{
			Stopwatch = new Stopwatch();
			Stopwatch.Start();
		}

		internal void SetMinimumLogLevel(LogLevel logLevel)
		{
			_minimumLogLevel = logLevel;
		}

		internal Logger(LogManager logManager, Type loggingType)
		{
			_logManager = logManager;
			_loggingType = loggingType;
			_loggingTypeName = loggingType.Name;
			_minimumLogLevel = LogLevel.None;
		}

		internal static readonly Stopwatch Stopwatch;
		private const string UniqueString = "57f93633-b10e-454c-ab11-133168817a7b";

		private readonly LogManager _logManager;
		private readonly Type _loggingType;
		private readonly string _loggingTypeName;
		private LogLevel _minimumLogLevel;
	}

}
