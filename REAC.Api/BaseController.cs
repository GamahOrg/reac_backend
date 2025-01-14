using Core.GenericRepository.Shared.BaseEntities;
using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts;

namespace REAC.Api
{
    public class BaseController<TEntity, TEntityCreateUpdateDto, TEntityGetDto, TPrimaryKey> : ControllerBase
        where TEntityGetDto : BaseEntity<TPrimaryKey>
        where TEntityCreateUpdateDto : BaseEntity<TPrimaryKey>

    {
        /// <summary>
        /// Required if you want to use Authorization Permission To give Module for permission
        /// </summary>

        IBasicService<TEntity, TPrimaryKey> _basicService;

        public BaseController(IBasicService<TEntity, TPrimaryKey> basicService)
        {
            _basicService = basicService;
        }

        [PermissionRequired(Policy.Create)]
        [HttpPost(nameof(Create))]
        public virtual async Task<IActionResult> Create([FromBody] TEntityCreateUpdateDto entity,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.AddAsync<TEntityCreateUpdateDto>(entity, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.Create)]
        [HttpPost(nameof(CreateRange))]
        public async Task<IActionResult> CreateRange([FromBody] List<TEntityCreateUpdateDto> entities,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.AddRangeAsync<TEntityCreateUpdateDto>(entities, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.Edit)]
        [HttpPut(nameof(Update))]
        public virtual async Task<IActionResult> Update([FromBody] TEntityCreateUpdateDto entity,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.UpdateAsync<TEntityCreateUpdateDto>(entity);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.Edit)]
        [HttpPut(nameof(UpdateRange))]
        public async Task<IActionResult> UpdateRange([FromBody] List<TEntityCreateUpdateDto> entities,
            CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.UpdateRangeAsync<TEntityCreateUpdateDto>(entities);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.View)]
        [HttpGet(nameof(Get))]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.GetListAsync<TEntityGetDto>(cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }


        [PermissionRequired(Policy.View)]
        [HttpGet(nameof(GetPaged))]
        public IActionResult GetPaged(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = _basicService.GetList<TEntityGetDto>(pageIndex, pageSize, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.View)]
        [HttpGet(nameof(GetById))]
        public async Task<IActionResult> GetById(TPrimaryKey id, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.GetByIdAsync<TEntityGetDto>(id, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.Delete)]
        [HttpDelete(nameof(Delete))]
        public async Task<IActionResult> Delete(TPrimaryKey id, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.RemoveAsync(id, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [PermissionRequired(Policy.Delete)]
        [HttpDelete(nameof(DeleteRange))]

        public async Task<IActionResult> DeleteRange(List<TPrimaryKey> ids, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _basicService.RemoveRangeAsync(ids, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }
    }
}