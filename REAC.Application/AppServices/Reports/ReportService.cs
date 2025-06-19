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

        #region List Of Litigation Reports

        public async Task<BaseResponse> GetListOfLitigationReviewReport(GenericReportsFilter filter)
        {
            if (filter.IsAdmin != true)
            {
                Guid employee = Guid.Empty;
                _jwtReader.GetReferenceId(ref employee);
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            var inputParms = BuildListOfLitigationReviewParam(filter);

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
        public async Task<BaseResponse> GetListOfLitigationReformerReport(GenericReportsFilter filter)
        {
            if (filter.IsAdmin != true)
            {
                Guid reformer = Guid.Empty;
                _jwtReader.GetReferenceId(ref reformer);
                filter.ReformersIds = new List<string>() { reformer.ToString() };
            }
            var inputParms = BuildListOfLitigationReformerParam(filter);

            var resultList = new List<GetListOfLitigationReformerReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetListOfLitigationReformerReport]", conn))
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
                        var item = new GetListOfLitigationReformerReportResult
                        {
                            ReformerName = reader["ReformerName"] as string,
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

            return new DataResponse<PagedResult<GetListOfLitigationReformerReportResult>>
            {
                Data =
                  new PagedResult<GetListOfLitigationReformerReportResult>
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
        
        public async Task<BaseResponse> GetListOfLitigationArbitrationReport(GenericReportsFilter filter)
        {
            if (filter.IsAdmin != true)
            {
                Guid arbitrator = Guid.Empty;
                _jwtReader.GetReferenceId(ref arbitrator);
                filter.ArbitratorsIds = new List<string>() { arbitrator.ToString() };
            }
            var inputParms = BuildListOfLitigationArbitrationParam(filter);

            var resultList = new List<GetListOfLitigationArbitrationReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetListOfLitigationArbitrationReport]", conn))
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
                        var item = new GetListOfLitigationArbitrationReportResult
                        {
                            ArbitratorChairmanName = reader["ArbitratorChairmanName"] as string,
                            ArbitratorsFullNames = reader["ArbitratorsFullNames"] as string,
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

            return new DataResponse<PagedResult<GetListOfLitigationArbitrationReportResult>>
            {
                Data =
                  new PagedResult<GetListOfLitigationArbitrationReportResult>
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
        
        public async Task<BaseResponse> GetListOfLitigationForAdminReport(GenericReportsFilter filter)
        {
            
            var inputParms = BuildListOfLitigationForAdminParam(filter);

            var resultList = new List<GetListOfLitigationForAdminReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetListOfLitigationForAdminReport]", conn))
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
                        var item = new GetListOfLitigationForAdminReportResult
                        {
                            ArbitratorChairmanName = reader["ArbitratorChairmanName"] as string,
                            ArbitratorsFullNames = reader["ArbitratorsFullNames"] as string,
                            ReformerName = reader["ReformerName"] as string,
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

            return new DataResponse<PagedResult<GetListOfLitigationForAdminReportResult>>
            {
                Data =
                  new PagedResult<GetListOfLitigationForAdminReportResult>
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

        #endregion

        #region Litigation Statistics Reports

        public async Task<BaseResponse> GetLitigationStatisticsReconciliationReport(GenericReportsFilter filter)
        {
            Guid employee = Guid.Empty;
            _jwtReader.GetReferenceId(ref employee);

            if (filter.IsAdmin != true)
            {
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            //"2025-05-21"
            filter.LitigationRequestStageId = 2;

            var inputParms = BuildFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsReportResult>("[dbo].[GetLitigationStatisticsReport]", inputParms);

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

        public async Task<BaseResponse> GetLitigationStatisticsArbitrationsReport(GenericReportsFilter filter)
        {
            Guid employee = Guid.Empty;
            _jwtReader.GetReferenceId(ref employee);

            if (filter.IsAdmin != true)
            {
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            //"2025-05-21"
            filter.LitigationRequestStageId = 3;
            var inputParms = BuildFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsReportResult>("[dbo].[GetLitigationStatisticsReport]", inputParms);

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

        public async Task<BaseResponse> GetLitigationStatisticsReviewReport(GenericReportsFilter filter)
        {
            Guid employee = Guid.Empty;
            _jwtReader.GetReferenceId(ref employee);

            if (filter.IsAdmin != true)
            {
                filter.EmployeeIds = new List<string>() { employee.ToString() };
            }
            //"2025-05-21"
            filter.LitigationRequestStageId = 1;

            var inputParms = BuildFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsReportResult>("[dbo].[GetLitigationStatisticsReport]", inputParms);

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
        
        public async Task<BaseResponse> GetLitigationStatisticsForAdminReport(GenericReportsFilter filter)
        {
            var inputParms = BuildLitigationStatisticsForAdminParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsForAdminReportResult>("[dbo].[GetLitigationStatisticsForAdminReport]", inputParms);

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


        #endregion


        

        public async Task<BaseResponse> GetLitigationStatisticsReformerReport(GenericReportsFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsReformerReport]", inputParms);
            return result;
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

        public async Task<BaseResponse> GetReviewDashbourdReport(GenericReportsFilter filter)
        {
            try
            {
                if (filter.IsAdmin != true)
                {
                    Guid employee = Guid.Empty;
                    _jwtReader.GetReferenceId(ref employee);

                    filter.EmployeeIds = new List<string>() { employee.ToString() };
                }
                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;
                //var ss = DateTime.ParseExact("2025-02-25", "yyyy-MM-dd", provider);

                var inputParms = new Dictionary<string, object>()
            {
                { "FromDate", filter.FromDate != null ? DateTime.ParseExact( filter.FromDate.ToString(),  "yyyy-MM-dd", provider ) : Convert.DBNull},
                { "ToDate", filter.ToDate  != null ? DateTime.ParseExact( filter.ToDate.ToString(),  "yyyy-MM-dd", provider) :Convert.DBNull },
                { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
            };
                var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetReviwersDashbourdResult>("[dbo].[GetReviewDashbourdReport]", inputParms);
                if (resultList.IsDone)
                {
                    return resultList;
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

        public async Task<BaseResponse> GetReconciliationDashbourdReport(GenericReportsFilter filter)
        {
            try
            {
                if (filter.IsAdmin != true)
                {
                    Guid employee = Guid.Empty;
                    _jwtReader.GetReferenceId(ref employee);

                    filter.EmployeeIds = new List<string>() { employee.ToString() };
                }
                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;
                //var ss = DateTime.ParseExact("2025-02-25", "yyyy-MM-dd", provider);

                var inputParms = new Dictionary<string, object>()
            {
                { "FromDate", filter.FromDate != null ? DateTime.ParseExact( filter.FromDate.ToString(),  "yyyy-MM-dd", provider ) : Convert.DBNull},
                { "ToDate", filter.ToDate  != null ? DateTime.ParseExact( filter.ToDate.ToString(),  "yyyy-MM-dd", provider) :Convert.DBNull },
                { "ReformersId", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
            };



                var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetReformerDashbourdResult>("[dbo].[GetReconciliationDashbourdReport]", inputParms);
                if (resultList.IsDone)
                {
                    return resultList;
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

        public async Task<BaseResponse> GetArbitrationDashbourdReport(GenericReportsFilter filter)
        {
            try
            {
                if (filter.IsAdmin != true)
                {
                    Guid employee = Guid.Empty;
                    _jwtReader.GetReferenceId(ref employee);

                    filter.EmployeeIds = new List<string>() { employee.ToString() };
                }
                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;
                //var ss = DateTime.ParseExact("2025-02-25", "yyyy-MM-dd", provider);

                var inputParms = new Dictionary<string, object>()
            {
                { "FromDate", filter.FromDate != null ? DateTime.ParseExact( filter.FromDate.ToString(),  "yyyy-MM-dd", provider ) : Convert.DBNull},
                { "ToDate", filter.ToDate  != null ? DateTime.ParseExact( filter.ToDate.ToString(),  "yyyy-MM-dd", provider) :Convert.DBNull },
                { "ArbitratorsIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
            };



                var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetArbitratorDashbourdResult>("[dbo].[GetArbitrationDashbourdReport]", inputParms);
                if (resultList.IsDone)
                {
                    return resultList;
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

        #region Reviewer Arbitration Reformer Reports
        public async Task<BaseResponse> GetReviewersReport(GenericEmployeeReportsFilter filter)
        {
            var inputParms = BuildReviewersReportsFilter(filter);

            var resultList = new List<GetReviwersReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetReviewersReport]", conn))
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
                        var item = new GetReviwersReportResult
                        {
                            ReviewerName = reader["ReviewerName"] as string,
                            ReviewerStatus = reader["ReviewerStatus"] as string,
                            TotalLitigationCount = reader["TotalLitigationCount"]!=null? (int) reader["TotalLitigationCount"]:null,
                            NotFinishedCount = reader["NotFinishedCount"] != null ? (int) reader["NotFinishedCount"]:null,
                            FinishedCount = reader["FinishedCount"] != null ? (int)reader["FinishedCount"] : null,
                            TotalReviewDueDateCount = reader["TotalReviewDueDateCount"] != null ? (int)reader["TotalReviewDueDateCount"] : null,
                            NotFinishedRatio = !Convert.IsDBNull(reader["NotFinishedRatio"]) ? (decimal)reader["NotFinishedRatio"] : (decimal?)null,
                            FinishedRatio = !Convert.IsDBNull(reader["FinishedRatio"]) ? (decimal)reader["FinishedRatio"] : (decimal?)null,
                            TotalReviewDueDateRatio = !Convert.IsDBNull(reader["TotalReviewDueDateRatio"]) ? (decimal)reader["TotalReviewDueDateRatio"] : (decimal?)null,
                            TotalCount = reader["TotalCount"] != DBNull.Value ? (int)reader["TotalCount"] : 0,

                        };
                        resultList.Add(item);
                    }

                    
                }
            }

            totalCount = resultList.Count > 0 ? resultList.First().TotalCount : 0;

            var pageCount = (double)totalCount / filter.Take;

            return new DataResponse<PagedResult<GetReviwersReportResult>>
            {
                Data =
                  new PagedResult<GetReviwersReportResult>
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

        public async Task<BaseResponse> GetReformersReport(GenericEmployeeReportsFilter filter)
        {
            var inputParms = BuildReformersReportsFilter(filter);

            var resultList = new List<GetReformersReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetReformersReport]", conn))
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
                        var item = new GetReformersReportResult
                        {
                            ReformerName = reader["ReformerName"] as string,
                            ReformerStatus = reader["ReformerStatus"] as string,
                            ExperienceYears = reader["ExperienceYears"] != null ? (int)reader["ExperienceYears"] : null,
                            TotalLitigationCount = reader["TotalLitigationCount"] != null ? (int)reader["TotalLitigationCount"] : null,
                            NotFinishedCount = reader["NotFinishedCount"] != null ? (int)reader["NotFinishedCount"] : null,
                            FinishedCount = reader["FinishedCount"] != null ? (int)reader["FinishedCount"] : null,
                            DueDateCount = reader["DueDateCount"] != null ? (int)reader["DueDateCount"] : null,
                            NotFinishedRatio = !Convert.IsDBNull(reader["NotFinishedRatio"])? (decimal)reader["NotFinishedRatio"]: (decimal?)null,
                            FinishedRatio = !Convert.IsDBNull(reader["FinishedRatio"]) ? (decimal)reader["FinishedRatio"] : (decimal?)null,
                            DueDateRatio = !Convert.IsDBNull(reader["DueDateRatio"]) ? (decimal)reader["DueDateRatio"] : (decimal?)null,
                            TotalCount = reader["TotalCount"] != DBNull.Value ? (int)reader["TotalCount"] : 0,

                        };
                        resultList.Add(item);
                    }

                   
                }
            }
            totalCount = resultList.Count > 0 ? resultList.First().TotalCount : 0;

            var pageCount = (double)totalCount / filter.Take;

            return new DataResponse<PagedResult<GetReformersReportResult>>
            {
                Data =
                  new PagedResult<GetReformersReportResult>
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

        public async Task<BaseResponse> GetArbitratorsReport(GenericEmployeeReportsFilter filter)
        {
            var inputParms = BuildArbitratorReportsFilter(filter);

            var resultList = new List<GetArbitratorsReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetArbitratorsReport]", conn))
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
                        var item = new GetArbitratorsReportResult
                        {
                            ArbitratorName = reader["ArbitratorName"] as string,
                            ArbitratorStatus = reader["ArbitratorStatus"] as string,
                            ExperienceYears = reader["ExperienceYears"] != null ? (int)reader["ExperienceYears"] : null,
                            TotalLitigationCount = reader["TotalLitigationCount"] != null ? (int)reader["TotalLitigationCount"] : null,
                            NotFinishedCount = reader["NotFinishedCount"] != null ? (int)reader["NotFinishedCount"] : null,
                            FinishedCount = reader["FinishedCount"] != null ? (int)reader["FinishedCount"] : null,
                            DueDateCount = reader["DueDateCount"] != null ? (int)reader["DueDateCount"] : null,
                            NotFinishedRatio = !Convert.IsDBNull(reader["NotFinishedRatio"]) ? (decimal)reader["NotFinishedRatio"] : (decimal?)null,
                            FinishedRatio = !Convert.IsDBNull(reader["FinishedRatio"]) ? (decimal)reader["FinishedRatio"] : (decimal?)null,
                            DueDateRatio = !Convert.IsDBNull(reader["DueDateRatio"]) ? (decimal)reader["DueDateRatio"] : (decimal?)null,
                            TotalCount = reader["TotalCount"] != DBNull.Value ? (int)reader["TotalCount"] : 0,

                        };
                        resultList.Add(item);
                    }

                   
                }
            }

            totalCount = resultList.Count > 0 ? resultList.First().TotalCount : 0;

            var pageCount = (double)totalCount / filter.Take;

            return new DataResponse<PagedResult<GetArbitratorsReportResult>>
            {
                Data =
                  new PagedResult<GetArbitratorsReportResult>
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

        public async Task<BaseResponse> GetReviewrsReformersArbitratorsReport(GenericEmployeeReportsFilter filter)
        {
            var inputParms = BuildReviewerReformerArbitratorReportsFilter(filter);

            var resultList = new List<GetReviewersReformersArbitratorsReportResult>();
            int totalCount = 0;

            // Adjust this to get your connection string

            using (var conn = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")))
            using (var cmd = new SqlCommand("[dbo].[GetReviewersReformersArbitratorsReport]", conn))
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
                        var item = new GetReviewersReformersArbitratorsReportResult
                        {
                            PersonnelName = reader["PersonnelName"] as string,
                            PersonnelType = reader["PersonnelType"] as string,
                            PersonnelTypeId = reader["PersonnelTypeId"] != null ? (int)reader["PersonnelTypeId"] : null,
                            PersonnelStatus = reader["PersonnelStatus"] as string,
                            ExperienceYears = reader["ExperienceYears"] != null ? (int)reader["ExperienceYears"] : null,

                            TotalLitigationCount = reader["TotalLitigationCount"] != null ? (int)reader["TotalLitigationCount"] : null,
                            NotFinishedCount = reader["NotFinishedCount"] != null ? (int)reader["NotFinishedCount"] : null,
                            FinishedCount = reader["FinishedCount"] != null ? (int)reader["FinishedCount"] : null,
                            DueDateCount = reader["DueDateCount"] != null ? (int)reader["DueDateCount"] : null,
                            
                            ChairmanTotalCount = reader["ChairmanTotalCount"] != null ? (int)reader["ChairmanTotalCount"] : null,
                            ChairmanNotFinishedCount = reader["ChairmanNotFinishedCount"] != null ? (int)reader["ChairmanNotFinishedCount"] : null,
                            ChairmanFinishedCount = reader["ChairmanFinishedCount"] != null ? (int)reader["ChairmanFinishedCount"] : null,
                            ChairmanDueDateCount = reader["ChairmanDueDateCount"] != null ? (int)reader["ChairmanDueDateCount"] : null,
                            
                            ArbitratorTotalCount = reader["ArbitratorTotalCount"] != null ? (int)reader["ArbitratorTotalCount"] : null,
                            ArbitratorNotFinishedCount = reader["ArbitratorNotFinishedCount"] != null ? (int)reader["ArbitratorNotFinishedCount"] : null,
                            ArbitratorFinishedCount = reader["ArbitratorFinishedCount"] != null ? (int)reader["ArbitratorFinishedCount"] : null,
                            ArbitratorDueDateCount = reader["ArbitratorDueDateCount"] != null ? (int)reader["ArbitratorDueDateCount"] : null,

                            NotFinishedRatio = reader["NotFinishedRatio"] != DBNull.Value ? (decimal)reader["NotFinishedRatio"] : null,
                            FinishedRatio = reader["FinishedRatio"] != DBNull.Value ? (decimal)reader["FinishedRatio"] : null,
                            DueDateRatio = reader["DueDateRatio"] != DBNull.Value ? (decimal)reader["DueDateRatio"] : null,

                            TotalCount = reader["TotalCount"] != DBNull.Value ? (int)reader["TotalCount"] : 0,
                        };
                        resultList.Add(item);
                    }

                    
                }
            }
            totalCount = resultList.Count > 0 ? resultList.First().TotalCount : 0;
            var pageCount = (double)totalCount / filter.Take;

            return new DataResponse<PagedResult<GetReviewersReformersArbitratorsReportResult>>
            {
                Data =
                  new PagedResult<GetReviewersReformersArbitratorsReportResult>
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
        public async Task<BaseResponse> GetReviewrsReformersArbitratorsStatisticsReport()
        {

            var result = await _entityRepository.ExecuteParameterlessStoredProcedureAsync<GetReviewersReformersArbitratorsStatisticsReportResult>("[dbo].[GetReviewrsReformersArbitratorsStatisticsReport]");

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
        #endregion
        
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
                        { "ReformerIds", filter.ReformersIds ?? (object)DBNull.Value },
                        { "ArbitratorIds", filter.ArbitratorsIds ?? (object)DBNull.Value },
                        { "ArbitratorChairmanIds", filter.ArbitratorChairmanIds ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }

        private Dictionary<string, object> BuildReviewersReportsFilter(GenericEmployeeReportsFilter filter)
        {
            return new Dictionary<string, object>()
            {
                        { "ReviewerName", filter.ReviewerName ?? (object)DBNull.Value },
                        { "ReviewerStatus", filter.ReviewerStatus ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }

        private Dictionary<string, object> BuildReformersReportsFilter(GenericEmployeeReportsFilter filter)
        {
            return new Dictionary<string, object>()
            {
                        { "ReformerName", filter.ReformerName ?? (object)DBNull.Value },
                        { "ReformerStatus", filter.ReformerStatus ?? (object)DBNull.Value },
                        { "ExperienceYears", filter.ExperienceYears ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }

        private Dictionary<string, object> BuildArbitratorReportsFilter(GenericEmployeeReportsFilter filter)
        {
            return new Dictionary<string, object>()
            {
                        { "ArbitratorName", filter.ArbitratorName ?? (object)DBNull.Value },
                        { "ArbitratorStatus", filter.ArbitratorStatus ?? (object)DBNull.Value },
                        { "ExperienceYears", filter.ExperienceYears ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }
        
        private Dictionary<string, object> BuildReviewerReformerArbitratorReportsFilter(GenericEmployeeReportsFilter filter)
        {
            return new Dictionary<string, object>()
            {
                        { "PersonnelTypeId", filter.PersonnelTypeId != null && filter.PersonnelTypeId.Any() ? string.Join(",", filter.PersonnelTypeId) :(object) DBNull.Value },

                        { "PersonnelStatusId", filter.PersonnelStatusId ?? (object)DBNull.Value },
                        { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
                        { "ReformersIds", filter.ReformersIds != null && filter.ReformersIds.Any() ? string.Join(",", filter.ReformersIds) :(object) DBNull.Value },
                        { "ArbitratorsIds", filter.ArbitratorsIds != null && filter.ArbitratorsIds.Any() ? string.Join(",", filter.ArbitratorsIds) :(object) DBNull.Value },
                        { "ArbitratorClassIds", filter.ArbitratorClassIds != null && filter.ArbitratorClassIds.Any() ? string.Join(",", filter.ArbitratorClassIds) :(object) DBNull.Value },
                        
                        { "ExperienceYears", filter.ExperienceYears ?? (object)DBNull.Value },
                        { "Skip", filter.Skip ?? (object)DBNull.Value },
                        { "Take", filter.Take ?? (object)DBNull.Value }
            };

        }
        private Dictionary<string, object> BuildFilterParam(GenericReportsFilter filter)
        {
            return new Dictionary<string, object>()
           {
               { "LitigationRequestStageId", filter.LitigationRequestStageId ?? (object)DBNull.Value}, 
               { "FromDate", filter.FromDate ?? (object)DBNull.Value },
               { "ToDate", filter.ToDate ?? (object)DBNull.Value },
               { "AreaIds", filter.AreaIds != null && filter.AreaIds.Any() ? string.Join(",", filter.AreaIds) : (object)DBNull.Value },
               { "CitiesIds", filter.CitiesIds != null && filter.CitiesIds.Any()  ? string.Join(",", filter.CitiesIds) : (object)DBNull.Value},
               { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds != null && filter.LitigationRequestCategoryIds.Any() ? string.Join(",", filter.LitigationRequestCategoryIds) :(object) DBNull.Value },
               { "LitigationRequestAmountId", filter.LitigationRequestAmountId != null  ? filter.LitigationRequestAmountId :(object) DBNull.Value },
               { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
           };
        }
        
        private Dictionary<string, object> BuildLitigationStatisticsForAdminParam(GenericReportsFilter filter)
        {
            return new Dictionary<string, object>()
           {
               { "FromDate", filter.FromDate ?? (object)DBNull.Value },
               { "ToDate", filter.ToDate ?? (object)DBNull.Value },
               { "AreaIds", filter.AreaIds != null && filter.AreaIds.Any() ? string.Join(",", filter.AreaIds) : (object)DBNull.Value },
               { "CitiesIds", filter.CitiesIds != null && filter.CitiesIds.Any()  ? string.Join(",", filter.CitiesIds) : (object)DBNull.Value},
               { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds != null && filter.LitigationRequestCategoryIds.Any() ? string.Join(",", filter.LitigationRequestCategoryIds) :(object) DBNull.Value },
               { "LitigationRequestStageIds", filter.LitigationRequestStageIds != null && filter.LitigationRequestStageIds.Any() ? string.Join(",", filter.LitigationRequestStageIds) :(object) DBNull.Value },
               { "LitigationRequestAmountId", filter.LitigationRequestAmountId != null  ? filter.LitigationRequestAmountId :(object) DBNull.Value },
               { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
               { "ReformersIds", filter.ReformersIds != null && filter.ReformersIds.Any() ? string.Join(",", filter.ReformersIds) :(object) DBNull.Value },
               { "ArbitratorsIds", filter.ArbitratorsIds != null && filter.ArbitratorsIds.Any() ? string.Join(",", filter.ArbitratorsIds) :(object) DBNull.Value },
           };
        }

        private Dictionary<string, object> BuildListOfLitigationReviewParam(GenericReportsFilter filter)
        {
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
        
        private Dictionary<string, object> BuildListOfLitigationReformerParam(GenericReportsFilter filter)
        {
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
               { "ReformersIds", filter.ReformersIds != null && filter.ReformersIds.Any() ? string.Join(",", filter.ReformersIds) :(object) DBNull.Value },
               { "SortDirection", filter.SortDirection ?? (object)DBNull.Value },
               { "Skip", filter.Skip ?? (object)DBNull.Value },
               { "Take", filter.Take ?? (object)DBNull.Value },

           };
        }
        
        private Dictionary<string, object> BuildListOfLitigationArbitrationParam(GenericReportsFilter filter)
        {
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
               { "ArbitratorsIds", filter.ArbitratorsIds != null && filter.ArbitratorsIds.Any() ? string.Join(",", filter.ArbitratorsIds) :(object) DBNull.Value },
               { "SortDirection", filter.SortDirection ?? (object)DBNull.Value },
               { "Skip", filter.Skip ?? (object)DBNull.Value },
               { "Take", filter.Take ?? (object)DBNull.Value },

           };
        }
        
        private Dictionary<string, object> BuildListOfLitigationForAdminParam(GenericReportsFilter filter)
        {
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
               { "ArbitratorsIds", filter.ArbitratorsIds != null && filter.ArbitratorsIds.Any() ? string.Join(",", filter.ArbitratorsIds) :(object) DBNull.Value },
               { "ReformersIds", filter.ReformersIds != null && filter.ReformersIds.Any() ? string.Join(",", filter.ReformersIds) :(object) DBNull.Value },
               { "EmployeeIds", filter.EmployeeIds != null && filter.EmployeeIds.Any() ? string.Join(",", filter.EmployeeIds) :(object) DBNull.Value },
               { "SortDirection", filter.SortDirection ?? (object)DBNull.Value },
               { "Skip", filter.Skip ?? (object)DBNull.Value },
               { "Take", filter.Take ?? (object)DBNull.Value },

           };
        }
        #endregion
    }
}
