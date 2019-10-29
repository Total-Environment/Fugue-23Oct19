using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.RestWebClient
{
	public class RestWebClient<T> : IWebClient<T>
	{
		private readonly HttpClient _client;
		private readonly string _baseUrl;
		private AuthenticationResult _authenticationResult;

		public RestWebClient(string baseUrl)
		{
			_baseUrl = baseUrl;
			_client = new HttpClient();
		}

		public async Task<RestClientResponse<T>> Post(T payload, string relativeUrl)
		{
			var payloadString = JsonConvert.SerializeObject(payload);
			HttpContent httpContent = new StringContent(payloadString);
			httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

			HttpResponseMessage response = null;
			try
			{
				response = await _client.PostAsync(new Uri($"{_baseUrl}/{relativeUrl}"), httpContent);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return await GetResponseObject<T>(response);
		}

		public async Task<RestClientResponse<T>> Patch(T payload, string relativeUrl)
		{
			var body = JsonConvert.SerializeObject(payload);
			HttpContent content = new StringContent(body);
			content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
			var request = new HttpRequestMessage(new HttpMethod("Patch"), $"{_baseUrl}/{relativeUrl}") { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
			var response = await _client.SendAsync(request);
			return await GetResponseObject<T>(response);
		}

		private async Task<RestClientResponse<T>> GetResponseObject<T>(HttpResponseMessage response)
		{
			var responseBody = await response.Content.ReadAsStringAsync();

			if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.MultipleChoices)
			{
				throw new HttpResponseException(response);
			}
			var body = JsonConvert.DeserializeObject<T>(responseBody, new StringEnumConverter());
			return new RestClientResponse<T>(response.StatusCode, body);
		}

		public async Task<RestClientResponse<T>> Get(string url)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
			var response = await _client.GetAsync(new Uri($"{_baseUrl}/{url}"));
			var contentDeserialized = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
			return new RestClientResponse<T>(response.StatusCode, contentDeserialized);
		}

		public async Task<RestClientResponse<IEnumerable<T>>> Gets(string url)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
			var response = await _client.GetAsync(new Uri($"{_baseUrl}/{url}"));
			var contentDeserialized = JsonConvert.DeserializeObject<IEnumerable<T>>(response.Content.ReadAsStringAsync().Result);
			return new RestClientResponse<IEnumerable<T>>(response.StatusCode, contentDeserialized);
		}

		public async Task<RestClientResponse<T>> Put(string id, T payload, string relativeUrl)
		{
			var payloadString = JsonConvert.SerializeObject(payload);
			HttpContent httpContent = new StringContent(payloadString);
			httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());

			var response = await _client.PutAsync(new Uri($"{_baseUrl}/{relativeUrl}/{id}"), httpContent);
			return await GetResponseObject<T>(response);
		}

		public async Task<RestClientResponse<List<T>>> GetAll(string url)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
			var response = await _client.GetAsync(new Uri($"{_baseUrl}/{url}"));
			var contentDeserialized = JsonConvert.DeserializeObject<List<T>>(response.Content.ReadAsStringAsync().Result);
			return new RestClientResponse<List<T>>(response.StatusCode, contentDeserialized);
		}

		public async Task<RestClientResponse<T>> FindBy(string url)
		{
			_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetAccessToken());
			var response = await _client.GetAsync(new Uri($"{_baseUrl}/{url}"));
			var contentDeserialized = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
			return new RestClientResponse<T>(response.StatusCode, contentDeserialized);
		}

		public Task<RestClientResponse<T>> PostTest(string payload, string relativeUrl)
		{
			throw new NotImplementedException();
		}

		Task<RestClientResponse<IEnumerable<T>>> IWebClient<T>.PostFile(string filePath, string relativeUrl)
		{
			throw new NotImplementedException();
		}

		public async Task<RestClientResponse<PaginatedAndSortedListDto<T>>> GetPage(string url)
		{
			throw new NotImplementedException();
		}

		public async Task<RestClientResponse<T>> Post(object payload, string relativeUrl)
		{
			var payloadString = JsonConvert.SerializeObject(payload);
			HttpContent httpContent = new StringContent(payloadString);
			httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

			HttpResponseMessage response = null;
			try
			{
				response = await _client.PostAsync(new Uri($"{_baseUrl}/{relativeUrl}"), httpContent);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return await GetResponseObject<T>(response);
		}

		public Task<RestClientResponse<object>> PostWithoutParse(object payload, string relativeUrl)
		{
			throw new NotImplementedException();
		}

		private string GetAccessToken()
		{
			//if ((_authenticationResult != null) && (_authenticationResult.ExpiresOn > DateTimeOffset.UtcNow))
			//	return _authenticationResult.AccessToken;

			//var aadInstance = ConfigurationManager.AppSettings["AADInstance"];
			//var tenant = ConfigurationManager.AppSettings["Tenant"];
			//var authority = string.Format(CultureInfo.InvariantCulture, aadInstance, tenant);
			//var authContext = new AuthenticationContext(authority);
			//var apiResourceId = ConfigurationManager.AppSettings["ApiResourceId"];
			//var clientId = ConfigurationManager.AppSettings["ClientId"];
			//var appKey = ConfigurationManager.AppSettings["AppKey"];
			//var clientCredential = new ClientCredential(clientId, appKey);

			//try
			//{
			//	_authenticationResult = authContext.AcquireTokenAsync(apiResourceId, clientCredential).Result;
			//}
			//catch (AggregateException aggregateException)
			//{
			//	Console.WriteLine(string.Join(",", aggregateException.InnerExceptions.Select(ie => ie.Message)));
			//}
			//return _authenticationResult?.AccessToken;
		    return "NO_TOKEN";
		}
	}
}