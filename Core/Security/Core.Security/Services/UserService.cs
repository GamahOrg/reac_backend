using Core.Security.Contracts;
using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;

namespace Core.Security.Services
{
    public class UserService: IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
        }

        public async Task<BaseResponse> AddAsync(UserDto userDto, CancellationToken cancellationToken)
        {
            var userExist = _userManager.Users.Any(u => u.UserName.ToLower() == userDto.UserName.ToLower() || u.Email.ToLower() == userDto.Email.ToLower());

            if (userExist)
                return new DataResponse<User?> { IsDone = true,Data = null, Message = "UserName or Email Already Exist!" };

            //var roleExist = await roleManager.FindByNameAsync(userDto.Role.Trim());

            //if (roleExist == null && !string.IsNullOrEmpty(userDto.Role))
            //    return new BaseResponse { IsDone = true, Message = "Role Does Not Exist!" };

            User user = new User { UserName = userDto.UserName.Trim(), Email = userDto.Email, EmailConfirmed = true };

            var result = await _userManager.CreateAsync(user, userDto.Password);
            if (result.Succeeded)
                return new DataResponse<Guid?> { IsDone = true,  Data = user.Id };
            else
                return new DataResponse<User?> { IsDone = true, Data = null, Message = $"{result.Errors.FirstOrDefault().Description}" };

        }
    }
}
