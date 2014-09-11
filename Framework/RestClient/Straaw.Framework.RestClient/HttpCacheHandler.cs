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
				Log.Debug("Did read {0} bytes from cache.", cachedBody == null ? 0 : cachedBody.Length);
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
	
	// TODO: Remove the commented out stuff here as soon as the server stops sending NoCache!!!
				if (/*!responseCacheControl.NoCache &&*/ request.Method == HttpMethod.Get)
				{
					_httpCacheStore.Write(uriString, onlineResponseBody);
					Log.Debug("Did store {0} bytes in the cache.", onlineResponseBody == null ? 0 : onlineResponseBody.Length);
				}
				else if (responseCacheControl.NoCache)
				{
					Log.Debug("Skipping storing data in the cache, since the server specifically requests that nothing must be cached.");
				}
			}
			catch(Exception)
			{
				if (!requestCacheControl.NoCache)
				{
					response = new HttpResponseMessage();
					response.StatusCode = HttpStatusCode.OK;
					response.Content = new ByteArrayContent(cachedBody);
					
					return response;
				}
				else
				{
					response = new HttpResponseMessage();
					response.StatusCode = HttpStatusCode.GatewayTimeout;
	
					return response;
				}
			}
			
			return response;
	    }
		
		private IHttpCacheStore _httpCacheStore;
		private static readonly Logger Log = L.O.G(typeof(HttpCacheHandler));
	}
}

