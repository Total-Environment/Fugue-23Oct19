using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;
using TE.ComponentLibrary.RestWebClient;

namespace TE.ComponentLibrary.TestWebClient
{
    public class RestClient<T> : IWebClient<T>
    {
        private readonly HttpClient _client;

        public RestClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<RestClientResponse<T>> Get(string url)
        {
            var response = await _client.GetAsync(url);
            var result = await GetResponseObject<T>(response);
            return result;
        }

        public async Task<RestClientResponse<IEnumerable<T>>> Gets(string url)
        {
            var response = await _client.GetAsync(url);
            var result = await GetResponseObject<IEnumerable<T>>(response);
            return result;
        }

        public async Task<RestClientResponse<T>> Put(string id, T payload, string relativeUrl)
        {
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"{relativeUrl}/{id}") { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<T>(response);
        }

        public Task<RestClientResponse<T>> FindBy(string url)
        {
            throw new NotImplementedException();
        }

        private static async Task<RestClientResponse<T>> GetResponseObject<T>(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new RestClientResponse<T>(response.StatusCode, default(T));
            }

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return new RestClientResponse<T>(response.StatusCode, default(T));
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                return new RestClientResponse<T>(response.StatusCode, default(T));
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.MultipleChoices)
            {
                throw new ArgumentException(responseBody);
            }
            var body = JsonConvert.DeserializeObject<T>(responseBody, new StringEnumConverter());
            return new RestClientResponse<T>(response.StatusCode, body);
        }

        public async Task<RestClientResponse<T>> Post(T payload, string relativeUrl)
        {
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl) { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<T>(response);
        }

        public async Task<RestClientResponse<T>> Patch(T payload, string relativeUrl)
        {
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(new HttpMethod("Patch"), relativeUrl) { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<T>(response);
        }

        public async Task<RestClientResponse<IEnumerable<T>>> PostFile(string filePath, string relativeUrl)
        {
            var multiPartContent = new MultipartFormDataContent
            {
                {
                    new StreamContent(File.OpenRead(filePath)), "image",
                    filePath.Split('\\').Last()
                }
            };
            var request = new HttpRequestMessage(new HttpMethod("Post"), relativeUrl) { Content = multiPartContent, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("multipart/form-data") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<IEnumerable<T>>(response);
        }

        public async Task<RestClientResponse<T>> PostTest(string payload, string relativeUrl)
        {
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl) { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<T>(response);
        }

        public async Task<RestClientResponse<T>> Put<T>(string url, string body)
        {
            var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Put, url));
            return await GetResponseObject<T>(response);
        }

        public async Task<RestClientResponse<PaginatedAndSortedListDto<T>>> GetPage(string url)
        {
            var response = await _client.GetAsync(url);
            var contentDeserialized = JsonConvert.DeserializeObject<PaginatedAndSortedListDto<T>>(response.Content.ReadAsStringAsync().Result);
            return new RestClientResponse<PaginatedAndSortedListDto<T>>(response.StatusCode, contentDeserialized);
        }

        public async Task<RestClientResponse<T>> Post(object payload, string relativeUrl)
        {
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl) { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<T>(response);
        }

        public async Task<RestClientResponse<object>> PostWithoutParse(object payload, string relativeUrl)
        {
            var body = JsonConvert.SerializeObject(payload);
            var content = new StringContent(body, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, relativeUrl) { Content = content, Headers = { Accept = { MediaTypeWithQualityHeaderValue.Parse("application/json") } } };
            var response = await _client.SendAsync(request);
            return await GetResponseObject<object>(response);
        }
    }
}