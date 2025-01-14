namespace Core.GenericRepository.Shared.Pagging
{
    public class PagedResult<T> : PagedResultBase where T : class
    {
        /// <summary>
        /// Result data
        /// </summary>
        public IList<T> Results { get; set; }
        /// <summary>
        /// Initialize Results
        /// </summary>
        public PagedResult(List<T> items)
        {
            Results = new List<T>();
        }

        public PagedResult()
        {
        }
    }
}
