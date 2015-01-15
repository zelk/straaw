using System;
using Straaw.Framework.RestClient;
using System.Collections.Generic;

namespace Straaw.Framework.Application
{
	public class MemoryHttpCacheStore : IHttpCacheStore
	{
		public MemoryHttpCacheStore()
		{
			_memCache = new Dictionary<string, byte[]>(32);
		}
		
		public void Write(string key, byte[] value)
		{
			lock(_memCache)
			{
				if (_memCache.ContainsKey(key))
				{
					_memCache.Remove(key);
				}
				_memCache.Add(key, value);
			}
		}
		
		public byte[] Read(string key)
		{
			byte[] value = null;
			_memCache.TryGetValue(key, out value);
			return value;
		}

		private readonly IDictionary<string, byte[]> _memCache;
	}
}
