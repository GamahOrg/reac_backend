using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.Dtos.Reports;
using REAC.Application.Contracts.IAppServices.Reports;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Reports)]
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
        public async Task<ActionResult> GetLitigationStatisticsReviewReport([FromBody]GetLitigationStatisticsMainReportFilter filter)
        {           
          
                var response = await _IReportService.GetLitigationStatisticsReviewReport(filter);
                return Ok(response);           

        }
        [HttpPost(nameof(GetLitigationStatisticsReformerReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsReformerReport([FromBody] GetLitigationStatisticsMainReportFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsReformerReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetLitigationStatisticsArbitrationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsArbitrationReport([FromBody] GetLitigationStatisticsMainReportFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsArbitrationReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetLitigationStatisticsGeneralReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetLitigationStatisticsGeneralReport([FromBody] GetLitigationStatisticsMainReportFilter filter)
        {

            var response = await _IReportService.GetLitigationStatisticsGeneralReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetListOfLitigationReviewReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationReviewReport([FromBody] GetListOfLitigationReviewReportFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationReviewReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetListOfLitigationReformerReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationReformerReport([FromBody] GetListOfLitigationReformerReportFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationReformerReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetListOfLitigationArbitrationReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationArbitrationReport([FromBody] GetListOfLitigationArbitrationReportFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationArbitrationReport(filter);
            return Ok(response);

        }
        [HttpGet(nameof(GetListOfLitigationGeneralReport))]
        [AllowAnonymous]
        public async Task<ActionResult> GetListOfLitigationGeneralReport([FromBody] GetListOfLitigationGeneralReportFilter filter)
        {

            var response = await _IReportService.GetListOfLitigationGeneralReport(filter);
            return Ok(response);

        }
    }
}
