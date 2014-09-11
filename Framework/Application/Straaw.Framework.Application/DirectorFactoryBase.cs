using System;
using System.Threading.Tasks;

namespace Straaw.Framework.Application
{
	public class DirectorFactoryBase
	{
		protected DirectorFactoryBase(IDirectorRegistry directorRegistry)
		{
			if (directorRegistry == null)
				throw new NullReferenceException();

			DirectorRegistry = directorRegistry;
		}

		protected IDirectorRegistry DirectorRegistry { get; private set; }
	}
}
