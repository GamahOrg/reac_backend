using REAC.Application.Contracts.Dtos.Citizen;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Session
{
    public class LitigationRequestSessionGetListDto
    {
        public Guid? LitigationRequestId { get; set; }
        public string? RequestCode { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public string? Title { get; set; }
        public DateTime? SessionDateTime { get; set; }
        public virtual CitizenShortDto? FirstPartyCitizen { get; set; }
        public virtual FacilityShortDto? FirstPartyFacility { get; set; }
        public virtual CitizenShortDto? SecondPartyCitizen { get; set; }
        public virtual FacilityShortDto? SecondPartyFacility { get; set; }
    }
}
