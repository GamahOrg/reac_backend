using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class RentRequestDTO
    {
        public PartyDTO FirstPartyDto { get; set; }
        public PartyDTO SecondPartyDto { get; set; }
        public string ContractKey { get; set; }//رقم عقد الايجار
        [MaxLength(4)]
        public string VersionNumber { get; set; } = null!; // رقم النسخة
        public LitigationRequestOwnerType FirstPartyCapacity { get; set; }
        public string ContractAddress { get; set; }
        public Guid Id { get; set; }
    }

    public class EjarApiResponse
    {
        [JsonProperty("Header")]
        public Header Header { get; set; }

        [JsonProperty("Body")]
        public Body Body { get; set; }
    }

    public class Header
    {
        [JsonProperty("ResTime")]
        public DateTime ResTime { get; set; }

        [JsonProperty("ChId")]
        public string ChId { get; set; }

        [JsonProperty("RefId")]
        public string RefId { get; set; }

        [JsonProperty("ReqID")]
        public string ReqID { get; set; }

        [JsonProperty("Status")]
        public Status Status { get; set; }
    }

    public class Status
    {
        [JsonProperty("Code")]
        public int Code { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    }

    public class Body
    {
        [JsonProperty("data")]
        public List<RentContract> Data { get; set; }

        [JsonProperty("included")]
        public List<Included> Included { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class RentContract
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public ContractAttributes Attributes { get; set; }

        [JsonProperty("relationships")]
        public ContractRelationships Relationships { get; set; }
    }

    public class ContractAttributes
    {
        [JsonProperty("contract_no")]
        public string ContractNo { get; set; }

        [JsonProperty("contract_version")]
        public int ContractVersion { get; set; }

        [JsonProperty("contract_minor_number")]
        public int ContractMinorNumber { get; set; }

        [JsonProperty("contract_start_date")]
        public DateTime ContractStartDate { get; set; }

        [JsonProperty("contract_end_date")]
        public DateTime ContractEndDate { get; set; }

        [JsonProperty("contract_type")]
        public LocalizedString ContractType { get; set; }

        [JsonProperty("termination_date")]
        public DateTime? TerminationDate { get; set; }

        [JsonProperty("contract_revoke_date")]
        public DateTime? ContractRevokeDate { get; set; }

        [JsonProperty("contract_status")]
        public LocalizedString ContractStatus { get; set; }

        [JsonProperty("is_sublease")]
        public bool IsSublease { get; set; }

        [JsonProperty("total_rent_amount")]
        public string TotalRentAmount { get; set; }
    }

    public class LocalizedString
    {
        [JsonProperty("en")]
        public string English { get; set; }

        [JsonProperty("ar")]
        public string Arabic { get; set; }
    }

    public class ContractRelationships
    {
        [JsonProperty("lessor_individuals")]
        public RelationshipData LessorIndividuals { get; set; }

        [JsonProperty("tenant_individuals")]
        public RelationshipData TenantIndividuals { get; set; }

        [JsonProperty("lessor_representative")]
        public RelationshipData LessorRepresentative { get; set; }

        [JsonProperty("tenant_representative")]
        public RelationshipData TenantRepresentative { get; set; }

        [JsonProperty("property")]
        public RelationshipData Property { get; set; }

        [JsonProperty("rental_fee")]
        public RelationshipData RentalFee { get; set; }
    }

    public class RelationshipData
    {
        [JsonProperty("data")]
        public object Data { get; set; }
    }

    public class Included
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("attributes")]
        public dynamic Attributes { get; set; } // Replace with specific class if necessary

        [JsonProperty("relationships")]
        public object Relationships { get; set; }
    }

    public class Meta
    {
        [JsonProperty("count")]
        public int Count { get; set; }
    }

    public interface ILessorTenant
    {

    }

    public class MultipleLessors
    {
        public List<LessorAttributes> Lessors { get; set; }
        public bool? Multiple { get; set; }

    }

    // المؤجر
    public class LessorAttributes : ILessorTenant
    {
        [JsonProperty("contract_id")]
        public dynamic ContractId { get; set; }

        [JsonProperty("party_role")]
        public dynamic PartyRole { get; set; }

        [JsonProperty("party_type")]
        public dynamic PartyType { get; set; }

        [JsonProperty("full_lessor_name")]
        public dynamic FullLessorName { get; set; }

        [JsonProperty("lessor_id_type")]
        public dynamic LessorIdType { get; set; }

        [JsonProperty("lessor_id_number")]
        public dynamic LessorIdNumber { get; set; }

    }

    //المستأجر
    public class TenantAttributes : ILessorTenant
    {
        [JsonProperty("contract_id")]
        public dynamic ContractId { get; set; }

        [JsonProperty("party_role")]
        public dynamic PartyRole { get; set; }

        [JsonProperty("party_type")]
        public dynamic PartyType { get; set; }

        [JsonProperty("full_name_of_tenant")]
        public dynamic FullTenantName { get; set; }

        [JsonProperty("tenant_id_type")]
        public dynamic TenantIdType { get; set; }

        [JsonProperty("tenant_id_number")]
        public dynamic TenantIdNumber { get; set; }

        [JsonProperty("tenant_dob_hijri")]
        public dynamic TenantDobHijri { get; set; }
    }

    // العقار
    public class PropertyAttributes
    {
        public dynamic City { get; set; }
        public dynamic Province { get; set; }
        public dynamic Region { get; set; }
        public dynamic StreetName { get; set; }
        public dynamic BuildingNumber { get; set; }
        public dynamic PostCost { get; set; }
        public dynamic AdditionalNumber { get; set; }
        public dynamic Coordiantes { get; set; }
        public dynamic Latitude { get; set; }
        public dynamic Longitude { get; set; }
    }

    public class EjarPartiesDto
    {

        public ILessorTenant Plaintiff { get; set; }
        public ILessorTenant Defendant { get; set; }
        public PropertyAttributes Property { get; set; }

    }

    public class PlaintiffDefendantSharedEjarDto
    {
        public string PlaintiffEjarNationalId { get; set; }
        public string PlaintiffEjarName { get; set; }
        public string PlaintiffEjarCapacity { get; set; }
        public string PlaintiffEjarType { get; set; }
        public string? PlaintiffDOBHijri { get; set; }

        public string DefendantEjarNationalId { get; set; }
        public string DefendantEjarName { get; set; }
        public string DefendantEjarCapacity { get; set; }
        public string DefendantEjarType { get; set; }
        public string? DefendnatDOBHijri { get; set; }

        public EjarPropertyDto EjarProperty { get; set; }

    }

    public class EjarPropertyDto
    {
        public string City { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PostCost { get; set; }
        public string AdditionalNumber { get; set; }
        public string Coordiantes { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
}
