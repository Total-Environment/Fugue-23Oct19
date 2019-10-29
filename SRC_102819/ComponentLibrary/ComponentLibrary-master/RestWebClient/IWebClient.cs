using System.Collections.Generic;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Controller.Dto;

namespace TE.ComponentLibrary.RestWebClient
{
    public interface IWebClient<T>
    {
        Task<RestClientResponse<T>> Post(T payload, string relativeUrl);

        Task<RestClientResponse<T>> Get(string url);

        Task<RestClientResponse<IEnumerable<T>>> Gets(string url);

        Task<RestClientResponse<PaginatedAndSortedListDto<T>>> GetPage(string url);

        Task<RestClientResponse<T>> Put(string id, T payload, string relativeUrl);

        Task<RestClientResponse<T>> FindBy(string url);

        Task<RestClientResponse<T>> PostTest(string payload, string relativeUrl);

        Task<RestClientResponse<T>> Patch(T payload, string relativeUrl);

        Task<RestClientResponse<IEnumerable<T>>> PostFile(string filePath, string relativeUrl);

        Task<RestClientResponse<T>> Post(object payload, string relativeUrl);
        Task<RestClientResponse<object>> PostWithoutParse(object payload, string relativeUrl);
    }
}