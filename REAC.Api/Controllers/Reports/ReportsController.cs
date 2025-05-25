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

        //[HttpPost(nameof(GetLitigationStatisticsReviewReport))]
        //[AllowAnonymous]
        //public async Task<ActionResult> GetLitigationStatisticsReviewReport([FromBody] GenericReportsFilter filter)
        //{

        //    var response = await _IReportService.GetLitigationStatisticsReviewReport(filter);
        //    return Ok(response);

        //}

        [HttpPost(nameof(GetLitigationStatisticsReviewReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsReviewReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsReviewReport(filter);
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
        [HttpGet(nameof(GetListOfLitigationReformerReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationReformerReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationReformerReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetListOfLitigationArbitrationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationArbitrationReport([FromBody] GenericReportsFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationArbitrationReport(filter);
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
    }
}
