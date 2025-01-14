using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Common;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.UserManage
{
    public class OTPService : IOTPService
    {
        private ISysSettingService _settingService;
        private readonly IEntityRepository _repository;
        private readonly ISMSService _sMSService;
        private readonly IJsonStringLocalizer _localization;

        public OTPService(ISysSettingService settingService, IEntityRepository repository, ISMSService sMSService, IJsonStringLocalizer localization)
        {
            _settingService = settingService;
            _repository = repository;
            _sMSService = sMSService;
            _localization = localization;
        }

        public async Task<BaseResponse> SaveOTP(string mobileNumber)
        {
            mobileNumber = mobileNumber.Trim();
            string message = string.Empty;
            var response = new DataResponse<string>() { IsDone = false, Message = message };

            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                response.Message = await GetStatusMessageLocalization(OTPStatus.HasIssue);
                return response;
            }

            if (!long.TryParse(mobileNumber, out long value))
            {
                response.Message = await GetStatusMessageLocalization(OTPStatus.Wrong);
                return response;
            }

            var otpObj = await GetOTPByMobileNumber(mobileNumber);

            // If null create new instance
            otpObj ??= new SysMobileOTP();

            var validityTimeout = await GetOTPTimeoutSettingValue();

            if (!validityTimeout.IsDone || validityTimeout.Data == -1)
            {
                response.Message = await GetStatusMessageLocalization(OTPStatus.HasIssue);
                return response;
            }

            var otpGeneratedCode = await GenerateStringOTP();

            otpObj.ExpiryDateTime = DateTime.UtcNow.AddSeconds(validityTimeout.Data);
            otpObj.Code = otpGeneratedCode;
            otpObj.Mobile = mobileNumber;

            if (otpObj.Id == Guid.Empty)
                await _repository.AddAsync<SysMobileOTP, Guid>(otpObj);
            else
                _repository.Update<SysMobileOTP, Guid>(otpObj);

            if ((await _repository.SaveChangesAsync()).IsDone)
            {

                var disableIntegrationsResult = await _settingService.CheckIntegrationDisabledAsync(SysSettingKeyConstants.Disable_SMS_Integration);

                if (!disableIntegrationsResult.IsDone)
                    return new BaseResponse { IsDone = false, Message = disableIntegrationsResult.Message };

                var disableIntegrations = disableIntegrationsResult.Data;

                if (!disableIntegrations)
                {
                    SMSRequestDto sMSRequestDto = new SMSRequestDto()
                    {
                        message_text = "رمز التحقق " + otpGeneratedCode, // _localization["OTPCode"] + otpGeneratedCode,
                        recipients = new List<string> { mobileNumber },
                        notification_type = (await _settingService.GetBySettingKey(SysSettingKeyConstants.SMS_Notification_Type)).Data?.Value,
                    };
                    var sendSMSResponse = await _sMSService.SendSMS(sMSRequestDto);
                    //response.Data = otpObj.Code;
                    if (!sendSMSResponse.IsDone)
                    {
                        response.Message = await GetStatusMessageLocalization(OTPStatus.SMS_Sent_Failed);
                        return response;
                    }
                    response.Message = await GetStatusMessageLocalization(OTPStatus.SMS_Sent_Success);
                    response.IsDone = true;
                    return response;
                }
                else
                {
                    return new DataResponse<string> { IsDone = true, Data = otpObj.Code };

                }
            }

            response.Message = await GetStatusMessageLocalization(OTPStatus.HasIssue);
            return response;
        }


        public async Task<BaseResponse> ValidateOTP(string mobileNumber, string otpCode)
        {
            string message = string.Empty;
            var result = new BaseResponse { IsDone = false, Message = message };

            if (string.IsNullOrWhiteSpace(mobileNumber) ||
                string.IsNullOrWhiteSpace(otpCode))
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.HasIssue);
                return result;
            }

            if (!long.TryParse(otpCode, out long value))
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.Wrong);
                return result;
            }

            var otpRes = await GetOTPByMobileNumber(mobileNumber);

            if (otpRes is null)
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.NotExist);
                return result;
            }

            if (otpRes?.Code != otpCode)
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.Wrong);
                return result;
            }

            if (otpRes?.ExpiryDateTime > DateTime.UtcNow)
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.Valid);
                result.IsDone = true;
                // Delete if Expired
                _repository.Remove<SysMobileOTP, Guid>(otpRes);
                await _repository.SaveChangesAsync();
            }
            else
            {
                result.Message = await GetStatusMessageLocalization(OTPStatus.Expired);
                // Delete if Expired
                _repository.Remove<SysMobileOTP, Guid>(otpRes);
                await _repository.SaveChangesAsync();
            }

            return result;
        }

        public async Task<DataResponse<int>> GetOTPTimeoutSettingValue()
        {
            if (!int.TryParse(((await _settingService.GetBySettingKey("OTP.TimeoutInSeconds")) as DataResponse<SysSetting>)?.Data?.Value, out int value))
                value = -1;

            return new DataResponse<int> { IsDone = true, Data = value };
        }


        private Task<string> GetStatusMessageLocalization(OTPStatus enumCode)
        {
            return Task.FromResult(
                enumCode switch
                {
                    OTPStatus.Valid => string.Format(_localization["OTPCodeVALID"]),
                    OTPStatus.Expired => string.Format(_localization["OTPCodeEXPIRED"]),
                    OTPStatus.HasIssue => string.Format(_localization["OTPCodeHAS_ISSUE"]),
                    OTPStatus.NotExist => string.Format(_localization["OTPCodeNOT_EXIST"]),
                    OTPStatus.Wrong => string.Format(_localization["OTP_Code_WRONG"]),
                    OTPStatus.SMS_Sent_Success => string.Format(_localization["OTP_SMS_Sent_SUCCESS"]),
                    OTPStatus.SMS_Sent_Failed => string.Format(_localization["OTP_SMS_Sent_FAILED"]),
                    _ => string.Format(_localization["OTPCodeHAS_ISSUE"])
                });
        }

        private async Task<SysMobileOTP> GetOTPByMobileNumber(string mobileNumber)
        {
            var result = await _repository.GetAsync<SysMobileOTP, Guid>(x => x.Mobile == mobileNumber);
            if (!result.IsDone) return null;

            return (result as DataResponse<SysMobileOTP>)?.Data;
        }

        public async Task<string> GenerateStringOTP()
        {
            var settingObj = await _settingService.GetBySettingKey("OTP.Lenght");

            if (!settingObj.IsDone)
                return string.Empty;

            int Lenght = Int32.Parse(settingObj.Data?.Value);

            string numbers = "0123456789";
            string otp;
            Random random;
            do
            {
                otp = string.Empty;
                random = new();
                for (int i = 0; i < Lenght; i++)
                {
                    int tempval = random.Next(0, numbers.Length);
                    otp += tempval;
                }
            }
            while (await IsOTPExists(otp));

            return otp;
        }

        private async Task<bool> IsOTPExists(string otpCode)
        {
            var isExists = await _repository.ExistsAsync<SysMobileOTP, Guid>(x => x.Code == otpCode);

            if (!isExists.IsDone)
                return false;

            return (isExists as DataResponse<bool>).Data;
        }


        public async Task<string> GetOTPByMobile(string mobileNumber)
        {
            var otpRes = await GetOTPByMobileNumber(mobileNumber);
            if (otpRes != null)
                return otpRes.Code;

            return "Not FOund";
        }

    }
}
