using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Instance;
using Core.Log.Domain.Shared.Enums;
using Core.Log.Domain.Shared.Extentions;

namespace Core.Log.Application.Manager
{
    public class LoggerManager : ILoggerManager
    {

        private readonly DbLogger _dbLogger;
        private readonly FileLogger _fileLogger;
        private readonly MailLogger _mailLogger;
        //private readonly Lazy<SMSLogger> _smsLogger;
        private readonly Lazy<ConsoleLogger> _consoleLogger;

        public LoggerManager(DbLogger dbLogger, FileLogger fileLogger, MailLogger mailLogger)
        {

            _dbLogger = dbLogger;
            _fileLogger = fileLogger;
            _mailLogger = mailLogger;
            //_smsLogger = new Lazy<SMSLogger>(() => new SMSLogger());
            _consoleLogger = new Lazy<ConsoleLogger>(() => new ConsoleLogger());
        }

        public void Log(LoggerType loggerType, LogDto logDto)
        {
            var loggers = loggerType.GetFlags();
            foreach (var type in loggers)
            {
                switch (type)
                {
                    case LoggerType.Console:
                        _mailLogger.Log(logDto);
                        break;
                    case LoggerType.Database:
                        _dbLogger.Log(logDto);
                        break;
                    case LoggerType.File:
                        _dbLogger.Log(logDto);
                        break;
                        //case LoggerType.SMS:
                        //    _smsLogger.Value.Log(logDto);
                        //    break;
                        //case LoggerType.Email:
                        //    _mailLogger.Value.Log(logDto);
                        //    break;

                }
            }
        }
    }
}