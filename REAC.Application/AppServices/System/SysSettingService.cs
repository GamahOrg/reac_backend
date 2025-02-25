using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Localizations;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.Extensions.Options;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Common;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;



namespace REAC.Application.AppServices.System
{
    public class SysSettingService : BasicService<SysSetting, Guid>, ISysSettingService
    {
        private readonly IJsonStringLocalizer _localization;
        public SysSettingService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, IJsonStringLocalizer localization) : base(repository, mapper, loggerManager, jwtReader)
        {
            _repository = repository;
            _localization = localization;
        }

        public async Task<DataResponse<SysSetting>> GetBySettingKey(string SettingKey)
        {
            var SettingObj = (_repository.GetQueryable<SysSetting, Guid>())
                            .Result?.Where(x => x.Key == SettingKey)?.FirstOrDefault();
            if (SettingObj != null)
                return new DataResponse<SysSetting> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<SysSetting> { Data = null, IsDone = false };

        }
        public async Task<DataResponse<List<SysSetting>>> GetReconciliationSetting()
        {
            var SettingObj = (_repository.GetQueryable<SysSetting, Guid>())
                            .Result?.Where(x => SystemSettingsConfiguration.ReconciliationSystemSettings.Contains(x.Key))?.ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };

        }
        public async Task<DataResponse<List<SysSetting>>> GetArbitrationSetting()
        {
            var SettingObj = (_repository.GetQueryable<SysSetting, Guid>())
                            .Result?.Where(x => SystemSettingsConfiguration.ArbitrationSystemSettings.Contains(x.Key))?.ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };

        }

        public async Task<DataResponse<List<SysSetting>>> GetReviewSetting()
        {
            var SettingObj = (_repository.GetQueryable<SysSetting, Guid>())
                            .Result?.Where(x => SystemSettingsConfiguration.ReviewSystemSetting.Contains(x.Key))?.ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };

        }
        public async Task<DataResponse<SysSetting>> UpdateSettingsAsync(Dictionary<string, string> settingsToUpdate)
        {
            foreach (var setting in settingsToUpdate)
            {
                var sysSetting = _repository.GetQueryable<SysSetting, Guid>().Result.FirstOrDefault
                    (s => s.Key == setting.Key);

                if (sysSetting != null)
                {
                    sysSetting.Value = setting.Value; // Update the value
                }
            }

            var saveResponse = await _repository.SaveChangesAsync();
            return new DataResponse<SysSetting> { Data = null, IsDone = true, Message = Localizations.UpdateSuccefully };
        }

        public async Task<DataResponse<List<SysSetting>>> GetByAllSetting()
        {
            var SettingObj = (_repository.GetQueryable<SysSetting, Guid>()).Result.ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };
        }
        
        public async Task<DataResponse<List<SysSetting>>> GetReconciliationFeesSettings()
        {
           
            var SettingObj = (await _repository.GetQueryable<SysSetting, Guid>())
                .Where(x => SystemSettingsConfiguration.ReconciliationFeesSystemSettings.Contains(x.Key))?
                .ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };
        }
        
        public async Task<DataResponse<List<SysSetting>>> GetArbitrationFeesSettings()
        {
           
            var SettingObj = (await _repository.GetQueryable<SysSetting, Guid>())
                .Where(x => SystemSettingsConfiguration.ArbitrationFeesSystemSettings.Contains(x.Key))?
                .ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };
        }

        public async Task<DataResponse<SysSetting>> UpdateBySettingKey(SysSetting updateSetting)
        {
            _repository.Update<SysSetting, Guid>(updateSetting);
            await _repository.SaveChangesAsync();
            return new DataResponse<SysSetting> { Data = updateSetting, IsDone = true ,Message = Localizations.UpdateSuccefully};
        }

        public async Task<DataResponse<List<SysSetting>>> GetByIntegrationKey(string key)
        {
            var SettingObj = (await _repository.GetQueryable<SysSetting, Guid>())
                             .Where(x => x.Key.StartsWith(key))?.ToList();
            if (SettingObj != null)
                return new DataResponse<List<SysSetting>> { Data = SettingObj, IsDone = true };
            else
                return new DataResponse<List<SysSetting>> { Data = null, IsDone = false };

        }

        public async Task<DataResponse<bool>> CheckIntegrationDisabledAsync(string sysSettingKeyConstants)
        {
            var disableIntegrationsResult = await GetBySettingKey(sysSettingKeyConstants);
            if (disableIntegrationsResult.IsDone)
            {
                var disableIntegrations = bool.Parse((disableIntegrationsResult as DataResponse<SysSetting>).Data.Value);
                return new DataResponse<bool> { IsDone = true, Data = disableIntegrations };
            }
            return new DataResponse<bool> { IsDone = false, Message = _localization["CannotConnectWithIntegrationsApi"] };
        }

    }
}
