using Core.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.UserManage
{
    public interface IOTPService
    {
        Task<BaseResponse> SaveOTP(string mobileNumber);
        Task<BaseResponse> ValidateOTP(string mobileNumber, string otpCode);
        Task<DataResponse<int>> GetOTPTimeoutSettingValue();
        Task<string> GetOTPByMobile(string mobileNumber);
    }
}
