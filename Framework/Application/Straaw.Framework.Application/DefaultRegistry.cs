using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Straaw.Framework.Logging;

using Straaw.Framework.UiContract;

namespace Straaw.Framework.Application
{
	/// <summary>
	/// This is the class that keeps a registry of all created Directors and their attached Surfaces.
	/// NOT THREAD SAFE!!!
	/// </summary>
	public class DefaultRegistry : IDirectorRegistry, ISurfaceRegistry
	{
		public DefaultRegistry()
		{
			Log.Info("Starting up the DefaultRegistry.");
		}

		// ----------------------------------------------------------------------------------------
		// IDirectorRegistry
		// ----------------------------------------------------------------------------------------

		public void AddDirector(DirectorBaseBase director)
		{
			Log.Verbose("");

			var id = GenerateNewDirectorId();
			director.DirectorId = id;

			_directors.Add(id, director);
			
			Log.Info("Added director {0} with DirectorId {1}", director.GetType().Name, id);
		}

		public void RemoveDirector(DirectorBaseBase director)
		{
			Log.Verbose("");

			var id = director.DirectorId;
			var directorName = director.GetType().Name;
			
			if (director.AttachedSurfaceCount != 0)
			{
				throw new Exception
				(
					string.Format
					(
						"Unable to remove director {0}[DirId:{1}] because it still has {2} surfaces attached.",
						directorName,
						id,
						director.AttachedSurfaceCount
					)
				);
			}
			
		    if (!_directors.ContainsKey(id))
		    {
                // TODO Kolla vad som orsakar detta på Android
                //	throw new Exception(string.Format("Unable to remove director {0} since no director with DirectorId {1} was found.", directorName, id));
                Log.Warning(string.Format("Unable to remove director {0} since no director with DirectorId {1} was found.", directorName, id));
		        return;
			}
			_directors.Remove(id);
			Log.Info("Removed director {0} with DirectorId {1}", directorName, id);
		}

		// ----------------------------------------------------------------------------------------
		// ISurfaceRegistry
		// ----------------------------------------------------------------------------------------

		public void AttachToDirector(ISurface surface, long directorId)
		{
			var director = FindDirector(directorId);
			surface.SetDirector(director);
			surface.WillAttachToDirector();
			director.AttachedSurfaceCount++;
			director.SurfaceAttached();
			surface.DidAttachToDirector();
		}

		public void DetachFromDirector(ISurface surface, long directorId)
		{
			var director = FindDirector(directorId);
			if (director.AttachedSurfaceCount == 0)
			{
				throw new Exception(string.Format("Unable to detach surface {0} from director {1} since it has no attached surfaces.", surface.GetType().Name, director.GetType().Name));
			}
			
			surface.WillDetachFromDirector();
			surface.SetDirector(null);
			director.AttachedSurfaceCount--;
			
			if (director.AttachedSurfaceCount == 0)
			{
				// Since the last surface detached, we know that we want to clear all delegates in the Director
				// TODO: Add reflection code to clear all delegates.
			}
			
			director.SurfaceDetached();
			surface.DidDetachFromDirector();
		}

		// ----------------------------------------------------------------------------------------
		// Private
		// ----------------------------------------------------------------------------------------

		private TDirector FindDirector<TDirector>(long directorId) where TDirector : class, IDirector
		{
			DirectorBaseBase directorBase = FindDirector(directorId);

			var director = directorBase as TDirector;
			if (director == null)
				throw new Exception(string.Format("Unable to attach to a {0} since DirectorId {1} is a {2}.", typeof(TDirector).Name, directorId, directorBase.GetType().Name));

			return director;
		}

		private DirectorBaseBase FindDirector(long directorId)
		{
			DirectorBaseBase directorBaseBase;
			if (!_directors.TryGetValue(directorId, out directorBaseBase))
				throw new Exception(string.Format("Unable to find a Director with DirectorId {0}.", directorId));

			return directorBaseBase;
		}

		private long GenerateNewDirectorId()
		{
			Log.Verbose("GenerateNewDirectorId");
			return Interlocked.Increment(ref _nextDirectorId);
		}

		private static readonly Logger Log = L.O.G(typeof(DefaultRegistry));
		private static long _nextDirectorId = -1;
		private readonly Dictionary<long, DirectorBaseBase> _directors = new Dictionary<long, DirectorBaseBase>(16);
	}
}
