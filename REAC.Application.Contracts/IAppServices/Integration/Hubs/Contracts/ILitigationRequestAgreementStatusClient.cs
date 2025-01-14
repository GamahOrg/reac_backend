using Core.Shared.Responses;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts
{
    public interface ILitigationRequestAgreementStatusClient
    {
        Task ReceivedLitigationRequestAgreementStatus(BaseResponse litigationRequestAgreementStatus);
    }
}
