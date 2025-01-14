using Microsoft.EntityFrameworkCore;

namespace Core.GenericRepository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected  delegate void ExceptionHandler(Exception exception);
        protected  event ExceptionHandler OnException;
    }
}
