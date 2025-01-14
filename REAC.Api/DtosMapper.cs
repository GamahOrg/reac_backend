using AutoMapper;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Integration.Rent;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Defendant;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Contract;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Get;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.Dtos.System.CityDto;
using REAC.Application.Contracts.Dtos.System.DepartmentDtos;
using REAC.Application.Contracts.Dtos.System.EmployeeDtos;
using REAC.Application.Contracts.Dtos.System.GenderDtos;
using REAC.Application.Contracts.Dtos.System.IdentityTypeDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestAmountDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCorrespondenceDestinationDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestExtendPeriodRequestStatusDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;
using REAC.Application.Contracts.Dtos.System.VacationDtos;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLog;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;
using SysVacationRequestCreateUpdateDto = REAC.Application.Contracts.Dtos.System.VacationDtos.SysVacationRequestCreateUpdateDto;

namespace REAC.Application.Contracts
{
    public class DtosMapper : Profile
    {
        public DtosMapper()
        {
            CreateMap<SysArea, SysAreaDto>()
                                       .ForMember(des => des.AreaName,
                                       opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                       string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AreaName)) ?
                                       src.AreaName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AreaName)).ReverseMap();

            CreateMap<SysDelegationRelationshipType, SysDelegationRelationshipTypeDto>()
                                      .ForMember(des => des.DelegationRelationshipTypeName,
                                      opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                      string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).DelegationRelationshipTypeName)) ?
                                      src.DelegationRelationshipTypeName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).DelegationRelationshipTypeName)).ReverseMap();

            CreateMap<SysCity, SysCityDto>()
                                       .ForMember(des => des.CityName,
                                       opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                       string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).CityName)) ?
                                       src.CityName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).CityName)).ReverseMap();

            CreateMap<LitigationRequest, LitigationCreateUpdateDto>();
            ; CreateMap<SysGender, SysGenderDto>()
                                                   .ForMember(des => des.Gender,
                                                   opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                                   string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Gender)) ?
                                                   src.Gender : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Gender)).ReverseMap();

            CreateMap<SysEmployee, SysEmployeeDto>()
                                       .ForMember(des => des.EmployeeName,
                                       opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                       string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).EmployeeName)) ?
                                       src.EmployeeName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).EmployeeName)).ReverseMap();

            CreateMap<SysIdentityType, SysIdentityTypeDto>()
                                       .ForMember(des => des.IdentityType,
                                       opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                       string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).IdentityType)) ?
                                       src.IdentityType : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).IdentityType)).ReverseMap();


            CreateMap<SysLitigationRequestCategory, SysLitigationRequestCategoryDto>()
                    .ForMember(des => des.CategoryName,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).CategoryName)) ? src.CategoryName : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).CategoryName))



             .ForMember(des => des.TermsAndConditions,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TermsAndConditions)) ? src.TermsAndConditions : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TermsAndConditions))
             .ForMember(des => des.AcknowledgmentContent,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AcknowledgmentContent)) ? src.AcknowledgmentContent : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AcknowledgmentContent)).ReverseMap();


            CreateMap<SysLitigationRequestContractType, SysLitigationRequestContractTypeDto>()
                    .ForMember(des => des.TypeName,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TypeName)) ? src.TypeName : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TypeName))



             .ForMember(des => des.TermsAndConditions,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TermsAndConditions)) ? src.TermsAndConditions : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).TermsAndConditions))
             .ForMember(des => des.AcknowledgmentContent,
                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AcknowledgmentContent)) ? src.AcknowledgmentContent : src.Translations.FirstOrDefault(
                     x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).AcknowledgmentContent)).ReverseMap();


            CreateMap<SysLitigationRequestCorrespondenceDestination, SysLitigationRequestCorrespondenceDestinationDto>()
                                      .ForMember(des => des.Destination,
                                      opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                      string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Destination)) ?
                                      src.Destination : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).Destination)).ReverseMap();

            CreateMap<SysLitigationRequestExtendPeriodRequestStatus, SysLitigationRequestExtendPeriodRequestStatusDto>()
                                     .ForMember(des => des.StatusName,
                                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                     string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).StatusName)) ?
                                     src.StatusName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).StatusName)).ReverseMap();


            CreateMap<SysLitigationRequestOwnerType, SysLitigationRequestOwnerTypeDto>()
                                     .ForMember(des => des.LitigationRequestOwnerType,
                                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                     string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).LitigationRequestOwnerType)) ?
                                     src.LitigationRequestOwnerType : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).LitigationRequestOwnerType)).ReverseMap();

            CreateMap<SysLitigationRequestStatus, SysLitigationRequestStatusDto>()
                                     .ForMember(des => des.RequestStatus,
                                     opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                                     string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).RequestStatus)) ?
                                     src.RequestStatus : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).RequestStatus)).ReverseMap();



            CreateMap<SysDepartment, SysDepartmentDto>()
                           .ForMember(des => des.DepartmentName,
                           opt => opt.MapFrom(src => (src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null ||
                           string.IsNullOrEmpty(src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).DepartmentName)) ?
                           src.DepartmentName : src.Translations.FirstOrDefault(x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).DepartmentName)).ReverseMap();




            CreateMap<Delegator, DelegatorDto>().ReverseMap();
            CreateMap<Delegator, LawyerDelegatorDto>().ReverseMap();
            CreateMap<Citizen, CitizenCreateUpdateDto>().ReverseMap();
            CreateMap<DelegationRequest, DelegationRequestCreateUpdateDto>().ReverseMap();
            CreateMap<Facility, FacilityCreateUpdateDto>().ReverseMap();
            CreateMap<Delegator, FacilityDelegatorCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequest, LitigationRequestCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestContract, LitigationRequestContractCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestContract, LitigationRequestContractShortDto>().ReverseMap();
            CreateMap<LitigationRequestCorrespondence, LitigationRequestCorrespondenceCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestCorrespondenceFile, LitigationRequestCorrespondenceFileCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestExtendPeriodRequest, LitigationRequestExtendPeriodRequestGetDto>().ReverseMap();


            CreateMap<LitigationRequestExtendPeriodRequest, LitigationRequestExtendPeriodRequestCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestExtendPeriodRequestFile, LitigationRequestExtendPeriodRequestFileCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestGround, LitigationRequestGroundCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestGroundFile, LitigationRequestGroundFileCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestReconciliationReportItem, LitigationRequestReconciliationReportItemCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestNote, LitigationRequestNoteCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestCorrespondence, LitigationRequestCorrespondenceDto>().ReverseMap();
            CreateMap<LitigationRequestCorrespondenceReply, LitigationRequestCorrespondenceReplyDto>().ReverseMap();
            CreateMap<LitigationRequestCorrespondenceFile, SysFileManagerCreateUpdateDto>()
                .ForMember(des => des.FilePath, opt => opt.MapFrom(src => src.FileManager.FilePath))
                .ForMember(des => des.FileType, opt => opt.MapFrom(src => src.FileManager.FileType))
                .ForMember(des => des.FileDescription, opt => opt.MapFrom(src => src.FileManager.FileDescription))
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.FileManager.Id))
                .ReverseMap();
            
            CreateMap<LitigationRequestCorrespondenceReplyFile, SysFileManagerCreateUpdateDto>()
                .ForMember(des => des.FilePath, opt => opt.MapFrom(src => src.FileManager.FilePath))
                .ForMember(des => des.FileType, opt => opt.MapFrom(src => src.FileManager.FileType))
                .ForMember(des => des.FileDescription, opt => opt.MapFrom(src => src.FileManager.FileDescription))
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.FileManager.Id))
                .ReverseMap();

            CreateMap<LitigationRequestNote, LitigationRequestNotesDto>().ReverseMap();
            CreateMap<LitigationRequestNoteFile, SysFileManagerCreateUpdateDto>()
                .ForMember(des => des.FilePath, opt => opt.MapFrom(src => src.FileManager.FilePath))
                .ForMember(des => des.FileType, opt => opt.MapFrom(src => src.FileManager.FileType))
                .ForMember(des => des.FileDescription, opt => opt.MapFrom(src => src.FileManager.FileDescription))
                .ForMember(des => des.Id, opt => opt.MapFrom(src => src.FileManager.Id))
                .ReverseMap();
            CreateMap<LitigationRequestNoteFile, LitigationRequestNoteFileCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestReconciliationReport, LitigationRequestReconciliationReportCreateUpdateDto>().ReverseMap();
            CreateMap<Dtos.Litigation.CreateUpdate.Litigation.LitigationRequestRequest, LitigationRequestRequestCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestRequestFile, LitigationRequestRequestFileCreateUpdateDto>().ReverseMap();
            CreateMap<SysFileManager, SysFileManagerCreateUpdateDto>().ReverseMap();
            CreateMap<SysLitigationRequestAmount, SysLitigationRequestAmountCreateUpdateDto>().ReverseMap();
            CreateMap<SysSetting, SysSettingCreateUpdateDto>().ReverseMap();
            CreateMap<SysVacationRequest, SysVacationRequestCreateUpdateDto>()
                .ForMember(des => des.ApprovedBy, opt => opt.MapFrom(src => src.SysApprovedByEmployee.EmployeeName))
                .ForMember(des => des.EmployeeName, opt => opt.MapFrom(src => src.SysEmployee.EmployeeName))
                .ReverseMap();
            CreateMap<SysRegistrationRequest, SysRegistrationRequestCreateUpdateDto>().ReverseMap();

            #region Litigation Contract




            CreateMap<OtherCreateUpdateDto, LitigationRequestContract>()
            .ForMember(des => des.LitigationRequestContractTypeId, opt => opt.MapFrom(src => 78 /*LitigationRequestContractType.Other*/))
             .ForMember(des => des.SecondPartyCapacity, opt => opt.MapFrom(src => ""));


            #endregion

            #region Litigation Defendant
            CreateMap<DefendantCreateUpdateDto, LitigationRequestContract>();
            CreateMap<RequestStatus, SysLitigationRequestStatus>();
            #endregion

            #region Litigation 
            CreateMap<LitigationCreateUpdateDto, LitigationRequest>()
                 .ForMember(des => des.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                 .ForMember(des => des.ApplicantCitizenId, opt => opt.MapFrom(src => "1c1b6018-69c8-43f5-b5b1-72cea64d0f9f"))
                 .ForMember(des => des.IsPaid, opt => opt.MapFrom(src => false));
            CreateMap<LitigationRequestCategory, SysLitigationRequestCategory>();
            CreateMap<int, SysLitigationRequestStatus>();
            //CreateMap<LitigationListDto, LitigationRequest>();
            CreateMap<LitigationRequest, LitigationListDto>()
                //.ForMember(dest => dest.InternalStatusId, opt => opt.MapFrom(src => src.InternalStatusId))
                .ForMember(dest => dest.InternalStatusId, opt => opt.MapFrom(src => src.InternalStatusId))
                .ForMember(dest => dest.ExternalStatusId, opt => opt.MapFrom(src => src.ExternalStatusId))

                 .ForMember(dest => dest.RequestCode, opt => opt.MapFrom(src => src.RequestCode))

                 .ForMember(des => des.FirstPartyName,
                           opt => opt.MapFrom(src => src.Contract.FirstPartyCitizenId.HasValue ?
                           src.Contract.FirstPartyCitizen.FullName : src.Contract.FirstPartyFacility.ArabicFacilityName))


                 .ForMember(des => des.SecondPartyName,
                           opt => opt.MapFrom(src => src.Contract.SecondPartyCitizenId.HasValue ?
                           src.Contract.SecondPartyCitizen.FullName : src.Contract.SecondPartyFacility.ArabicFacilityName))


                   .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                    .ForMember(dest => dest.LitigationRequestContractTypeId, opt => opt.MapFrom(src => src.Contract.LitigationRequestContractTypeId))
                    .ForMember(dest => dest.LitigationRequestContractType, opt => opt.MapFrom(src => src.Contract.LitigationRequestContractType.TypeName));

            CreateMap<LitigationRequest, AuditingListDto>()
     .ForMember(dest => dest.InternalStatusId, opt => opt.MapFrom(src => src.InternalStatusId))
     .ForMember(dest => dest.RequestCode, opt => opt.MapFrom(src => src.RequestCode))
          .ForMember(dest => dest.InternalStatus, opt => opt.MapFrom(src => src.InternalStatus.RequestStatus))

 ///////////
 .ForMember(des => des.SecondPartyName,
                           opt => opt.MapFrom(src => src.Contract.SecondPartyCitizenId.HasValue ?
                           src.Contract.SecondPartyCitizen.FullName : src.Contract.SecondPartyFacility.ArabicFacilityName))
      .ForMember(dest => dest.FirstPartyName, opt => opt.MapFrom(src => src.Contract.FirstPartyCitizen.FullName))
       .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
        .ForMember(dest => dest.LitigationRequestContractTypeId, opt => opt.MapFrom(src => src.Contract.LitigationRequestContractTypeId))
             .ForMember(dest => dest.LitigationRequestContractType, opt => opt.MapFrom(src => src.Contract.LitigationRequestContractType.TypeName));
            CreateMap<LitigationRequest, LitigationRatingDto>()
.ForMember(dest => dest.PretenderRating, opt => opt.MapFrom(src => src.PretenderRating))
.ForMember(dest => dest.DefendantRating, opt => opt.MapFrom(src => src.DefendantRating))
.ForMember(dest => dest.LitigationId, opt => opt.MapFrom(src => src.Id));

            CreateMap<LitigationRequest, LitigationPartiesDetailsDto>()
                 .ForMember(dest => dest.SubKey, opt => opt.MapFrom(src => src.Contract.VersionNumber))
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Contract.ContractKey))
                .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
     .ForMember(dest => dest.FirstPartyDto, opt => opt.MapFrom(src => new PartyDTO
     {
         Capacity = src.Contract.FirstPartyCapacity,
         IdentityNumber = src.Contract.FirstPartyCitizen.IdentityNumber,
         NationalAddress = src.Contract.FirstPartyCitizen.NationalAddress,
         Nationality = src.Contract.FirstPartyCitizen.Nationality.NationalityName,
         FullName = src.Contract.FirstPartyCitizen.FullName,
         Mobile = src.Contract.FirstPartyCitizen.Mobile,
         Email = src.Contract.FirstPartyCitizen.Email,
         BirthDate = src.Contract.FirstPartyCitizen.BirthDate
     }))
                .ForMember(dest => dest.SecondPartyDto, opt => opt.MapFrom(src => new PartyDTO
                {
                    Capacity = src.Contract.SecondPartyCapacity,
                    IdentityNumber = src.Contract.SecondPartyCitizen.IdentityNumber,
                    NationalAddress = src.Contract.SecondPartyCitizen.NationalAddress,
                    Nationality = src.Contract.SecondPartyCitizen.Nationality.NationalityName,
                    FullName = src.Contract.SecondPartyCitizen.FullName,
                    Mobile = src.Contract.SecondPartyCitizen.Mobile,
                    Email = src.Contract.SecondPartyCitizen.Email,
                    BirthDate = src.Contract.SecondPartyCitizen.BirthDate
                }));
            CreateMap<SysFileManager, PartyDTO>();
            CreateMap<SysFileManager, RentSecondPartyDto>();
            CreateMap<LititgationRequestModificationRequest, LitigationModificationRequestDto>();
            CreateMap<LitigationRequest, LitigationDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RequestCode, opt => opt.MapFrom(src => src.RequestCode))
                .ForMember(dest => dest.Fees, opt => opt.MapFrom(src => src.Fees))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId))
                .ForMember(dest => dest.CityId, opt => opt.MapFrom(src => src.CityId))
                .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(src => src.ContractAddress))
                //.ForMember(dest => dest.Requests, opt => opt.MapFrom(src => src.LitigationRequestRequests))
                //.ForMember(dest => dest.References, opt => opt.MapFrom(src => src.LitigationRequestGrounds))
                //.ForMember(dest => dest.References, opt => opt.MapFrom(src => src.LitigationRequestGrounds))

                .ForMember(dest => dest.LitigationRequestContractTypeId, opt => opt.MapFrom(src => src.Contract.LitigationRequestContractTypeId));


            #endregion

            CreateMap<FileParameterDto, SysFileManager>();
            CreateMap<LitigationRequestRequestCreateUpdateDto, Dtos.Litigation.CreateUpdate.Litigation.LitigationRequestRequest>();
            CreateMap<LitigationRequestRequestFileCreateUpdateDto, LitigationRequestRequestFile>();
            CreateMap<LitigationRequestGroundCreateUpdateDto, LitigationRequestGround>();
            CreateMap<LitigationRequestGroundFileCreateUpdateDto, LitigationRequestGroundFile>();

            CreateMap<SysLitigationRequestContractType, SysLitigationRequestContractTypeGetDto>();
            CreateMap<SysLitigationRequestContractType, SysLitigationRequestContractTypeDto>();


            CreateMap<SysLitigationRequestAmount, SysLitigationRequestAmountGetDto>();
            CreateMap<SysExemptionReductionCategory, SysExemptionReductionCategoryDto>();


            CreateMap<SysArea, SysAreaDto>();
            CreateMap<SysCity, SysCityDto>();
            CreateMap<WorkFlowLogDto, WorkFlowLog>();

            //workflow 
            CreateMap<WorkFlowLogCreateUpdateDto, WorkFlowLog>();
            CreateMap<WorkFlowLog, WorkFlowLogCreateUpdateDto>();
            CreateMap<WorkFlowProcessingLog, AuditNotesDto>()
                .ForMember(des => des.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(des => des.Notes, opt => opt.MapFrom(src => src.Notes));
            CreateMap<SysFileManager, AuditNotesDto>().ReverseMap();

            CreateMap<WorkFlowProcessingLogCreateUpdateDto, WorkFlowProcessingLog>();
            CreateMap<WorkFlowProcessingLog, WorkFlowProcessingLogCreateUpdateDto>();

        }
    }
}
