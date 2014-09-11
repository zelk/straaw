using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Straaw.Framework.Logging.Droid
{
	public class DroidInformUserLogWriter : LogWriter
	{
		public DroidInformUserLogWriter(LogManager logManager, Func<LogEvent, string> logFormatter = null)
			: base(logManager, logFormatter)
		{
		}

		public void SetAndroidActivity(Activity activity)
		{
			_activity = activity;
		}

		protected override void WriteLine(string textLine, LogEvent logEvent)
		{
			if (_activity == null)
				return;

			_activity.RunOnUiThread(() =>
			{
			    try
			    {
                    var adBuilder = new AlertDialog.Builder(_activity);
                    adBuilder.SetTitle(string.Format("LogWriter: {0}", logEvent.Logger.LoggingType.Name));
                    adBuilder.SetMessage(textLine);
                    adBuilder.SetNegativeButton("OK", (s, e) =>
                    {
                        var alertDialog = s as AlertDialog;
                        if (alertDialog != null)
                        {
                            alertDialog.Dismiss();
                            alertDialog.Cancel();
                        }
                    });

                    adBuilder.Create().Show();
			    }
			    catch (Exception e)
			    {
                    // TODO detta måste fixas
			        // something went terribly wrong
			    }
				
			});
		}

		private Activity _activity;
	}
}