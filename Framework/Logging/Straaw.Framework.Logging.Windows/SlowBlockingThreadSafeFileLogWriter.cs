using System;
using System.IO;

namespace Straaw.Framework.Logging.Windows
{
	public class SlowBlockingThreadSafeFileLogWriter : LogWriter
	{
		public SlowBlockingThreadSafeFileLogWriter(string logFilePath, LogManager logManager, Func<LogEvent, string> logFormatter = null) : base(logManager, logFormatter)
		{
		    var fileInfo = new FileInfo(logFilePath);

			if (!fileInfo.Exists)
			{
			    if (!fileInfo.Directory.Exists)
			    {
			        fileInfo.Directory.Create();
			    }

			    _streamWriter = fileInfo.CreateText();
			}
			else
			{
			    _streamWriter = fileInfo.AppendText();
			}	

		}

		public override void Dispose()
		{
			_streamWriter.Close();
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			lock (this)
			{
				_streamWriter.WriteLine(textLine);
//				var count = Interlocked.Increment(ref _writeLineCallCount);
//				if (count%_lineCountPerFlush == 0)
//				{
					_streamWriter.Flush();
//				}
			}
		}

		readonly StreamWriter _streamWriter;
//		readonly long _lineCountPerFlush = 10;
		long _writeLineCallCount = 0;
	}
}
