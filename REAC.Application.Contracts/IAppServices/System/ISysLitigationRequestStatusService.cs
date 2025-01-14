using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysLitigationRequestStatusService : IBasicService<SysLitigationRequestStatus, RequestStatus>
    {
    }
}
