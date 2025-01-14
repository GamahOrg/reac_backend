using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Integration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class NafathAgreementRequestService :  INafathAgreementRequestService
    {
        private List<NafathAgreementRequestTransactionsDto> cachedAgreementRequestsList = new List<NafathAgreementRequestTransactionsDto>();
        
        public void Add(NafathAgreementRequestTransactionsDto nafathAgreementTransaction)
        {
            cachedAgreementRequestsList.Add(nafathAgreementTransaction);
        }

        public void Remove(NafathAgreementRequestTransactionsDto nafathAgreementTransaction)
        {
            cachedAgreementRequestsList.Remove(nafathAgreementTransaction);
        }

        public List<NafathAgreementRequestTransactionsDto> GetAll()
        {
            return cachedAgreementRequestsList;
        }

    }
}
