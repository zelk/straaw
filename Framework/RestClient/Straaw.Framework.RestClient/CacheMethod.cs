using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Straaw.Framework.RestClient
{
	public enum CacheMethod
	{
		OnlyCached,
		PreferCached,
		Default,
		PreferOnline,
		OnlyOnline,
	}
}
