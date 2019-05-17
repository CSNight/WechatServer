using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using WebApi.Model;

namespace WebApi.Utils
{
	public class ApiClient
	{
		public static class HttpClientHelper
		{
			private static readonly object LockObj = new object();

			private static HttpClient _client;

			public static HttpClient HttpClient
			{
				get
				{
					if (_client == null)
					{
						lock (LockObj)
						{
							if (_client == null)
							{
								_client = new HttpClient(new HttpClientHandler
								{
									AutomaticDecompression = DecompressionMethods.GZip
								});
								_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
							}
						}
					}
					return _client;
				}
			}
		}

		private static volatile ApiClient instance;

		private static readonly object obj = new object();

		public static ApiClient Instance
		{
			get
			{
				if (instance == null)
				{
					lock (obj)
					{
						if (instance == null)
						{
							instance = new ApiClient();
						}
					}
				}
				return instance;
			}
		}

		private ApiClient()
		{
		}

		public object Post(string url, object outPutView)
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(outPutView.GetType());
			using (MemoryStream memoryStream = new MemoryStream())
			{
				dataContractJsonSerializer.WriteObject((Stream)memoryStream, outPutView);
				memoryStream.Position = 0L;
				using (HttpContent httpContent = new StreamContent(memoryStream))
				{
					httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
					HttpResponseMessage httpResponseMessage = null;
					try
					{
						Task<HttpResponseMessage> task = HttpClientHelper.HttpClient.PostAsync(url, httpContent);
						task.Wait();
						httpResponseMessage = task.Result;
						new object();
						if (httpResponseMessage.IsSuccessStatusCode)
						{
							Task<string> task2 = httpResponseMessage.Content.ReadAsStringAsync();
							task2.Wait();
							return JsonConvert.DeserializeObject<ApiServerMsg>(task2.Result);
						}
						return new object();
					}
					catch (Exception)
					{
						return new object();
					}
					finally
					{
						httpResponseMessage?.Dispose();
					}
				}
			}
		}

		/// <summary>
		/// 请求服务器的Get方式
		/// </summary>
		/// <param name="url">服务器请求地址</param>
		/// <returns></returns>
		public object Get(string url)
		{
			HttpResponseMessage httpResponseMessage = null;
			try
			{
				Task<HttpResponseMessage> async = HttpClientHelper.HttpClient.GetAsync(url);
				async.Wait();
				httpResponseMessage = async.Result;
				new object();
				if (httpResponseMessage.IsSuccessStatusCode)
				{
					Task<string> task = httpResponseMessage.Content.ReadAsStringAsync();
					task.Wait();
					return JsonConvert.DeserializeObject<ApiServerMsg>(task.Result);
				}
				return new object();
			}
			catch (Exception)
			{
				return new object();
			}
			finally
			{
				httpResponseMessage?.Dispose();
			}
		}
	}
}
