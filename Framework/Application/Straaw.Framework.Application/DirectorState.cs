using System;

namespace Straaw.Framework.Application
{

	public abstract class DirectorStateBase
	{
		internal bool IsSurfacePresent { get; set; }
	}

	public class StringDirectorState : DirectorState<string>
	{
		public void TrySetValue(string source, int start, int end, string dest, int dstart, int dend)
		{
		}
	}

	public class DirectorState<TValue> : DirectorStateBase
	{
		private TValue _value;
		private int _isUpdating; // Without this bool, the Director and Surface might update eachother in an endless loop.

		public DirectorState() { }

		public DirectorState(TValue value)
		{
			Value = value;
		}

		public bool IsPaused { get { return _isUpdating > 0; } }

		public void PauseUpdates()
		{
			++_isUpdating;
		}

		public void ResumeUpdates()
		{
			--_isUpdating;
			#if DEBUG
			if (_isUpdating < 0)
				throw new Exception(string.Format("DirectorState<{0}>._isUpdating is less than zero. This probably means that RestoreUpates has been called without a prior call to SuppressUpdates (or worse, threading issues).", typeof(TValue).Name));
			#endif
		}

		public	TValue Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				if (IsSurfacePresent && !IsPaused)
				{
					PauseUpdates();
					OnUpdateSurface();
					ResumeUpdates();
				}
			}
		}
		public event Action OnUpdateSurface;
	}
}
