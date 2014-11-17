using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

using Straaw.Framework.Model;
using Straaw.Framework.Logging;
using Straaw.Framework.Json;

namespace Straaw.Framework.RestClient
{
	public class RestClientRequest
	{
		internal RestClientRequest(HttpClient httpClient, HttpMethod method, string resource)
		{
			if (httpClient == null)
			{
				throw new NullReferenceException();
			}
			_httpClient = httpClient;

// FIXME: This is not what I wanted... I want to create a relative URL here... I think...
			var uri = new Uri(httpClient.BaseAddress + resource);

			RequestMessage = new HttpRequestMessage(method, uri);
		}

		public HttpRequestMessage RequestMessage { get; private set; }

		public async Task<RestClientResponse> ExecuteAsync(byte[] requestBody = null, CacheMethod cacheMethod = CacheMethod.PreferOnline)
		{
			if (RequestMessage.Method != HttpMethod.Get && cacheMethod != CacheMethod.Default)
			{
				throw new Exception(string.Format("Http method {0} does not support CacheMethod {1}.", RequestMessage.Method.ToString(), cacheMethod.ToString()));
			}

			Log.Info("HTTP {0} {1} (CacheMethod={2})", RequestMessage.Method.ToString(), RequestMessage.RequestUri.ToString(), cacheMethod.ToString());
			
			byte[] responseBody = null;
			HttpResponseMessage httpResponseMessage = null;
			
			CacheControlHeaderValue cacheControl = new CacheControlHeaderValue();
			RequestMessage.Headers.CacheControl = cacheControl;
			switch (cacheMethod)
			{
				case CacheMethod.OnlyCached:
				{
					cacheControl.OnlyIfCached = (cacheMethod == CacheMethod.OnlyCached);
					break;
				}
				case CacheMethod.PreferCached:
				{
					cacheControl.Extensions.Add(new NameValueHeaderValue("Straaw", "PreferCached"));
					break;
				}
				case CacheMethod.Default:
				{
					break;
				}
				case CacheMethod.PreferOnline:
				{
					cacheControl.Extensions.Add(new NameValueHeaderValue("Straaw", "PreferOnline"));
					break;
				}
				case CacheMethod.OnlyOnline:
				{
					cacheControl.NoCache = (cacheMethod == CacheMethod.OnlyOnline);
					break;
				}
			}

			if (requestBody != null)
			{
				RequestMessage.Content = new ByteArrayContent(requestBody);
				RequestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			}

			using (Log.Scope(LogLevel.Verbose, "HTTP Request Timing"))
			{
				try
				{
					httpResponseMessage = await _httpClient.SendAsync(RequestMessage, HttpCompletionOption.ResponseHeadersRead);
				}
				catch (Exception e)
				{
					Log.Warning(e.Message);
					throw;
				}
			}

			using (Log.Scope(LogLevel.Verbose, "HTTP Response Timing"))
			{
				if (httpResponseMessage.Content != null)
				{
					responseBody = await httpResponseMessage.Content.ReadAsByteArrayAsync();
				}
			}

			if (responseBody != null && responseBody.Length > 0)
			{
				Log.Info("HTTP Response body {0} bytes: {1}", responseBody.Length, Encoding.UTF8.GetString(responseBody, 0, responseBody.Length));
			}
			else
			{
				Log.Info("HTTP Response body 0 bytes");
			}

			return new RestClientResponse(httpResponseMessage, responseBody);
		}

		public void AddBasicAuthentication(string username, string password)
		{
			var authString = string.Format("{0}:{1}", username, password);
			var authBytes = Encoding.UTF8.GetBytes(authString);
			var encodedAuthString = Convert.ToBase64String(authBytes);
			RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedAuthString);
		}

		public async Task<RestClientResponse<TResult>> ExecuteAsync<TResult>(Encoding encoding, CacheMethod cacheMethod = CacheMethod.PreferOnline)
			where TResult : IMutableModel, new()
		{
			if (encoding == null)
				throw new NullReferenceException();
		
			var restClientResponse = await ExecuteAsync(null, cacheMethod);
			return new RestClientResponse<TResult>(restClientResponse, encoding);
		}

		static private readonly Logger Log = L.O.G(typeof(RestClientRequest));
		private readonly HttpClient _httpClient;
	}

// ************************************************************************************************
// ************************************************************************************************

	public class RestClientRequest<TArgument> : RestClientRequest
		where TArgument : IImmutableModel
	{
		internal RestClientRequest(HttpClient httpClient, HttpMethod method, string resource, Encoding encoding)
			: base(httpClient, method, resource)
		{
			if (encoding == null)
				throw new NullReferenceException();

			_encoding = encoding;
		}

		public async Task<RestClientResponse<TResult>> ExecuteAsync<TResult>(TArgument argument, CacheMethod cacheMethod = CacheMethod.PreferOnline)
			where TResult : IMutableModel, new()
		{
			byte[] requestBytes = null;

			if (argument != null)
			{
				requestBytes = _encoding.GetBytes(JsonSerializer.SerializeDataContract(argument, false));
			}

			var restClientResponse = await ExecuteAsync(requestBytes, cacheMethod);

			return new RestClientResponse<TResult>(restClientResponse, _encoding);
		}

		public async Task<RestClientResponse> ExecuteAsync(TArgument argument, CacheMethod cacheMethod = CacheMethod.PreferOnline)
		{
			byte[] requestBytes = null;

			if (argument != null)
			{
				requestBytes = _encoding.GetBytes(JsonSerializer.SerializeDataContract(argument, false));
			}

			return await ExecuteAsync(requestBytes, cacheMethod);
		}

		private readonly Encoding _encoding;

	}
}
