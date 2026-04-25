using System;
using System.Collections.Generic;

namespace REAC.Application.DTOs.Litigation
{
    public class LitigationRequestMircoDto
    {
        public Guid Id { get; set; }
        public string RequestCode { get; set; }
        public DateTime RequestDate { get; set; }
        public string FirstPartyName { get; set; }
        public string SecondPartyName { get; set; }
        public string RequestTypeName { get; set; }

        public string Status { get; set; }
        public string RequestStatusName { get; set; }
        public bool IsFinished { get; set; }
        public string LitigationRequestStage { get; set; }

        public string CurrentEmployee { get; set; }

        public bool AllowAssign { get; set; }
        public bool AllowFinish { get; set; }
        public bool AllowRevert { get; set; }
        public bool AllowConsultation { get; set; }
        public bool AllowExpert { get; set; }
        public bool AllowSession { get; set; }
        public bool AllowProcedure { get; set; }
        public bool AllowExtendPeriod { get; set; }
        public bool AllowAddSession { get; set; }
        public bool AllowPostponeSession { get; set; }
        public bool AllowJudgment { get; set; }

        public string ExtendPeriodStatus { get; set; }
        public bool? IsApproved { get; set; }
        public int TotalReconciliationPeriod { get; set; }
        public int TotalArbitrationPeriod { get; set; }
        public string IsDone { get; set; }
    }
}
