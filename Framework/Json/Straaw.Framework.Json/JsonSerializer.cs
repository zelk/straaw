using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Json;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
// TODO: Add support for arrays
// TODO: Add support for dictionaries and other collections
// TODO: Optimize: Make the class based on generics. When instantiated (or upon startup),
//       each generic version analyzes its class to find all properties with the proper
//       attributes etc. OR, it could still be dynamic by sending in the type but preparing
//       in the constructor, not upon every serialization/deserialization...
// TODO: Add logging!
using System.Text;
using Straaw.Framework.Logging;

namespace Straaw.Framework.Json
{
	public class JsonSerializer
	{
		/// <summary>
		/// Creates a Json string from an instance of a class that is decorated with the WCF DataContract and DataMember attributes.
		/// </summary>
		/// <param name="dataContract">An instance of a class that is decorated with the WCF DataContract and DataMember attributes.</param>
		/// <param name="encloseInDataContractName">
		/// Specifies whether the JsonString includes the DataContract name or not.
		/// Example 1 (not enclosed in DataContract name) {"firstName":"Ricky","lastName":"Helgesson"}
		/// Example 2 (enclosed in DataContract name) {"user":{"firstName":"Ricky","lastName":"Helgesson"}}
		/// </param>
		/// <returns>A json string.</returns>
		public static string SerializeDataContract(object dataContract, bool encloseInDataContractName = false)
		{
			var classAttribute = GetCustomAttribute<DataContractAttribute>(dataContract.GetType());
			if (classAttribute == null)
				throw new ArgumentException(string.Format("{0} is not attributed as a DataContract.", dataContract.GetType().Name), "dataContract");

			var jsonValue = DataContractToJson(dataContract);
			if (encloseInDataContractName)
			{
				var dataContractName = classAttribute.Name ?? dataContract.GetType().Name;
				return new JsonObject(new KeyValuePair<string, System.Json.JsonValue>(dataContractName, jsonValue)).ToString();
			}
			else
				return jsonValue.ToString();
		}

		public static Stream SerializeDataContractToStream(object dataContract, bool encloseInDataContractName, Encoding encoding)
		{
			return new MemoryStream(encoding.GetBytes(SerializeDataContract(dataContract, encloseInDataContractName)));
		}

		/// <summary>
		/// Deserializes the json structure and interprets it as (and creates) a model class of the specified type.
		/// Only properties with the attribute DataMember will be read from the json string.
		/// Other data in the json will not be touched and will not cause an error if present.
		/// If Json values has the wrong datatype, they are skipped and the problem is logged but not thrown.
		/// The thought behind this method is to map anything in the json that fits the model for
		/// DataMember attributed properties. The method is recursive when the model has properties
		/// of model types or Lists of model types.
		/// </summary>
		/// <returns>
		/// A model class of the specified type, even if no matching data was found in the json.
		/// </returns>
		/// <param name="jsonString">
		/// A JsonString to deserialize. It can be enclosed in the DataContract name or not.
		/// Example 1 (not enclosed in DataContract name) {"firstName":"Ricky","lastName":"Helgesson"}
		/// Example 2 (enclosed in DataContract name) {"user":{"firstName":"Ricky","lastName":"Helgesson"}}
		/// </param>
		/// <param name="encloseInDataContractName">
		/// Specifies whether the JsonString includes the DataContract name or not.
		/// Example 1 (not enclosed in DataContract name) {"firstName":"Ricky","lastName":"Helgesson"}
		/// Example 2 (enclosed in DataContract name) {"user":{"firstName":"Ricky","lastName":"Helgesson"}}
		/// </param>
		public static TDataContract DeserializeDataContract<TDataContract>(string jsonString, bool encloseInDataContractName = false) where TDataContract : new()
		{
			return (TDataContract)DeserializeDataContract(typeof(TDataContract), jsonString, encloseInDataContractName);
		}

		public static bool TryDeserializeDataContract<TDataContract>(string jsonString, out TDataContract dataContract, bool encloseInDataContractName = false) where TDataContract : new()
		{
			dataContract = DeserializeDataContract<TDataContract>(jsonString, encloseInDataContractName);
			return !dataContract.IsDefault();
		}

		public static TDataContract DeserializeDataContract<TDataContract>(Stream jsonStream, Encoding encoding, bool encloseInDataContractName = false) where TDataContract : new()
		{
			string jsonString = new StreamReader(jsonStream).ReadToEnd();
			return (TDataContract)DeserializeDataContract(typeof(TDataContract), jsonString, encloseInDataContractName);
		}

