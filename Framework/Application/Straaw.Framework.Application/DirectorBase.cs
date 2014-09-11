using System;
using System.Reflection;
using System.Threading.Tasks;
using Straaw.Framework.Logging;using Straaw.Framework.UiContract;
namespace Straaw.Framework.Application
{
	public abstract class DirectorBaseBase : IDirector
	{
		//-----------------------------------------------------------------------------------------------
		// Public Interface
		//-----------------------------------------------------------------------------------------------

		public long DirectorId { get; set; }
		public event Action<bool> OnDismissed;
		public abstract void UserDismiss(bool animated);
		//-----------------------------------------------------------------------------------------------
		/// Sub Class Interface
		//-----------------------------------------------------------------------------------------------

		protected DirectorBaseBase(IDirectorRegistry directorRegistry)
		{
			Log.Debug("{0}.CONSTRUCTOR", GetType().Name);

			if (directorRegistry == null)
				throw new NullReferenceException();

			_directorRegistry = directorRegistry;
			_directorRegistry.AddDirector(this);
		}

		protected virtual void OnPresent()
		{
			Log.Verbose("{0}[DirId:{1}].OnPresent", this.GetType().Name, DirectorId);
		}

		protected virtual void OnSurfaceAttached()
		{
			Log.Verbose("{0}[DirId:{1}].OnSurfaceAttached", this.GetType().Name, DirectorId);
		}

		protected virtual void OnSurfaceDetached()
		{
			Log.Verbose("{0}[DirId:{1}].OnSurfaceDetached", this.GetType().Name, DirectorId);
		}
		
		protected bool IsDismissed { get; private set; }

		//-----------------------------------------------------------------------------------------------
		// Internal Interface
		//-----------------------------------------------------------------------------------------------
	
		internal int AttachedSurfaceCount { get; set; }
		
		internal void SurfaceAttached()
		{
			Log.Verbose("{0}[DirId:{1}].SurfaceAttached before OnSurfaceAttached", this.GetType().Name, DirectorId);
			OnSurfaceAttached();
			Log.Verbose("{0}[DirId:{1}].SurfaceAttached After OnSurfaceAttached", this.GetType().Name, DirectorId);
		}

		internal void SurfaceDetached()
		{
			Log.Verbose("{0}[DirId:{1}].SurfaceDetached before OnSurfaceDetached", this.GetType().Name, DirectorId);
			OnSurfaceDetached();
			Log.Verbose("{0}[DirId:{1}].SurfaceDetached after OnSurfaceDetached", this.GetType().Name, DirectorId);
		}

		internal void InternalDismiss(bool animated)
		{
			Log.Verbose("{0}[DirId:{1}].InternalDismiss before OnDismissed", this.GetType().Name, DirectorId);
			IsDismissed = true;
			if (OnDismissed != null)
			{
				Log.Verbose("{0}[DirId:{1}].InternalDismiss before OnDismissed (OnDismissed != null)", this.GetType().Name, DirectorId);
				OnDismissed(animated);
				Log.Verbose("{0}[DirId:{1}].InternalDismiss after OnDismissed (OnDismissed != null)", this.GetType().Name, DirectorId);
			}
			_directorRegistry.RemoveDirector(this);
			Log.Verbose("{0}[DirId:{1}].InternalDismiss after RemoveDirector", this.GetType().Name, DirectorId);
		}

		//-----------------------------------------------------------------------------------------------
		// Private
		//-----------------------------------------------------------------------------------------------

		~DirectorBaseBase()
		{
			Log.Info("{0}[DirId:{1}].FINALIZER", GetType().Name, DirectorId);
		}

		static private readonly	Logger Log = L.O.G(typeof(DirectorBase));
		private readonly IDirectorRegistry _directorRegistry;
	}

	public abstract class DirectorBase : DirectorBaseBase
	{
		protected DirectorBase(IDirectorRegistry directorRegistry) : base(directorRegistry)
		{
			ResultTask = new Task(() => {});		}

		public Task Present()
		{
			Log.Verbose("{0}[DirId:{1}].Present before OnPresent()", this.GetType().Name, DirectorId);
			OnPresent();
			Log.Verbose("{0}[DirId:{1}].Present after OnPresent()", this.GetType().Name, DirectorId);
			return ResultTask;
		}

		protected void Dismiss(bool animated)
		{
			Log.Verbose("{0}[DirId:{1}].Dismiss before InternalDismiss(animated)", this.GetType().Name, DirectorId);
			InternalDismiss(animated);
			Log.Verbose("{0}[DirId:{1}].Dismiss before ResultTask.Start()", this.GetType().Name, DirectorId);
			ResultTask.Start();
			Log.Verbose("{0}[DirId:{1}].Dismiss after ResultTask.Start()", this.GetType().Name, DirectorId);
		}

		protected Task ResultTask { get; private set; }

		private static readonly Logger Log = L.O.G(typeof(DirectorBase));
	}

	public abstract class DirectorBase<TResult1> : DirectorBaseBase
	{
		protected DirectorBase(IDirectorRegistry directorRegistry)
			: base(directorRegistry)
		{
			ResultTask = new Task<TResult1>(GetResult);		}

		protected void Dismiss(TResult1 result)
		{
			Log.Verbose("{0}[DirId:{1}].Dismiss before InternalDismiss", this.GetType().Name, DirectorId);
			Result = result;
			InternalDismiss(false);
			Log.Verbose("{0}[DirId:{1}].Dismiss before ResultTask.Start()", this.GetType().Name, DirectorId);
			ResultTask.Start();
			Log.Verbose("{0}[DirId:{1}].Dismiss after ResultTask.Start()", this.GetType().Name, DirectorId);
		}

		protected void Dismiss(TResult1 result, bool animated)
		{
			Log.Verbose("{0}[DirId:{1}].Dismiss before InternalDismiss", this.GetType().Name, DirectorId);
			Result = result;
			InternalDismiss(animated);
			Log.Verbose("{0}[DirId:{1}].Dismiss before ResultTask.Start()", this.GetType().Name, DirectorId);
			ResultTask.Start();
			Log.Verbose("{0}[DirId:{1}].Dismiss after ResultTask.Start()", this.GetType().Name, DirectorId);
		}

		public Task<TResult1> Present()
		{
			Log.Verbose("{0}[DirId:{1}].Present before OnPresent()", this.GetType().Name, DirectorId);
			OnPresent();
			Log.Verbose("{0}[DirId:{1}].Present after OnPresent()", this.GetType().Name, DirectorId);
			return ResultTask;
		}

		private TResult1 GetResult()
		{
			return Result;
		}

		protected TResult1 Result { get; set; }
		protected Task<TResult1> ResultTask { get; private set; }

		private static readonly Logger Log = L.O.G(typeof(DirectorBase<TResult1>));
	}
}
