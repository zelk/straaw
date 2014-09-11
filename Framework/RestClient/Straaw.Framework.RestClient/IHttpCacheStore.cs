using System;

namespace Straaw.Framework.RestClient
{
	public interface IHttpCacheStore
	{
		void Write(string key, byte[] value);
		byte[] Read(string key);
	}
}
