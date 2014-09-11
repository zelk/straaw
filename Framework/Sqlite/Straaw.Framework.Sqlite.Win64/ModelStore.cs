using System;
using System.Runtime.Serialization;
using System.Text;
using Straaw.Framework.Json;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

// TODO: Change this class into working with Mutable and Immutable models instead or add another class?

namespace Straaw.Framework.Sqlite
{
	public class ModelStore : EntityStore, IModelStore
	{
		public ModelStore(string databaseFullPath, DestructorType destructorType = DestructorType.None)
			: base(databaseFullPath, destructorType)
		{
		}

		public TJsonDataContract GetModel<TJsonDataContract>(string key) where TJsonDataContract : class, new()
		{
			string entityTag;
			DateTime? lastModified;

			var dataContractName = (Attribute.GetCustomAttribute(typeof(TJsonDataContract), typeof(DataContractAttribute)) as DataContractAttribute).Name;
			dataContractName = dataContractName ?? typeof(TJsonDataContract).Name;

			byte[] blob = GetEntity(dataContractName, key, out entityTag, out lastModified);
			if (blob == null)
				return null;

			string s = Encoding.UTF8.GetString(blob);
			return JsonSerializer.DeserializeDataContract<TJsonDataContract>(s, false);
		}

		public IDictionary<string, TJsonDataContract> GetAllModels<TJsonDataContract>() where TJsonDataContract : class, new()
		{
			var dataContractName = (Attribute.GetCustomAttribute(typeof(TJsonDataContract), typeof(DataContractAttribute)) as DataContractAttribute).Name;
			dataContractName = dataContractName ?? typeof(TJsonDataContract).Name;
			
			IDictionary<string, byte[]> entities = GetAllEntities(dataContractName);
			if (entities == null)
				return null;

			var dict = new Dictionary<string, TJsonDataContract>();

			foreach (var entity in entities)
			{
				var s = Encoding.UTF8.GetString(entity.Value);
				if (s != null)
					dict.Add(entity.Key, JsonSerializer.DeserializeDataContract<TJsonDataContract>(s, false));
			}
			
			return dict;
		}

		public async Task<TJsonDataContract> GetModelAsync<TJsonDataContract>(string key) where TJsonDataContract : class, new()
		{
			return await Task.Factory.StartNew(() => GetModel<TJsonDataContract>(key));
		}

		public void PutModel(string key, object jsonDataContract)
		{
			byte[] blob = null;

			var dataContractName = (Attribute.GetCustomAttribute(jsonDataContract.GetType(), typeof(DataContractAttribute)) as DataContractAttribute).Name;
			dataContractName = dataContractName ?? jsonDataContract.GetType().Name;

			string s = JsonSerializer.SerializeDataContract(jsonDataContract, false);
			if (s != null)
				blob = Encoding.UTF8.GetBytes(s);

			PutEntity(dataContractName, key, blob);
		}

		public async Task PutModelAsync(string key, object jsonDataContract)
		{
			await Task.Factory.StartNew(() => PutModel(key, jsonDataContract));
		}

	}
}
