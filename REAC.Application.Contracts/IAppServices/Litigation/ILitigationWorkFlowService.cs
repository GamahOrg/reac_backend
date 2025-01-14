using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public  interface ILitigationWorkFlowService 
    {
        Task<BaseResponse> UpdateLitigationStatusAsync(ResponseWorkFlowDto model, Guid requestId);
    }
}
