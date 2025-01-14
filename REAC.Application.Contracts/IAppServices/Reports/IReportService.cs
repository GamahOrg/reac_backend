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
        public Task<BaseResponse> GetLitigationStatisticsReviewReport(GetLitigationStatisticsMainReportFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsReformerReport(GetLitigationStatisticsMainReportFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsArbitrationReport(GetLitigationStatisticsMainReportFilter filter);
        public Task<BaseResponse> GetLitigationStatisticsGeneralReport(GetLitigationStatisticsMainReportFilter filter);
        public Task<BaseResponse> GetListOfLitigationReviewReport(GetListOfLitigationReviewReportFilter filter);
        public Task<BaseResponse> GetListOfLitigationReformerReport(GetListOfLitigationReformerReportFilter filter);
        public Task<BaseResponse> GetListOfLitigationArbitrationReport(GetListOfLitigationArbitrationReportFilter filter);
        public Task<BaseResponse> GetListOfLitigationGeneralReport(GetListOfLitigationGeneralReportFilter filter);

    }
}
