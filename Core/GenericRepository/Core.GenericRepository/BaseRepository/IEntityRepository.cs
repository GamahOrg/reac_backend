using Core.GenericRepository.Common;
using Core.GenericRepository.Shared.BaseEntities;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
// ReSharper disable InvalidXmlDocComment

namespace Core.GenericRepository.BaseRepository
{
    /// <summary>
    /// Contains all the repository methods. If you register the multiple DbContexts, it will use the last one.
    /// To use specific <see cref="DbContext"/> please use <see cref="IRepository{TDbContext}"/>.
    /// </summary>
    public interface IEntityRepository : IEntityRepositoryEvents
    {
        /// <summary>
        /// Begin a new database transaction.
        /// </summary>
        /// <param name="isolationLevel"><see cref="IsolationLevel"/> to be applied on this transaction. (Default to <see cref="IsolationLevel.Unspecified"/>).</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns a <see cref="IDbContextTransaction"/> instance.</returns>
        Task<BaseResponse> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Reset the DbContext state by removing all the tracked and attached entities.
        /// </summary>
        BaseResponse ClearChangeTracker();
        Task<BaseResponse> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<BaseResponse> SaveChangesAsync();
        #region CRUD Functions
        /// <summary>
        /// Execute raw sql command against the configured database asynchronously.
        /// </summary>
        /// <param name="sql">The sql string.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> ExecuteSqlCommandAsync(string sql, CancellationToken cancellationToken = default);

        /// <summary>
        /// Execute raw sql command against the configured database asynchronously.
        /// </summary>
        /// <param name="sql">The sql string.</param>
        /// <param name="parameters">The paramters in the sql string.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> ExecuteSqlCommandAsync(string sql, params object[] parameters);

        /// <summary>
        /// Execute raw sql command against the configured database asynchronously.
        /// </summary>
        /// <param name="sql">The sql string.</param>
        /// <param name="parameters">The paramters in the sql string.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default);

