/*
using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Lang;

namespace Straaw.Framework.Ui.Droid
{
	public abstract class WebViewSurfaceActivity<TDirector, TSurface> : SurfaceActivity<TDirector, TSurface>
		where TDirector : IDirector, new()
		where TSurface : class, IDirector
	{
		protected WebView WebView { get; private set; }
		protected JavaScriptResponder Responder { get; private set; }
		protected string Html { get; private set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			this.Html = GetHtml();
			this.Responder = GetWebViewResponder();

			var container = new LinearLayout(this) { Orientation = Orientation.Vertical };
			this.WebView = new WebView(this);
			this.WebView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent);
			container.AddView(this.WebView);
			SetContentView(container);

			this.WebView.Settings.JavaScriptEnabled = true;
			this.WebView.AddJavascriptInterface(Responder, "Straaw");
			this.WebView.LoadData(Html, "text/html", null);
		}

		protected abstract string GetHtml();
		protected abstract JavaScriptResponder GetWebViewResponder();
	}
}
*/
