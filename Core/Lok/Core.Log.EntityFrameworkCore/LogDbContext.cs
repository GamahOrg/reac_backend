using Core.GenericRepository;
using Core.Log.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace Core.Log.EntityFrameworkCore
{
    public class LogDbContext : AppDbContext
    {

        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<DbLog> DbLogs { get; set; }

    }
}
