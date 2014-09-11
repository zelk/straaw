using System;
using Android.Text;
using System.Text;

namespace Straaw.Framework.Ui.Droid
{
	public class DelegatedInputFilter : Java.Lang.Object, IInputFilter
	{
		public DelegatedInputFilter(Func<string, bool> onChangeDelegate)
			: base()
		{
			_onChangeDelegate = onChangeDelegate;
		}

		public Java.Lang.ICharSequence FilterFormatted(Java.Lang.ICharSequence source, int start, int end, global::Android.Text.ISpanned dest, int dstart, int dend)
		{
			var s = source.ToString();
			var d = dest.ToString();
			var proposal = d.Remove(dstart, dend - dstart).Insert(dstart, s.Substring(start, end - start));
			if (_onChangeDelegate(proposal))
				return null;
			else
			{
				var r = new Java.Lang.String(d.Substring(dstart, dend - dstart));
				if (source is ISpanned)
				{
					// This spannable thing here, is needed to copy information about which part of
					// the string is being under composition by the keyboard (or other spans). A
					// spannable string has support for extra information to the string, besides
					// its characters and that information must not be lost!
					var ssb = new SpannableString(r);
					var spannableEnd = (end <= ssb.Length()) ? end : ssb.Length();
					global::Android.Text.TextUtils.CopySpansFrom((ISpanned)source, start, spannableEnd, null, ssb, 0);
					return ssb;
				}
				else
					return r;
			}
		}

		private readonly Func<string, bool> _onChangeDelegate;
	}
}
