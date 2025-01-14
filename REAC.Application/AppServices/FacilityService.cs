using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Facility;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using static REAC.Application.Contracts.IAppServices.Litigation.ILitigationRequestService;

namespace REAC.Application.AppServices
{
    public class FacilityService : BasicService<Facility, Guid>, IFacilityService
    {
        private readonly IJwtReader _jwtReader;
        private readonly IMCService _mCService;


        public FacilityService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, IMCService mCService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _mapper = mapper;
            _jwtReader = jwtReader;
            _mCService = mCService;
        }

        public async Task<BaseResponse> GetAllFacilitiesAsync(int pageIndex, int pageSize, string? searchKey)
        {
            try
            {
                
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var facilities = (await _repository.GetQueryable<Facility, Guid>()).Where(f => f.CitizenId == referenceId).ToList();

                var list = _mapper.Map<List<GetAllFacilitiesResponseDto>>(facilities);



                var result = await PaginatedList<GetAllFacilitiesResponseDto>.CreateAsync(list, pageIndex, pageSize);
                if (result.Items.Count == 0) return new BaseResponse() { IsDone = true, Message = "غير موجود" };

                return new DataResponse<PaginatedList<GetAllFacilitiesResponseDto>>()
                {
                    Data = result,
                    IsDone = true
                };


            }
            catch (Exception ex) { return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" }; }
        }
        public async Task<BaseResponse> GetAllFacilitiesDropdown()
       {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var facilities = (await _repository.GetQueryable<Facility, Guid>()).Where(f => f.CitizenId == referenceId).ToList();

                var list = _mapper.Map<List<GetAllFacilitiesForDropdownDto>>(facilities);


                return new DataResponse<List<GetAllFacilitiesForDropdownDto>>()
                {
                    Data = list,
                    IsDone = true
                };


            }
            catch (Exception ex) { return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" }; }
        }

        public async Task<BaseResponse> GetFacilityByIdAsync(Guid facilityId)
        {
            try
            {


                var facility = (await _repository.GetQueryable<Facility, Guid>())
                    .AsNoTracking().Where(x => x.Id == facilityId)
                              .FirstOrDefault();

                if (facility == null) return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var result = _mapper.Map<GetFacilityByIdResponse>(facility);


                return new DataResponse<GetFacilityByIdResponse>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex) { return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" }; }
        }

        public async Task<BaseResponse> EditFacilityAsync(EditFacilityRequestDto editFacilityRequestDto)
        {
            try
            {


                var response = await _repository.GetByIdAsync<Facility, Guid>(editFacilityRequestDto.Id, default);

                if (((DataResponse<Facility>)response).Data == null)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                if (response.IsDone != true)
                    return response;

                var facility = ((DataResponse<Facility>)response).Data;

                facility.Street = editFacilityRequestDto.Street;
                facility.City = editFacilityRequestDto.City;
                facility.District = editFacilityRequestDto.District;
                facility.PostalCode = editFacilityRequestDto.PostalCode;
                facility.Email = editFacilityRequestDto.Email;
                facility.SubNumber = editFacilityRequestDto.SubNumber;
                facility.BuildingNumber = editFacilityRequestDto.BuildingNumber;
                facility.UnitNumber = editFacilityRequestDto.UnitNumber;
                facility.Fax = editFacilityRequestDto.Fax;


                var saveResponse = _repository.SaveChangesAsync().Result;
                if (saveResponse.IsDone != true)
                    return response;

                var updatedResponse = await _repository.GetByIdAsync<Facility, Guid>(editFacilityRequestDto.Id, default);
                if (updatedResponse.IsDone != true)
                    return response;

                var updatedFacility = _mapper.Map<EditFacilityRequestDto>(((DataResponse<Facility>)updatedResponse).Data);
                return new DataResponse<EditFacilityRequestDto>()
                {
                    IsDone = true,
                    Data = updatedFacility
                };

            }
            catch (Exception ex) { return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" }; }
        }

        public async Task<BaseResponse> ValidateAndAddFacility(string crNumber, Guid? citizenId)
        {
            try
            {
                // Get Facility Information
                if (string.IsNullOrWhiteSpace(crNumber))
                    return new BaseResponse() { IsDone = false, Message = "رقم السجل التجاري غير موجود" };



                // Get CR Owners
                var crOwnersnresult = await _mCService.GetCRInformationByCROwners(crNumber);

                if (crOwnersnresult.Data == null || !crOwnersnresult.IsDone || crOwnersnresult.Data.Data is null
                    || crOwnersnresult.Data.Error is not null || crOwnersnresult.Data.UnauthorizedError is not null)
                    return new BaseResponse() { IsDone = false, Message = "البيانات المدخله غير صحيحه" };

                Guid referenceId = Guid.Empty;
                if (citizenId.Value != Guid.Empty)
                    referenceId = citizenId.Value;
                else
                    _jwtReader.GetReferenceId(ref referenceId);

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                        .AsNoTracking().Where(x => x.Id == referenceId).FirstOrDefault();

                // Check if User is an owner
                if (!crOwnersnresult.Data.Data.Contains(citizen.IdentityNumber))
                    return new BaseResponse() { IsDone = false, Message = "البيانات المدخله غير صحيحه" };



                var crInformationResult = await _mCService.GetCRInformationByCRNumber(crNumber);

                if (crInformationResult.Data == null || !crInformationResult.IsDone || crInformationResult.Data.Data is null
                    || crInformationResult.Data.Error is not null || crInformationResult.Data.UnauthorizedError is not null)
                    return new BaseResponse() { IsDone = false, Message = "البيانات المدخله غير صحيحه" };

                if (crInformationResult.Data.Data.CRStatus != CRStatusType.Active.ToString())
                    return new BaseResponse() { IsDone = false, Message = "رقم السجل التجاري غير فعال" };

                if (crInformationResult.Data.Data.CRExpiryDate < DateTime.Now)
                    return new BaseResponse() { IsDone = false, Message = "رقم السجل التجاري منتهي" };




                var isExist = (await _repository.GetQueryable<Facility, Guid>()).Any(x => x.CommercialRegisterNumber == crNumber || x.Facility700Number == crNumber);
                if (isExist)
                    return new BaseResponse() { IsDone = false, Message = "رقم السجل التجاري موجود مسبقا" };


                // Add new Facility

                // Generate Facility Code
                var facilityRequestList = (await _repository.GetQueryable<Facility, Guid>()).Where(x => x.Code != null).ToList();


                string code = "FAC_00000001";

                if (facilityRequestList is null)
                    code = "FAC_}}";
                else
                    code = "FAC_" + (facilityRequestList.Count() + 1).ToString("D8");

                Facility facility = new Facility()
                {
                    Id = Guid.NewGuid(),
                    CitizenId = citizen.Id,
                    ArabicFacilityName = crInformationResult.Data.Data.CRName,
                    Code = code,
                    Facility700Number = crInformationResult.Data.Data.CRNationalNumber,
                    CommercialRegisterNumber = crInformationResult.Data.Data.CRNumber,
                    Street = crInformationResult.Data.Data.CRAddress,
                };

                await _repository.AddAsync<Facility, Guid>(facility);

                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone) return result;

                return new BaseResponse() { IsDone = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR LOG ========================>" + ex.ToString());
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }

        }

    }
}
