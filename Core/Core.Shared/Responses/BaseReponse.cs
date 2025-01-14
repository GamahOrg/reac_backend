namespace Core.Shared.Responses
{
    public class BaseResponse 
    {
        public BaseResponse()
        {
        }

        public BaseResponse(bool isDone)
        {
            IsDone = isDone;
        }

        public BaseResponse(bool isDone, string message)
        {
            IsDone = isDone;
            Message = message;
        }

        /// <summary>
        /// Flage to represent process status
        /// </summary>
        public bool IsDone { get; set; }

        /// <summary>
        /// Process Return Message
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}