using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using iText.Kernel.Geom;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Reports;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Reports;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Reports
{
    public class ReportService : IReportService
    {
        private IEntityRepository _entityRepository;
        private readonly IJwtReader _jwtReader;
        private readonly IConfiguration _configuration;

        public ReportService(IEntityRepository repository, IJwtReader jwtReader, IConfiguration configuration)
        {
            _entityRepository = repository;
            _jwtReader = jwtReader;
            _configuration = configuration;
        }

        //public async Task<BaseResponse> GetListOfLitigationReviewReport(GenericReportsFilter filter)
        //{
        //    if (filter.IsAdmin != true)
        //    {

        //        Guid employee = Guid.Empty;
        //        _jwtReader.GetReferenceId(ref employee);

        //        filter.EmployeeIds = new List<string>() { employee.ToString() };
        //    }
        //    var inputParms = BuildListOfLitigationReviewReport(filter);

        //    //var baseResponseStaticsResult = await GetLitigationStatisticsReviewReports(filter);
        //    var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetListOfLitigationReviewReportResult>("[dbo].[GetListOfLitigationReviewReport]", inputParms);
        //    if (/*baseResponseStaticsResult.IsDone &&*/ resultList.IsDone)
        //    {

        //        dynamic obj = new ExpandoObject();
        //        //obj.LitigationStatisticsReview = (baseResponseStaticsResult as DataResponse<List<GetLitigationStatisticsMainReportResult>>)?.Data;
        //        obj.LitigationListReview = (resultList as DataResponse<List<GetListOfLitigationReviewReportResult>>)?.Data;
        //        return new DataResponse<object>()
        //        {
        //            IsDone = true,
        //            Data = obj
        //        };
        //    }
        //    return new BaseResponse()
        //    {
        //        IsDone = false,
        //        Message = string.Join(", ",
        //                    new[] { resultList?.Message/*, baseResponseStaticsResult?.Message */}
        //                    .Where(msg => !string.IsNullOrEmpty(msg)))

        //    };
        //}



        public async Task<BaseResponse> GetListOfLitigationReviewReport(GenericReportsFilter filter)
        {
            if (filter.IsAdmin != true)
            {
                Guid employee = Guid.Empty;
                _jwtReader.GetReferenceId(ref employee);
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            var inputParms = BuildListOfLitigationReviewReport(filter);

            var resultList = new List<GetListOfLitigationReviewReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetListOfLitigationReviewReport]", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Add parameters
                foreach (var kvp in inputParms)
                {
                    cmd.Parameters.AddWithValue("@" + kvp.Key, kvp.Value ?? DBNull.Value);
                }

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    // First result set: the list
                    while (await reader.ReadAsync())
                    {
                        var item = new GetListOfLitigationReviewReportResult
                        {
                            EmployeeName = reader["EmployeeName"] as string,
                            CreatedOn = reader["CreatedOn"] as DateTime?,
                            RequestCode = reader["RequestCode"] as string,
                            ContractAddress = reader["ContractAddress"] as string,
                            LitigationAmount = reader["LitigationAmount"] as string,
                            AreaName = reader["AreaName"] as string,
                            CityName = reader["CityName"] as string,
                            CategoryName = reader["CategoryName"] as string,
                            RequestStatus = reader["RequestStatus"] as string,
                            FirstParty = reader["FirstParty"] as string,
                            SecondParty = reader["SecondParty"] as string,
                        };
                        resultList.Add(item);
                    }

                    // Second result set: the total count (if you return it separately)
                    if (await reader.NextResultAsync() && await reader.ReadAsync())
                    {
                        totalCount = reader.GetInt32(0);
                    }
                }
            }

            var pageCount = (double)totalCount / filter.Take;

            return new DataResponse<PagedResult<GetListOfLitigationReviewReportResult>>
            {
                Data =
                  new PagedResult<GetListOfLitigationReviewReportResult>
                  {
                      Results = resultList,
                      CurrentPage = (int)filter.Skip + 1,
                      PageCount = (int)Math.Ceiling((double)pageCount),
                      PageSize = (int)filter.Take,
                      TotalCount = totalCount
                  },
                IsDone = true
            };
        }



        public async Task<BaseResponse> GetLitigationStatisticsReviewReport(GenericReportsFilter filter)
        {
            Guid employee = Guid.Empty;
            _jwtReader.GetReferenceId(ref employee);

            if (filter.IsAdmin != true)
            {
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            //"2025-05-21"

            var inputParms = BuildReviewFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsReportResult>("[dbo].[GetLitigationStatisticsReviewReport]", inputParms);

            if (result.IsDone)
            {
                return result;
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = string.Join(", ",
                            new[] { result?.Message, result?.Message }
                            .Where(msg => !string.IsNullOrEmpty(msg)))

            };



        }

        //public async Task<BaseResponse> GetLitigationStatisticsReviewReport(GenericReportsFilter filter)
        //{
        //    var inputParms = BuildMainFilterParam(filter);
        //    var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsReviewReport]", inputParms);
        //    return result;
        //}
        public async Task<BaseResponse> GetListOfLitigationReformerReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            //inputParms.Add("ReformerNames", filter.ReformerNames ?? Convert.DBNull);
            //var out=new Dictionary(){{"id","int"}};for example
            var baseResponseStaticsResult = await GetLitigationStatisticsReformerReport(filter);
            var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetListOfLitigationReformerReportResult>("[dbo].[GetListOfLitigationReformerReport]", inputParms);
            if (baseResponseStaticsResult.IsDone && resultList.IsDone)
            {

                dynamic obj = new ExpandoObject();
                obj.LitigationStatisticsReformer = (baseResponseStaticsResult as DataResponse<List<GetLitigationStatisticsMainReportResult>>)?.Data;
                obj.LitigationListReformer = (resultList as DataResponse<List<GetListOfLitigationReformerReportResult>>)?.Data;
                return new DataResponse<object>()
                {
                    IsDone = true,
                    Data = obj
                };
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = string.Join(", ",
                            new[] { resultList?.Message, baseResponseStaticsResult?.Message }
                            .Where(msg => !string.IsNullOrEmpty(msg)))

            };
        }

        public async Task<BaseResponse> GetLitigationStatisticsReformerReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsReformerReport]", inputParms);
            return result;
        }

        public async Task<BaseResponse> GetListOfLitigationArbitrationReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            //inputParms.Add("ArbitrationNames", filter.ArbitrationNames ?? Convert.DBNull);
            //var out=new Dictionary(){{"id","int"}};for example
            var baseResponseStaticsResult = await GetLitigationStatisticsArbitrationReport(filter);
            var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetListOfLitigationArbitrationReportResult>("[dbo].[GetListOfLitigationArbitrationReport]", inputParms);
            if (baseResponseStaticsResult.IsDone && resultList.IsDone)
            {

                dynamic obj = new ExpandoObject();
                obj.LitigationStatisticsArbitration = (baseResponseStaticsResult as DataResponse<List<GetLitigationStatisticsMainReportResult>>)?.Data;
                obj.LitigationListArbitration = (resultList as DataResponse<List<GetListOfLitigationArbitrationReportResult>>)?.Data;
                return new DataResponse<object>()
                {
                    IsDone = true,
                    Data = obj
                };
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = string.Join(", ",
                            new[] { resultList?.Message, baseResponseStaticsResult?.Message }
                            .Where(msg => !string.IsNullOrEmpty(msg)))

            };
        }
        public async Task<BaseResponse> GetLitigationStatisticsArbitrationReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsArbitrationReport]", inputParms);
            return result;
        }
        public async Task<BaseResponse> GetListOfLitigationGeneralReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            inputParms.Add("LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds ?? Convert.DBNull);
            //var out=new Dictionary(){{"id","int"}};for example
            var baseResponseStaticsResult = await GetLitigationStatisticsGeneralReport(filter);
            var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetListOfLitigationGeneralReportResult>("[dbo].[GetListOfLitigationGeneralReport]", inputParms);
            if (baseResponseStaticsResult.IsDone && resultList.IsDone)
            {

                dynamic obj = new ExpandoObject();
                obj.LitigationStatisticsGeneral = (baseResponseStaticsResult as DataResponse<List<GetLitigationStatisticsMainReportResult>>)?.Data;
                obj.LitigationListGeneral = (resultList as DataResponse<List<GetListOfLitigationGeneralReportResult>>)?.Data;
                return new DataResponse<object>()
                {
                    IsDone = true,
                    Data = obj
                };
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = string.Join(", ",
                            new[] { resultList?.Message, baseResponseStaticsResult?.Message }
                            .Where(msg => !string.IsNullOrEmpty(msg)))

            };
        }
        public async Task<BaseResponse> GetLitigationStatisticsGeneralReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsGeneralReport]", inputParms);
            return result;
        }

        public async Task<BaseResponse> GetLitigationStatisticsForAllStages(GenericReportsFilter filter)
        {
            try
            {
                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;
                //var ss = DateTime.ParseExact("2025-02-25", "yyyy-MM-dd", provider);

                var inputParms = new Dictionary<string, object>()
            {
                { "FromDate", filter.FromDate != null ? DateTime.ParseExact( filter.FromDate.ToString(),  "yyyy-MM-dd", provider ) : Convert.DBNull},
                { "ToDate", filter.ToDate  != null ? DateTime.ParseExact( filter.ToDate.ToString(),  "yyyy-MM-dd", provider) :Convert.DBNull }
            };

                var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsForAllStagesReportResult>("[dbo].[GetLitigationStatisticsForAllStages]", inputParms);
                if (resultList.IsDone)
                {

                    dynamic obj = new ExpandoObject();
                    obj.LitigationStatisticsForAllStages = (resultList as DataResponse<List<GetLitigationStatisticsForAllStagesReportResult>>)?.Data;
                    return new DataResponse<object>()
                    {
                        IsDone = true,
                        Data = obj
                    };
                }
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = string.Join(", ",
                                new[] { resultList?.Message }
                                .Where(msg => !string.IsNullOrEmpty(msg)))

                };

            }
            catch (Exception)
            {

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> GetLitigationStatisticsDetailsForAllStages(GenericReportsFilter filter)
        {
            try
            {
                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;

                var inputParms = new Dictionary<string, object>()
            {
                { "FromDate", filter.FromDate != null ? DateTime.ParseExact( filter.FromDate.ToString(),  "yyyy-MM-dd", provider ).ToLocalTime() : Convert.DBNull},
                { "ToDate", filter.ToDate  != null ? DateTime.ParseExact( filter.ToDate.ToString(),  "yyyy-MM-dd", provider).ToLocalTime() :Convert.DBNull }
            };

                var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsDetailsForAllStagesReportResult>("[dbo].[GetLitigationStatisticsDetailsForAllStages]", inputParms);
                if (resultList.IsDone)
                {

                    dynamic obj = new ExpandoObject();
                    obj.LitigationStatisticsForAllStages = (resultList as DataResponse<List<GetLitigationStatisticsDetailsForAllStagesReportResult>>)?.Data;
                    return new DataResponse<object>()
                    {
                        IsDone = true,
                        Data = obj
                    };
                }
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = string.Join(", ",
                                new[] { resultList?.Message }
                                .Where(msg => !string.IsNullOrEmpty(msg)))

                };

            }
            catch (Exception)
            {

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }
        }
        public async Task<BaseResponse> GetLitigationStatisticsForAllStagesProc(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsForAllStagesReportResult>("[dbo].[GetLitigationStatisticsForAllStages]", inputParms);
            return result;
        }

        #region helpers

        private Dictionary<string, object> BuildMainFilterParam(GenericReportsFilter filter)
        {
            return new Dictionary<string, object>()
            {
                        { "RequestCode", filter.RequestCode ?? (object)DBNull.Value },
                        { "FromDate", filter.FromDate ?? (object)DBNull.Value },
                        { "ToDate", filter.ToDate ?? (object)DBNull.Value },
                        { "AreaIds", filter.AreaIds ?? (object)DBNull.Value },
                        { "CitiesIds", filter.CitiesIds ?? (object)DBNull.Value },
                        { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds ?? (object)DBNull.Value },
                        { "LitigationRequestStatusIds", filter.LitigationRequestStatusIds ?? (object)DBNull.Value },
                        { "LitigationRequestAmountIds", filter.LitigationRequestAmountId != null  ? filter.LitigationRequestAmountId :(object) DBNull.Value },
                        { "EmployeeIds", filter.EmployeeIds ?? (object)DBNull.Value },
                        { "ReformerIds", filter.ReformerIds ?? (object)DBNull.Value },
                        { "ArbitratorIds", filter.ArbitratorIds ?? (object)DBNull.Value },
                        { "ArbitratorChairmanIds", filter.ArbitratorChairmanIds ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }

        private Dictionary<string, object> BuildReviewFilterParam(GenericReportsFilter filter)
        {
            var ss = filter.FromDate ?? (object)DBNull.Value;
            return new Dictionary<string, object>()
           {
               { "LitigationRequestStageId", "1"}, //for review stage
               { "FromDate", filter.FromDate ?? (object)DBNull.Value },
               { "ToDate", filter.ToDate ?? (object)DBNull.Value },
               { "AreaIds", filter.AreaIds != null && filter.AreaIds.Any() ? string.Join(",", filter.AreaIds) : (object)DBNull.Value },
               { "CitiesIds", filter.CitiesIds != null && filter.CitiesIds.Any()  ? string.Join(",", filter.CitiesIds) : (object)DBNull.Value},
               { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds != null && filter.LitigationRequestCategoryIds.Any() ? string.Join(",", filter.LitigationRequestCategoryIds) :(object) DBNull.Value },
               { "LitigationRequestAmountId", filter.LitigationRequestAmountId != null  ? filter.LitigationRequestAmountId :(object) DBNull.Value },
               { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
           };
        }

        private Dictionary<string, object> BuildListOfLitigationReviewReport(GenericReportsFilter filter)
        {
            var ss = filter.FromDate ?? (object)DBNull.Value;
            return new Dictionary<string, object>()
           {
               { "RequestCode", filter.RequestCode ?? (object)DBNull.Value },
               { "FromDate", filter.FromDate ?? (object)DBNull.Value },
               { "ToDate", filter.ToDate ?? (object)DBNull.Value },
               { "AreaIds", filter.AreaIds != null && filter.AreaIds.Any() ? string.Join(",", filter.AreaIds) : (object)DBNull.Value },
               { "CitiesIds", filter.CitiesIds != null && filter.CitiesIds.Any()  ? string.Join(",", filter.CitiesIds) : (object)DBNull.Value},
               { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds != null && filter.LitigationRequestCategoryIds.Any() ? string.Join(",", filter.LitigationRequestCategoryIds) :(object) DBNull.Value },
               { "LitigationRequestStatusIds", filter.LitigationRequestStatusIds != null && filter.LitigationRequestStatusIds.Any() ? string.Join(",", filter.LitigationRequestStatusIds) :(object) DBNull.Value },
               { "LitigationRequestAmountId", filter.LitigationRequestAmountId != null  ? filter.LitigationRequestAmountId :(object) DBNull.Value },
               { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
               { "SortDirection", filter.SortDirection ?? (object)DBNull.Value },
               { "Skip", filter.Skip ?? (object)DBNull.Value },
               { "Take", filter.Take ?? (object)DBNull.Value },

           };
        }
        #endregion
    }
}
