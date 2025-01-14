using Core.GenericRepository.Common;
using Core.GenericRepository.Shared.BaseEntities;
using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq.Expressions;
using Core.GenericRepository.Extensions;
using static Core.GenericRepository.Shared.RepositoryDelegates;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Core.GenericRepository.BaseRepository
{
    public class EntityRepository : IEntityRepository
    {
        private readonly DbContext _dbContext;

        public event BeforeUpdateDelegate BeforeUpdateEvent;
        public event AfterUpdateDelegate AfterUpdateEvent;
        public event BeforeAddDelegate BeforeAddEvent;
        public event AfterAddDelegate AfterAddEvent;
        public event BeforeDeleteDelegate BeforeDeleteEvent;
        public event AfterDeleteDelegate AfterDeleteEvent;
        public event AfterSaveDelegate AfterSaveEvent;
        public event BeforeSaveDelegate BeforeSaveEvent;

        public EntityRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            AfterAddEvent += _repository_AfterAddEvent;
            AfterDeleteEvent += _repository_AfterDeleteEvent;
            AfterUpdateEvent += _repository_AfterUpdateEvent;
            AfterSaveEvent += EntityRepository_AfterSaveEvent;
            BeforeSaveEvent += EntityRepository_BeforeSaveEvent;
            BeforeUpdateEvent += _repository_BeforeUpdateEvent;
            BeforeAddEvent += _repository_BeforeAddEvent;
            BeforeDeleteEvent += _repository_BeforeDeleteEvent;
        }

        #region DbContext

        public async Task<BaseResponse> BeginTransactionAsync(
            IsolationLevel isolationLevel = IsolationLevel.Unspecified,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IDbContextTransaction dbContextTransaction =
                    await _dbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken);
                return new DataResponse<IDbContextTransaction>()
                {
                    IsDone = true,
                    Data = dbContextTransaction,
                    Message = ""
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString(),
                    Message = ""
                };
            }
        }

        public BaseResponse ClearChangeTracker()
        {
            try
            {
                _dbContext.ChangeTracker.Clear();
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                BeforeSaveEvent(_dbContext);
                int count = await _dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                AfterSaveEvent(_dbContext);
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = count
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> SaveChangesAsync()
        {
            try
            {
                BeforeSaveEvent(_dbContext);
                int count = await  _dbContext.SaveChangesAsync();
                AfterSaveEvent(_dbContext);
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = count
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.InnerException != null ? e.InnerException.Message : e.Message
                };
            }
        }

        #endregion

        #region CRUD Functions

        public async Task<BaseResponse> ExecuteSqlCommandAsync(string sql,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await _dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            try
            {
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> ExecuteSqlCommandAsync(string sql, IEnumerable<object> parameters,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> AddAsync<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                if (typeof(TPrimaryKey) == typeof(Guid))
                {
                    var entityId = Convert.ToString(entity.Id);
                    if (Guid.TryParse(entityId, out Guid id))
                    {
                        if (id == Guid.Empty)
                        {
                            entity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                        }
                    }
                }

                var response = BeforeAddEvent.Invoke(_dbContext, entity);
                if (!response.IsDone)
                    return response;
                await _dbContext.Set<TEntity>().AddAsync(entity);
                response = AfterAddEvent.Invoke(_dbContext, entity);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> AddAsync<TEntity, TPrimaryKey>(TEntity entity,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                if (typeof(TPrimaryKey) == typeof(Guid))
                {
                    var entityId = Convert.ToString(entity.Id);
                    if (Guid.TryParse(entityId, out Guid id))
                    {
                        if (id == Guid.Empty)
                        {
                            entity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                        }
                    }
                }

                var response = BeforeAddEvent.Invoke(_dbContext, entity);
                if (!response.IsDone)
                    return response;
                await _dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
                response = AfterAddEvent.Invoke(_dbContext, entity);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> AddRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (typeof(TPrimaryKey) == typeof(Guid))
                {
                    foreach (var entity in entities)
                    {
                        var entityId = Convert.ToString(entity.Id);
                        if (Guid.TryParse(entityId, out Guid id))
                        {
                            if (id == Guid.Empty)
                            {
                                entity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                            }
                        }
                    }
                }

                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                var response = BeforeAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                await _dbContext.Set<TEntity>().AddRangeAsync(entities);
                response = AfterAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> AddRangeAsync<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (typeof(TPrimaryKey) == typeof(Guid))
                {
                    foreach (var entity in entities)
                    {
                        var entityId = Convert.ToString(entity.Id);
                        if (Guid.TryParse(entityId, out Guid id))
                        {
                            if (id == Guid.Empty)
                            {
                                entity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                            }
                        }
                    }
                }

                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                var response = BeforeAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                await _dbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken).ConfigureAwait(false);
                response = AfterAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse Update<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                EntityEntry<TEntity> trackedEntity = _dbContext.ChangeTracker
                    .Entries<TEntity>().FirstOrDefault(x => x.Entity == entity);

                if (trackedEntity == null)
                {
                    IEntityType entityType = _dbContext.Model.FindEntityType(typeof(TEntity));

                    if (entityType == null)
                    {
                        throw new InvalidOperationException(
                            $"{typeof(TEntity).Name} is not part of EF Core DbContext model");
                    }

                    string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();

                    if (primaryKeyName != null)
                    {
                        Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType)
                            .FirstOrDefault();

                        object primaryKeyDefaultValue =
                            primaryKeyType.IsValueType ? Activator.CreateInstance(primaryKeyType) : null;

                        object primaryValue = entity.GetType().GetProperty(primaryKeyName).GetValue(entity, null);

                        if (primaryKeyDefaultValue.Equals(primaryValue))
                        {
                            throw new InvalidOperationException(
                                "The primary key value of the entity to be updated is not valid.");
                        }
                    }

                    var response = BeforeUpdateEvent.Invoke(_dbContext, entity);
                    if (!response.IsDone)
                        return response;
                    _dbContext.Set<TEntity>().Update(entity);
                    response = AfterUpdateEvent.Invoke(_dbContext, entity);
                    if (!response.IsDone)
                        return response;
                }

                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse UpdateRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                var response = BeforeAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                _dbContext.Set<TEntity>().UpdateRange(entities);
                response = AfterAddEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse Remove<TEntity, TPrimaryKey>(TEntity entity)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException(nameof(entity));
                }

                //var response = BeforeDeleteEvent.Invoke(_dbContext, entity);
                //if (!response.IsDone)
                //    return response;
                _dbContext.Set<TEntity>().Remove(entity);
                var response = AfterDeleteEvent.Invoke(_dbContext, entity);
                if (!response.IsDone)
                    return response;
                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse RemoveRange<TEntity, TPrimaryKey>(IEnumerable<TEntity> entities)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (entities == null)
                {
                    throw new ArgumentNullException(nameof(entities));
                }

                var response = BeforeDeleteEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;

                _dbContext.Set<TEntity>().RemoveRange(entities);

                response = AfterDeleteEvent.Invoke(_dbContext, entities);
                if (!response.IsDone)
                    return response;

                return new BaseResponse()
                {
                    IsDone = true
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        #endregion

        #region Query Functions

        public Task<IQueryable<TEntity>> GetQueryable<TEntity, TPrimaryKey>()
            where TEntity : BaseEntity<TPrimaryKey>
        {
            return Task.FromResult(_dbContext.Set<TEntity>().AsQueryable());
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetListAsync<TEntity, TPrimaryKey>(false, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = ((DataResponse<List<TEntity>>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> nullValue = null;
                var response = await GetListAsync<TEntity, TPrimaryKey>(nullValue, asNoTracking, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = ((DataResponse<List<TEntity>>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetListAsync<TEntity, TPrimaryKey>(includes, false, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = ((DataResponse<List<TEntity>>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<T, TPrimaryKey>(
            Func<IQueryable<T>, IIncludableQueryable<T, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

                if (includes != null)
                {
                    query = includes(query);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                List<T> items = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
                return new DataResponse<List<T>>
                {
                    IsDone = true,
                    Data = items
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetListAsync<TEntity, TPrimaryKey>(condition, false, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = ((DataResponse<List<TEntity>>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response =
                    await GetListAsync<TEntity, TPrimaryKey>(condition, null, asNoTracking, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = ((DataResponse<List<TEntity>>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                if (includes != null)
                {
                    query = includes(query);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await query.ToListAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Specification<TEntity, TPrimaryKey> specification, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                return await GetListAsync<TEntity, TPrimaryKey>(specification, false, cancellationToken);
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey>(
            Specification<TEntity, TPrimaryKey> specification,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (specification != null)
                {
                    query = query.GetSpecifiedQuery(specification);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await query.ToListAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                return new DataResponse<List<TProjectedType>>()
                {
                    IsDone = true,
                    Data = await _dbContext.Set<TEntity>().Select(selectExpression).ToListAsync(cancellationToken)
                        .ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                return new DataResponse<List<TProjectedType>>()
                {
                    IsDone = true,
                    Data = await query.Select(selectExpression).ToListAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetListAsync<TEntity, TPrimaryKey, TProjectedType>(
            Specification<TEntity, TPrimaryKey> specification,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (specification != null)
                {
                    query = query.GetSpecifiedQuery(specification);
                }

                return new DataResponse<List<TProjectedType>>
                {
                    IsDone = true,
                    Data = await query.Select(selectExpression)
                        .ToListAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse GetList<TEntity, TPrimaryKey>(
            PaginationSpecification<TEntity, TPrimaryKey> specification,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (specification == null)
                {
                    throw new ArgumentNullException(nameof(specification));
                }

                return new DataResponse<PagedResult<TEntity>>
                {
                    IsDone = true,
                    Data = _dbContext.Set<TEntity>().GetPaged(specification.PageIndex, specification.PageSize)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public BaseResponse GetList<TEntity, TPrimaryKey>(
            PaginationSpecification<TEntity, TPrimaryKey> specification,
            Expression<Func<TEntity, TEntity>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (specification == null)
                {
                    throw new ArgumentNullException(nameof(specification));
                }

                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                    .GetSpecifiedQuery((BaseSpecification<TEntity, TPrimaryKey>)specification);

                return new DataResponse<PagedResult<TEntity>>
                {
                    IsDone = true,
                    Data = query.Select(selectExpression)
                        .GetPaged<TEntity>(specification.PageIndex, specification.PageSize)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(object id,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                var response = await GetByIdAsync<TEntity, TPrimaryKey>(id, false, cancellationToken);
                if (!response.IsDone)
                    return response;
                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(object id, bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                var response = await GetByIdAsync<TEntity, TPrimaryKey>(id, null, asNoTracking, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(
            object id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                var response = await GetByIdAsync<TEntity, TPrimaryKey>(id, includes, false, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetByIdAsync<TEntity, TPrimaryKey>(
            object id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking = false,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                IEntityType entityType = _dbContext.Model.FindEntityType(typeof(TEntity));

                string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
                Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

                if (primaryKeyName == null || primaryKeyType == null)
                {
                    return new ExceptionResponse()
                    {
                        IsDone = false,
                        Exception = "Entity does not have any primary key defined" + nameof(id)
                    };
                }

                object primayKeyValue = null;

                try
                {
                    primayKeyValue = Convert.ChangeType(id, primaryKeyType, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new ArgumentException(
                        $"You can not assign a value of type {id.GetType()} to a property of type {primaryKeyType}");
                }

                ParameterExpression pe = Expression.Parameter(typeof(TEntity), "entity");
                MemberExpression me = Expression.Property(pe, primaryKeyName);
                ConstantExpression constant = Expression.Constant(primayKeyValue, primaryKeyType);
                BinaryExpression body = Expression.Equal(me, constant);
                Expression<Func<TEntity, bool>> expressionTree =
                    Expression.Lambda<Func<TEntity, bool>>(body, new[] { pe });

                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (includes != null)
                {
                    query = includes(query);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = await query.FirstOrDefaultAsync(expressionTree, cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetByIdAsync<T, TPrimaryKey, TProjectedType>(
            object id,
            Expression<Func<T, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IEntityType entityType = _dbContext.Model.FindEntityType(typeof(T));

                string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
                Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

                if (primaryKeyName == null || primaryKeyType == null)
                {
                    throw new ArgumentException("Entity does not have any primary key defined", nameof(id));
                }

                object primayKeyValue = null;

                try
                {
                    primayKeyValue = Convert.ChangeType(id, primaryKeyType, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new ArgumentException(
                        $"You can not assign a value of type {id.GetType()} to a property of type {primaryKeyType}");
                }

                ParameterExpression pe = Expression.Parameter(typeof(T), "entity");
                MemberExpression me = Expression.Property(pe, primaryKeyName);
                ConstantExpression constant = Expression.Constant(primayKeyValue, primaryKeyType);
                BinaryExpression body = Expression.Equal(me, constant);
                Expression<Func<T, bool>> expressionTree = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

                IQueryable<T> query = _dbContext.Set<T>();
                return new DataResponse<TProjectedType>()
                {
                    IsDone = true,
                    Data = await query.Where(expressionTree).Select(selectExpression)
                        .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetAsync<TEntity, TPrimaryKey>(condition, null, false, cancellationToken);
                if (!response.IsDone)
                    return response;
                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetAsync<TEntity, TPrimaryKey>(condition, null, asNoTracking, cancellationToken);
                if (!response.IsDone)
                    return response;
                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetAsync<TEntity, TPrimaryKey>(condition, includes, false, cancellationToken);
                if (!response.IsDone)
                    return response;
                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Expression<Func<TEntity, bool>> condition,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes,
            bool asNoTracking,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                if (includes != null)
                {
                    query = includes(query);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey>(
            Specification<TEntity, TPrimaryKey> specification, CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await GetAsync<TEntity, TPrimaryKey>(specification, false, cancellationToken);
                if (!response.IsDone)
                    return response;

                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = ((DataResponse<TEntity>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<T, TPrimaryKey>(Specification<T, TPrimaryKey> specification,
            bool asNoTracking, CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

                if (specification != null)
                {
                    query = query.GetSpecifiedQuery(specification);
                }

                if (asNoTracking)
                {
                    query = query.AsNoTracking();
                }

                return new DataResponse<T>()
                {
                    IsDone = true,
                    Data = await query.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<TEntity, TPrimaryKey, TProjectedType>(
            Expression<Func<TEntity, bool>> condition,
            Expression<Func<TEntity, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IQueryable<TEntity> query = _dbContext.Set<TEntity>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                return new DataResponse<TProjectedType>()
                {
                    IsDone = true,
                    Data = await query.Select(selectExpression).FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false)
                };
            }

            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetAsync<T, TPrimaryKey, TProjectedType>(
            Specification<T, TPrimaryKey> specification,
            Expression<Func<T, TProjectedType>> selectExpression,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (selectExpression == null)
                {
                    throw new ArgumentNullException(nameof(selectExpression));
                }

                IQueryable<T> query = _dbContext.Set<T>();

                if (specification != null)
                {
                    query = query.GetSpecifiedQuery(specification);
                }

                return new DataResponse<TProjectedType>()
                {
                    IsDone = true,
                    Data = await query.Select(selectExpression).FirstOrDefaultAsync(cancellationToken)
                        .ConfigureAwait(false)
                };
            }

            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> ExistsAsync<TEntity, TPrimaryKey>(CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                var response = await ExistsAsync<TEntity, TPrimaryKey>(null, cancellationToken);
                if (!response.IsDone)
                    return response;
                return new DataResponse<bool>()
                {
                    IsDone = true,
                    Data = ((DataResponse<bool>)response).Data
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> ExistsAsync<TEntity, TPrimaryKey>(Expression<Func<TEntity, bool>> condition,
            CancellationToken cancellationToken = default)
            where TEntity : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<TEntity> query = _dbContext.Set<TEntity>();
                if (condition == null)
                {
                    return new DataResponse<bool>()
                    {
                        IsDone = true,
                        Data = await query.AnyAsync(cancellationToken)
                    };
                }

                return new DataResponse<bool>()
                {
                    IsDone = true,
                    Data = await query.AnyAsync(condition, cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> ExistsByIdAsync<T, TPrimaryKey>(object id,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                if (id == null)
                {
                    throw new ArgumentNullException(nameof(id));
                }

                IEntityType entityType = _dbContext.Model.FindEntityType(typeof(T));

                string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
                Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

                if (primaryKeyName == null || primaryKeyType == null)
                {
                    throw new ArgumentException("Entity does not have any primary key defined", nameof(id));
                }

                object primayKeyValue = null;

                try
                {
                    primayKeyValue = Convert.ChangeType(id, primaryKeyType, CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    throw new ArgumentException(
                        $"You can not assign a value of type {id.GetType()} to a property of type {primaryKeyType}");
                }

                ParameterExpression pe = Expression.Parameter(typeof(T), "entity");
                MemberExpression me = Expression.Property(pe, primaryKeyName);
                ConstantExpression constant = Expression.Constant(primayKeyValue, primaryKeyType);
                BinaryExpression body = Expression.Equal(me, constant);
                Expression<Func<T, bool>> expressionTree = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

                IQueryable<T> query = _dbContext.Set<T>();

                return new DataResponse<bool>()
                {
                    IsDone = true,
                    Data = await query.AnyAsync(expressionTree, cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetCountAsync<T, TPrimaryKey>(CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await _dbContext.Set<T>().CountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetCountAsync<T, TPrimaryKey>(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await query.CountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetCountAsync<T, TPrimaryKey>(IEnumerable<Expression<Func<T, bool>>> conditions,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();
                if (conditions != null)
                {
                    foreach (Expression<Func<T, bool>> expression in conditions)
                    {
                        query = query.Where(expression);
                    }
                }

                return new DataResponse<int>()
                {
                    IsDone = true,
                    Data = await query.CountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetLongCountAsync<T, TPrimaryKey>(CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                return new DataResponse<long>()
                {
                    IsDone = true,
                    Data = await _dbContext.Set<T>().LongCountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetLongCountAsync<T, TPrimaryKey>(Expression<Func<T, bool>> condition,
            CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

                if (condition != null)
                {
                    query = query.Where(condition);
                }

                return new DataResponse<long>()
                {
                    IsDone = true,
                    Data = await query.LongCountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetLongCountAsync<T, TPrimaryKey>(
            IEnumerable<Expression<Func<T, bool>>> conditions, CancellationToken cancellationToken = default)
            where T : BaseEntity<TPrimaryKey>
        {
            try
            {
                IQueryable<T> query = _dbContext.Set<T>();

                if (conditions != null)
                {
                    foreach (Expression<Func<T, bool>> expression in conditions)
                    {
                        query = query.Where(expression);
                    }
                }

                return new DataResponse<long>()
                {
                    IsDone = true,
                    Data = await query.LongCountAsync(cancellationToken).ConfigureAwait(false)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        #endregion

        #region DbConext level members

        public async Task<BaseResponse> GetFromRawSqlAsync<TEntity>(string sql,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    throw new ArgumentNullException(nameof(sql));
                }

                IEnumerable<object> parameters = new List<object>();
                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await _dbContext.GetFromQueryAsync<TEntity>(sql, parameters, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetFromRawSqlAsync<TEntity>(string sql, object parameter,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    throw new ArgumentNullException(nameof(sql));
                }

                List<object> parameters = new List<object>() { parameter };

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await _dbContext.GetFromQueryAsync<TEntity>(sql, parameters, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }

            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException(nameof(sql));
            }
        }

        public async Task<BaseResponse> GetFromRawSqlAsync<TEntity>(string sql, IEnumerable<DbParameter> parameters,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    throw new ArgumentNullException(nameof(sql));
                }

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await _dbContext.GetFromQueryAsync<TEntity>(sql, parameters, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetFromRawSqlAsync<TEntity>(string sql, IEnumerable<object> parameters,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sql))
                {
                    throw new ArgumentNullException(nameof(sql));
                }

                return new DataResponse<List<TEntity>>()
                {
                    IsDone = true,
                    Data = await _dbContext.GetFromQueryAsync<TEntity>(sql, parameters, cancellationToken)
                };
            }
            catch (Exception e)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = e.ToString()
                };
            }
        }

        #endregion

        #region Events Implementions

        private BaseResponse _repository_BeforeDeleteEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_BeforeUpdateEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_BeforeAddEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_AfterUpdateEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_AfterDeleteEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_AfterAddEvent(DbContext dbContext, object baseEntity)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse EntityRepository_BeforeSaveEvent(DbContext dbContext)
        {
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse EntityRepository_AfterSaveEvent(DbContext dbContex)
        {
            return new BaseResponse() { IsDone = true };
        }

        #endregion
        #region StoredProcedures Functions
        public async Task<(BaseResponse, Dictionary<string, object>)> ExecuteStoredProcedureAsync<T>(string storedProcedureName, Dictionary<string, object> parameters = null, Dictionary<string, object> outputParameters = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                List<SqlParameter> sqlParameters = new List<SqlParameter>();
                foreach (var kvp in parameters)
                {
                    var parameter = new SqlParameter($"@{kvp.Key}", kvp.Value ?? DBNull.Value);
                    if (kvp.Value != null)
                    {
                        parameter.SqlDbType = GetSqlDbTypeFromValue(kvp.Value);
                    }
                    sqlParameters.Add(parameter);
                }
                // Construct the SQL command string with parameters and their values directly embedded
                var parameterValuesWithAt = string.Join(", ", parameters
                    .Select(kvp => $"@{kvp.Key}=@{kvp.Key}")
                    );

                #region Prepare OutPut parameters

                if (outputParameters != null && outputParameters.Count > 0)
                {

                    // Construct the output parameters string
                    //var outputParameterValuesWithAt = string.Join(", ", outputParameters
                    //    .Select(kvp => $"@{kvp.Key}={(kvp.Value == null || kvp.Value == "string" || kvp.Value == "int" ? $"@{kvp.Key} OUTPUT" : kvp.Value)}")
                    //    );

                    var outputParameterValuesWithAt = string.Join(", ", outputParameters
                       .Select(kvp => $"@{kvp.Key}={(kvp.Value == null || kvp.Value == "string" || kvp.Value == "int" ? $"@{kvp.Key} OUTPUT" : $"@{kvp.Key}")}")
                      );


                    // Append the output parameters to the main parameters string
                    parameterValuesWithAt += ", " + outputParameterValuesWithAt;

                    foreach (var outputParam in outputParameters)
                    {
                        var outputParamSql = new SqlParameter
                        {
                            Value = DBNull.Value,
                            ParameterName = "@" + outputParam.Key,
                            Direction = ParameterDirection.Output,
                            SqlDbType = GetSqlDbTypeFromOutPutValue(outputParam.Value) // Helper method to infer SqlDbType
                        };
                        if (!outputParam.Value.Equals("int") && !outputParam.Value.Equals("string"))
                        {
                            outputParamSql.Value = outputParam.Value;
                            outputParamSql.Direction = ParameterDirection.InputOutput;
                            //continue;// break the foreach if the output has a value
                        }

                        sqlParameters.Add(outputParamSql);
                    }

                }
                #endregion

                var sqlCommand = $"EXEC {storedProcedureName} {parameterValuesWithAt}";


                //var result = await _context.SqlQueryAsync<T>(sqlCommand, sqlParameters.ToArray(), cancellationToken);
                var result = await SqlQueryAsync<T>(sqlCommand, sqlParameters.ToArray(), cancellationToken);
                var outputValues = new Dictionary<string, object>();
                if (outputParameters != null)
                {
                    foreach (var outputParam in outputParameters)
                    {
                        var outputParameter = sqlParameters.FirstOrDefault(p => p.ParameterName == "@" + outputParam.Key);
                        if (outputParameter != null && outputParameter.Direction == ParameterDirection.Output)
                        {
                            outputValues.Add(outputParam.Key, outputParameter.Value);
                        }
                    }
                }
                var dataResponse = new DataResponse<List<T>>()
                {
                    IsDone = true,
                    Data = result
                };
                return (dataResponse, outputValues);
            }
            catch (Exception e)
            {

                var excep = new BaseResponse()
                {
                    IsDone = false,
                    Message = e.ToString()
                };
                return (excep, default);
            
        }
          
        }
        private async Task<List<T>> SqlQueryAsync<T>(string sql, object[] parameters = null, CancellationToken cancellationToken = default) where T : class
        {
            if (parameters is null)
            {
                parameters = new object[] { };
            }

            if (typeof(T).GetProperties().Any())
            {
                // return await db.Database.SqlQueryRaw<T>(sql, parameters).ToListAsync();
                // string sqlQuery =  db.Set<T>().FromSqlRaw(sql, parameters).ToQueryString();
                return await _dbContext.Set<T>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
            }
            else
            {
                await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
                return default;
            }
        }
        private SqlDbType GetSqlDbTypeFromOutPutValue(object value)
        {
            // This function attempts to infer the SqlDbType based on the value's type
            if (value is "int") return SqlDbType.Int;
            else if (value is "string") return SqlDbType.VarChar;
            else
            {
                return GetSqlDbTypeFromValue(value);
            }
           

        }
        private SqlDbType GetSqlDbTypeFromValue(object value)
        {
            // This function attempts to infer the SqlDbType based on the value's type
            if (value is int) return SqlDbType.Int;
            if (value is string) return SqlDbType.VarChar;
            if (value is DateTime) return SqlDbType.DateTime;
            if (value is DateTimeOffset) return SqlDbType.DateTimeOffset;
            // Add more type mappings as needed

            // Default to SqlDbType.NVarChar for unknown types
            return SqlDbType.NVarChar;
        }
        #endregion

    }
}