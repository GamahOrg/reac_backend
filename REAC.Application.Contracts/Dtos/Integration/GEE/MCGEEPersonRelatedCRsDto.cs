namespace REAC.Application.Contracts.Dtos.Integration.GEE;

public class MCGEEPersonRelatedCRsDto
{
    public YefiResponseHeader? yefiResponseHeader { get; set; }
    public YefiErrorResponse? YefiErrorResponse { get; set; }
    public Metadata? metadata { get; set; }
    public string parityNameAr { get; set; }
    public string parityNameEn { get; set; }
    public string identifierNo { get; set; }
    public IdentifierType? identifierType { get; set; }
    public List<RelatedCRList>? relatedCRList { get; set; }
}

public class YefiErrorResponse
{
    public YefiResponseHeader yefiResponseHeader { get; set; }
    public YefiError yefiError { get; set; }
}


public class CompanyCharacterList
{
    public int? companyCharacterID { get; set; }
    public string companyCharacterDescriptionAr { get; set; }
    public string companyCharacterDescriptionEn { get; set; }
}


public class CrBasicInfoPersonalRelated
{
    public string crNationalNumber { get; set; }
    public string entityFullNameAr { get; set; }
    public double? capital { get; set; }
    public CapitalCurrency capitalCurrency { get; set; }
    public EntityNameLang entityNameLang { get; set; }
    public EntityType entityType { get; set; }
    public CompanyForm companyForm { get; set; }
    public CrStatus crStatus { get; set; }
    public List<CompanyCharacterList> companyCharacterList { get; set; }
    public List<ManagerPositionList> managerPositionList { get; set; }
    public ParityInfo parityInfo { get; set; }
}


public class IdentifierType
{
    public int? identifierTypeID { get; set; }
    public string identifierTypeDescAr { get; set; }
    public string identifierTypeDescEn { get; set; }
}

public class ManagerPositionList
{
    public int? managerPositionID { get; set; }
    public string managerPositionDescriptionAr { get; set; }
    public string managerPositionDescriptionEn { get; set; }
}

public class Metadata
{
    public int? count { get; set; }
    public int? limit { get; set; }
    public int? offset { get; set; }
}

public class ParityInfo
{
    public ParityType parityType { get; set; }
    public PartnershipType partnershipType { get; set; }
    public PartnerShare partnerShare { get; set; }
}

public class ParityType
{
    public int? parityTypeID { get; set; }
    public string parityTypeDescriptionAr { get; set; }
    public string parityTypeDescriptionEn { get; set; }
}

public class PartnerShare
{
    public int? cashContributionCount { get; set; }
    public int? inkindContributionCount { get; set; }
    public int? totalContributionCount { get; set; }
}

public class PartnershipType
{
    public int? partnershipTypeID { get; set; }
    public string partnershipTypeDescriptionAr { get; set; }
    public string partnershipTypeDescriptionEn { get; set; }
}

public class RelatedCRList
{
    public List<RelationTypeList> relationTypeList { get; set; }
    public CrBasicInfoPersonalRelated crBasicInfo { get; set; }
}

public class RelationTypeList
{
    public int? relationTypeID { get; set; }
    public string relationTypeDescAr { get; set; }
    public string relationTypeDescEn { get; set; }
}
