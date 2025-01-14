using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Common;
using Core.GenericRepository.Shared.BaseEntities;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts;
using REAC.Application.Contracts.IAppServices;

namespace REAC.Application
{
    public class BasicService<TEntity, TPrimaryKey> : IBasicService<TEntity, TPrimaryKey> where TEntity : BaseEntity<TPrimaryKey>
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ILoggerManager _loggerManager;
        private readonly IJwtReader _jwtReader;


        public BasicService(IEntityRepository repository,
            IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader)
        {
            _repository = repository;
            _mapper = mapper;
            _repository.BeforeAddEvent += _repository_BeforeAddEvent;
            _repository.BeforeDeleteEvent += _repository_BeforeDeleteEvent;
            _repository.BeforeUpdateEvent += _repository_BeforeUpdateEvent;
            _repository.AfterAddEvent += _repository_AfterAddEvent;
            _repository.AfterUpdateEvent += _repository_AfterUpdateEvent;
            _repository.AfterDeleteEvent += _repository_AfterDeleteEvent;
            _jwtReader = jwtReader;
            _loggerManager = loggerManager;

        }

        private BaseResponse _repository_AfterDeleteEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            _jwtReader.GetUserId(ref userId);

            //if (!_jwtReader.GetUserId(ref userId))
            //{
            //    return new BaseResponse()
            //    {
            //        IsDone = false
            //    };
            //}
            var deletedAuditedEntities = dbContext.ChangeTracker.Entries()
                                       .Where(p => p.State == EntityState.Deleted)
                                       .Select(p => p.Entity);

