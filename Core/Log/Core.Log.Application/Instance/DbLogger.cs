using Core.GenericRepository.Shared.BaseEntities;
using Core.Log.Application.Contracts.Dto;
using Core.Log.EntityFrameworkCore;
using Core.Log.EntityFrameworkCore.Entities;
using Core.Shared.Responses;
using ILogger = Core.Log.Application.Contracts.IAppServices.ILogger;

namespace Core.Log.Application.Instance
{
    public class DbLogger : ILogger
    {
        private readonly LogDbContext _db;

        public DbLogger(LogDbContext db)
        {
            _db = db;
        }
        public async Task<BaseResponse> Log(LogDto logDto)
        {
            return await AddDbLoger(logDto);
        }

        public async Task<BaseResponse> AddDbLoger<LogDto>(LogDto logDto, CancellationToken cancellationToken = default) where LogDto : BaseEntity<Guid>
        {
            var entity = logDto as Log.Application.Contracts.Dto.LogDto;
            DbLog dbLog = new DbLog();
            dbLog.Id = Guid.NewGuid();
            dbLog.Type = entity.Type.ToString();
            dbLog.Message = entity.Message;
            dbLog.DateTime = DateTime.Now;
            try
            {
                await _db.DbLogs.AddAsync(dbLog);
                await _db.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exc)
            {
                return new ExceptionResponse()
                {
                    IsDone = true,
                    Exception = exc.ToString(),
                    Message = exc.Message
                };
            }
            return new BaseResponse()
            {
                IsDone = true
            };
        }
    }

}



