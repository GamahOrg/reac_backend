using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.UserManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.UserManage
{
    public interface IUserRegisterService
    {
        Task<BaseResponse> Register(UserRegisterDto userRegisterDto);
    }
}
