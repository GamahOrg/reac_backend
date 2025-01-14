using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Core.GenericRepository.Common
{
    /// <summary>
    /// This object hold the query specifications.
    /// </summary>
    /// <typeparam name="TEntity">The database entity.</typeparam>
    public class BaseSpecification<TEntity, TPrimaryKey>
        where TEntity : BaseEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets or sets the <see cref="Expression{TDelegate}"/> list you want to pass with your EF Core query.
        /// </summary>
        public List<Expression<Func<TEntity, bool>>> Conditions { get; set; } = new List<Expression<Func<TEntity, bool>>>();

        /// <summary>
        /// Gets or sets the navigation entities to be eager loadded with EF Core query.
        /// </summary>
        public Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> Includes { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Func{T, TResult}"/> to order by your query.
        /// </summary>
        public Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> OrderBy { get; set; }

        /// <summary>
        /// Gets or sets dynmic order by option in string format.
        /// </summary>
        public (string ColumnName, string SortDirection) OrderByDynamic { get; set; }
    }
}
