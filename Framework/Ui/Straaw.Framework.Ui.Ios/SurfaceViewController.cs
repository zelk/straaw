using System;
using MonoTouch.UIKit;
using Straaw.Framework.UiContract;
using MonoTouch.Foundation;
using Straaw.Framework.Logging;

namespace Straaw.Framework.Ui.Ios
{
	public abstract class SurfaceViewController<TDirector> : UIViewController, ISurface where TDirector : class, IDirector
	{
	    public SurfaceViewController (long directorId)
		{
		    _directorId = directorId;
		    LogVerbose("{0}.ctor", this.GetType().Name);
		}

	    public TDirector Director { get; set; }

		//////////////////////////////////////////////////////////////////////////////////////////////////
		// ISurface
		//////////////////////////////////////////////////////////////////////////////////////////////////

		public abstract ISurfaceRegistry GetSurfaceRegistry();

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
		// iOS View Controller related
		//////////////////////////////////////////////////////////////////////////////////////////////////
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			LogInfo("Attaching {0}[DirId:{2}] to {1}[DirId:{2}]", this.GetType().Name, typeof(TDirector).Name, _directorId);
			GetSurfaceRegistry().AttachToDirector(this, _directorId);
		}

		#if __IOS_5__ || __IOS_4__ || __IOS_3__ || __IOS_2__
			public override void ViewWillUnload ()
			{
				LogVerbose("{0}.ViewWillUnload", this.GetType().Name);
				base.ViewWillUnload ();
			}
		#endif
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			LogVerbose("{0}.ViewWillDisappear", this.GetType().Name);
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			LogVerbose("{0}.ViewWillAppear", this.GetType().Name);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			LogVerbose("{0}.ViewDidAppear", this.GetType().Name);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);

			LogVerbose("{0}.ViewDidDisappear", this.GetType().Name);
		}

		#if __IOS_5__ || __IOS_4__ || __IOS_3__ || __IOS_2__
			public override void ViewDidUnload()
			{
				LogVerbose("{0}.ViewDidUnload", this.GetType().Name);
				base.ViewDidUnload();
			}
		#endif
		
		//////////////////////////////////////////////////////////////////////////////////////////////////
		// Private
		//////////////////////////////////////////////////////////////////////////////////////////////////

		private void OnDismissed(bool animated)
		{
			LogInfo("Detaching {0}[DirId:{2}] from {1}[DirId:{2}]", this.GetType().Name, typeof(TDirector).Name, Director.DirectorId);
			GetSurfaceRegistry().DetachFromDirector(this, Director.DirectorId);
			
			this.NavigationController.PopViewControllerAnimated(animated);
		}

		private void LogVerbose(string format, object obj0 = null, object obj1 = null, object obj2 = null)
		{
			if (L.IsInitialized)
				L.O.G(typeof(SurfaceViewController<TDirector>)).Verbose(format, obj0, obj1, obj2);
		}

		private void LogInfo(string format, object obj0 = null, object obj1 = null, object obj2 = null)
		{
			if (L.IsInitialized)
				L.O.G(typeof(SurfaceViewController<TDirector>)).Info(format, obj0, obj1, obj2);
		}

	    private readonly long _directorId;
	}
}
