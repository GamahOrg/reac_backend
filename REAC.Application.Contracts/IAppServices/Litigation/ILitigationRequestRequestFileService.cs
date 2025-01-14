using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestRequestFileService : IBasicService<LitigationRequestRequestFile, Guid>
    {
        Task<BaseResponse> AddRequestFiles(LitigationRequestRequestFileCreateUpdateDto model);
    }
}
