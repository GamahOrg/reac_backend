using Core.GenericRepository.Shared.BaseEntities;
using Core.Shared.Responses;

namespace REAC.Application.Contracts
{
    public interface IBasicService<TEntity, TPrimaryKey>
    {
        #region CRUD Functions
        /// <summary>
        /// This method takes an <typeparamref name="TEntityCreateUpdateDto"/> object, mark the object as <see cref="EntityState.Added"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entity"/> to be added.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntityCreateUpdateDto"/> object to be inserted to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> Add<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;


        /// <summary>
        /// This method takes an object of <typeparamref name="TEntityCreateUpdateDto"/>, adds it to the change tracker and will
        /// be inserted into the database when <see cref="IRepository.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the entity.</typeparam>
        /// <param name="entity">The entity to be inserted.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/>.</returns>
        Task<BaseResponse> AddAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity, CancellationToken cancellationToken = default) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;


        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntityCreateUpdateDto}"/> objects, mark the objects as <see cref="EntityState.Added"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entities"/> to be added.</typeparam>
        /// <param name="entities">The <typeparamref name="TEntityCreateUpdateDto"/> objects to be inserted to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> AddRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;


        /// <summary>
        /// This method takes a collection of <typeparamref name="TEntityCreateUpdateDto"/> object, adds them to the change tracker and will
        /// be inserted into the database when <see cref="IRepository.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the entity.</typeparam>
        /// <param name="entities">The entities to be inserted.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/>.</returns>
        Task<BaseResponse> AddRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities, CancellationToken cancellationToken = default) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method takes an <typeparamref name="TEntityCreateUpdateDto"/> object, mark the object as <see cref="EntityState.Modified"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entity"/> to be marked as modified.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntityCreateUpdateDto"/> object to be updated to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> UpdateAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntityCreateUpdateDto}"/> objects, mark the objects as <see cref="EntityState.Modified"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entities"/> to be marked as modified.</typeparam>
        /// <param name="entities">The entity objects to be updated to the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> UpdateRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method takes an <typeparamref name="TEntityCreateUpdateDto"/> object, mark the object as <see cref="EntityState.Deleted"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entity"/> to be marked as deleted.</typeparam>
        /// <param name="entity">The <typeparamref name="TEntityCreateUpdateDto"/> object to be deleted from the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> RemoveAsync(TPrimaryKey id, CancellationToken cancellationToken);
        /// <summary>
        /// This method takes <see cref="IEnumerable{TEntityCreateUpdateDto}"/> objects, mark the objects as <see cref="EntityState.Deleted"/> to the <see cref="ChangeTracker"/> of the <see cref="DbContext"/>.
        /// <para>
        /// Call <see cref="SaveChangesAsync(CancellationToken)"/> to persist the changes to the database.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the <paramref name="entities"/> to be marked as deleted.</typeparam>
        /// <param name="entities">The <typeparamref name="TEntityCreateUpdateDto"/> objects to be deleted from the database on <see cref="SaveChangesAsync(CancellationToken)"/>.</param>
        Task<BaseResponse> RemoveRangeAsync(List<TPrimaryKey> ids, CancellationToken cancellationToken);
        /// <summary>
        /// this methods shoud insert record if not exist or update it if exist alreadys
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto"></typeparam>
        /// <param name="saveDto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<BaseResponse> SaveAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto saveDto, CancellationToken cancellationToken) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;
        #endregion
        #region Query Functions 

        /// <summary>
        /// This method returns <see cref="List{TEntityCreateUpdateDto}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListAsync<TEntityCreateUpdateDto>(CancellationToken cancellationToken = default)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;
        /// <summary>
        /// This method returns a <see cref="PaginatedList{T}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="specification">An object of <see cref="PaginationSpecification{TEntity,TPrimaryKey}"/>.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="PaginatedList{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="specification"/> is smaller than 1.</exception>
        BaseResponse GetList<TEntityCreateUpdateDto>(
            int pageIndex, int pageSize,
            CancellationToken cancellationToken = default)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>;

        /// <summary>
        /// This method takes <paramref name="id"/> which is the primary key value of the entity and returns the entity
        /// if found otherwise <see langword="null"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="id">The primary key value of the entity.</param>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
        Task<BaseResponse> GetByIdAsync<TEntityCreateUpdateDto>(object id, CancellationToken cancellationToken = default);
        #endregion
    }
}
