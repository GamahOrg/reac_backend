using REAC.EntityFrameworkCore.Entities.Litigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestForCorrespondencesDto
    {
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public bool AllowAdd { get; set; }
        public List<LitigationRequestCorrespondenceDto>? Correspondences { get; set; }
    }
}
