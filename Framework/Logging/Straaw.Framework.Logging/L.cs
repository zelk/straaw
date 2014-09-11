using System;

namespace Straaw.Framework.Logging
{
	public class L
	{
		public static LogManager O
		{
			get
			{
				if (_sharedInstance == null)
				{
					throw new NullReferenceException("L.O.G() was used prior to L.SetSharedInstance()");
				}
				return _sharedInstance;
			}
		}
		
		public static bool IsInitialized
		{
			get
			{
				return _sharedInstance != null;
			}
		}

		public static void SetSharedInstance(LogManager sharedLogManager)
		{
			if (sharedLogManager == null)
				throw new NullReferenceException();
		
			if (_sharedInstance != null)
			{
				_sharedInstance.GetLogger(typeof(LogManager)).Error("The shared LogManager should only be set once.");
				sharedLogManager.GetLogger(typeof(LogManager)).Error("The shared LogManager should only be set once.");
			}

			_sharedInstance = sharedLogManager;
		}

		private static LogManager _sharedInstance;
	}
}
