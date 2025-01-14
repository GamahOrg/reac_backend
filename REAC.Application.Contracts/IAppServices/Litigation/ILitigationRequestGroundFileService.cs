using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestGroundFileService : IBasicService<LitigationRequestGroundFile, Guid>
    {
        Task<BaseResponse> AddReferenceFiles(LitigationRequestGroundFileCreateUpdateDto model);
    }
}
