using System;
using System.Collections.Generic;
using System.Data;
using Straaw.Framework.Logging;
using System.IO;


using SQLiteDataReader	= Mono.Data.Sqlite.SqliteDataReader;
using SQLiteConnection	= Mono.Data.Sqlite.SqliteConnection;
using SQLiteCommand		= Mono.Data.Sqlite.SqliteCommand;
using SQLiteParameter	= Mono.Data.Sqlite.SqliteParameter;


namespace Straaw.Framework.Sqlite
{
	public enum DestructorType
	{
		None,
		DeleteDatabase
	}

	public class EntityStore : IEntityStore
	{
		public EntityStore(string databaseFullPath, DestructorType destructorType = DestructorType.None)
		{
			_databaseFullPath = databaseFullPath;
			_destructorType = destructorType;
			_connectionString = String.Format("Data Source={0}; Version=3; Read Only=False; Pooling=True; Max Pool Size=10", _databaseFullPath);
			
			if (!File.Exists(_databaseFullPath))
			{
				SQLiteConnection.CreateFile(_databaseFullPath);
				Log.Info("Successfully created the EntityStore database file at {0}", databaseFullPath);
			}
			else
			{
				Log.Info("Successfully confirmed that the EntityStore database exists at {0}", databaseFullPath);
			}

			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();
				using (var cmd = connection.CreateCommand())
				{
					Log.Debug("About to create or check for the Entities table in the EntityStore database.");
					cmd.CommandText = "CREATE TABLE IF NOT EXISTS Entities (entityType TEXT, entityKey TEXT, entityBlob TEXT, entityTag TEXT, lastModified DATETIME, PRIMARY KEY (entityType, entityKey))";
					cmd.CommandType = CommandType.Text;
					cmd.ExecuteNonQuery();
					Log.Info("Successfully created or checked that the Entities table exists in the EntityStore database.");
				}
			}
		}

		~EntityStore()
		{
			if (_destructorType == DestructorType.DeleteDatabase)
			{
				Log.Info("Database {0} is set to be deleted after usage.", _databaseFullPath);
				if (!File.Exists(_databaseFullPath))
				{
					Log.Warning("Unable to delete database {0} as it does not exist.", _databaseFullPath);
				}
				else
				{
					File.Delete(_databaseFullPath);
					Log.Info("Deleted database {0}.", _databaseFullPath);
				}
			}
		}

		public byte[] GetEntity(string entityType, string entityKey, out string entityTag, out DateTime? lastModified)
		{
			using (Log.Scope(LogLevel.Debug, "GetEntity"))
			{
				using (var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					using (var cmd = connection.CreateCommand())
					{
						cmd.CommandText = "SELECT entityTag, lastModified, entityBlob FROM Entities WHERE entityType=? AND entityKey=?";
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType", Value = entityType });
						cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey", Value = entityKey });
						var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
						if (!reader.Read())
						{
							Log.Debug("Could not find Entity with EntityType '{0}' and EntityKey '{1}'", entityType, entityKey);
							entityTag = null;
							lastModified = null;
							return null;
						}
						else
						{
							entityTag = null;
							if (!reader.IsDBNull(0))
								entityTag = reader.GetString(0);
							lastModified = null;
							if (!reader.IsDBNull(1))
								lastModified = reader.GetDateTime(1);
							Log.Debug("Found Entity with EntityType '{0}' and EntityKey '{1}'. It had EntityTag '{2}' and was LastModified '{3}'", entityType, entityKey, entityTag, lastModified);
							if (!reader.IsDBNull(2))
								return ReadBytes(reader, 2);
							else
								return null;
						}
					}
				}
			}
		}

		public IDictionary<string, byte[]> GetAllEntities(string entityType)
		{
			using (Log.Scope(LogLevel.Debug, "GetAllEntities"))
			{
				using (var connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					using (var cmd = connection.CreateCommand())
					{
						cmd.CommandText = "SELECT entityKey, entityBlob FROM Entities WHERE entityType=?";
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType", Value = entityType });
						var reader = cmd.ExecuteReader(CommandBehavior.SingleResult);

						var dict = new Dictionary<string, byte[]>(8);

						while(reader.Read())
						{
							string entityKey = null;
							if (!reader.IsDBNull(0))
								entityKey = reader.GetString(0);

							Log.Debug("Found Entity with EntityType '{0}' and EntityKey '{1}'.", entityType, entityKey);

							if (reader.IsDBNull(1))
								continue;

							var bytes = ReadBytes(reader, 1);
							dict.Add(entityKey, bytes);
						}
						return dict;
					}
				}
			}
		}