            if (deletedAuditedEntities.Any())
            {
                foreach (var entity in deletedAuditedEntities)
                {
                    if (entity is SoftDeleteBaseEntity<TPrimaryKey> softDeleteBaseEntity)
                    {
                        softDeleteBaseEntity.IsDeleted = true;
                        softDeleteBaseEntity.DeletedBy = userId;
                        softDeleteBaseEntity.DeletedOn = DateTime.Now;

                    }
                    if (entity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
                    {
                        fullAuditedBaseEntity.IsDeleted = true;
                        fullAuditedBaseEntity.DeletedBy = userId;
                        fullAuditedBaseEntity.DeletedOn = DateTime.Now;
                    }
                }
            }
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_AfterUpdateEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            _jwtReader.GetUserId(ref userId);
            //if (!_jwtReader.GetUserId(ref userId))
            //{
            //    return new BaseResponse()
            //    {
            //        IsDone = false
            //    };
            //}
            var updatedAuditedEntities = dbContext.ChangeTracker.Entries()
                                       .Where(p => p.State == EntityState.Modified)
                                       .Select(p => p.Entity);

            if (updatedAuditedEntities.Any())
            {
                foreach (var entity in updatedAuditedEntities)
                {
                    if (entity is AuditedBaseEntity<TPrimaryKey> auditedBaseEntity)
                    {
                        auditedBaseEntity.UpdatedBy = userId;
                        auditedBaseEntity.UpdatedOn = DateTime.Now;
                    }
                    if (entity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
                    {
                        fullAuditedBaseEntity.UpdatedBy = userId;
                        fullAuditedBaseEntity.UpdatedOn = DateTime.Now;
                    }
                    if (entity is ModificationBaseEntity<TPrimaryKey> modificationBaseEntity)
                    {
                        modificationBaseEntity.UpdatedBy = userId;
                        modificationBaseEntity.UpdatedOn = DateTime.Now;
                    }
                }
            }
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_AfterAddEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            _jwtReader.GetUserId(ref userId);

            //if (!_jwtReader.GetUserId(ref userId))
            //{
            //    return new BaseResponse()
            //    {
            //        IsDone = false
            //    };
            //}
            var addedAuditedEntities = dbContext.ChangeTracker.Entries()
                                       .Where(p => p.State == EntityState.Added)
                                       .Select(p => p.Entity);

            if (addedAuditedEntities.Any())
            {
                foreach (var entity in addedAuditedEntities)
                {
                    if (entity is BaseEntity<TPrimaryKey> addedEntity)
                    {
                        if (typeof(TPrimaryKey) == typeof(Guid))
                        {
                            var entityId = Convert.ToString(addedEntity.Id);
                            if (Guid.TryParse(entityId, out Guid id))
                            {
                                if (id == Guid.Empty)
                                {
                                    addedEntity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                                }
                            }
                        }
                    }

                    if (entity is AuditedBaseEntity<TPrimaryKey> auditedBaseEntity)
                    {

                        auditedBaseEntity.CreatedBy = userId;
                        auditedBaseEntity.CreatedOn = DateTime.Now;
                    }
                    if (entity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
                    {
                        fullAuditedBaseEntity.CreatedBy = userId;
                        fullAuditedBaseEntity.CreatedOn = DateTime.Now;
                    }
                    if (entity is CreationBaseEntity<TPrimaryKey> creationBaseEntity)
                    {
                        creationBaseEntity.CreatedBy = userId;
                        creationBaseEntity.CreatedOn = DateTime.Now;

                    }
                }
            }
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_BeforeUpdateEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            _jwtReader.GetUserId(ref userId);
            //if (!_jwtReader.GetUserId(ref userId))
            //{
            //    return new BaseResponse()
            //    {
            //        IsDone = false
            //    };
            //}
            if (baseEntity is AuditedBaseEntity<TPrimaryKey> auditedBaseEntity)
            {
                auditedBaseEntity.UpdatedBy = userId;
                auditedBaseEntity.UpdatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };

            }
            if (baseEntity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
            {
                fullAuditedBaseEntity.UpdatedBy = userId;
                fullAuditedBaseEntity.UpdatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is ModificationBaseEntity<TPrimaryKey> modificationBaseEntity)
            {
                modificationBaseEntity.UpdatedBy = userId;
                modificationBaseEntity.UpdatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };

            }

            if (baseEntity is IEnumerable<AuditedBaseEntity<TPrimaryKey>> auditedBaseEntities)
            {
                foreach (var auditedBaseEntityItem in auditedBaseEntities)
                {
                    auditedBaseEntityItem.UpdatedBy = userId;
                    auditedBaseEntityItem.UpdatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is IEnumerable<FullAuditedBaseEntity<TPrimaryKey>> fullAuditedBaseEntities)
            {
                foreach (var fullAuditedBaseEntityItem in fullAuditedBaseEntities)
                {
                    fullAuditedBaseEntityItem.UpdatedBy = userId;
                    fullAuditedBaseEntityItem.UpdatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is IEnumerable<CreationBaseEntity<TPrimaryKey>> creationBaseEntities)
            {
                foreach (var creationBaseEntityItem in creationBaseEntities)
                {
                    creationBaseEntityItem.CreatedBy = userId;
                    creationBaseEntityItem.CreatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_BeforeDeleteEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            if (!_jwtReader.GetUserId(ref userId))
            {
                return new BaseResponse()
                {
                    IsDone = false
                };
            }
            if (baseEntity is SoftDeleteBaseEntity<TPrimaryKey> softDeleteBaseEntity)
            {
                softDeleteBaseEntity.IsDeleted = true;
                softDeleteBaseEntity.DeletedBy = userId;
                softDeleteBaseEntity.DeletedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };

            }
            if (baseEntity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
            {
                fullAuditedBaseEntity.IsDeleted = true;
                fullAuditedBaseEntity.DeletedBy = userId;
                fullAuditedBaseEntity.DeletedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };

            }

            if (baseEntity is IEnumerable<FullAuditedBaseEntity<TPrimaryKey>> fullAuditedBaseEntities)
            {
                foreach (var fullAuditedBaseEntityItem in fullAuditedBaseEntities)
                {
                    fullAuditedBaseEntityItem.IsDeleted = true;
                    fullAuditedBaseEntityItem.DeletedBy = userId;
                    fullAuditedBaseEntityItem.DeletedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is IEnumerable<SoftDeleteBaseEntity<TPrimaryKey>> creationBaseEntities)
            {
                foreach (var creationBaseEntityItem in creationBaseEntities)
                {
                    creationBaseEntityItem.IsDeleted = true;
                    creationBaseEntityItem.DeletedBy = userId;
                    creationBaseEntityItem.DeletedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            return new BaseResponse() { IsDone = true };
        }

        private BaseResponse _repository_BeforeAddEvent(DbContext dbContext, object baseEntity)
        {
            Guid? userId = null;
            _jwtReader.GetUserId(ref userId);

            //if (!_jwtReader.GetUserId(ref userId))
            //{
            //    return new BaseResponse()
            //    {
            //        IsDone = false
            //    };
            //}
            if (baseEntity is AuditedBaseEntity<TPrimaryKey> auditedBaseEntity)
            {
                auditedBaseEntity.CreatedBy = userId;
                auditedBaseEntity.CreatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
            {
                fullAuditedBaseEntity.CreatedBy = userId;
                fullAuditedBaseEntity.CreatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is CreationBaseEntity<TPrimaryKey> creationBaseEntity)
            {
                creationBaseEntity.CreatedBy = userId;
                creationBaseEntity.CreatedOn = DateTime.Now;
                return new BaseResponse() { IsDone = true };

            }


            if (baseEntity is IEnumerable<AuditedBaseEntity<TPrimaryKey>> auditedBaseEntities)
            {
                foreach (var auditedBaseEntityItem in auditedBaseEntities)
                {
                    auditedBaseEntityItem.CreatedBy = userId;
                    auditedBaseEntityItem.CreatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is IEnumerable<FullAuditedBaseEntity<TPrimaryKey>> fullAuditedBaseEntities)
            {
                foreach (var fullAuditedBaseEntityItem in fullAuditedBaseEntities)
                {
                    fullAuditedBaseEntityItem.CreatedBy = userId;
                    fullAuditedBaseEntityItem.CreatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            if (baseEntity is IEnumerable<CreationBaseEntity<TPrimaryKey>> creationBaseEntities)
            {
                foreach (var creationBaseEntityItem in creationBaseEntities)
                {
                    creationBaseEntityItem.CreatedBy = userId;
                    creationBaseEntityItem.CreatedOn = DateTime.Now;
                }
                return new BaseResponse() { IsDone = true };
            }
            return new BaseResponse() { IsDone = true };
        }

        public virtual async Task<BaseResponse> Add<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
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
            TEntity originalEntity = _mapper.Map<TEntity>(entity);
            await _repository.AddAsync<TEntity, TPrimaryKey>(originalEntity);
            var response = await _repository.SaveChangesAsync();
            if (response.IsDone)
            {
                return new DataResponse<TEntity>()
                {
                    IsDone = response.IsDone,
                    Data = originalEntity,
                    Message = response.Message
                };
            }
            return response;
        }

        public virtual async Task<BaseResponse> AddAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity, CancellationToken cancellationToken = default)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            var entityId = Convert.ToString(entity.Id);
            if (Guid.TryParse(entityId, out Guid id))
            {
                if (id == Guid.Empty)
                {
                    entity.Id = (TPrimaryKey)Convert.ChangeType(Guid.NewGuid(), typeof(TPrimaryKey));
                }
            }

            TEntity originalEntity = _mapper.Map<TEntity>(entity);
            await _repository.AddAsync<TEntity, TPrimaryKey>(originalEntity, cancellationToken);
            var response = await _repository.SaveChangesAsync(cancellationToken);
            if (response.IsDone)
            {
                return new DataResponse<TEntity>()
                {
                    IsDone = true,
                    Data = originalEntity,
                    Message = response.Message
                };
            }
            return response;
        }

        public async Task<BaseResponse> AddRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
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
            List<TEntity> originalEntity = _mapper.Map<List<TEntity>>(entities);
            await _repository.AddRangeAsync<TEntity, TPrimaryKey>(originalEntity);
            var returnValue = await _repository.SaveChangesAsync();
            if (returnValue.IsDone)
            {
                return new DataResponse<List<TEntity>>()
                {
                    IsDone = returnValue.IsDone,
                    Data = originalEntity,
                    Message = returnValue.Message
                };
            }
            return returnValue;
        }

        public async Task<BaseResponse> AddRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities, CancellationToken cancellationToken = default) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
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
            List<TEntity> originalEntity = _mapper.Map<List<TEntity>>(entities);
            await _repository.AddRangeAsync<TEntity, TPrimaryKey>(originalEntity, cancellationToken);
            var returnValue = await _repository.SaveChangesAsync(cancellationToken);
            if (returnValue.IsDone)
            {
                return new DataResponse<List<TEntity>>()
                {
                    IsDone = returnValue.IsDone,
                    Data = originalEntity,
                    Message = returnValue.Message
                };
            }
            return returnValue;
        }

        public virtual async Task<BaseResponse> GetByIdAsync<TEntityCreateUpdateDto>(object id, CancellationToken cancellationToken = default)
        {
            return await _repository.GetByIdAsync<TEntity, TPrimaryKey>(id, cancellationToken);
        }

        public virtual async Task<BaseResponse> GetListAsync<TEntityCreateUpdateDto>(CancellationToken cancellationToken = default) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            //_loggerManager.Log(LoggerType.File | LoggerType.Database | LoggerType.Console, new LogDto()
            //{ 
            //    Type = LogType.Information,
            //    Message = "GetListAsync Executed"
            //});

            var response = await _repository.GetListAsync<TEntity, TPrimaryKey>(cancellationToken);
            if (!response.IsDone)
                return response;

            List<TEntityCreateUpdateDto> entityDtos
                = _mapper.Map<List<TEntityCreateUpdateDto>>(((DataResponse<List<TEntity>>)response).Data);
            return new DataResponse<List<TEntityCreateUpdateDto>>()
            {
                IsDone = response.IsDone,
                Data = entityDtos,
                Message = response.Message
            };
        }

        public virtual BaseResponse GetList<TEntityCreateUpdateDto>(int pageIndex, int pageSize, CancellationToken cancellationToken = default) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            return _repository.GetList<TEntity, TPrimaryKey>(new PaginationSpecification<TEntity, TPrimaryKey>()
            { PageSize = pageSize, PageIndex = pageIndex },
                 cancellationToken);
        }
        public virtual async Task<IQueryable<TEntity>> GetQueryableAsync(bool noTracking = false)
        {
            var entity = await _repository
                .GetQueryable<TEntity, TPrimaryKey>();
            return noTracking
                ? entity.AsSplitQuery()
                : entity
                    .AsNoTracking()
                    .AsSplitQuery();
        }

        public virtual async Task<BaseResponse> RemoveAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var response = await _repository.GetByIdAsync<TEntity, TPrimaryKey>(id, cancellationToken);
            if (!response.IsDone)
                return response;
            if (((DataResponse<TEntity>)response).Data == null)
                return response;
            if (((DataResponse<TEntity>)response).Data is FullAuditedBaseEntity<TPrimaryKey> fullAuditedBaseEntity)
            {

            }
            _repository.Remove<TEntity, TPrimaryKey>(((DataResponse<TEntity>)response).Data);

            var returnValue = await _repository.SaveChangesAsync();
            if (returnValue.IsDone)
            {
                return new DataResponse<List<TEntity>>()
                {
                    IsDone = returnValue.IsDone,
                    Message = returnValue.Message
                };
            }
            return returnValue;
        }

        public async Task<BaseResponse> RemoveRangeAsync(List<TPrimaryKey> ids, CancellationToken cancellationToken)
        {
            foreach (var id in ids)
            {
                var response = await _repository.GetByIdAsync<TEntity, TPrimaryKey>(id, cancellationToken);
                if (!response.IsDone)
                    return response;
                if (((DataResponse<TEntity>)response).Data == null)
                    return response;
                _repository.Remove<TEntity, TPrimaryKey>(((DataResponse<TEntity>)response).Data);

            }
            var returnValue = await _repository.SaveChangesAsync(cancellationToken);
            if (returnValue.IsDone)
            {
                return new DataResponse<List<TEntity>>()
                {
                    IsDone = returnValue.IsDone,
                    Message = returnValue.Message
                };
            }
            return returnValue;
        }

        public virtual async Task<BaseResponse> UpdateAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto entity)
            where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            if (entity == null)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = "undefined object"
                };
            }
            if (entity.Id == null)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = "undefined object"
                };
            }
            var response = (await _repository.GetByIdAsync<TEntity, TPrimaryKey>(entity.Id));
            if (!response.IsDone)
                return response;

            var originalEntity = ((DataResponse<TEntity>)response).Data;
            if (originalEntity == null)
                return response;

            _mapper.Map<TEntityCreateUpdateDto, TEntity>(entity, originalEntity);

            response = _repository.Update<TEntity, TPrimaryKey>(originalEntity);

            if (!response.IsDone)
                return response;

            response = await _repository.SaveChangesAsync();
            if (!response.IsDone)
                return response;

            return new DataResponse<TEntity>()
            {
                IsDone = response.IsDone,
                Data = originalEntity,
                Message = response.Message
            };
        }

        public async Task<BaseResponse> UpdateRangeAsync<TEntityCreateUpdateDto>(IEnumerable<TEntityCreateUpdateDto> entities) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            List<TEntity> originalEntities = new List<TEntity>();
            foreach (var entity in entities)
            {
                var queryResponse = await _repository.GetByIdAsync<TEntity, TPrimaryKey>(entity.Id);
                if (!queryResponse.IsDone)
                    return queryResponse;

                var originalEntity = ((DataResponse<TEntity>)queryResponse).Data;
                if (originalEntity == null)
                    return queryResponse;

                _mapper.Map<TEntityCreateUpdateDto, TEntity>(entity, originalEntity);
                originalEntities.Add(originalEntity);
            }


            var response = _repository.UpdateRange<TEntity, TPrimaryKey>(originalEntities);
            if (!response.IsDone)
                return response;
            response = await _repository.SaveChangesAsync();
            if (!response.IsDone)
                return response;
            return new DataResponse<IEnumerable<TEntity>>()
            {
                IsDone = response.IsDone,
                Data = originalEntities,
                Message = response.Message
            };
        }

        public virtual async Task<BaseResponse> SaveAsync<TEntityCreateUpdateDto>(TEntityCreateUpdateDto saveDto, CancellationToken cancellationToken) where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>
        {
            TEntity entityBase;
            Guid id = (Guid)Convert.ChangeType(saveDto.Id, typeof(TPrimaryKey));
            if (id == Guid.Empty)
            {
                var returnValue = await AddAsync<TEntityCreateUpdateDto>(saveDto, cancellationToken);
                if (!returnValue.IsDone) return returnValue;
                entityBase = (returnValue as DataResponse<TEntity>)?.Data;
            }
            else
            {
                var returnValueGetById = await GetByIdAsync<TEntityCreateUpdateDto>(saveDto.Id, cancellationToken);
                if (!returnValueGetById.IsDone) return returnValueGetById;

                if (((DataResponse<TEntity>)returnValueGetById).Data == null)
                {
                    var returnValue = await AddAsync(saveDto, cancellationToken);
                    if (!returnValue.IsDone) return returnValue;
                    entityBase = (returnValue as DataResponse<TEntity>)?.Data;
                }
                else
                {
                    var returnValue = await UpdateAsync(saveDto);
                    if (!returnValue.IsDone) return returnValue;
                    entityBase = (returnValue as DataResponse<TEntity>)?.Data;
                }
            }

            return new DataResponse<TEntity> { IsDone = true, Data = entityBase };
        }
    }
}
