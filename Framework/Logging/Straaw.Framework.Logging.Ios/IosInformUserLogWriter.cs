using System;
using MonoTouch.UIKit;

namespace Straaw.Framework.Logging.Ios
{
	public class IosInformUserLogWriter : LogWriter
	{
		public IosInformUserLogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null)
			: base(logManager, logFormatter)
		{
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			var title = string.Format("LogWriter: {0}", logEvent.Logger.LoggingType.Name);
			var message = textLine;
			new UIAlertView(title, message, null, "OK", null).Show();
		}
	}
}