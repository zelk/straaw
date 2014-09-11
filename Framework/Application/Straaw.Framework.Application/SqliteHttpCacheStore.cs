using System;
using Straaw.Framework.RestClient;
using Straaw.Framework.Sqlite;

namespace Straaw.Framework.Application
{
	public class SqliteHttpCacheStore : IHttpCacheStore
	{
		public SqliteHttpCacheStore(IEntityStore entityStore)
		{
			if (entityStore == null)
				throw new NullReferenceException();
			_entityStore = entityStore;
		}
		
		public void Write(string key, byte[] value)
		{
			_entityStore.PutEntity(ENTITY_TYPE, key, value);
		}
		
		public byte[] Read(string key)
		{
			string entityTag;
			DateTime? lastModified;
			return _entityStore.GetEntity(ENTITY_TYPE, key, out entityTag, out lastModified);
		}

		private const string ENTITY_TYPE = "SqliteHttpCacheStore";
		private IEntityStore _entityStore;
	}
}
