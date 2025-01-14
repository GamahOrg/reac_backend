using System.Net;

namespace Core.Shared.Responses
{
    public class HttpExceptionResponse : ExceptionResponse
    {
        /// <summary>
        /// Process returned http status code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
