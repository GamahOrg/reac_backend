using Microsoft.EntityFrameworkCore;

namespace Core.GenericRepository.Shared.Pagging
{
    public static class IPaggingExtention
    {
        public static PagedResult<T> GetPaged<T>(this IQueryable<T> query, int page, int pagesize) where T : class
        {
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pagesize,
                TotalCount = query.CountAsync().Result
            };
            var pageCount = (double)result.TotalCount / pagesize;
            result.PageCount = (int)Math.Ceiling(pageCount);

            var skip = (page - 1) * pagesize;
            result.Results = query.Skip(skip).Take(pagesize).ToListAsync().Result;
            return result;
        }

    }
}
