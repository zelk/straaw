using System;
using System.Text;
using System.Net.Http;
using Straaw.Framework.Model;
namespace Straaw.Framework.RestClient
{
	/// <summary>
	/// Subclass this for your project and override CreateRestClientRequest to add project specific stuff to the HttpWebRequest.
	/// Subclass your project specific class for each resource that you wish to access - e.g. first MyProjectRestClient and then
	/// MyResourceRestClient derives from that to add even more specific settings and methods.
	/// </summary>
	public abstract class RestClientBase
	{
		protected RestClientBase(string baseAddress, Encoding encoding, IHttpCacheStore httpCacheStore, string userAgent = null)
		{
			if (encoding == null)
			{
				throw new NullReferenceException();
			}

			if (httpCacheStore != null)
			{
				HttpClient = new HttpClient
				(
					new HttpCacheHandler(httpCacheStore)
					{
						InnerHandler = new HttpClientHandler()
					}
				);
			}
			else
			{
				HttpClient = new HttpClient();
			}
			HttpClient.BaseAddress = new Uri(baseAddress);
			HttpClient.Timeout = TimeSpan.FromMilliseconds (10000);
//			HttpClient.DefaultRequestHeaders.UserAgent = userAgent;

			UserAgent = userAgent;
			BaseAddress = baseAddress;
			Encoding = encoding;
		}

		protected virtual RestClientRequest CreateRestClientRequest(HttpMethod method, string resourceFormatString, params object[] formatArguments)
		{
			var r = new RestClientRequest(HttpClient, method, string.Format(resourceFormatString, formatArguments));
			return r;
		}

		protected virtual RestClientRequest<TArgument> CreateRestClientRequest<TArgument>(HttpMethod method, string resourceFormatString, params object[] formatArguments)
			where TArgument : IImmutableModel
		{
			return new RestClientRequest<TArgument>(HttpClient, method, string.Format(resourceFormatString, formatArguments), Encoding);
		}

		protected HttpClient HttpClient { get; private set; }
		protected string UserAgent { get; private set; }
		protected string BaseAddress { get; private set; }
		protected Encoding Encoding { get; private set; }
	}

}
