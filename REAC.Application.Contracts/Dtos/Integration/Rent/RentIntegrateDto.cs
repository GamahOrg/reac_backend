using Microsoft.EntityFrameworkCore.Query;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Integration.Wafi;
using REAC.Domain.Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Integration.Rent
{
    public class RentIntegrateDto
    {
        public WafiData Wafiresponse { get; set; }
        public PartyDTO FirstPartyDto { get; set; }
        public PartyDTO SecondPartyDto { get; set; }
        public EjarPropertyDto Property { get; set; }
        public string Key { get; set; }//رقم عقد الايجار
        [MaxLength(4)]
        public string SubKey { get; set; } = null!; // رقم النسخة
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
        public string ContractAddress { get; set; }
        public Guid Id { get; set; }

        public Data Project { get; set; }
        public bool IsDefendantRegistered { get; set; } = true;
        public bool IsPlaintiffRegistered { get; set; } = true;
    }

    public class WafiProjectDetails
    {
        public Data data { get; set; }
        public bool isDone { get; set; }
        public string message { get; set; }
    }

    public class Data
    {
        public string projectName { get; set; }
        public List<string> moJDeed { get; set; }
        public object realEstateNumber { get; set; }
        public string projectStatus { get; set; }
        public string licenseNumber { get; set; }
        public string projectDuration { get; set; }
        public DateTime projectStartDate { get; set; }
        public string projectDescription { get; set; }
        public int projectAccomplishmentPercentage { get; set; }
        public DateTime projectEndDate { get; set; }
        public int projectType { get; set; }
        public string licenseStatus { get; set; }
        public int licenseStatusId { get; set; }
        public DateTime licenseDate { get; set; }
        public string realEstateDeveloperName { get; set; }
        public string developerLicenseType { get; set; }
        public string realEstateDeveloperCRNumber { get; set; }
        public object entity700Number { get; set; }
        public string realEstateDeveloperRepresentativeName { get; set; }
        public string realEstateDeveloperRepresentativeNID { get; set; }
        public List<Unit> units { get; set; }
        public string totalNumberOfUnits { get; set; }
        public int totalLandAreaSize { get; set; }
        public int totalAreaSizeForProject { get; set; }
        public int numberOfBlocks { get; set; }
        public int numberOfBuildings { get; set; }
        public List<object> attachments { get; set; }
        public List<UnitDetail> unitDetails { get; set; }
    }

    public class Unit
    {
        public int type { get; set; }
        public int unitsNumber { get; set; }
    }

    public class UnitDetail
    {
        public int unitReferenceNumber { get; set; }
        public string unitCode { get; set; }
        public int unitTotalAreaSize { get; set; }
        public int unitAreaSize { get; set; }
        public string modelDescription { get; set; }
        public string modelNameUnit { get; set; }
    }

}