		public bool PutEntity(string entityType, string entityKey, byte[] entityBlob, string entityTag = null, DateTime? lastModified = null, bool alwaysOverwrite = true)
		{
			using (Log.Scope(LogLevel.Debug, "PutEntity"))
			{
				bool worked = false;

				SQLiteConnection connection;
				using (connection = new SQLiteConnection(_connectionString))
				{
					connection.Open();
					SQLiteCommand cmd;
					using (cmd = connection.CreateCommand())
					{
						cmd.CommandText = "SELECT COUNT(entityType) FROM Entities WHERE entityType=? AND entityKey=?";
						cmd.CommandType = CommandType.Text;
						cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType",	Value = entityType});
						cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey",		Value = entityKey });
						var someType = cmd.ExecuteScalar();
						var foundCount = (long)someType;
						if (foundCount > 0)
						{
							if (alwaysOverwrite)
							{
								using (cmd = connection.CreateCommand())
								{
									cmd.CommandText = "UPDATE Entities SET entityBlob=?, entityTag=?, lastModified=? WHERE entityType=? AND entityKey=?";
									cmd.CommandType = CommandType.Text;
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.Object, ParameterName = "entityBlob",		Value = entityBlob });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityTag",			Value = entityTag });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.DateTime, ParameterName = "lastModified",	Value = lastModified });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType",		Value = entityType });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey",			Value = entityKey });
									if (cmd.ExecuteNonQuery() > 0)
									{
										worked = true;
										Log.Debug("Put (update) Entity with EntityType '{0}' and EntityKey '{1}'.", entityType, entityKey);
									}
								}
							}
							else
							{
								using (cmd = connection.CreateCommand())
								{
									cmd.CommandText = "SELECT COUNT(entityType) FROM Entities WHERE entityType=? AND entityKey=? AND entityTag=? AND lastModified=?";
									cmd.CommandType = CommandType.Text;
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType",	Value = entityType });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey",		Value = entityKey });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityTag",		Value = entityTag });
									cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.DateTime, ParameterName = "lastModified",	Value = lastModified });
									if ((int)cmd.ExecuteScalar() > 0)
									{
										using (cmd = connection.CreateCommand())
										{
											cmd.CommandText = "UPDATE Entities SET entityBlob=?, entityTag=?, lastModified=? WHERE entityType=? AND entityKey=?";
											cmd.CommandType = CommandType.Text;
											cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.Object, ParameterName = "entityBlob",		Value = entityBlob });
											cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityTag",			Value = entityTag });
											cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.DateTime, ParameterName = "lastModified",	Value = lastModified });
											cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType",		Value = entityType });
											cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey",			Value = entityKey });
											if (cmd.ExecuteNonQuery() > 0)
											{
												worked = true;
												Log.Debug("Put (update) Entity with EntityType '{0}' and EntityKey '{1}'.", entityType, entityKey);
											}
										}
									}
								}
							}
						}
						else
						{
							using (cmd = connection.CreateCommand())
							{
								cmd.CommandText = "INSERT INTO Entities (entityType, entityKey, entityBlob, entityTag, lastModified) VALUES (?, ?, ?, ?, ?)";
								cmd.CommandType = CommandType.Text;
								cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType", Value = entityType });
								cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey", Value = entityKey });
								cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.Object, ParameterName = "entityBlob", Value = entityBlob });
								cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityTag", Value = entityTag });
								cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.DateTime, ParameterName = "lastModified", Value = lastModified });
								if (cmd.ExecuteNonQuery() > 0)
								{
									worked = true;
									Log.Debug("Put (insert) Entity with EntityType '{0}' and EntityKey '{1}'.", entityType, entityKey);
								}
							}
						}
					}
				}
				return worked;
			}
		}

		public bool DeleteEntity(string entityType, string entityKey)
		{
			using (var connection = new SQLiteConnection(_connectionString))
			{
				connection.Open();
				using (var cmd = connection.CreateCommand())
				{
					cmd.CommandText = "DELETE FROM Entities WHERE entityType=? AND entityKey=?";
					cmd.CommandType = CommandType.Text;
					cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityType", Value = entityType });
					cmd.Parameters.Add(new SQLiteParameter() { DbType = DbType.String, ParameterName = "entityKey", Value = entityKey });
					var affectedCount = cmd.ExecuteNonQuery();
					Log.Debug("Deleted Entity with EntityType '{0}' and EntityKey '{1}'.", entityType, entityKey);
					return affectedCount == 1;
				}
			}
		}

		private static byte[] ReadBytes(SQLiteDataReader reader, int fieldIndex)
		{
			const int CHUNK_SIZE = 2 * 1024;
			var buffer = new byte[CHUNK_SIZE];
			long fieldOffset = 0;
			using (var stream = new MemoryStream())
			{
				long bytesRead;
				while ((bytesRead = reader.GetBytes(fieldIndex, fieldOffset, buffer, 0, buffer.Length)) > 0)
				{
					stream.Write(buffer, 0, (int)bytesRead);
					fieldOffset += bytesRead;
				}
				return stream.ToArray();
			}
		}

		static private readonly Logger Log = L.O.G(typeof(EntityStore));

		private readonly string _connectionString;
		private readonly string _databaseFullPath;
		private readonly DestructorType _destructorType;
	}
}
