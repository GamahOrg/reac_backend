using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reports;
using REAC.Application.Contracts.IAppServices.Reports;
using REAC.EntityFrameworkCore.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.AppServices.Reports
{
    public class ReportService:IReportService
    {
        private IEntityRepository _entityRepository;
        public ReportService(IEntityRepository repository)
        {
            _entityRepository = repository;
        }

        public async Task<BaseResponse> GetListOfLitigationReviewReport(GetListOfLitigationReviewReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            inputParms.Add("EmployeeNames", filter.EmployeeNames?? Convert.DBNull);
            //var out=new Dictionary(){{"id","int"}};for example
            var baseResponseStaticsResult = await GetLitigationStatisticsReviewReport(filter);
            var (resultList, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetListOfLitigationReviewReportResult>("[dbo].[GetListOfLitigationReviewReport]", inputParms);
            if(baseResponseStaticsResult.IsDone && resultList.IsDone)
            {
                
                dynamic obj = new ExpandoObject();
                obj.LitigationStatisticsReview = (baseResponseStaticsResult as DataResponse<List<GetLitigationStatisticsMainReportResult>>)?.Data;              
                    obj.LitigationListReview = (resultList as DataResponse<List<GetListOfLitigationReviewReportResult>>)?.Data;
                return new DataResponse<object>()
                {
                    IsDone = true,
                    Data = obj
                };
            }
            return new BaseResponse() { 
                IsDone=false,
                Message = string.Join(", ",
                            new[] { resultList?.Message, baseResponseStaticsResult?.Message }
                            .Where(msg => !string.IsNullOrEmpty(msg)))

            };
        }

        public async Task<BaseResponse> GetLitigationStatisticsReviewReport(GetLitigationStatisticsMainReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsReviewReport]",inputParms);
            return result;
        }
        public async Task<BaseResponse> GetListOfLitigationReformerReport(GetListOfLitigationReformerReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            inputParms.Add("ReformerNames", filter.ReformerNames ?? Convert.DBNull);
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

        public async Task<BaseResponse> GetLitigationStatisticsReformerReport(GetLitigationStatisticsMainReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsReformerReport]", inputParms);
            return result;
        }

        public async Task<BaseResponse> GetListOfLitigationArbitrationReport(GetListOfLitigationArbitrationReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            inputParms.Add("ArbitrationNames", filter.ArbitrationNames ?? Convert.DBNull);
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
        public async Task<BaseResponse> GetLitigationStatisticsArbitrationReport(GetLitigationStatisticsMainReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsArbitrationReport]", inputParms);
            return result;
        }
        public async Task<BaseResponse> GetListOfLitigationGeneralReport(GetListOfLitigationGeneralReportFilter filter)
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
        public async Task<BaseResponse> GetLitigationStatisticsGeneralReport(GetLitigationStatisticsMainReportFilter filter)
        {
            var inputParms = BuildMainFilterParam(filter);
            var (result, outPut) = await _entityRepository.ExecuteStoredProcedureAsync<GetLitigationStatisticsMainReportResult>("[dbo].[GetLitigationStatisticsGeneralReport]", inputParms);
            return result;
        }
        private Dictionary<string, object> BuildMainFilterParam(GetLitigationStatisticsMainReportFilter filter)
        {
            return new Dictionary<string, object>()
            {
                { "EmployeeId", filter.EmployeeId ?? Convert.DBNull },
                { "LitigationRequestStageId", filter.LitigationRequestStageId ?? Convert.DBNull },
                { "FromDate", filter.FromDate ?? Convert.DBNull},
                { "ToDate", filter.ToDate ?? Convert.DBNull },
                { "AreaIds", filter.AreaIds ?? Convert.DBNull },
                { "CitiesIds", filter.CitiesIds ?? Convert.DBNull },
                { "LitigationRequestCategoryIds", filter.LitigationRequestCategoryIds ?? Convert.DBNull},
                { "FromFees", filter.FromFees ?? Convert.DBNull },
                { "ToFees", filter.ToFees ?? Convert.DBNull }
            };

        }
    }
}