		public static bool TryDeserializeDataContract<TDataContract>(Stream jsonStream, Encoding encoding, out TDataContract dataContract, bool encloseInDataContractName = false) where TDataContract : new()
		{
			dataContract = DeserializeDataContract<TDataContract>(jsonStream, encoding, encloseInDataContractName);
			return !dataContract.IsDefault();
		}

		public static object DeserializeDataContract(Type dataContractType, string jsonString, bool encloseInDataContractName = false)
		{
			var classAttribute = GetCustomAttribute<DataContractAttribute>(dataContractType);
			if (classAttribute == null)
				throw new ArgumentException("JsonSerializer works only with DataContracts.", "dataContract");
			
			JsonValue jsonValue = JsonValue.Parse(jsonString);
			if (encloseInDataContractName)
			{
				var dataContractName = classAttribute.Name ?? dataContractType.Name;
				if (!jsonValue.ContainsKey(dataContractName))
					throw new Exception(string.Format("Unable to deserialize json string into class {0} since the class' serialized name ({1}) does not match the json string's outer object name.", dataContractType.Name, dataContractName));
				jsonValue = jsonValue[dataContractName];
			}
			return DeserializeDataContract(jsonValue, dataContractType);
		}

		#region private

		private static string CleanForJSON(string s)
		{
			if (s == null)
			{
				return null;
			}
			if (s == "")
			{
				return "";
			}

			var		c = '\0';
			int		i;
			int		len = s.Length;
			var		sb	= new StringBuilder(len + 4);
			string	t;

			for (i = 0; i < len; i += 1) {
				c = s[i];
				switch (c) {
					case '\\':
					case '"':
						sb.Append('\\');
						sb.Append(c);
						break;
					case '/':
						sb.Append('\\');
						sb.Append(c);
						break;
					case '\b':
						sb.Append("\\b");
						break;
					case '\t':
						sb.Append("\\t");
						break;
					case '\n':
						sb.Append("\\n");
						break;
					case '\f':
						sb.Append("\\f");
						break;
					case '\r':
						sb.Append("\\r");
						break;
					default:
						if (c < ' ') {
							t = "000" + String.Format("X", c);
							sb.Append("\\u" + t.Substring(t.Length - 4));
						} else {
							sb.Append(c);
						}
						break;
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Method is needed to mimic missing method in Mono version of System.Json.
		/// </summary>
		private static bool TryCreate(object obj, out JsonPrimitive jsonPrimitive)
		{
			if (obj is bool)
				jsonPrimitive = new JsonPrimitive((bool)obj);
			else if (obj is Int16)
				jsonPrimitive = new JsonPrimitive((Int16)obj);
			else if (obj is Int32)
				jsonPrimitive = new JsonPrimitive((Int32)obj);
			else if (obj is Int64)
				jsonPrimitive = new JsonPrimitive((Int64)obj);
			else if (obj is UInt16)
				jsonPrimitive = new JsonPrimitive((UInt16)obj);
			else if (obj is UInt32)
				jsonPrimitive = new JsonPrimitive((UInt32)obj);
			else if (obj is UInt64)
				jsonPrimitive = new JsonPrimitive((UInt64)obj);
			else if (obj is Single)
				jsonPrimitive = new JsonPrimitive((Single)obj);
			else if (obj is double)
				jsonPrimitive = new JsonPrimitive((double)obj);
			else if (obj is decimal)
				jsonPrimitive = new JsonPrimitive((decimal)obj);
			else if (obj is DateTime)
				jsonPrimitive = new JsonPrimitive(((DateTime)obj).ToUniversalTime().ToString("r"));
			else if (obj is string)
				jsonPrimitive = new JsonPrimitive(CleanForJSON((string)obj));
			else if (obj is Guid)
				jsonPrimitive = new JsonPrimitive(obj.ToString());
			else
			{
				jsonPrimitive = null;
				return false;
			}
			return true;
		}

		private static JsonValue ToJsonValue(object obj)
		{
			if (obj == null)
				return null;

			var objectType = obj.GetType();
			JsonValue jsonValue = null;

			if (obj.GetType().GetTypeInfo().IsValueType || objectType == typeof(string))
			{
				JsonPrimitive jsonPrimitive;
// The following line is commented out for Mono Compatibility
//                if (JsonPrimitive.TryCreate(obj, out jsonPrimitive))
				if (TryCreate(obj, out jsonPrimitive))
					jsonValue = jsonPrimitive;
				else
					throw new ArgumentException(string.Format("Unable to create JsonPrimitive from {0}", objectType.Name));
			}
			else if (typeof(IDictionary<string, string>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
			{
				var iDict = (IDictionary<string, string>)obj;
				var list = new List<KeyValuePair<string, JsonValue>>();
				foreach (var kvp in iDict)
				{
					string key = kvp.Key;
					JsonValue value = kvp.Value == null ? null : new JsonPrimitive((string)kvp.Value);
					list.Add(new KeyValuePair<string, JsonValue>(key, value));
				}
				jsonValue = new JsonObject(list);
			}
			else if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
			{
				var list = new List<JsonValue>();
				foreach (object o in (IEnumerable)obj)
					list.Add(ToJsonValue(o));
				jsonValue = new JsonArray(list);
			}
			else
			{
				var dataContractAttribute = GetCustomAttribute<DataContractAttribute>(objectType);
				if (dataContractAttribute != null)
				{
					JsonValue subValue = DataContractToJson(obj);
					if (subValue != null)
						jsonValue = subValue;
				}
			}

			return jsonValue;
		}

		private static JsonValue DataContractToJson(object dataContractObject)
		{
			if (dataContractObject == null)
				return null;

			if (typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(dataContractObject.GetType().GetTypeInfo()))
			{
				return ToJsonValue(dataContractObject);
			}

			var jsonObject = new JsonObject();

			foreach (var property in dataContractObject.GetType().GetTypeInfo().DeclaredProperties)
			{
				var dataMemberAttribute = GetCustomAttribute<DataMemberAttribute>(property);
				if (dataMemberAttribute == null)
					continue;

				if (property.GetIndexParameters().Length != 0)
					throw new ArgumentException("JsonSerializer does not yet support indexed properties - use IList<>.", string.Format("{0}.{1}", dataContractObject.GetType().Name, property.Name));

				object value = property.GetValue(dataContractObject, null);
				if (value == null)
					continue;

				var fieldName = dataMemberAttribute.Name ?? property.Name;
				object propertyValue = property.GetValue(dataContractObject, null);

				if (propertyValue == null)
					continue;

				jsonObject.Add(fieldName, ToJsonValue(propertyValue));
			}
			return jsonObject;
		}

		/// <summary>
		/// Method is needed to mimic missing method in Mono version of System.Json.
		/// </summary>
		private static bool TryReadAs(JsonPrimitive jsonPrimitive, Type objectType, out object obj)
		{
			if (objectType == typeof(bool))
				obj = (bool)jsonPrimitive;
			else if (objectType == typeof(Int16))
				obj = (Int16)jsonPrimitive;
			else if (objectType == typeof(Int32))
				obj = (Int32)jsonPrimitive;
			else if (objectType == typeof(Int64))
				obj = (Int64)jsonPrimitive;
			else if (objectType == typeof(UInt16))
				obj = (UInt16)jsonPrimitive;
			else if (objectType == typeof(UInt32))
				obj = (UInt32)jsonPrimitive;
			else if (objectType == typeof(UInt64))
				obj = (UInt64)jsonPrimitive;
			else if (objectType == typeof(Single))
				obj = (Single)jsonPrimitive;
			else if (objectType == typeof(double))
				obj = (double)jsonPrimitive;
			else if (objectType == typeof(decimal))
				obj = (decimal)jsonPrimitive;
			else if (objectType == typeof(DateTime))
				obj = DateTime.Parse((string)jsonPrimitive);
			else if (objectType == typeof(string))
				obj = (string)jsonPrimitive;
			else if (objectType == typeof(Guid))
				obj = Guid.Parse((string)jsonPrimitive);
			else
			{
				obj = null;
				return false;
			}
			return true;
		}

		private static object FromJsonValue(JsonValue jsonValue, Type objectType)
		{
			if (jsonValue == null)
				return null;

			object obj = null;
			JsonType jsonType = jsonValue.JsonType;

			Log.Verbose("In FromJsonValue. jsonValue: {0}, objecttype: {1}", jsonValue, objectType);
			switch (jsonType)
			{
				case JsonType.Array:
				{
					if (typeof(IList).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
					{
						var list = (IList) System.Activator.CreateInstance(objectType);
						var jsonArray = (JsonArray) jsonValue;

						Log.Verbose("Json Array = {0}", jsonArray.ToString());

						foreach(JsonValue jsonValueItem in jsonArray)
						{
							Log.Verbose("- jsonValueItem {0}", jsonValueItem);
							Type type = objectType.GenericTypeArguments[objectType.GenericTypeArguments.Length-1];
								
							if (jsonValueItem is JsonObject)
							{
								list.Add(DeserializeDataContract(jsonValueItem, type));
							}
							else
							{
								list.Add(FromJsonValue(jsonValueItem, type));
							}
						}
						obj = list;
					}
					break;
				}
				case JsonType.String:
				case JsonType.Boolean:
				case JsonType.Number:
				{
					var jsonPrimitive = (JsonPrimitive) jsonValue;
					if (objectType.GetTypeInfo().IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>))
						objectType = objectType.GetTypeInfo().GenericTypeArguments[0];
					//The following line is commented out for Mono Compatibility
					//if (!jsonPrimitive.TryReadAs(objectType, out obj))
					if (!TryReadAs(jsonPrimitive, objectType, out obj))
						obj = null;
					break;
				}
				case JsonType.Object:
				{
					if (typeof(IDictionary<string, string>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo()))
					{
						var jsonObject = (JsonObject) jsonValue;
						var dictionary = new Dictionary<string, string>();
						var keys = jsonObject.Keys;
						foreach (var key in keys)
						{
							var value = (string)FromJsonValue(jsonObject[key], typeof(string));
							dictionary.Add(key, value);
						}
						obj = dictionary;
					}
					else
					{
						// Keeping old behaviour just in case, but I cannot see what good this would do.
						obj = jsonValue;
					}
					break;
				}
			}
			return obj;
		}
		
		private static object DeserializeDataContract(JsonValue jsonValue, Type dataContractType)
		{
			if (GetCustomAttribute<DataContractAttribute>(dataContractType) == null)
				throw new ArgumentException(string.Format("Unable to deserialize object {0} without the DataContract attribute.", dataContractType.Name));

			Log.Verbose("DeserializeDataContract jsonValue: {0}, datacontractType: {1}", jsonValue, dataContractType);

			object dataContractObject = null;

			if (jsonValue.JsonType == JsonType.Array)
			{
				dataContractObject = FromJsonValue(jsonValue, dataContractType);
			}

			if (jsonValue.JsonType == JsonType.Object)
			{
				dataContractObject = System.Activator.CreateInstance(dataContractType);

				Log.Verbose("casting jsonvalue to jsonObject");
				var jsonObject = (JsonObject)jsonValue;
				Log.Verbose("success");

				foreach (var property in dataContractType.GetTypeInfo().DeclaredProperties)
				{
					Log.Verbose("parsing property {0} of datacontract", property.ToString());
					var dataMemberAttribute = GetCustomAttribute<DataMemberAttribute>(property);
					if (dataMemberAttribute == null)
						continue;

					Log.Verbose("datamamber: {0}", dataMemberAttribute.ToString());

					string fieldName = dataMemberAttribute.Name ?? property.Name;
					Log.Verbose("fieldName: {0}", fieldName);

					JsonValue jsonSubValue;
					if (!jsonObject.TryGetValue(fieldName, out jsonSubValue))
						continue;

					if (jsonSubValue == null)
						continue;

					var dataContractAttribute = GetCustomAttribute<DataContractAttribute>(property.PropertyType);
					if (dataContractAttribute != null)
					{
						Log.Verbose("dataContractAttribute: {0}", dataContractAttribute.ToString());
						property.SetValue(dataContractObject, DeserializeDataContract(jsonSubValue, property.PropertyType), null);
					}
					else
					{
						Log.Verbose("jsonSubValue: {0}, propertyType: {1}", jsonSubValue, property.PropertyType);
						object obj = FromJsonValue(jsonSubValue, property.PropertyType);
						if (obj == null)
							continue;

						Log.Verbose("Setting value on property: {0} (dataContractObject: {1},  obj: {2})", property, dataContractObject, obj);
						property.SetValue(dataContractObject, obj, null);

					}
				}
			}
			Log.Verbose("returning dataContractObject");
			return dataContractObject;
		}

		/// <summary>
		/// Returns the expected attribute for the specified type or null if the attribute was not present
		/// </summary>
		private static TAttribute GetCustomAttribute<TAttribute>(PropertyInfo prop) where TAttribute : Attribute
		{
			return (TAttribute) prop.GetCustomAttribute(typeof(TAttribute));
		}

		private static TAttribute GetCustomAttribute<TAttribute>(Type type) where TAttribute : Attribute
		{
			return (TAttribute) type.GetTypeInfo().GetCustomAttribute(typeof(TAttribute));
		}

		#endregion private

		static private readonly Logger Log = L.O.G(typeof(JsonSerializer));
	}
}
