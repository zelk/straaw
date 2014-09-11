﻿using Straaw.Framework.Json;
using Straaw.Framework.Logging;
using Straaw.Framework.Model;
using System;
using System.Net.Http;
using System.Text;

namespace Straaw.Framework.RestClient
{
	public abstract class RestClientResponseBase
		public HttpResponseMessage ResponseMessage { get; private set; }

	public class RestClientResponse : RestClientResponseBase

	public class RestClientResponse<TResult> : RestClientResponse
	{
		internal RestClientResponse(RestClientResponse restClientResponse, Encoding encoding)
			if (encoding == null)
				throw new NullReferenceException();
			Encoding = encoding;
		}
		internal RestClientResponse(HttpResponseMessage responseMessage, byte[] responseBytes, Encoding encoding)
				throw new NullReferenceException();

			Encoding = encoding;
		public TResult ResponseObject
					var responseString = this.Encoding.GetString(ResponseBytes, 0, ResponseBytes.Length); 
					_responseObject = JsonSerializer.DeserializeDataContract<TResult>(responseString, false);
				}
	}
}