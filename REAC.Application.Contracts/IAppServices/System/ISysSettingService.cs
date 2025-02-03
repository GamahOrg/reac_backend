using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysSettingService : IBasicService<SysSetting, Guid>
    {
        public Task<DataResponse<SysSetting>> GetBySettingKey(string SettingKey);
        Task<DataResponse<List<SysSetting>>> GetByAllSetting();
        Task<DataResponse<SysSetting>> UpdateBySettingKey(SysSetting updateSetting);
        Task<DataResponse<List<SysSetting>>> GetByIntegrationKey(string key);
        Task<DataResponse<bool>> CheckIntegrationDisabledAsync(string sysSettingKeyConstants);
        Task<DataResponse<List<SysSetting>>> GetReconciliationSetting();
        Task<DataResponse<List<SysSetting>>> GetReconciliationFeesSettings();
        Task<DataResponse<SysSetting>> UpdateSettingsAsync(Dictionary<string, string> settingsToUpdate);
    }
}
