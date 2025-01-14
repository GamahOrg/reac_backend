using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Seed.Integration
{
    public static class ContractIntegration
    {
        public static List<Contract> GetContracts()
        {
            return new List<Contract>()
            {
                //ايجار
                new Contract()
                {
                    ContractKey ="123456789012",
                    VersionNumber = "1234",
                    FirstPartyCitizenId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                },
                new Contract()
                {
                    ContractKey ="234567890112",
                    VersionNumber = "2345",
                    FirstPartyCitizenId = Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("0518066f-8555-4036-bb01-66398ab56a12"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                },
                new Contract()
                {
                    ContractKey ="345678901212",
                    VersionNumber = "3451",
                    FirstPartyCitizenId = Guid.Parse("38c675cb-ae4f-42a4-af22-ca6fdd0b3cee"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("37fe42f5-e8c2-4c6d-a560-141637abddee"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                },
                new Contract()
                {
                    ContractKey ="456789012312",
                    VersionNumber = "4512",
                    FirstPartyCitizenId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                },
                new Contract()
                {
                    ContractKey = "567890123412",
                    VersionNumber = "5123",
                    FirstPartyCitizenId = Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                },

                //املاك
                new Contract()
                {
                    AssociationName = "برج التقوى",
                    ContractKey ="123456789012",
                    FirstPartyCitizenId = Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                    FirstPartyCapacity = "مالك",
                    SecondPartyCitizenId= Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                    SecondPartyCapacity = "مالك",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                },
                new Contract()
                {
                    AssociationName = "برج اليمامة",
                    ContractKey ="234567890112",
                    FirstPartyCitizenId = Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    FirstPartyCapacity = "مالك",
                    SecondPartyCitizenId= Guid.Parse("0518066f-8555-4036-bb01-66398ab56a12"),
                    SecondPartyCapacity = "رئيس الجمعية",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                },
                new Contract()
                {
                    AssociationName = "برج الهنا",
                    ContractKey ="345678901212",
                    FirstPartyCitizenId = Guid.Parse("38c675cb-ae4f-42a4-af22-ca6fdd0b3cee"),
                    FirstPartyCapacity = "مالك",
                    SecondPartyCitizenId= Guid.Parse("37fe42f5-e8c2-4c6d-a560-141637abddee"),
                    SecondPartyCapacity = "مالك",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                },
                new Contract()
                {
                    AssociationName = "برج الريف",
                    ContractKey ="456789012312",
                    FirstPartyCitizenId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                    FirstPartyCapacity = "رئيس الجمعية",
                    SecondPartyCitizenId= Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    SecondPartyCapacity = "نائب رئيس الجمعية",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                },
                new Contract()
                {
                    AssociationName = "برج السلامة",
                    ContractKey = "567890123412",
                    FirstPartyCitizenId = Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                },

                //وافى
                new Contract()
                {
                    AssociationName = "",
                    ContractKey ="12345678901234567890",
                    FirstPartyFacilityId = Guid.Parse("25d5b134-3f63-4daf-a92d-0ce224ee024c"),
                    FirstPartyCapacity = "مطور عقارى",
                    SecondPartyFacilityId = Guid.Parse("3cddd129-2597-4258-b5fb-12987e1f1b94"),
                    SecondPartyCapacity = "إستشارى هندسي",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Wafi,
                },
                new Contract()
                {
                    AssociationName = "",
                    ContractKey ="23456789012345678901",
                    FirstPartyFacilityId = Guid.Parse("deef9a46-e906-4f89-950b-0d0e8cb7e5a1"),
                    FirstPartyCapacity = "مطور عقارى",
                    SecondPartyFacilityId= Guid.Parse("3cddd129-2597-4258-b5fb-12987e1f1b94"),
                    SecondPartyCapacity = "إستشارى هندسي",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Wafi,
                },
                new Contract()
                {
                    AssociationName = "",
                    ContractKey ="34567890123456789012",
                    FirstPartyFacilityId = Guid.Parse("25d5b134-3f63-4daf-a92d-0ce224ee024c"),
                    FirstPartyCapacity = "مطور عقارى",
                    SecondPartyCitizenId= Guid.Parse("9b09716f-1122-4d61-a18c-11b0f62587e0"),
                    SecondPartyCapacity = "محاسب قانونى",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Wafi,
                },
                new Contract()
                {
                    AssociationName = "",
                    ContractKey ="45678901234567890123",
                    FirstPartyFacilityId = Guid.Parse("deef9a46-e906-4f89-950b-0d0e8cb7e5a1"),
                    FirstPartyCapacity = "مطور عقارى",
                    SecondPartyFacilityId= Guid.Parse("9b09716f-1122-4d61-a18c-11b0f62587e0"),
                    SecondPartyCapacity = "محاسب قانونى",
                    LitigationRequestContractTypeId = LitigationRequestContractType.Wafi,
                },

                // الوساطة العقارية
                new Contract()
                {
                    ContractKey ="12345678901234567890",
                    VersionNumber = "1234",
                    FirstPartyCitizenId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("f6df6f09-e1ab-4203-ad36-71d0464ec50a"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                },
                new Contract()
                {
                    ContractKey ="23456789012345678901",
                    VersionNumber = "2345",
                    FirstPartyCitizenId = Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("0518066f-8555-4036-bb01-66398ab56a12"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                },
                new Contract()
                {
                    ContractKey ="34567890123456789012",
                    VersionNumber = "3451",
                    FirstPartyCitizenId = Guid.Parse("38c675cb-ae4f-42a4-af22-ca6fdd0b3cee"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("37fe42f5-e8c2-4c6d-a560-141637abddee"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                },
                new Contract()
                {
                    ContractKey ="45678901234567890123",
                    VersionNumber = "4512",
                    FirstPartyCitizenId = Guid.Parse("e5bfd67b-0527-47f5-afa1-78b2b08c15ef"),
                    FirstPartyCapacity = "مستأجر",
                    SecondPartyCitizenId= Guid.Parse("fa351127-a037-4902-9ab7-3cb3db58b546"),
                    SecondPartyCapacity = "مؤجر",
                    LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                },
            };
        }

    }
    public class Contract
    {
        public LitigationRequestContractType LitigationRequestContractTypeId { get; set; }

        public string? ContractKey { get; set; }

        public string? VersionNumber { get; set; }

        public string? AssociationName { get; set; }

        public Guid? FirstPartyCitizenId { get; set; }

        public Guid? FirstPartyFacilityId { get; set; }

        public string FirstPartyCapacity { get; set; } = null!;

        public Guid? SecondPartyCitizenId { get; set; }

        public Guid? SecondPartyFacilityId { get; set; }

        public string SecondPartyCapacity { get; set; } = null!;
    }
}
