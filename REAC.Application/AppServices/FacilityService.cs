using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Facility;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
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

        public async Task<BaseResponse> ValidateAndAddFacility(string crNationalNumber, Guid? citizenId, bool AllowAddFacility)
        {
            try
            {
                // Get Facility Information
                if (string.IsNullOrWhiteSpace(crNationalNumber))
                    return new BaseResponse() { IsDone = false, Message = "الرقم الوطنى الموحد للسجل التجارى غير موجود" };

                var nationalNumberInfomationResponse = await _mCService.GetCRNationalNumberInformation(crNationalNumber);

                if (nationalNumberInfomationResponse == null)
                    return new BaseResponse { Message = "البيانات المدخلة غير صحيحة" };

                if (!nationalNumberInfomationResponse.IsDone)
                    return new BaseResponse { Message = nationalNumberInfomationResponse.Message };

                if (nationalNumberInfomationResponse.Data == null)
                    return new BaseResponse { Message = "البيانات المدخلة غير صحيحة" };

                //Validate owners

                Guid referenceId = Guid.Empty;
                if (citizenId.Value != Guid.Empty)
                    referenceId = citizenId.Value;
                else
                    _jwtReader.GetReferenceId(ref referenceId);

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                        .AsNoTracking()
                        .Include(x => x.Nationality)
                        .Where(x => x.Id == referenceId).FirstOrDefault();


                var _identityTypeId = citizen?.IdentityTypeId != null ? (int)citizen.IdentityTypeId : citizen?.IdentityNumber[0] == '1' ? 1 : 3;

                var _nationalityCode = citizen?.Nationality?.NationalityCode != null ? (int)citizen.Nationality.NationalityCode : 113;
                //var validateOwner = await _mCService.ValidateRelatedCRs(nationalNumberInfomationResponse.Data.CRNationalNumber, _identityTypeId.ToString(), citizen.IdentityNumber, _nationalityCode.ToString());
                //if (validateOwner == null)
                //    return new BaseResponse { Message = "البيانات المدخلة غير صحيحة" };

                //if (!validateOwner.IsDone)
                //    return new BaseResponse { Message = validateOwner.Message };



                var isExist = (await _repository.GetQueryable<Facility, Guid>()).Any(x => x.CommercialRegisterNumber == nationalNumberInfomationResponse.Data.CRNumber || x.Facility700Number == nationalNumberInfomationResponse.Data.CRNationalNumber);
                if (!isExist)
                //return new BaseResponse() { IsDone = false, Message = "الرقم الوطنى الموحد للسجل التجارى او السجل التجارى موجود مسبقا" };

                {
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
                        ArabicFacilityName = nationalNumberInfomationResponse.Data.CRName,
                        Code = code,
                        Facility700Number = nationalNumberInfomationResponse.Data.CRNationalNumber,
                        CommercialRegisterNumber = nationalNumberInfomationResponse.Data.CRNumber,
                        Street = nationalNumberInfomationResponse.Data.CRAddress,
                    };

                    await _repository.AddAsync<Facility, Guid>(facility);

                    var result = await _repository.SaveChangesAsync();
                    if (!result.IsDone) return result;

                }
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
