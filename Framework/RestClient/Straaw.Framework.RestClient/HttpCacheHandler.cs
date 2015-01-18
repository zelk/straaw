using Straaw.Framework.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

namespace Straaw.Framework.RestClient
{
	public class HttpCacheHandler : DelegatingHandler
	{
		public HttpCacheHandler(IHttpCacheStore httpCacheStore)
		{
			if (httpCacheStore == null)
			{
				throw new NullReferenceException();
			}
			_httpCacheStore = httpCacheStore;
		}

	    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	    {
			Log.Verbose("Incoming request");

			CacheControlHeaderValue requestCacheControl = request.Headers.CacheControl;
			string uriString = request.RequestUri.ToString();

			HttpResponseMessage response = null;
			byte[] cachedBody = null;

			if (request.Method == HttpMethod.Get && !requestCacheControl.NoCache)
			{
				cachedBody = _httpCacheStore.Read(uriString);
				if (cachedBody != null)
				{
					Log.Debug("Did read {0} bytes from cache.", cachedBody.Length);
				}
				else
				{
					Log.Verbose("Found nothing in the http cache.");
				}
			}

			if (cachedBody != null)
			{
				if (!requestCacheControl.Extensions.Contains(new NameValueHeaderValue("Straaw", "PreferOnline")))
				{
					response = new HttpResponseMessage();
					response.StatusCode = HttpStatusCode.OK;
					response.Content = new ByteArrayContent(cachedBody);
					
					return response;
				}
			}

			if (requestCacheControl.OnlyIfCached)
			{
				response = new HttpResponseMessage();
				response.StatusCode = HttpStatusCode.GatewayTimeout;

				return response;
			}

			try
			{
				Log.Verbose("Starting online request");
		        response = await base.SendAsync(request, cancellationToken);
				byte[] onlineResponseBody = await response.Content.ReadAsByteArrayAsync();
				CacheControlHeaderValue responseCacheControl = response.Headers.CacheControl;
				Log.Verbose("Finished online request");
	
				if (!(responseCacheControl != null && responseCacheControl.NoCache) && request.Method == HttpMethod.Get)
				{
					_httpCacheStore.Write(uriString, onlineResponseBody);
					Log.Debug("Did store {0} bytes in the cache.", onlineResponseBody == null ? 0 : onlineResponseBody.Length);
				}
				else if (responseCacheControl != null && responseCacheControl.NoCache)
				{
					Log.Debug("Skipping storing data in the cache, since the server specifically requests that nothing must be cached.");
				}
			}
			catch(Exception e)
			{
				if (!requestCacheControl.NoCache && cachedBody != null)
				{
					Log.Warning("Online request failed but returning cached version. {0}", e);

					response = new HttpResponseMessage();
					response.StatusCode = HttpStatusCode.OK;
					response.Content = new ByteArrayContent(cachedBody);

					return response;
				}

				throw;
			}
			
			return response;
	    }
		
		private IHttpCacheStore _httpCacheStore;
		private static readonly Logger Log = L.O.G(typeof(HttpCacheHandler));
	}
}

