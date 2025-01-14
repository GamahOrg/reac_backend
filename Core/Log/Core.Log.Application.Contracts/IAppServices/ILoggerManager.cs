using Core.Log.Application.Contracts.Dto;
using Core.Log.Domain.Shared.Enums;

namespace Core.Log.Application.Contracts.IAppServices
{
    public interface ILoggerManager
    {
        void Log(LoggerType loggerType, LogDto logDto);

    }
}
