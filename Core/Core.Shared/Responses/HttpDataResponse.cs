using System.Net;

namespace Core.Shared.Responses
{
    public class HttpDataResponse<T> : DataResponse<T>
    {
        /// <summary>
        /// Process returned http status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
