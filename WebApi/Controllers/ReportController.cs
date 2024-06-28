
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost("sales-summary")]
        public async Task<IActionResult> GetSalesSummary(DateTime startDate, DateTime endDate)
        {
            var data = await _reportService.SalesReportSummary(startDate, endDate);
            return Ok(new
            {
                message = $"Revenue from date {startDate.Date} - {endDate.Date}",
                data
            }) ;
        }



    }
}
