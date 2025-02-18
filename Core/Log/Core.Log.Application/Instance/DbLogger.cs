using Core.GenericRepository.Shared.BaseEntities;
using Core.Log.Application.Contracts.Dto;
using Core.Log.EntityFrameworkCore;
using Core.Log.EntityFrameworkCore.Entities;
using Core.Shared.Responses;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using REAC.Domain.Shared.Enum;
using System;
using System.Data;
using System.Security.Claims;

using ILogger = Core.Log.Application.Contracts.IAppServices.ILogger;

namespace Core.Log.Application.Instance
{
    public class DbLogger : ILogger
    {
        private readonly LogDbContext _db;
        private readonly IConfiguration _configuration;

        public DbLogger(LogDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public async Task<BaseResponse> Log(LogDto logDto)
        {
            return await AddDbLoger(logDto);
        }

        //public async Task<BaseResponse> AddDbLoger<LogDto>(LogDto logDto, CancellationToken cancellationToken = default) where LogDto : BaseEntity<Guid>
        //{
        //    var entity = logDto as Log.Application.Contracts.Dto.LogDto;
        //    DbLog dbLog = new DbLog();
        //    dbLog.Id = Guid.NewGuid();
        //    dbLog.Type = entity.Type.ToString();
        //    dbLog.Message = entity.Message;
        //    dbLog.DateTime = DateTime.Now;
        //    try
        //    {
        //        await _db.DbLogs.AddAsync(dbLog);
        //        await _db.SaveChangesAsync(cancellationToken);
        //    }
        //    catch (Exception exc)
        //    {
        //        return new ExceptionResponse()
        //        {
        //            IsDone = true,
        //            Exception = exc.ToString(),
        //            Message = exc.Message
        //        };
        //    }
        //    return new BaseResponse()
        //    {
        //        IsDone = true
        //    };
        //}

        public async Task<BaseResponse> AddDbLoger<LogDto>(LogDto logDto, CancellationToken cancellationToken = default) where LogDto : BaseEntity<Guid>
        {
            try
            {
                var entity = logDto as Log.Application.Contracts.Dto.LogDto;
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")?.ToString()))
                {
                    db.Open();


                    string query = "INSERT INTO [dbo].[DbLogs] (Id,Type,DateTime,Message) VALUES (@Id, @Type,@DateTime,@Message)";

                    using (IDbCommand command = db.CreateCommand())
                    {
                        command.CommandText = query;

                        // Add parameters
                        var idParam = command.CreateParameter();
                        idParam.ParameterName = "@Id";
                        idParam.Value = Guid.NewGuid();
                        command.Parameters.Add(idParam);

                        var typeParam = command.CreateParameter();
                        typeParam.ParameterName = "@Type";
                        typeParam.Value = 2;
                        command.Parameters.Add(typeParam);

                        var dateParam = command.CreateParameter();
                        dateParam.ParameterName = "@DateTime";
                        dateParam.Value = DateTime.Now;
                        command.Parameters.Add(dateParam);

                        var messageParam = command.CreateParameter();
                        messageParam.ParameterName = "@Message";
                        messageParam.Value = entity.Message;
                        command.Parameters.Add(messageParam);



                        // Execute the command
                        command.ExecuteNonQuery();
                    }


                    db.Close();

                    return new BaseResponse()
                    {
                        IsDone = true
                    };
                }
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = true,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }
        }
    }

}



