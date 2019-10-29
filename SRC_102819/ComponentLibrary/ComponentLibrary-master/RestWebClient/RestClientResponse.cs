using System.Net;

namespace TE.ComponentLibrary.RestWebClient
{
    public class RestClientResponse<T>
    {
        public RestClientResponse(HttpStatusCode statusCode, T body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public T Body { get; }
        public HttpStatusCode StatusCode { get; }
    }
}