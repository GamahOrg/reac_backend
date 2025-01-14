using Core.Shared.Responses;
using Microsoft.AspNetCore.Http;
using REAC.Application.Contracts.Dtos.Shared;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysFileManagerService : IBasicService<SysFileManager, Guid>
    {
        Task<BaseResponse> AddFile(SysFileManagerCreateUpdateDto model);
        Task<Guid> GetLatestFile();
        Task<BaseResponse> UploadAsync(IFormFileCollection files);
        Task<List<SysFileManager>> GetAsync(List<Guid>? ids);
        Task RemoveAsync(Guid id);
    }
}
