using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.UserManage;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;

namespace REAC.Application.AppServices.UserManage
{
    public class UserRegisterService : IUserRegisterService
    {
        private readonly IEntityRepository _repository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IDelegatorService _delegatorService;
        private readonly IJwtReader _jwtReader;

        public UserRegisterService(IEntityRepository repository, UserManager<User> userManager, IMapper mapper, IDelegatorService delegatorService, IJwtReader jwtReader)
        {
            _repository = repository;
            _userManager = userManager;
            _mapper = mapper;
            _delegatorService = delegatorService;
            _jwtReader = jwtReader;
        }

        public async Task<BaseResponse> ChangePassword(ChangeUserPasswordDto changeUserPasswordDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false, Message = "غير مصرح" };
                var user = _userManager.Users.Where(x => x.ReferenceId == referenceId).FirstOrDefault();

                if (user is null)
                    return new BaseResponse() { IsDone = false, Message = "رقم هويه غير موجود" };


                var checkPassowrd = await _userManager.CheckPasswordAsync(user, changeUserPasswordDto.OldPassword);

                if(!checkPassowrd)
                    return new BaseResponse() { IsDone = false, Message = "كلمة المرور القديمه غير صحيحه" };

                var changePassword = await _userManager.ChangePasswordAsync(user,changeUserPasswordDto.OldPassword,changeUserPasswordDto.NewPassword);

                if (!changePassword.Succeeded || changePassword.Errors.Any())
                    return new BaseResponse() { IsDone = false, Message = "حدث خطأ برجاء التأكد من كلمة السر والمحاوله مره اخري" };

                return new BaseResponse() { IsDone = true, Message = "تم تغيير كلمة السر بنجاح" };


            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> Register(UserRegisterDto userRegisterDto)
        {
            if (userRegisterDto == null)
                return new BaseResponse() { IsDone = false, Message = "invalid user register obj" };

            var isExistUser = _userManager.Users.Any(x => x.UserName == userRegisterDto.UserInfo.NationalId);

            if (isExistUser && !userRegisterDto.UserInfo.IsDraftAccount)
                return new BaseResponse() { IsDone = false, Message = "user exist" };


            var identityUser = new IdentityResult();
            User user = new User { UserName = userRegisterDto.UserInfo.NationalId.Trim(), Email = userRegisterDto.Email.Trim(), EmailConfirmed = true };

            if (!isExistUser && !userRegisterDto.UserInfo.IsDraftAccount)
            {
                identityUser = await _userManager.CreateAsync(user, "123456");
                if (!identityUser.Succeeded || identityUser.Errors.Any())
                    return new BaseResponse() { IsDone = false, Message = "user can't be created" };
            }

            CitizenCreateUpdateDto citizenCreateUpdateDto = new CitizenCreateUpdateDto()
            {
                AreaId = userRegisterDto.AreaId,
                CityId = userRegisterDto.CityId,
                BuildingNumber = userRegisterDto.BuildingNumber,
                District = userRegisterDto.District,
                Email = userRegisterDto.Email,
                Mobile = userRegisterDto.MobileNumber,
                Street = userRegisterDto.Street,
                UnitNumber = userRegisterDto.UnitNumber,
                UserId = user.Id,
                PostalCode = userRegisterDto.PostalCode,
                FullName = userRegisterDto.UserInfo.FullName,
                ShortName = userRegisterDto.UserInfo.FirstName,
                IdentityNumber = userRegisterDto.UserInfo.NationalId,
                IdentityExpiryDate = userRegisterDto.UserInfo.NationalIdExpiryDate.Value,
                GenderId = (int)userRegisterDto.UserInfo.Gender,
                NationalityId = userRegisterDto.UserInfo.NationalityId.Value,
                IdentityTypeId = (int)(userRegisterDto.UserInfo.NationalId.StartsWith("10") ? IdentityType.NationalIdentity : userRegisterDto.UserInfo.NationalId.StartsWith("20") ? IdentityType.Residence : IdentityType.GulfIdentity),
                PassAccountId = Guid.NewGuid(),
                Employer = string.Empty,
                BirthDate = userRegisterDto.UserInfo.DateOfBirth.Value,
            };


            if (userRegisterDto.UserInfo.IsDraftAccount)
            {
                var draftCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.IdentityNumber == userRegisterDto.UserInfo.NationalId);
                citizenCreateUpdateDto.Id = draftCitizen.Id;
                citizenCreateUpdateDto.UserId = draftCitizen.UserId;
                citizenCreateUpdateDto.Employer = draftCitizen.Employer;
                citizenCreateUpdateDto.SubNumber = draftCitizen.SubNumber;
                citizenCreateUpdateDto.Code = await _delegatorService.GenerateCitizenCode();
                var updateCitizen = _mapper.Map(citizenCreateUpdateDto, draftCitizen);
                updateCitizen.IsDraftAccount = false;
                _repository.Update<Citizen, Guid>(updateCitizen);
            }
            else
            {
                var citizen = _mapper.Map<Citizen>(citizenCreateUpdateDto);
                citizen.IsDraftAccount = false;
                citizen.Code = await _delegatorService.GenerateCitizenCode();
                await _repository.AddAsync<Citizen, Guid>(citizen);
                user.ReferenceId = citizen.Id;
                var updateUser = await _userManager.UpdateAsync(user);
                if (!identityUser.Succeeded || identityUser.Errors.Any())
                    return new BaseResponse() { IsDone = false, Message = "user can't be updated" };
            }

            var result = await _repository.SaveChangesAsync();
            if (!result.IsDone) return result;

            return new BaseResponse() { IsDone = true };
        }
    }
}
