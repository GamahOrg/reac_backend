using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysCityService : IBasicService<SysCity, Guid>
    {
        /// <summary>
        /// This method returns <see cref="List{TEntityCreateUpdateDto}"/> without any filter. Call only when you want to pull all the data from the source.
        /// </summary>
        /// <typeparam name="TEntityCreateUpdateDto">The type of the entity.</typeparam>
        /// <param name="cancellationToken"> A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>Returns <see cref="Task"/> of <see cref="List{T}"/>.</returns>
        Task<BaseResponse> GetListByAreaIdAsync<TEntityCreateUpdateDto>(Guid AreaId, CancellationToken cancellationToken = default);
    }
}
