using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationRequestForNotes
    {
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public bool AllowAdd { get; set; }
        public List<LitigationRequestNotesDto>? Notes { get; set; }
    }
}
