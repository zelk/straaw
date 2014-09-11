using System;
using Android.App;
using Android.OS;
using Android.Widget;
using Android.Content;
using Straaw.Framework.Logging;
using System.Reflection;

namespace Straaw.Framework.Ui.Droid
{
	public class StraawEditText : EditText
	{
		static private readonly Logger Log = L.O.G(typeof(StraawEditText));
		public event Action<StraawEditText, string> TextDidChange;
		private string _stringBefore;
		private int _selectionStart;
		private int _selectionEnd;

		public StraawEditText(IntPtr javaReference, global::Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer) { AddListeners(); }
		public StraawEditText(Context context, global::Android.Util.IAttributeSet attrs) : base(context, attrs) { AddListeners(); }
		public StraawEditText(Context context, global::Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { AddListeners(); }
		public StraawEditText(Context context) : base(context) { AddListeners(); }
		public void AddListeners()
		{
			this.BeforeTextChanged += (s, e) =>
			{
				_stringBefore = e.Text.ToString();
				_selectionStart = SelectionStart;
				_selectionEnd = SelectionEnd;
			};
			this.AfterTextChanged += (s, e) =>
			{
				string stringAfter = e.Editable.ToString();
				if (stringAfter != _stringBefore)
				{
					Log.Debug("TextDidChange");
					TextDidChange(this, stringAfter);
				}
				else
				{
					Log.Debug("TextDidNotChange");
					SetSelection(_selectionStart, _selectionEnd);
				}
			};
		}
	}
}
