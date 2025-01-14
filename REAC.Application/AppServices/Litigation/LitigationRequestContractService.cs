using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Localizations;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Integration.Properties;
using REAC.Application.Contracts.Dtos.Integration.RealState;
using REAC.Application.Contracts.Dtos.Integration.Rent;
using REAC.Application.Contracts.Dtos.Integration.Wafi;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Contract;
using REAC.Application.Contracts.Dtos.Litigation.Create.Defendant;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Contract;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.Litigation
{
    public partial class LitigationRequestContractService : BasicService<LitigationRequestContract, Guid>, IUserProviderService
    {
        private REACDBContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        //private readonly ICitizenLitigationRequestLawyerAgentService _citizenLawyerAgent;
        private readonly ISysFileManagerService _fileService;
        public LitigationRequestContractService(REACDBContext context, ISysFileManagerService fileService, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen) : base(repository, mapper, loggerManager, jwtReader)
        {
            _context = context;
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
        }


        //Get Contract by contractKey
        public async Task<LitigationRequestContract> GetContract(string contractKey)
        {
            var quarable = await GetQueryableAsync();
            var result = quarable.Include(x => x.FirstPartyCitizen)
                .LastOrDefault(x => x.ContractKey == contractKey);
            return result;
        }

        #region Add Rent Contract
        public async Task<BaseResponse> AddContractDocuments(ContractDocuments model)
        {
            try
            {
                var contract = (await _repository.GetQueryable<LitigationRequestContract, Guid>())
                                       .Where(x => x.Id == model.ContractId).ToList().FirstOrDefault();

                switch (contract.LitigationRequestContractTypeId)
                {
                    case LitigationRequestContractType.Rent:
                        contract.ContractFileId = model.ContractFileId;
                        break;
                    case LitigationRequestContractType.Wafi:
                        contract.ContractFileId = model.ContractFileId;
                        break;
                    case LitigationRequestContractType.Properties:
                        contract.FoundingLetterFileId = model.FoundingLetterFileId;
                        contract.AssociationRegistrationCertificateFileId = model.AssociationRegistrationCertificateFileId;
                        contract.InternalRegulationsFileId = model.InternalRegulationsFileId;
                        break;
                    case LitigationRequestContractType.RealEstateBrokerage:
                        contract.ContractFileId = model.ContractFileId;
                        break;
                    case LitigationRequestContractType.OtherServices:
                        break;
                }
                var response = _repository.SaveChangesAsync().Result;
                if (!response.IsDone)
                    return response;

                var contractUpdateDto = new LitigationRequestContractCreateUpdateDto
                {
                    Id = contract.Id,
                    LitigationRequestContractTypeId = (int)contract.LitigationRequestContractTypeId,
                    Key = contract.ContractKey,
                    SubKey = contract.VersionNumber,
                    ContractFileId = contract.ContractFileId,
                    FirstPartyFacilityId = contract.FirstPartyFacilityId ?? null,
                    FirstPartyCapacity = contract.FirstPartyCapacity,
                    SecondPartyFacilityId = contract.SecondPartyFacilityId,
                    SecondPartyCapacity = contract.SecondPartyCapacity,
                    SecondPartyName = contract.SecondPartyName,

                };

                return await base.UpdateAsync(contractUpdateDto);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {

                    Exception = ex.Message + "\n" + ex.ToString(),
                    IsDone = false
                };
            }
        }
        #endregion




        #region Add Defendant 
        public async Task<BaseResponse> AddDefendant(DefendantCreateUpdateDto model)
        {
            return await base.UpdateAsync(model);
        }
        #endregion


        public async Task<(bool, Guid, Guid, Guid)> CheckContractExist(string contractKey, string versionNumber)
        {
            var quarable = await GetQueryableAsync();
            bool contractExist = false;
            Guid firstPartyCitizenId = new Guid();
            Guid secondPartyCitizenId = new Guid();
            Guid Id = new Guid();
            var exist = quarable.Where(x => x.ContractKey == contractKey && x.VersionNumber == versionNumber).FirstOrDefault();
            if (exist != null)
            {
                contractExist = true;
                firstPartyCitizenId = exist.FirstPartyCitizenId.Value;
                secondPartyCitizenId = exist.SecondPartyCitizenId.Value;
                Id = exist.Id;

            }
            return (contractExist, firstPartyCitizenId, secondPartyCitizenId, Id);
        }

        public async Task<BaseResponse> RentIntegrationValidation(RentIntegrateDto rentIntegrateDto, LitigationRequestOwnerType ownerType, string contractNumber, string versionNumber)
        {
            if (rentIntegrateDto.Key == contractNumber && rentIntegrateDto.SubKey == versionNumber && ownerType == LitigationRequestOwnerType.LawyerForIndividual)
            {
                var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/
                string logdenUserIdentityNumber = "12122nnnnnnnnn";

                switch (ownerType)
                {
                    case LitigationRequestOwnerType.Personal:
                        if (logdenUserIdentityNumber == rentIntegrateDto.FirstPartyDto.IdentityNumber)
                            rentIntegrateDto.FirstPartyDto.IsPretender = true;
                        else if (logdenUserIdentityNumber == rentIntegrateDto.SecondPartyDto.IdentityNumber)
                            rentIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.CompanyRepresentative:
                        var representativeCitizenList = await _citizen.GetRepresentativeCitizens(logdenUser);
                        if (representativeCitizenList.Any(x => x.Facility700Number == rentIntegrateDto.FirstPartyDto.FacilityNumber && x.CommercialRegisterNumber == rentIntegrateDto.FirstPartyDto.CommercialRegistrationNo))
                            rentIntegrateDto.FirstPartyDto.IsPretender = true;

                        if (representativeCitizenList.Any(x => x.Facility700Number == rentIntegrateDto.SecondPartyDto.FacilityNumber && x.CommercialRegisterNumber == rentIntegrateDto.SecondPartyDto.CommercialRegistrationNo))
                            rentIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.LawyerForIndividual:
                        //var lawyerCitizenList = await _citizenLawyerAgent.GetLawyerCitizens(logdenUser);
                        //if (lawyerCitizenList.Contains(rentIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    rentIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (lawyerCitizenList.Contains(rentIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    rentIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.AgentForIndividual:
                        //var agentCitizenList = await _citizenLawyerAgent.GetAgentCitizens(logdenUser);
                        //if (agentCitizenList.Contains(rentIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    rentIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (agentCitizenList.Contains(rentIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    rentIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    default:
                        throw new Exception("");
                        //throw new ExceptionResponse(Localizations.NoPermissionToViewLitigation);
                }
                if (rentIntegrateDto.FirstPartyDto.IsPretender == true)
                {
                    rentIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    rentIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (rentIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    rentIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    rentIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }


                return new DataResponse<RentIntegrateDto> { IsDone = true, Message = Localizations.RetrievedSuccefully, Data = rentIntegrateDto };
            }
            else
                return new DataResponse<RentIntegrateDto> { IsDone = false, Message = Localizations.ContractNotFound, Data = new RentIntegrateDto() };


        }


        #region Real Integration Simulation
        public async Task<BaseResponse> RealStateIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string contractNumber = "")
        {
            var realStateIntegrateDto = new RealStateIntegrateDto
            {
                ContractKey = "1234abcd",
                FirstPartyDto = new PartyDTO
                {
                    FullName = "FirstPartyName",
                    IdentityNumber = "123abcdooooo",
                    NationalAddress = "testAddress",
                    Nationality = "Egyption",
                    FacilityNumber = null,
                    CommercialRegistrationNo = null,
                    FacilityName = null,
                    Capacity = "مستاجر",
                    Mobile = "12345678",
                    Email = "first@gmail.com",
                    BirthDate = new DateTime(1999, 1, 1)
                },
                SecondPartyDto = new PartyDTO
                {
                    Nationality = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Egyption",
                    FullName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "SecondPArtyName",
                    IdentityNumber = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "987wwwccccccc",
                    NationalAddress = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "testAddress",
                    FacilityNumber = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "123987" : null,
                    CommercialRegistrationNo = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "9999999" : null,
                    FacilityName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "FacilityName" : null,
                    Capacity = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "مالك",
                    Mobile = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "321654987",
                    Email = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Second@gmail.com",
                    BirthDate = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? DateTime.MinValue : new DateTime(1999, 2, 2)

                },
                FirstPartyCapacity = FirstPartyCapacity,
                ContractAddress = "ContractAddressTest"
            };
            //   var contractExist = CheckRealStateContractExist(contractNumber);
            //if (contractExist.Result.Item1 == false)
            //{
            #region Insert Contract and Parties
            var contract = new LitigationRequestContract
            {

                ContractKey = realStateIntegrateDto.ContractKey,
                FirstPartyCapacity = realStateIntegrateDto.FirstPartyCapacity.ToString(),
                LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                SecondPartyCapacity = realStateIntegrateDto.SecondPartyDto.Capacity,
                SecondPartyEmail = realStateIntegrateDto.SecondPartyDto.Email,
                ContractFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                SecondPartyIdentityNumber = realStateIntegrateDto.SecondPartyDto.IdentityNumber,
                SecondPartyMobile = realStateIntegrateDto.SecondPartyDto.FullName,
                SecondPartyName = realStateIntegrateDto.SecondPartyDto.FullName,

            };
            var checkFirstParty = _citizen.CheckCitizenExist(realStateIntegrateDto.FirstPartyDto.IdentityNumber);
            var checkSecondParty = _citizen.CheckCitizenExist(realStateIntegrateDto.SecondPartyDto.IdentityNumber);
            if (checkFirstParty.Result.Item2 == true)
            {
                contract.FirstPartyCitizenId = checkFirstParty.Result.Item1;
            }
            else
            {
                contract.FirstPartyCitizen = new Citizen
                {

                    NationalityId = (await _repository.GetQueryable<SysNationality, Guid>()).FirstOrDefault(x => x.NationalityName == realStateIntegrateDto.FirstPartyDto.Nationality).Id,
                    NationalAddress = "NationalAddress",
                    Mobile = "01255855852",
                    BirthDate = realStateIntegrateDto.FirstPartyDto.BirthDate.Value,
                    BuildingNumber = "1234567891",
                    AreaId = Guid.NewGuid(),
                    CityId = Guid.NewGuid(),
                    District = "District",
                    Email = "firstparty@gmail.com",
                    FullName = realStateIntegrateDto.FirstPartyDto.FullName,
                    GenderId = Gender.Male,
                    IdentityExpiryDate = DateTime.Now.AddDays(5),
                    IdentityNumber = realStateIntegrateDto.FirstPartyDto.IdentityNumber,
                    IdentityFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                    IdentityTypeId = IdentityType.NationalIdentity,
                    Street = "Street",
                    UnitNumber = "UnitNumber",
                    ShortName = "ShortName",
                };
            }
            if (checkSecondParty.Result.Item2 == true)
            {
                contract.SecondPartyCitizenId = checkSecondParty.Result.Item1;
            }
            else
            {
                contract.SecondPartyCitizen = new Citizen
                {

                    NationalAddress = "NationalAddress",
                    Mobile = "01025588558",
                    BirthDate = realStateIntegrateDto.SecondPartyDto.BirthDate.Value,
                    BuildingNumber = "1234567891",
                    AreaId = Guid.NewGuid(),
                    CityId = Guid.NewGuid(),
                    District = "District",
                    Email = "firstparty@gmail.com",
                    FullName = realStateIntegrateDto.SecondPartyDto.FullName,
                    GenderId = Gender.Male,
                    IdentityExpiryDate = DateTime.Now.AddDays(5),
                    IdentityNumber = realStateIntegrateDto.SecondPartyDto.IdentityNumber,
                    IdentityFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                    IdentityTypeId = IdentityType.NationalIdentity,
                    Street = "Street",
                    UnitNumber = "UnitNumber",
                    ShortName = "ShortName",
                    NationalityId = (await _repository.GetQueryable<SysNationality, Guid>()).FirstOrDefault(x => x.NationalityName == realStateIntegrateDto.SecondPartyDto.Nationality).Id,
                };
            }
            await base.AddAsync(contract, default);
            realStateIntegrateDto.FirstPartyDto.CitizenId = contract.FirstPartyCitizen == null ? checkFirstParty.Result.Item1 : contract.FirstPartyCitizen.Id;
            realStateIntegrateDto.SecondPartyDto.CitizenId = contract.SecondPartyCitizen == null ? checkSecondParty.Result.Item1 : contract.SecondPartyCitizen.Id;
            realStateIntegrateDto.Id = contract.Id;
            #endregion
            //    }
            //else
            //{
            //    realStateIntegrateDto.Id = contractExist.Result.Item4;
            //    realStateIntegrateDto.FirstPartyDto.FirstPartyCitizenId = contractExist.Result.Item2;
            //    realStateIntegrateDto.SecondPartyDto.SecondPartyCitizenId = contractExist.Result.Item3;
            //}
            return await RealStatentegrationValidation(realStateIntegrateDto, FirstPartyCapacity, contractNumber);
        }

        public async Task<(bool, Guid, Guid, Guid)> CheckRealStateContractExist(string contractKey)
        {
            var quarable = await GetQueryableAsync();
            bool contractExist = false;
            Guid firstPartyCitizenId = new Guid();
            Guid secondPartyCitizenId = new Guid();
            Guid Id = new Guid();
            var exist = quarable.Where(x => x.ContractKey == contractKey).FirstOrDefault();
            if (exist != null)
            {
                contractExist = true;
                firstPartyCitizenId = exist.FirstPartyCitizenId.Value;
                secondPartyCitizenId = exist.SecondPartyCitizenId.Value;
                Id = exist.Id;

            }
            return (contractExist, firstPartyCitizenId, secondPartyCitizenId, Id);
        }


        public async Task<BaseResponse> RealStatentegrationValidation(RealStateIntegrateDto realStateIntegrateDto, LitigationRequestOwnerType ownerType, string contractNumber)
        {
            if (realStateIntegrateDto.ContractKey == contractNumber && ownerType == LitigationRequestOwnerType.LawyerForIndividual)
            {
                var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/
                string logdenUserIdentityNumber = "12122nnnnnnnnn";

                switch (ownerType)
                {
                    case LitigationRequestOwnerType.Personal:
                        if (logdenUserIdentityNumber == realStateIntegrateDto.FirstPartyDto.IdentityNumber)
                            realStateIntegrateDto.FirstPartyDto.IsPretender = true;
                        else if (logdenUserIdentityNumber == realStateIntegrateDto.SecondPartyDto.IdentityNumber)
                            realStateIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.CompanyRepresentative:
                        var representativeCitizenList = await _citizen.GetRepresentativeCitizens(logdenUser);
                        if (representativeCitizenList.Any(x => x.Facility700Number == realStateIntegrateDto.FirstPartyDto.FacilityNumber && x.CommercialRegisterNumber == realStateIntegrateDto.FirstPartyDto.CommercialRegistrationNo))
                            realStateIntegrateDto.FirstPartyDto.IsPretender = true;

                        if (representativeCitizenList.Any(x => x.Facility700Number == realStateIntegrateDto.SecondPartyDto.FacilityNumber && x.CommercialRegisterNumber == realStateIntegrateDto.SecondPartyDto.CommercialRegistrationNo))
                            realStateIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.LawyerForIndividual:
                        //var lawyerCitizenList = await _citizenLawyerAgent.GetLawyerCitizens(logdenUser);
                        //if (lawyerCitizenList.Contains(realStateIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    realStateIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (lawyerCitizenList.Contains(realStateIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    realStateIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.AgentForIndividual:
                        //var agentCitizenList = await _citizenLawyerAgent.GetAgentCitizens(logdenUser);
                        //if (agentCitizenList.Contains(realStateIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    realStateIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (agentCitizenList.Contains(realStateIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    realStateIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    default:
                        throw new Exception("");
                        //throw new ExceptionResponse(Localizations.NoPermissionToViewLitigation);
                }
                if (realStateIntegrateDto.FirstPartyDto.IsPretender == true)
                {
                    realStateIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    realStateIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (realStateIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    realStateIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    realStateIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                return new DataResponse<RealStateIntegrateDto> { IsDone = true, Message = Localizations.RetrievedSuccefully, Data = realStateIntegrateDto };
            }
            else
                return new DataResponse<RealStateIntegrateDto> { IsDone = false, Message = Localizations.ContractNotFound, Data = new RealStateIntegrateDto() };
        }
        #endregion

        #region Wafi Integration Simulation
        public async Task<BaseResponse> WafiIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string WafiLicenseNo, WafiRequestOwnerType firstPartType, WafiRequestOwnerType secondPartyType)
        {
            var wafiIntegrateDto = new WafiIntegrateDto
            {
                FirstPartyDto = new PartyDTO
                {
                    Nationality = "Egyption",
                    IdentityNumber = "987wwwccccccc",
                    FullName = "SecondPartyName",
                    NationalAddress = "testAddress",
                    FacilityNumber = null,
                    CommercialRegistrationNo = null,
                    FacilityName = null,
                    Capacity = "مطور عقاري",
                    Mobile = "321654987",
                    Email = "Second@gmail.com",
                    BirthDate = DateTime.Parse("5-5-1980")
                },
                SecondPartyDto = new PartyDTO
                {
                    Nationality = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Egyption",
                    IdentityNumber = "987www",
                    FullName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "SecondPartyName",
                    NationalAddress = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "testAddress",
                    FacilityNumber = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "123987" : null,
                    CommercialRegistrationNo = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "9999999" : null,
                    FacilityName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "FacilityName" : null,
                    Capacity = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "محاسب قانوني",
                    Mobile = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "321654987",
                    Email = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Second@gmail.com",
                    BirthDate = DateTime.Parse("2/2/1888")
                },
                //ThirdPartyDto = new PartyDTO
                //{

                //    Nationality = "Egyption",
                //    IdentityNumber = "987wwwccccccc",
                //    FullName = "SecondPartyName",
                //    NationalAddress = "testAddress",
                //    FacilityNumber = null,
                //    CommercialRegistrationNo = null,
                //    FacilityName = null,
                //    Capacity = "استشاري هتدسي",
                //    Mobile = "321654987",
                //    Email = "Second@gmail.com",
                //    BirthDate = DateTime.Parse("5-5-1980")

                //},
                FirstPartyCapacity = FirstPartyCapacity,
                ContractAddress = "contractAddressTest"
            };
            return await WafiIntegrationValidation(wafiIntegrateDto, FirstPartyCapacity);
        }
        public async Task<BaseResponse> WafiIntegrationValidation(WafiIntegrateDto wafiIntegrateDto, LitigationRequestOwnerType ownerType)
        {
            var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/
            string logdenUserIdentityNumber = "12122nnnnnnnnn";

            switch (ownerType)
            {
                case LitigationRequestOwnerType.Personal:
                    if (logdenUserIdentityNumber == wafiIntegrateDto.FirstPartyDto.IdentityNumber)
                        wafiIntegrateDto.FirstPartyDto.IsPretender = true;
                    else if (logdenUserIdentityNumber == wafiIntegrateDto.SecondPartyDto.IdentityNumber)
                        wafiIntegrateDto.SecondPartyDto.IsPretender = true;
                    else if (logdenUserIdentityNumber == wafiIntegrateDto.ThirdPartyDto.IdentityNumber)
                        wafiIntegrateDto.SecondPartyDto.IsPretender = true;
                    break;
                case LitigationRequestOwnerType.CompanyRepresentative:
                    var representativeCitizenList = await _citizen.GetRepresentativeCitizens(logdenUser);
                    if (representativeCitizenList.Any(x => x.Facility700Number == wafiIntegrateDto.FirstPartyDto.FacilityNumber && x.CommercialRegisterNumber == wafiIntegrateDto.FirstPartyDto.CommercialRegistrationNo))
                        wafiIntegrateDto.FirstPartyDto.IsPretender = true;

                    if (representativeCitizenList.Any(x => x.Facility700Number == wafiIntegrateDto.SecondPartyDto.FacilityNumber && x.CommercialRegisterNumber == wafiIntegrateDto.SecondPartyDto.CommercialRegistrationNo))
                        wafiIntegrateDto.SecondPartyDto.IsPretender = true;
                    break;
                case LitigationRequestOwnerType.LawyerForIndividual:
                    //var lawyerCitizenList = await _citizenLawyerAgent.GetLawyerCitizens(logdenUser);
                    //if (lawyerCitizenList.Contains(wafiIntegrateDto.FirstPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.FirstPartyDto.IsPretender = true;

                    //if (lawyerCitizenList.Contains(wafiIntegrateDto.SecondPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.SecondPartyDto.IsPretender = true;

                    //if (lawyerCitizenList.Contains(wafiIntegrateDto.ThirdPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.SecondPartyDto.IsPretender = true;
                    break;
                case LitigationRequestOwnerType.AgentForIndividual:
                    //var agentCitizenList = await _citizenLawyerAgent.GetAgentCitizens(logdenUser);
                    //if (agentCitizenList.Contains(wafiIntegrateDto.FirstPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.FirstPartyDto.IsPretender = true;

                    //if (agentCitizenList.Contains(wafiIntegrateDto.SecondPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.SecondPartyDto.IsPretender = true;

                    //if (agentCitizenList.Contains(wafiIntegrateDto.ThirdPartyDto.IdentityNumber))
                    //    wafiIntegrateDto.SecondPartyDto.IsPretender = true;
                    break;
                default:
                    throw new Exception("");
                    //throw new ExceptionResponse(Localizations.NoPermissionToViewLitigation);
            }

            if (wafiIntegrateDto.FirstPartyDto.IsPretender == true)
            {
                wafiIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                wafiIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
            }
            if (wafiIntegrateDto.SecondPartyDto.IsPretender == true)
            {
                wafiIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                wafiIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
            }

            return new DataResponse<WafiIntegrateDto> { IsDone = true, Message = Localizations.RetrievedSuccefully, Data = wafiIntegrateDto };
        }
        //public async Task<BaseResponse> WafiCapacties(WafiIntegrateDto wafiIntegrateDto, WafiRequestOwnerType firstPartType, WafiRequestOwnerType secondPartyType)
        //{

        //    return new DataResponse<WafiIntegrateDto> { IsDone = true, Message = Localizations.RetrievedSuccefully, Data = wafiIntegrateDto };

        //}
        #endregion


        public async Task<BaseResponse> PropertiesIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string associationCode, string associationName)
        {
            var PropertiesIntegrateDto = new PropertiesIntegrateDto
            {
                AssociationCode = "1234abcd",
                AssociationName = "7777",
                FirstPartyDto = new PartyDTO
                {
                    FullName = "FirstPartyName",
                    IdentityNumber = "123abcdooooo",
                    NationalAddress = "testAddress",
                    Nationality = "Egyption",
                    FacilityNumber = null,
                    CommercialRegistrationNo = null,
                    FacilityName = null,
                    Capacity = "مستاجر",
                    Mobile = "12345678",
                    Email = "first@gmail.com",
                    BirthDate = new DateTime(1999, 1, 1)

                },
                SecondPartyDto = new PartyDTO
                {
                    FullName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "SecondPArtyName",
                    IdentityNumber = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "987wwwccccccc",
                    NationalAddress = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "testAddress",
                    FacilityNumber = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "123987" : null,
                    CommercialRegistrationNo = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "9999999" : null,
                    Nationality = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Egyption",
                    FacilityName = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? "FacilityName" : null,
                    Capacity = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "مالك",
                    Mobile = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "321654987",
                    Email = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? null : "Second@gmail.com",
                    BirthDate = FirstPartyCapacity == LitigationRequestOwnerType.CompanyRepresentative ? DateTime.MinValue : new DateTime(1999, 2, 2)
                },
                FirstPartyCapacity = FirstPartyCapacity,
                ContractAddress = "ContractAddressTest"
            };
            var contractExist = CheckPropertiesContractExist(associationCode, associationName);
            if (contractExist.Result.Item1 == false)
            {
                #region Insert Contract and Parties
                var contract = new LitigationRequestContract
                {

                    ContractKey = PropertiesIntegrateDto.AssociationCode,
                    FirstPartyCapacity = PropertiesIntegrateDto.FirstPartyCapacity.ToString(),
                    LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                    SecondPartyCapacity = PropertiesIntegrateDto.SecondPartyDto.Capacity,
                    SecondPartyEmail = PropertiesIntegrateDto.SecondPartyDto.Email,
                    ContractFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                    SecondPartyIdentityNumber = PropertiesIntegrateDto.SecondPartyDto.IdentityNumber,
                    SecondPartyMobile = PropertiesIntegrateDto.SecondPartyDto.FullName,
                    VersionNumber = PropertiesIntegrateDto.AssociationName,
                    SecondPartyName = PropertiesIntegrateDto.SecondPartyDto.FullName,

                };
                var checkFirstParty = _citizen.CheckCitizenExist(PropertiesIntegrateDto.FirstPartyDto.IdentityNumber);
                var checkSecondParty = _citizen.CheckCitizenExist(PropertiesIntegrateDto.SecondPartyDto.IdentityNumber);
                if (checkFirstParty.Result.Item2 == true)
                {
                    contract.FirstPartyCitizenId = checkFirstParty.Result.Item1;
                }
                else
                {
                    contract.FirstPartyCitizen = new Citizen
                    {

                        NationalityId = (await _repository.GetQueryable<SysNationality, Guid>()).FirstOrDefault(x => x.NationalityName == PropertiesIntegrateDto.FirstPartyDto.Nationality).Id,
                        NationalAddress = "NationalAddress",
                        Mobile = "01255855852",
                        BirthDate = PropertiesIntegrateDto.FirstPartyDto.BirthDate.Value,
                        BuildingNumber = "1234567891",
                        AreaId = Guid.NewGuid(),
                        CityId = Guid.NewGuid(),
                        District = "District",
                        Email = "firstparty@gmail.com",
                        FullName = PropertiesIntegrateDto.FirstPartyDto.FullName,
                        GenderId = Gender.Male,
                        IdentityExpiryDate = DateTime.Now.AddDays(5),
                        IdentityNumber = PropertiesIntegrateDto.FirstPartyDto.IdentityNumber,
                        IdentityFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                        IdentityTypeId = IdentityType.NationalIdentity,
                        Street = "Street",
                        UnitNumber = "UnitNumber",
                        ShortName = "ShortName",
                    };
                }
                if (checkSecondParty.Result.Item2 == true)
                {
                    contract.SecondPartyCitizenId = checkSecondParty.Result.Item1;
                }
                else
                {
                    contract.SecondPartyCitizen = new Citizen
                    {

                        NationalAddress = "NationalAddress",
                        Mobile = "01025588558",
                        BirthDate = PropertiesIntegrateDto.SecondPartyDto.BirthDate.Value,
                        BuildingNumber = "1234567891",
                        AreaId = Guid.NewGuid(),
                        CityId = Guid.NewGuid(),
                        District = "District",
                        Email = "firstparty@gmail.com",
                        FullName = PropertiesIntegrateDto.SecondPartyDto.FullName,
                        GenderId = Gender.Male,
                        IdentityExpiryDate = DateTime.Now.AddDays(5),
                        IdentityNumber = PropertiesIntegrateDto.SecondPartyDto.IdentityNumber,
                        IdentityFileId = Guid.Parse("61a90c64-69c5-4c85-a95f-ad31333abec7"),
                        IdentityTypeId = IdentityType.NationalIdentity,
                        Street = "Street",
                        UnitNumber = "UnitNumber",
                        ShortName = "ShortName",
                        NationalityId = (await _repository.GetQueryable<SysNationality, Guid>()).FirstOrDefault(x => x.NationalityName == PropertiesIntegrateDto.SecondPartyDto.Nationality).Id,

                    };
                }
                await base.AddAsync(contract, default);
                PropertiesIntegrateDto.FirstPartyDto.CitizenId = contract.FirstPartyCitizen == null ? checkFirstParty.Result.Item1 : contract.FirstPartyCitizen.Id;
                PropertiesIntegrateDto.SecondPartyDto.CitizenId = contract.SecondPartyCitizen == null ? checkSecondParty.Result.Item1 : contract.SecondPartyCitizen.Id;
                PropertiesIntegrateDto.Id = contract.Id;
                #endregion
            }
            else
            {
                PropertiesIntegrateDto.Id = contractExist.Result.Item4;
                PropertiesIntegrateDto.FirstPartyDto.CitizenId = contractExist.Result.Item2;
                PropertiesIntegrateDto.SecondPartyDto.CitizenId = contractExist.Result.Item3;
            }
            return await PropertiesIntegrationValidation(PropertiesIntegrateDto, FirstPartyCapacity, associationCode, associationName);
        }

        public async Task<(bool, Guid, Guid, Guid)> CheckPropertiesContractExist(string associationCode, string associationName)
        {
            var quarable = await GetQueryableAsync();
            bool contractExist = false;
            Guid firstPartyCitizenId = new Guid();
            Guid secondPartyCitizenId = new Guid();
            Guid Id = new Guid();
            var exist = quarable.Where(x => x.ContractKey == associationCode && x.VersionNumber == associationName).FirstOrDefault();
            if (exist != null)
            {
                contractExist = true;
                firstPartyCitizenId = exist.FirstPartyCitizenId.Value;
                secondPartyCitizenId = exist.SecondPartyCitizenId.Value;
                Id = exist.Id;

            }
            return (contractExist, firstPartyCitizenId, secondPartyCitizenId, Id);

        }

        public async Task<BaseResponse> PropertiesIntegrationValidation(PropertiesIntegrateDto PropertiesIntegrateDto, LitigationRequestOwnerType ownerType, string associationCode, string associationName)
        {
            if (PropertiesIntegrateDto.AssociationCode == associationCode && PropertiesIntegrateDto.AssociationName == associationName && ownerType == LitigationRequestOwnerType.LawyerForIndividual)
            {
                var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/
                string logdenUserIdentityNumber = "12122nnnnnnnnn";

                switch (ownerType)
                {
                    case LitigationRequestOwnerType.Personal:
                        if (logdenUserIdentityNumber == PropertiesIntegrateDto.FirstPartyDto.IdentityNumber)
                            PropertiesIntegrateDto.FirstPartyDto.IsPretender = true;
                        else if (logdenUserIdentityNumber == PropertiesIntegrateDto.SecondPartyDto.IdentityNumber)
                            PropertiesIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.CompanyRepresentative:
                        var representativeCitizenList = await _citizen.GetRepresentativeCitizens(logdenUser);
                        if (representativeCitizenList.Any(x => x.Facility700Number == PropertiesIntegrateDto.FirstPartyDto.FacilityNumber && x.CommercialRegisterNumber == PropertiesIntegrateDto.FirstPartyDto.CommercialRegistrationNo))
                            PropertiesIntegrateDto.FirstPartyDto.IsPretender = true;

                        if (representativeCitizenList.Any(x => x.Facility700Number == PropertiesIntegrateDto.SecondPartyDto.FacilityNumber && x.CommercialRegisterNumber == PropertiesIntegrateDto.SecondPartyDto.CommercialRegistrationNo))
                            PropertiesIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.LawyerForIndividual:
                        //var lawyerCitizenList = await _citizenLawyerAgent.GetLawyerCitizens(logdenUser);
                        //if (lawyerCitizenList.Contains(PropertiesIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    PropertiesIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (lawyerCitizenList.Contains(PropertiesIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    PropertiesIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    case LitigationRequestOwnerType.AgentForIndividual:
                        //var agentCitizenList = await _citizenLawyerAgent.GetAgentCitizens(logdenUser);
                        //if (agentCitizenList.Contains(PropertiesIntegrateDto.FirstPartyDto.IdentityNumber))
                        //    PropertiesIntegrateDto.FirstPartyDto.IsPretender = true;

                        //if (agentCitizenList.Contains(PropertiesIntegrateDto.SecondPartyDto.IdentityNumber))
                        //    PropertiesIntegrateDto.SecondPartyDto.IsPretender = true;
                        break;
                    default:
                        throw new Exception("");
                        //throw new ExceptionResponse(Localizations.NoPermissionToViewLitigation);
                }
                if (PropertiesIntegrateDto.FirstPartyDto.IsPretender == true)
                {
                    PropertiesIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    PropertiesIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (PropertiesIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    PropertiesIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "123456789" : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? DateTime.Now.AddDays(5) : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? "789456123" : null;
                    PropertiesIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.LawyerForIndividual ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }


                return new DataResponse<PropertiesIntegrateDto> { IsDone = true, Message = Localizations.RetrievedSuccefully, Data = PropertiesIntegrateDto };
            }
            else
                return new DataResponse<PropertiesIntegrateDto> { IsDone = false, Message = Localizations.ContractNotFound, Data = new PropertiesIntegrateDto() };
        }

        public async Task<int> GetContractCountByContractType(LitigationRequestContractType litigationRequestContractTypeId)
        {
            var quarable = await GetQueryableAsync();
            int count = 0;
            count = quarable.Where(l => l.LitigationRequestContractTypeId == litigationRequestContractTypeId).Count();
            return count;
        }

        public Task<BaseResponse> AddOtherContract(OtherCreateUpdateDto model)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse> RentIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string contractNumber, string versionNumber)
        {
            throw new NotImplementedException();
        }
        public async Task<BaseResponse> AddModificationRequest(LitigationModificationRequestDto model)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var modifiacitonRequest = new LititgationRequestModificationRequest();
                if (model != null)
                {
                    modifiacitonRequest.Id = new Guid();
                    modifiacitonRequest.ModificationRequest = model.ModificationRequest;
                    modifiacitonRequest.LitigationRequestSessionId = model.LitigationRequestSessionId;
                    modifiacitonRequest.LitigationRequestId = model.LitigationRequestId;
                    modifiacitonRequest.CreatedBy = referenceId;
                    modifiacitonRequest.CreatedOn = DateTime.UtcNow;
                }
                var record = await _repository.AddAsync<LititgationRequestModificationRequest, Guid>(modifiacitonRequest);
                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone)
                    return result;

                var data = await GetModificationRequestByLitigationRequestId(modifiacitonRequest.LitigationRequestId);

                if(!data.IsDone)
                    return data;

                return new DataResponse<List<LitigationRequestModificationRequestsDto>>()
                {
                    IsDone = true,
                    Message = "",
                    Data = data.Data

                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false , Message = "خطأ داخلي" };
            }
            

        }
        
        private async Task<DataResponse<List<LitigationRequestModificationRequestsDto>>> GetModificationRequestByLitigationRequestId(Guid litigationRequestId)
        {
            try
            {
                var result = (await _repository.GetQueryable<LititgationRequestModificationRequest, Guid>())
                    .Include(x => x.Citizen)
                    .Where(x => x.LitigationRequestId == litigationRequestId)
                    .OrderByDescending(x => x.CreatedOn)
                    .AsNoTracking()
                    .ToList();
                    
                    
                if (result is null)
                    return new DataResponse<List<LitigationRequestModificationRequestsDto>>() { IsDone = false , Message = "طلب التعديل غير موجود"};

                var data = _mapper.Map<List<LitigationRequestModificationRequestsDto>>(result);

                return new DataResponse<List<LitigationRequestModificationRequestsDto>>()
                {
                    IsDone = true,
                    Message = "",
                    Data = data

                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestModificationRequestsDto>>() { IsDone = false, Message = "خطأ داخلي" };
            }


        }
    }
}
