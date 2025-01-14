using Core.GenericRepository.Shared.BaseEntities;
using Core.Localizations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.System
{
    public class UploadFilesDto : BaseEntity<Guid>
    {
        [Required(ErrorMessageResourceName = "FileRequired", ErrorMessageResourceType = typeof(Localizations))]
        public IFormFileCollection? Files { get; set; }
    }
}
