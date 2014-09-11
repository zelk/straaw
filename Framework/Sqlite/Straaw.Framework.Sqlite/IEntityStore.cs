#if !MONOTOUCH && !__ANDROID__

using System;
using System.Collections.Generic;

namespace Straaw.Framework.Sqlite
{
	public interface IEntityStore
	{
		/// <summary>
		/// Gets the specified entity blob.
		/// </summary>
		/// <param name="entityType">Specify the EntityType of interest.</param>
		/// <param name="entityKey">Specify the EntityKey (unique only together with EntityType) of the Entity to get.</param>
		/// <param name="entityTag">This out parameter holds the EntityTag that was stored along with the Entity in the EntityStore.</param>
		/// <param name="lastModified">This out parameter holds the LastModified value that was stored along with the Entity in the EntityStore.</param>
		/// <returns>The entity blob or null if it was not found.</returns>
		byte[] GetEntity(string entityType, string entityKey, out string entityTag, out DateTime? lastModified);
		IDictionary<string, byte[]> GetAllEntities(string entityType);

		/// <summary>
		/// Puts a blob into the EntityStore (inserting or updating).
		/// </summary>
		/// <param name="entityType">The type of entity to store.</param>
		/// <param name="entityKey">The key of the entity (unique only together with the EntityType).</param>
		/// <param name="entityBlob">The Entity blob to store.</param>
		/// <param name="entityTag">An entity tag (version) that can be attached to the Entity.</param>
		/// <param name="lastModified">A date that can be stored together with the blob to remember when this object was last modified.</param>
		/// <param name="alwaysOverwrite">When true, the entity is always stored. When false, if the entity existed prior to this call, the Entity is only stored if the EntityTag and LastModified are the same.</param>
		/// <returns>True if the Entity was stored and false otherwise. If errors occur, exceptions are thrown.</returns>
		bool PutEntity(string entityType, string entityKey, byte[] entityBlob, string entityTag = null, DateTime? lastModified = null, bool alwaysOverwrite = true);
		bool DeleteEntity(string entityType, string entityKey);
	}
}

#endif
