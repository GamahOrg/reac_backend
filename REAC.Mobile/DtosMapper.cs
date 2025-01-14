using AutoMapper;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;
using REAC.Application.Contracts.MobileDtos.Litigation.List;
using REAC.Application.Contracts.MobileDtos.Litigation.ShortDto;
using REAC.Application.Contracts.MobileDtos.Wpf;

using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Mobile
{
    public class DtosMapper : Profile
    {
        public DtosMapper()
        {
          
            #region Mobile
            CreateMap<LitigationRequest, LitigationListMobileDto>()
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


            CreateMap<LitigationRequest, LitigationRequestShortMobileDto>()
                    .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
                    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName))
                    //.ForMember(dest => dest.LitigationRequestGrounds, opt => opt.MapFrom(src => src.LitigationRequestGrounds))
                    //.ForMember(dest => dest.LitigationRequestRequests, opt => opt.MapFrom(src => src.LitigationRequestRequests))
                 .ReverseMap();

            CreateMap<SysFileManager, SysFileManagerCreateUpdateDto>().ReverseMap();
            CreateMap<LitigationRequestGround, LitigationRequestGroundShortMobileDto>();
            CreateMap<LitigationRequestRequest, LitigationRequestRequestShortMobileDto>();
            CreateMap<WorkFlowLogMobileDto, WorkFlowLog>().ReverseMap();
            CreateMap<LitigationRequest, LitigationRequestShortDto>()
                    .ForMember(dest => dest.AreaName, opt => opt.MapFrom(src => src.Area.AreaName))
                    .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.CityName))
                .ReverseMap();
            CreateMap<LitigationRequestContract, LitigationRequestContractShortDto>().ReverseMap();
            CreateMap<SysLitigationRequestContractType, SysLitigationRequestContractTypeGetDto>();
            CreateMap<SysLitigationRequestContractType, SysLitigationRequestContractTypeDto>();
            CreateMap<SysExemptionReductionCategory, SysExemptionReductionCategoryDto>();

            #endregion
        }
    }
}
