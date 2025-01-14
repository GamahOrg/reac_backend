using Core.GenericRepository.Shared.BaseEntities;
using Core.Localizations;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Litigation
{
    public partial class LitigationRequestService
    {
        private static List<string> ValidateAsync(LitigationCreateUpdateDto model)
        {
            var errors = new List<string>();
            #region Validations
           
            #endregion
            return errors;
        }
    }
}
