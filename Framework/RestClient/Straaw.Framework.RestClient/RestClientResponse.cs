using System.Diagnostics;
using Straaw.Framework.Json;
using Straaw.Framework.Logging;
using Straaw.Framework.Model;
using System;
using System.Net.Http;
using System.Text;

public static class ClassExtensions
{
	public static bool IsDefault<TValue>(this TValue self)
	{
		if (ReferenceEquals(self, null))
		{
			return true;
		}
		return self.Equals(default(TValue));
	}
}

namespace Straaw.Framework.RestClient
{
	public abstract class RestClientResponseBase
	{
		internal RestClientResponseBase(HttpResponseMessage responseMessage)
		{
			ResponseMessage = responseMessage;
		}

		public HttpResponseMessage ResponseMessage { get; private set; }
	}

	public class RestClientResponse : RestClientResponseBase
	{
		internal RestClientResponse(HttpResponseMessage responseMessage, byte[] responseBytes)
			: base(responseMessage)
		{
			ResponseBytes = responseBytes;
		}

		public byte[] ResponseBytes { get; private set; }
	}

	public class RestClientResponse<TResult> : RestClientResponse
		where TResult : IMutableModel, new()
	{

		internal RestClientResponse(RestClientResponse restClientResponse, Encoding encoding)
			: base(restClientResponse.ResponseMessage, restClientResponse.ResponseBytes)
		{
			if (encoding == null)
				throw new NullReferenceException();

			Encoding = encoding;
		}

		internal RestClientResponse(HttpResponseMessage responseMessage, byte[] responseBytes, Encoding encoding)
			: base(responseMessage, responseBytes)
		{
			if (encoding == null)
				throw new NullReferenceException();

			Encoding = encoding;
		}

		public TResult ResponseObject
		{
			get
			{
				if (_responseObject.IsDefault())
				{
					// We need to do this lazy read here, since an unsuccessful http request has no data so the
					// calling code can check the HttpStatus and never read the ResponseObject in such a case.
					if (ResponseBytes == null)
					{
						return default(TResult);
					}
					var responseString = this.Encoding.GetString(ResponseBytes, 0, ResponseBytes.Length); 
					_responseObject = JsonSerializer.DeserializeDataContract<TResult>(responseString, false);
				}
				return _responseObject;
			}
		}

		TResult _responseObject;
		readonly Encoding Encoding;
		static readonly Logger Log = L.O.G(typeof(RestClientResponse<TResult>));
	}
}
