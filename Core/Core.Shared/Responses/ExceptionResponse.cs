namespace Core.Shared.Responses
{
    public class ExceptionResponse : BaseResponse 
    {
        public ExceptionResponse()
        {
        }

        public ExceptionResponse(string exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Process Return Exception
        /// </summary>
        public string? Exception { get; set; }
        
    }
}