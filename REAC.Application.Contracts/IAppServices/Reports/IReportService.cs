using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reports;
using REAC.EntityFrameworkCore.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Reports
{
    public interface IReportService
    {
        public Task<BaseResponse> GetLitigationStatisticsReviewReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsReformerReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsArbitrationReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsForAdminReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsGeneralReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetListOfLitigationReviewReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetListOfLitigationReformerReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetListOfLitigationArbitrationReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetListOfLitigationForAdminReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetListOfLitigationGeneralReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsForAllStages(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsDetailsForAllStages(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsReconciliationReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsArbitrationsReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetReviewersReport(GenericEmployeeReportsFilter filter);
        public Task<BaseResponse> GetReformersReport(GenericEmployeeReportsFilter filter);
        public Task<BaseResponse> GetArbitratorsReport(GenericEmployeeReportsFilter filter);
        public Task<BaseResponse> GetReviewDashbourdReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetReconciliationDashbourdReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetArbitrationDashbourdReport(GenericReportsFilter filter);
        public Task<BaseResponse> GetReviewrsReformersArbitratorsReport(GenericEmployeeReportsFilter filter);
        public Task<BaseResponse> GetReviewrsReformersArbitratorsStatisticsReport();

    }
}