        // Newly added methods

        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/> object, mark the object as <see cref="EntityState.Added"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity"/> to be added.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntity"/> object to be inserted to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> AddAsync<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an object of <typeparamref name="TEntity"/>, adds it to the change tracker and will
        /// be inserted into the database when <see cref="IRepository.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/>.</returns>
        Task<BaseResponse> AddAsync<TEntity, TPrimaryKey>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntity}"/> objects, mark the objects as <see cref="EntityState.Added"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entities"/> to be added.</typeparam>
        /// <param name="entities">The <typeparamref name="TEntity"/> objects to be inserted to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> AddRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes a collection of <typeparamref name="TEntity"/> object, adds them to the change tracker and will
        /// be inserted into the database when <see cref="IRepository.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The entities to be inserted.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/>.</returns>
        Task<BaseResponse> AddRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/> object, mark the object as <see cref="EntityState.Modified"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity"/> to be marked as modified.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntity"/> object to be updated to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        BaseResponse Update<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntity}"/> objects, mark the objects as <see cref="EntityState.Modified"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entities"/> to be marked as modified.</typeparam>
        /// <param name="entities">The entity objects to be updated to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        BaseResponse UpdateRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <typeparamref name="TEntity"/> object, mark the object as <see cref="EntityState.Deleted"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entity"/> to be marked as deleted.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntity"/> object to be deleted from the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        BaseResponse Remove<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntity}"/> objects, mark the objects as <see cref="EntityState.Deleted"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the <paramref name="entities"/> to be marked as deleted.</typeparam>
        /// <param name="entities">The <typeparamref name="TEntity"/> objects to be deleted from the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        BaseResponse RemoveRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>;
        #endregion
        #region Query Functions

        /// <summary>
        /// Gets <see cref="IQueryable{T}"/> of the entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPrimaryKey"></typeparam>
        /// <returns>Returns <see cref="IQueryable{T}"/>.</returns>
        Task<IQueryable<TEntity>> GetQueryable<TEntity, TPrimaryKey>()
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method returns <see cref="List{T}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method returns <see cref="List{T}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(bool asNoTracking, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method returns <see cref="List{T}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="includes">Navigation properties to be loaded.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method returns <see cref="List{T}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="includes">Navigation properties to be loaded.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method takes a <see cref="Expression{Func}"/> as parameter and returns <see cref="List{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition on which entity list will be returned.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="List{TEntity}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes a <see cref="Expression{Func}"/> as parameter and returns <see cref="List{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition on which entity list will be returned.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="List{TEntity}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes a <see cref="Expression{Func}"/> as parameter and returns <see cref="List{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition on which entity list will be returned.</param>
        /// <param name="includes">Navigation properties to be loaded.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="List{TEntity}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an object of <see cref="Specification{TEntity}"/> as parameter and returns <see cref="List{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> <see cref="object"/> which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="List{TEntity}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(Specification<TEntity, TPrimaryKey> specification, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an object of <see cref="Specification{TEntity}"/> as parameter and returns <see cref="List{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> <see cref="object"/> which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="List{TEntity}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(Specification<TEntity, TPrimaryKey> specification, bool asNoTracking, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method returns <see cref="List{TProjectedType}"/> without any filter.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The type to which <typeparamref name="TEntity"/> will be projected.</typeparam>
        /// <param name="selectExpression">A <b>LINQ</b> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <see cref="List{TProjectedType}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The projected type.</typeparam>
        /// <param name="condition">The condition on which entity list will be returned.</param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <see cref="object"/> of <see cref="Specification{T}"/> and <paramref name="selectExpression"/> as parameters and
        /// returns <see cref="List{TProjectedType}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The projected type.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> object which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Return <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Specification<TEntity, TPrimaryKey> specification,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method returns a <see cref="PaginatedList{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">An object of <see cref="PaginationSpecification{T}"/>.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="PaginatedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="specification"/> is smaller than 1.</exception>
        BaseResponse GetList<TEntity, TPrimaryKey>(
            PaginationSpecification<TEntity, TPrimaryKey> specification,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method returns a <see cref="PaginatedList{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The projected type.</typeparam>
        /// <param name="specification">An object of <see cref="PaginationSpecification{T}"/>.</param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="specification"/> is smaller than 1.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is smaller than 1.</exception>

        BaseResponse GetList<T, TPrimaryKey>(
            PaginationSpecification<T, TPrimaryKey> specification,
            Expression<Func<T, T>> selectExpression,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(object id, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(object id, bool asNoTracking, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="includes">The navigation properties to be loaded.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(
            object id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="includes">The navigation properties to be loaded.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(
            object id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the specified projected entity
        /// if found otherwise null.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The projected type.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <typeparamref name="TProjectedType"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey, TProjectedType>(
            object id,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The conditon on which entity will be returned.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <typeparamref name="TEntity"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The conditon on which entity will be returned.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <typeparamref name="TEntity"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The conditon on which entity will be returned.</param>
        /// <param name="includes">Navigation properties to be loaded.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <typeparamref name="TEntity"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The conditon on which entity will be returned.</param>
        /// <param name="includes">Navigation properties to be loaded.</param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <typeparamref name="TEntity"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <see cref="object"/> of <see cref="Specification{T}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> object which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(Specification<TEntity, TPrimaryKey> specification, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <see cref="object"/> of <see cref="Specification{T}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> object which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="asNoTracking">A <see cref="bool"/> value which determines whether the return entity will be tracked by
        /// EF Core context or not. Default value is false i.e tracking is enabled by default.
        /// </param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(Specification<TEntity, TPrimaryKey> specification, bool asNoTracking, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <see cref="Expression{Func}"/> as parameter and returns <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The projected type.</typeparam>
        /// <param name="condition">The conditon on which entity will be returned.</param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Retuns <typeparamref name="TProjectedType"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes an <see cref="object"/> of <see cref="Specification{T}"/> and a <see cref="System.Linq"/> select  query
        /// and returns <typeparamref name="TProjectedType"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TProjectedType">The type of the projected entity.</typeparam>
        /// <param name="specification">A <see cref="Specification{TEntity}"/> object which contains all the conditions and criteria
        /// on which data will be returned.
        /// </param>
        /// <param name="selectExpression">The <see cref="System.Linq"/> select  query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Retuns <typeparamref name="TProjectedType"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="selectExpression"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetAsync<TEntity, TPrimaryKey, TProjectedType>(
            Specification<TEntity, TPrimaryKey> specification,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method checks whether the database table contains any record.
        /// and returns <see cref="Task"/> of <see cref="bool"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="bool"/>.</returns>
        Task<BaseResponse> ExistsAsync<TEntity, TPrimaryKey>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes a predicate based on which existence of the entity will be determined
        /// and returns <see cref="Task"/> of <see cref="bool"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition based on which the existence will checked.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="bool"/>.</returns>
        Task<BaseResponse> ExistsAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes primary key value of the entity whose existence be determined
        /// and returns <see cref="Task"/> of <see cref="bool"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity whose the existence will checked.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="bool"/>.</returns>
        Task<BaseResponse> ExistsByIdAsync<TEntity, TPrimaryKey>(object id, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method returns all count in <see cref="int"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="int"/>.</returns>
        Task<BaseResponse> GetCountAsync<TEntity, TPrimaryKey>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes one or more <em>predicates</em> and returns the count in <see cref="int"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition based on which count will be done.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="int"/>.</returns>
        Task<BaseResponse> GetCountAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes one or more <em>predicates</em> and returns the count in <see cref="int"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="conditions">The conditions based on which count will be done.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="int"/>.</returns>
        Task<BaseResponse> GetCountAsync<TEntity, TPrimaryKey>(IEnumerable<Expression<Func<TEntity, bool>>> conditions, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method returns all count in <see cref="long"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Retuns <see cref="Task"/> of <see cref="long"/>.</returns>
        Task<BaseResponse> GetLongCountAsync<TEntity, TPrimaryKey>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes one or more <em>predicates</em> and returns the count in <see cref="long"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="condition">The condition based on which count will be done.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Retuns <see cref="Task"/> of <see cref="long"/>.</returns>
        Task<BaseResponse> GetLongCountAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes one or more <em>predicates</em> and returns the count in <see cref="long"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="conditions">The conditions based on which count will be done.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Retuns <see cref="Task"/> of <see cref="long"/>.</returns>
        Task<BaseResponse> GetLongCountAsync<TEntity, TPrimaryKey>(IEnumerable<Expression<Func<TEntity, bool>>> conditions, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>;

        // Context level members

        /// <summary>
        /// This method takes <paramref name="sql"/> string as parameter and returns the result of the provided sql.
        /// </summary>
        /// <typeparam name="T">The <see langword="type"/> to which the result will be mapped.</typeparam>
        /// <param name="sql">The sql query string.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetFromRawSqlAsync<T>(string sql, CancellationToken cancellationToken = default);

        /// <summary>
        /// This method takes <paramref name="sql"/> string and the value of <paramref name="parameter"/> mentioned in the sql query as parameters
        /// and returns the result of the provided sql.
        /// </summary>
        /// <typeparam name="T">The <see langword="type"/> to which the result will be mapped.</typeparam>
        /// <param name="sql">The sql query string.</param>
        /// <param name="parameter">The value of the paramter mention in the sql query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetFromRawSqlAsync<T>(string sql, object parameter, CancellationToken cancellationToken = default);

        /// <summary>
        /// This method takes <paramref name="sql"/> string and values of the <paramref name="parameters"/> mentioned in the sql query as parameters
        /// and returns the result of the provided sql.
        /// </summary>
        /// <typeparam name="T">The <see langword="type"/> to which the result will be mapped.</typeparam>
        /// <param name="sql">The sql query string.</param>
        /// <param name="parameters">The values of the parameters mentioned in the sql query.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetFromRawSqlAsync<T>(string sql, IEnumerable<DbParameter> parameters, CancellationToken cancellationToken = default);

        /// <summary>
        /// This method takes <paramref name="sql"/> string and values of the <paramref name="parameters"/> mentioned in the sql query as parameters
        /// and returns the result of the provided sql.
        /// <para>
        /// The paramters names mentioned in the query should be like p0, p1,p2 etc.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The <see langword="type"/> to which the result will be mapped.</typeparam>
        /// <param name="sql">The sql query string.</param>
        /// <param name="parameters">The values of the parameters mentioned in the sql query. The values should be primitive types.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is <see langword="null"/>.</exception>
        Task<BaseResponse> GetFromRawSqlAsync<T>(string sql, IEnumerable<object> parameters, CancellationToken cancellationToken = default);

        #endregion
        #region StoredProcedures Functions
        Task<(BaseResponse, Dictionary<string, object>)> ExecuteStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters = null, Dictionary<string, object> outputParameters = null, CancellationToken cancellationToken = default) where T : class;
        #endregion
    }
}
