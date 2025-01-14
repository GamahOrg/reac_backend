using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface INafathAgreementRequestService
    {
        void Add(NafathAgreementRequestTransactionsDto nafathAgreementTransaction);
        void Remove(NafathAgreementRequestTransactionsDto nafathAgreementTransaction);
        List<NafathAgreementRequestTransactionsDto> GetAll();

    }
}
