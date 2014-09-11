using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Widget;
using Android.Views;
using Straaw.Framework.Logging;
using Straaw.Framework.UiContract;
using System;
using System.Reflection;

namespace Straaw.Framework.Ui.Droid
{
	/// <summary>
	/// This attribute makes it easier to work with Views in C# Android Activities.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class FindViewByIdAttribute : System.Attribute
	{
		public int Id { get; set; }
	}

	public class SurfaceHelper
	{
		public static string DirectorIdKey = "DIRECTOR_ID_KEY";
		static public void OpenSurface(Context context, Type surfaceActivityType, long directorId)
		{
			Log.Debug("Opening surface {0}[DirId:{1}]", surfaceActivityType.Name, directorId);
			var intent = new Intent(context, surfaceActivityType);
			intent.PutExtra(DirectorIdKey, directorId);
			context.StartActivity(intent);
		}

		private static readonly Logger Log = L.O.G(typeof(SurfaceHelper));
	}

	/// <summary>
	/// Use this base class instead of Activity when you want to implement Straaw Surface interfaces.
	/// </summary>
    public abstract class SurfaceActivity<TDirector> : Activity, ISurface where TDirector : class, IDirector
	{
		public TDirector Director { get; set; }

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// ISurface
		//////////////////////////////////////////////////////////////////////////////////////////////////

		public abstract ISurfaceRegistry GetSurfaceRegistry();

		public void SetSurfaceRegistry(ISurfaceRegistry surfaceRegistry)
		{
			if (surfaceRegistry == null)
			{
				throw new NullReferenceException();
			}
			
			_surfaceRegistry = surfaceRegistry;
		}

		public void SetDirector(IDirector director)
		{
			if (director == null)
				Director = null;
			else if (director is TDirector)
				Director = (TDirector)director;
			else
				throw new Exception(string.Format("Unable to set director {0} on surface {1}[DirId:{2}] as it expected director {3}.", director.GetType().Name, this.GetType().Name, Director.DirectorId, typeof(TDirector).Name));
		}
		
		public virtual void WillAttachToDirector()
		{
			Director.OnDismissed += OnDismissed;
		}

		public virtual void DidAttachToDirector()
		{
		}

		public virtual void WillDetachFromDirector()
		{
			Director.OnDismissed -= OnDismissed;
		}

		public virtual void DidDetachFromDirector()
		{
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// Android Activity related
		//////////////////////////////////////////////////////////////////////////////////////////////////

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            var directorId = FindOutDirectorId(bundle);

			if (_surfaceRegistry == null)
			{
				throw new Exception
				(
					string.Format
					(
						"Unable to create SurfaceActivity {0}[DirId:{1}] since there is no surfaceRegistry available.",
						this.GetType().Name,
						directorId
					)
				);
			}
			
			Log.Debug("Attaching {0}[DirId:{2}] to {1}[DirId:{2}]", this.GetType().Name, typeof(TDirector).Name, directorId);
			_surfaceRegistry.AttachToDirector(this, directorId);
		}

		public override void SetContentView(View view)
		{
			base.SetContentView(view);
			ConnectPropertiesWithFindViewByIdAttribute();
		}

		public override void SetContentView(View view, ViewGroup.LayoutParams @params)
		{
			base.SetContentView(view, @params);
			ConnectPropertiesWithFindViewByIdAttribute();
		}

		public override void SetContentView(int layoutResID)
		{
			base.SetContentView(layoutResID);
			ConnectPropertiesWithFindViewByIdAttribute();
		}

		protected override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);
			outState.PutLong(SurfaceHelper.DirectorIdKey, Director.DirectorId);
		}

		public override void OnBackPressed()
		{
		    if (Director != null) Director.UserDismiss(true); // TODO lcka?
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (Director != null)
			{
				Log.Info("Android is removing surface {0}[DirId:{1}], likely due to low memory.");
				Log.Debug("Detaching {0}[DirId:{2}] from {1}[DirId:{2}]", this.GetType().Name, typeof(TDirector).Name, Director.DirectorId);
				_surfaceRegistry.DetachFromDirector(this, Director.DirectorId);
			}
			
			Log.Info("{0}[DirId:?].OnDestroy", this.GetType().Name);

			// This should be a good time to run the garbage collector, to pick up any left-overs from this activity.
			GC.Collect();
		}

	    //////////////////////////////////////////////////////////////////////////////////////////////////
		// Helper methods
		//////////////////////////////////////////////////////////////////////////////////////////////////

		protected void AddWillChangeDelegate(EditText editText, Func<string, bool> willChangeDelegate)
		{
			var oldFilters = editText.GetFilters();
			var newFilters = new IInputFilter[oldFilters.Length+1];
			newFilters[newFilters.Length - 1] = new DelegatedInputFilter(willChangeDelegate);
			editText.SetFilters(newFilters);
		}

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// Private
		//////////////////////////////////////////////////////////////////////////////////////////////////

		private long FindOutDirectorId(Bundle bundle)
		{
			long returnValue;

			if (bundle != null)
			{
				returnValue = bundle.GetLong(SurfaceHelper.DirectorIdKey, long.MinValue);
				if (returnValue == long.MinValue)
					throw new Exception("Unable to read DirectorId from Android saved instance state.");
			}
			else
			{
				returnValue = Intent.GetLongExtra(SurfaceHelper.DirectorIdKey, -1);
				if (returnValue == -1)
					throw new Exception("Unable to read DirectorId from Android intent.");
			}

			return returnValue;
		}

		private void OnDismissed(bool animated)
		{
			Log.Debug("Detaching {0}[DirId:{2}] from {1}[DirId:{2}]", this.GetType().Name, typeof(TDirector).Name, Director.DirectorId);
			_surfaceRegistry.DetachFromDirector(this, Director.DirectorId);

			Finish();

			if (!animated)
			{
				OverridePendingTransition(0, 0);
			}
		}

		private void ConnectPropertiesWithFindViewByIdAttribute()
		{
			using (Log.Scope(LogLevel.Debug, ""))
			{
				// TODO: Don't I need to remove the references to all those objects to prevent memory leaks???
				// Connect all axml views to the surface's properties
				foreach (PropertyInfo property in this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance))
				{
					object[] attributes = property.GetCustomAttributes(typeof(FindViewByIdAttribute), false);
					if (attributes.Length == 1)
					{
						int viewId = ((FindViewByIdAttribute)attributes[0]).Id;
						object view = this.FindViewById(viewId);
						if (view == null)
							throw new System.Exception(string.Format("Unable to find view with Id {0} for property {1}.{2}", viewId, this.GetType().Name, property.Name));
						property.SetValue(this, view, null);
					}
				}
			}
		}

		~SurfaceActivity()
		{
			Log.Info("{0}[DirId:?].FINALIZER", GetType().Name);
		}

		static private readonly Logger Log = L.O.G(typeof(SurfaceActivity<TDirector>));
		private ISurfaceRegistry _surfaceRegistry;
	}
}
