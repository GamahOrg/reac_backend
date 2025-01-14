using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestGroundService : IBasicService<LitigationRequestGround, Guid>
    {
        Task<BaseResponse> AddReference(LitigationRequestGroundCreateUpdateDto model);
        Task<Guid> GetAddedReferenceId();
    }
}
