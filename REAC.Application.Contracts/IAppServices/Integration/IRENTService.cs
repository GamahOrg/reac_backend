using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface IRENTService
    {
        Task<DataResponse<EjarApiResponse>> GetRentContractDetails(long contractNumber);
        Task<DataResponse<EjarApiResponse>> GetRentContractFinancialDetails(long contractNumber);

    }
}
