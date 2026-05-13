using REAC.Domain.Shared.Enum;
using System.Collections.Generic;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Citizen;
using System;

namespace REAC.Application.Contracts.Dtos.Arbitration
{
    public class LitigationRequestForArbitrationReportDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitrationReportCreateUpdateDto? Report { get; set; }
    }

    public class LitigationRequestForArbitrationCorrectionRequestDto
    {
        public bool AllowAdd { get; set; }
        public bool AllowResponse { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public List<LitigationRequestArbitrationCorrectionRequestGetDto> CorrectionRequests { get; set; } = new();
    }

    public class LitigationRequestForArbitrationReportExplanationRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitrationReportExplanationRequestGetDto? Report { get; set; }
        public LitigationRequestArbitrationReportExplanationRequestGetDto? OtherReport { get; set; }
    }

    public class LitigationRequestForArbitratorRecusalRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitratorRecusalRequestGetDto? request { get; set; }
    }

    public class LitigationRequestForAllArbitratorCountDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorsCount { get; set; }
        public int? SecondPartyArbitratorsCount { get; set; }
        public int? FirstPartyArbitratorsCount { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForAllArbitratorNominationListDto
    {
        public bool AllowAdd { get; set; }
        public int? ListCount { get; set; }
        public bool AllowDecision { get; set; }
        public List<CitizenArbitratorShortStatusDto> CitizenArbitratorShortStatusDtos { get; set; } = new();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForAllArbitratorCharmanNominationListDto
    {
        public bool AllowAdd { get; set; }
        public bool AllowDecision { get; set; }
        public int? ListCount { get; set; }
        public List<CitizenArbitratorCharmanShortStatusDto> CitizenArbitratorCharmanShortStatusDtos { get; set; } = new();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForAllArbitratorNominationListWithPartiesValueDto
    {
        public bool AllowAdd { get; set; }
        public List<CitizenArbitratorShortStatusWithValueDto> CitizenArbitratorShortStatusDtos { get; set; } = new();
        public List<CitizenArbitratorShortStatusWithValueDto> CitizenSelectedArbitratorShortStatusDtos { get; set; } = new();
        public List<CitizenArbitratorShortStatusWithValueDto> CitizenSelectedUserArbitratorShortStatusDtos { get; set; } = new();
        public List<CitizenArbitratorShortStatusWithValueDto> CitizenRejectedArbitratorShortStatusDtos { get; set; } = new();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForArbitratorSelectionDurationDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorSelectionDuration { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForArbitratorCountDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorsCount { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
}
