namespace Core.Shared.Responses
{
    public class DataResponse<T> : BaseResponse
    {
        public DataResponse()
        {
        }

        public DataResponse(T data, bool isDone, string message)
        {
            Data = data;
            IsDone = isDone;
            Message = message;
        }

        /// <summary>
        /// Process returned data
        /// </summary>
        public T? Data { get; set; }
    }
}