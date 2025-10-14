using AutoMapper;
using Azure.Core;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Localizations;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.System;
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
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;
using System.Globalization;
using System.Threading;
using static Dapper.SqlMapper;
using static REAC.Application.Contracts.IAppServices.Litigation.ILitigationRequestService;

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
        private readonly INotificationService _notificationService;
        private readonly ISysEmployeeService _employeeService;
        protected ISysSettingService _sysSettingService;
        IBpfService _bpfService;

        public LitigationRequestContractService(REACDBContext context, ISysFileManagerService fileService, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen, INotificationService notificationService, ISysEmployeeService employeeService, ISysSettingService sysSettingService, IBpfService bpfService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _context = context;
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
            _notificationService = notificationService;
            _employeeService = employeeService;
            _sysSettingService = sysSettingService;
            _bpfService = bpfService;
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
                                        .Include(x => x.LitigationRequests)
                                        .ThenInclude(x => x.Delegator)
                                       .FirstOrDefault(x => x.Id == model.ContractId);
                if (contract == null)
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن الوصول لبيانات العقد" };

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
                        contract.ContractFileId = model.ContractFileId;
                        break;
                }

                if(contract.LitigationRequests != null)
                {
                    if(model.LitigationRequestDelegationDataDto is not null && model.LitigationRequestDelegationDataDto.DelegationRelationshipTypeId is not null && contract.LitigationRequests.First().Delegator is not null && contract.LitigationRequests.First().LitigationRequestOwnerTypeId != LitigationRequestOwnerType.Personal)
                    {
                       
                        var updateDelegatorFiles = await _repository.ExecuteUpdateAsync<Delegator,Guid>(
                            x => x.Id == contract.LitigationRequests.First().Delegator.Id
                            , y => y
                            .SetProperty(z => z.DelegateNumber,model.LitigationRequestDelegationDataDto.DelegationNumber)
                            .SetProperty(z => z.DelegationFileId,model.LitigationRequestDelegationDataDto.DelegationFileId)
                            .SetProperty(z => z.DecisionFileId,model.LitigationRequestDelegationDataDto.DecisionFileId)
                            .SetProperty(z => z.EstablishmentFileId,model.LitigationRequestDelegationDataDto.EstablishmentFileId)
                            .SetProperty(z => z.EstablishmentLetterFileId,model.LitigationRequestDelegationDataDto.EstablishmentLetterFileId)
                            .SetProperty(z => z.CustodyFileId,model.LitigationRequestDelegationDataDto.CustodyFileId)
                            .SetProperty(z => z.CommercialRegisterationFileId,model.LitigationRequestDelegationDataDto.CommercialRegisterationFileId)
                            .SetProperty(z => z.RepresentationFileId,model.LitigationRequestDelegationDataDto.RepresentationFileId)
                            ,default);

                        if(!updateDelegatorFiles.IsDone)
                            return new BaseResponse() { IsDone = false , Message = "حدث خطأ اثناء تعديل ملفات التفويض" };


                        if(model.LitigationRequestDelegationDataDto != null && model.LitigationRequestDelegationDataDto.DelegationRelationshipTypeId == DelegationRelationshipType.Individual && contract.LitigationRequests.First().Delegator.CitizenId is not null)
                        {
                            contract.FirstPartyCapacity = model.LitigationRequestDelegationDataDto.IndividualCapacity;
                            var updatedCitizen = await UpdateDraftCitizen(model.LitigationRequestDelegationDataDto, contract.LitigationRequests.First().Delegator.CitizenId);

                            if(!updatedCitizen.IsDone)
                                return updatedCitizen;

                        }else if((model.LitigationRequestDelegationDataDto != null && model.LitigationRequestDelegationDataDto.DelegationRelationshipTypeId == DelegationRelationshipType.Facility && contract.LitigationRequests.First().Delegator.FacilityId is not null)){

                            var updatedFacility = await UpdateFacility(model.LitigationRequestDelegationDataDto, contract.LitigationRequests.First().Delegator.FacilityId);

                            if(! updatedFacility.IsDone)
                                return updatedFacility;
                        }
                    }
                }


                var response =  _repository.SaveChangesAsync().Result;
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

                var result = await base.UpdateAsync(contractUpdateDto);

                if (result.IsDone)
                {
                    var data = (await _repository.GetQueryable<LitigationRequestContract, Guid>())
                                       .Include(x => x.ContractFile)
                                       .AsNoTracking()
                                       .FirstOrDefault(x => x.Id == model.ContractId);

                    return new DataResponse<LitigationRequestContract>()
                    {
                        IsDone = response.IsDone,
                        Data = data,
                        Message = response.Message
                    };
                }

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

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

        private async Task<BaseResponse> UpdateDraftCitizen(LitigationRequestDelegationDataDto input, Guid? citizenId)
        {
            // Get Nationality
            var nationalityId = (await _repository.GetQueryable<SysNationality, Guid>())
               .AsNoTracking()
               .FirstOrDefault(x => x.NationalityName == input.IndividualNationality)?
               .Id;

            // update Draft Citizen
            var updateDraftCitizen = await _repository.ExecuteUpdateAsync<Citizen, Guid>(
                             x => x.Id == citizenId
                             , y => y
                             .SetProperty(z => z.FullName, input.IndividualFullName)
                             .SetProperty(z => z.BirthDate, DateTime.Parse(input.IndividualBirthdate, CultureInfo.InvariantCulture))
                             .SetProperty(z => z.Mobile, input.IndividualMobile)
                             .SetProperty(z => z.Email, input.IndividualEmail)
                             .SetProperty(z => z.NationalAddress, input.IndividualNationalAddress)
                             .SetProperty(z => z.NationalityId, nationalityId)
                             .SetProperty(z => z.IdentityNumber, input.IndividualIdentityNumber)
                             
                             , default);

            if (!updateDraftCitizen.IsDone)
                return new DataResponse<Citizen>() { IsDone = false, Message = "خطأ في تعديل بيانات المدعي" };

            return new DataResponse<Citizen>() { IsDone = true, Message = "تم تعديل بيانات المدعي بنجاح" };
        }

        private async Task<BaseResponse> UpdateFacility(LitigationRequestDelegationDataDto input,Guid? facilityId)
        {
            if (input == null)
                return new DataResponse<Facility> { IsDone = false, Message = "خطأ في إضافة بيانات المنشأة" };

            // update Facility
            var updateFacility = await _repository.ExecuteUpdateAsync<Facility, Guid>(
                             x => x.Id == facilityId
                             , y => y
                             .SetProperty(z => z.ArabicFacilityName, input.ArabicFacilityName)
                             .SetProperty(z => z.Facility700Number,input.Facility700Number)
                             .SetProperty(z => z.CommercialRegisterNumber, input.FacilityCommercialRegisterationNumber)
                             
                             , default);
            if (!updateFacility.IsDone)
                return new DataResponse<Facility> { IsDone = false, Message = "خطأ في تعديل بيانات المنشأة" };

            return new DataResponse<Facility> { IsDone = true, Message = "تم تعديل بيانات المنشأة بنجاح",  };
        }
        #endregion

        public async Task<BaseResponse> UpdateContractDocuments(CreateUpdateContractDocuments model)
        {
            try
            {

                var result = await _fileService.UploadAsync(model.Files);
                if (!result.IsDone)
                {
                    return new BaseResponse() { IsDone = false, Message = Localizations.FailedUploadFile };
                }
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                       .Include(s => s.Contract)
                                       .Include(x => x.LitigationRequestGrounds)
                                       .ThenInclude(x => x.LitigationRequestGroundFiles)
                                       .FirstOrDefault(x => x.Id == model.LitigationRequestId);

                var contract = litigationRequest?.Contract;
                if (contract == null)
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن الوصول لبيانات العقد" };

                var resultDto = result.Data?.FirstOrDefault();

         
                switch (contract.LitigationRequestContractTypeId)
                {
                    case LitigationRequestContractType.Rent:
                        contract.ContractFileId = resultDto?.Id;
                        break;
                    case LitigationRequestContractType.Wafi:
                        contract.ContractFileId = resultDto?.Id;
                        break;
                    case LitigationRequestContractType.Properties:
                        switch (model.PropertiesDocumnetType)
                        {
                            case PropertiesDocumnetType.AssociationRegistrationCertificateFile:
                                contract.AssociationRegistrationCertificateFileId = resultDto?.Id;
                                break;
                            case PropertiesDocumnetType.FoundingLetterFile:
                                contract.FoundingLetterFileId = resultDto?.Id;
                                break;
                            case PropertiesDocumnetType.InternalRegulationsFile:
                                contract.InternalRegulationsFileId = resultDto?.Id;
                                break;

                        }
                        break;
                    case LitigationRequestContractType.RealEstateBrokerage:
                        contract.ContractFileId = resultDto?.Id;
                        break;
                    case LitigationRequestContractType.OtherServices:
                        contract.ContractFileId = resultDto?.Id;
                        break;
                }
                if (model.SupportsFiles != null)
                {
                    var SupportsResult = await _fileService.UploadAsync(model.SupportsFiles);
                    if (!SupportsResult.IsDone)
                    {
                        return new BaseResponse() { IsDone = false, Message = Localizations.FailedUploadFile };
                    }
                    var SupportsResultDto = result.Data?.FirstOrDefault();
                    if (litigationRequest.LitigationRequestGrounds.Count() == 0)
                    {
                        LitigationRequestGround litigationRequestGround = new LitigationRequestGround();
                        litigationRequestGround.Id = Guid.NewGuid();
                        litigationRequestGround.LitigationRequestId = litigationRequest.Id;
                        litigationRequestGround.Title = "اسانيد الدعوه تمت اضافتها من خلال التعديل ";

                        foreach (var file in model.SupportsFiles)
                        {
                            litigationRequestGround.LitigationRequestGroundFiles.Add(new LitigationRequestGroundFile()
                            {
                                LitigationRequestGroundId = litigationRequestGround.Id,
                                Id = Guid.NewGuid(),
                                FileManagerId = SupportsResultDto.Id
                            });
                        }

                        litigationRequest.LitigationRequestGrounds.Add(litigationRequestGround);
                    }
                    else
                    {
                        foreach (var file in model.SupportsFiles)
                        {
                            litigationRequest.LitigationRequestGrounds.FirstOrDefault(l => l.LitigationRequestId == litigationRequest.Id).LitigationRequestGroundFiles.Add(new LitigationRequestGroundFile()
                            {
                                LitigationRequestGroundId = litigationRequest.LitigationRequestGrounds.FirstOrDefault().Id,
                                Id = Guid.NewGuid(),
                                FileManagerId = SupportsResultDto.Id
                            });
                        }
                    }

                }

                var response = _repository.SaveChangesAsync().Result;
                if (!response.IsDone)
                    return response;

                return new DataResponse<string>()
                {
                    IsDone = response.IsDone,
                    Data = "تم تعديل الملف بنجاح",
                    Message = response.Message
                };


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
            if (rentIntegrateDto.Key == contractNumber && rentIntegrateDto.SubKey == versionNumber && ownerType == LitigationRequestOwnerType.IndividualRepresentative)
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
                    case LitigationRequestOwnerType.IndividualRepresentative:
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
                    rentIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    rentIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (rentIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    rentIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    rentIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    rentIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
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
            if (realStateIntegrateDto.ContractKey == contractNumber && ownerType == LitigationRequestOwnerType.IndividualRepresentative)
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
                    case LitigationRequestOwnerType.IndividualRepresentative:
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
                    realStateIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    realStateIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (realStateIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    realStateIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    realStateIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    realStateIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
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
                case LitigationRequestOwnerType.IndividualRepresentative:
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
                wafiIntegrateDto.FirstPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                wafiIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
            }
            if (wafiIntegrateDto.SecondPartyDto.IsPretender == true)
            {
                wafiIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                wafiIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                wafiIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
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
            if (PropertiesIntegrateDto.AssociationCode == associationCode && PropertiesIntegrateDto.AssociationName == associationName && ownerType == LitigationRequestOwnerType.IndividualRepresentative)
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
                    case LitigationRequestOwnerType.IndividualRepresentative:
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
                    PropertiesIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    PropertiesIntegrateDto.FirstPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.FirstPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.FirstPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (PropertiesIntegrateDto.SecondPartyDto.IsPretender == true)
                {
                    PropertiesIntegrateDto.SecondPartyDto.AgencyNumber = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "123456789" : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseExpirationDate = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? DateTime.Now.AddDays(5) : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseNumber = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? "789456123" : null;
                    PropertiesIntegrateDto.SecondPartyDto.EstablishmentContract = ownerType == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.SecondPartyDto.AgancyAttachment = ownerType == LitigationRequestOwnerType.AgentForIndividual || ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    PropertiesIntegrateDto.SecondPartyDto.LawLicenseAttachment = ownerType == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
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

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.WorkFlowProcessingLogs)
                    .ThenInclude(x => x.WorkFlowStep)
                    .Include(x => x.WorkFlowProcessingLogs)
                    .ThenInclude(x => x.Employee)
                    .Include(x => x.Contract)
                    .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.WorkFlowProcessingLogs.OrderByDescending(y => y.CreatedOn).Where(z => z.EmployeeId != null))
                    .ThenInclude(x => x.Employee)
                    .FirstOrDefault(x => x.Id == model.LitigationRequestId);



                if (model.ActorType is not null && model.ActorType == NotificationActorType.Applicant
                    && litigationRequest?.InternalStatusId != RequestStatus.New
                    && litigationRequest?.InternalStatusId != RequestStatus.Edited
                    && litigationRequest?.InternalStatusId != RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent
                    && litigationRequest?.InternalStatusId != RequestStatus.LitigationFeesHaveBeenCollected
                    && litigationRequest?.InternalStatusId != RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent
                    && litigationRequest?.InternalStatusId != RequestStatus.ReconciliationFeesHaveBeenCollected
                    && litigationRequest?.InternalStatusId != RequestStatus.AgreementRequestHasBeenSent
                    )
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن تقديم طلب تعديل علي هذه الدعوي في هذه المرحلة" };


                var modifiacitonRequest = new LititgationRequestModificationRequest();
                if (model != null)
                {
                    modifiacitonRequest.Id = new Guid();
                    modifiacitonRequest.ModificationRequest = model.ModificationRequest;
                    modifiacitonRequest.LitigationRequestSessionId = model.LitigationRequestSessionId;
                    modifiacitonRequest.LitigationRequestId = model.LitigationRequestId;
                    modifiacitonRequest.CreatedBy = referenceId;
                    modifiacitonRequest.CreatedOn = DateTime.UtcNow;
                    modifiacitonRequest.IsApproved = null;
                    modifiacitonRequest.ActorType = model.ActorType;
                }

                var record = await _repository.AddAsync<LititgationRequestModificationRequest, Guid>(modifiacitonRequest);
                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone)
                    return result;

                // Send Notifications
                try
                {
                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_ModificationRequest).Result.Data?.Value;
                    msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_ModificationRequest).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }

                    await SendNotificationMS(body, msg, litigationRequest, "طلب تعديل الدعوى", NotificationActorType.Applicant, RequestType.LitigationRequest,
                   true, false, true, true, false, true, false, true);
                }
                catch (Exception e)
                {
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                    , new LogDto()
                    {
                        Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                        Message = e.ToString()
                    });


                }


                var data = await GetModificationRequestByLitigationRequestId(modifiacitonRequest.LitigationRequestId);

                if (!data.IsDone)
                    return data;

                return new DataResponse<List<LitigationRequestModificationRequestsDto>>()
                {
                    IsDone = true,
                    Message = "تم اضافة طلب تعديل علي الدعوي",
                    Data = data.Data

                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
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
                    return new DataResponse<List<LitigationRequestModificationRequestsDto>>() { IsDone = false, Message = "طلب التعديل غير موجود" };

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

        public async Task<DataResponse<PagedResult<LitigationRequestModificationRequestsDto>>> GetModificationRequestForEmployee(int pageIndex, int pageSize)
        {
            try
            {
                Guid employeeId = Guid.Empty;
                _jwtReader.GetReferenceId(ref employeeId);

                var result = (await _repository.GetQueryable<LititgationRequestModificationRequest, Guid>())
                    .Include(x => x.LitigationRequest)
                    .ThenInclude(x => x.WorkFlowProcessingLogs)
                    .Where(x => x.LitigationRequest.WorkFlowProcessingLogs.Where(t => t.EmployeeId == employeeId).Count() > 0)
                    .Include(x => x.Citizen)
                    .OrderByDescending(x => x.CreatedOn)
                    .AsNoTracking()
                    .GetPaged(pageIndex, pageSize);



                var list = _mapper.Map<List<LitigationRequestModificationRequestsDto>>(result.Results);


                return new DataResponse<PagedResult<LitigationRequestModificationRequestsDto>>
                {
                    Data =
                  new PagedResult<LitigationRequestModificationRequestsDto>
                  {
                      Results = list,
                      CurrentPage = result.CurrentPage,
                      PageCount = result.PageCount,
                      PageSize = result.PageSize,
                      TotalCount = result.TotalCount
                  },
                    IsDone = true
                };

            }
            catch (Exception ex)
            {
                return new DataResponse<PagedResult<LitigationRequestModificationRequestsDto>> { IsDone = false, Message = "خطأ داخلي" };
            }


        }

        public async Task<BaseResponse> AddModificationRequestAction(AddModificationRequestActionDto addModificationRequestActionDto)
        {
            try
            {

                var result = (await _repository.GetQueryable<LititgationRequestModificationRequest, Guid>())
                    .FirstOrDefault(x => x.Id == addModificationRequestActionDto.ModificationRequestId);


                if (result is null)
                    return new DataResponse<List<LitigationRequestModificationRequestsDto>>() { IsDone = false, Message = "طلب التعديل غير موجود" };

                if (addModificationRequestActionDto.IsApproved)
                {
                    result.IsApproved = true;
                    await _repository.SaveChangesAsync();
                    return new BaseResponse { IsDone = true, Message = "تم قبول الطلب" };


                    //var workflowStepEvents = await _bpfService.GetWorkflowStepEventsForEmployee(result.LitigationRequestId, default);

                    //if (!workflowStepEvents.IsDone)
                    //    return new BaseResponse { IsDone = true, Message = "لا يمكن قبول طلب التعديل علي هذه الدعوي" };

                    //var workflowStepEventsObject = workflowStepEvents as DataResponse<WorkFlowStepEventForActorShortDto>;

                    //if (workflowStepEventsObject?.Data is not null && workflowStepEventsObject.Data.WorkFlowStepEventShortDtos.Any(x => x.EventName.Replace(" ","") == "إعادةالدعوىللتعديل"))
                    //{
                    //    var changeLitigationToSentBackForEdit = await _bpfService.ProcessWorkFlow(new Contracts.Dtos.Wpf.Business.ProcessRequestPayload()
                    //    {
                    //        RequestId = result.LitigationRequestId,
                    //        WorkflowStepEventId = workflowStepEventsObject.Data.WorkFlowStepEventShortDtos.First(x => x.EventName.Replace(" ", "") == "إعادةالدعوىللتعديل").Id,
                    //        ProcessingLogId = workflowStepEventsObject.Data.WorkFlowStepEventShortDtos.First(x => x.EventName.Replace(" ","") == "إعادةالدعوىللتعديل").ProcessingLogId,
                    //        Notes = "تعديل الدعوي بناء علي طلب مقدم الطلب",
                    //        UploadedFilesIds = new List<Guid>()

                    //    }, CancellationToken.None);

                    //    if (!changeLitigationToSentBackForEdit.IsDone)
                    //        return new BaseResponse { IsDone = true, Message = "لا يمكن قبول طلب التعديل علي هذه الدعوي" };

                    //    return new BaseResponse { IsDone = true, Message = "تم قبول الطلب" };

                    //}
                    //else
                    //{
                    //    return new BaseResponse { IsDone = true, Message = "لا يمكن قبول طلب التعديل علي هذه الدعوي" };

                    //}


                }
                else
                {
                    result.IsApproved = false;
                    await _repository.SaveChangesAsync();


                    return new BaseResponse { IsDone = true, Message = "تم رفض الطلب" };
                }



            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = "خطأ داخلي" };
            }


        }

        private async Task SendNotificationMS(string mailMsg, string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, bool notifyApplicant = false, bool notifyReformer = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false, bool notifyFirstPartyFacility = false, bool notifySecondPartyFacility = false, bool notifyEmployee = false)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>()
            };

            var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>(),
                notification_type = "SMS",
            };

            var lstNotification = new List<SysNotificationDto>();

            // Notify Applicant Citizen
            if (notifyApplicant && litigationRequest.ApplicantCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ApplicantCitizen.Email);
                objSms.recipients.Add(litigationRequest.ApplicantCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ApplicantCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Employee
            if (notifyEmployee && litigationRequest.WorkFlowProcessingLogs?.FirstOrDefault()?.Employee != null)
            {
                objEmail.To.Add(litigationRequest.WorkFlowProcessingLogs?.FirstOrDefault()?.Employee.Email);
                objSms.recipients.Add(litigationRequest.WorkFlowProcessingLogs?.FirstOrDefault()?.Employee.Mobile);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.WorkFlowProcessingLogs?.FirstOrDefault()?.EmployeeId,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Reformer Citizen
            if (notifyReformer && litigationRequest.ReformerCitizenId.HasValue)
            {
                objEmail.To.Add(litigationRequest.ReformerCitizen.Email);
                objSms.recipients.Add(litigationRequest.ReformerCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ReformerCitizenId.Value,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Reformer,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
            }

            // Notify Department Admin
            if (notifyDeptAdmin && deptAdminInfoReview != null)
            {
                objEmail.To.Add(deptAdminInfoReview.Data.Email);
                objSms.recipients.Add(deptAdminInfoReview.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Citizen
            if (notifyFirstParty && litigationRequest.Contract?.FirstPartyCitizen != null && litigationRequest.Contract.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Second Party Citizen
            if (notifySecondParty && litigationRequest.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Facility
            //if (notifyFirstPartyFacility && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.FirstPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}

            //// Notify Second Party Facility
            //if (notifySecondPartyFacility && litigationRequest.Contract?.SecondPartyFacility != null)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.SecondPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }
    }
}
