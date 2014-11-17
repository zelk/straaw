using System;
#if !MONOTOUCH && !__ANDROID__
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Straaw.Framework.Sqlite
{
	/// <summary>
	/// This specialized version of the EntityStore automatically serializes and deserializes classes as they
	/// are stored and retrieved from the EntityStore. The objects that are sent to these methods should
	/// be instances of classes that are decorated with the WCF DataContract and DataMember attributes.
	/// </summary>
	public interface IModelStore : IEntityStore
	{
		/// <summary>
		/// Looks in the EntityStore for an Entity that matches the given key.
		/// Then interprets the Entity data as json, to deserialize the Entity into a WCF DataContract.
		/// </summary>
		/// <typeparam name="TJsonDataContract">A type that is decorated with the WCF DataContract attribute.</typeparam>
		/// <param name="key">The Entity key to look for in the EntityStore</param>
		/// <returns>An instance of TJsonDataContract.</returns>
		TJsonDataContract GetModel<TJsonDataContract>(string key) where TJsonDataContract : class, new();
		IDictionary<string, TJsonDataContract> GetAllModels<TJsonDataContract>() where TJsonDataContract : class, new();
		Task<IDictionary<string, TJsonDataContract>> GetAllModelsAsync<TJsonDataContract>() where TJsonDataContract : class, new();
		IList<TJsonDataContract> FilterModels<TJsonDataContract>(Func<TJsonDataContract, bool> filter) where TJsonDataContract : class, new();
		Task<TJsonDataContract> GetModelAsync<TJsonDataContract>(string key) where TJsonDataContract : class, new();
		void PutModel(string key, object jsonDataContract);
		Task PutModelAsync(string key, object jsonDataContract);
		void DeleteModel<TJsonDataContract>(string key);
		Task DeleteModelAsync<TJsonDataContract>(string key);
	}
}

#endif
