using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Reports;
using REAC.Application.Contracts.IAppServices.Reports;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Security)]
    [AllowAnonymous]

    public class ReportsController : ControllerBase
    {
        private readonly IReportService _IReportService;

        public ReportsController(IReportService iReportService)
        {
            _IReportService = iReportService;
        }

        

        [HttpPost(nameof(GetLitigationStatisticsReviewReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsReviewReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsReviewReport(filter);
            return Ok(response);

        }

        [HttpPost(nameof(GetLitigationStatisticsReconciliationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsReconciliationReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsReconciliationReport(filter);
            return Ok(response);

        }

        [HttpPost(nameof(GetLitigationStatisticsArbitrationsReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsArbitrationsReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsArbitrationsReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetLitigationStatisticsForAdminReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsForAdminReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsForAdminReport(filter);
            return Ok(response);

        }
        


        [HttpPost(nameof(GetLitigationStatisticsReformerReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsReformerReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsReformerReport(filter);
            return Ok(response);

        }
        
        [HttpGet(nameof(GetLitigationStatisticsArbitrationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsArbitrationReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsArbitrationReport(filter);
            return Ok(response);

        }
        
        [HttpGet(nameof(GetLitigationStatisticsGeneralReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsGeneralReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsGeneralReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetListOfLitigationReviewReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationReviewReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationReviewReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetListOfLitigationReformerReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationReformerReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationReformerReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetListOfLitigationArbitrationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationArbitrationReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationArbitrationReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetListOfLitigationForAdminReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationForAdminReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationForAdminReport(filter);
            return Ok(response);

        }
        
        [HttpGet(nameof(GetListOfLitigationGeneralReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationGeneralReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationGeneralReport(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetLitigationStatisticsForAllStages))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsForAllStages([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsForAllStages(filter);
            return Ok(response);

        }
        
        [HttpPost(nameof(GetLitigationStatisticsDetailsForAllStages))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsDetailsForAllStages([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsDetailsForAllStages(filter);
            return Ok(response);

        }

        [HttpPost(nameof(GetReviewersReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetReviewersReport([FromBody] GenericEmployeeReportsFilter filter)
        {

            var response = await _IReportService.GetReviewersReport(filter);
            return Ok(response);

        }

        [HttpPost(nameof(GetReformersReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetReformersReport([FromBody] GenericEmployeeReportsFilter filter)
        {

            var response = await _IReportService.GetReformersReport(filter);
            return Ok(response);

        }

        [HttpPost(nameof(GetArbitratorsReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetArbitratorsReport([FromBody] GenericEmployeeReportsFilter filter)
        {

            var response = await _IReportService.GetArbitratorsReport(filter);
            return Ok(response);

        }
    }
}
