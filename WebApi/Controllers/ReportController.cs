
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

        [HttpPost("visited")]
        public async Task<IActionResult> GetReportVisited([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int top = 5)
        {
            var data = await _reportService.ReportVisited(startDate, endDate);
            return Ok(new
            {
                message = $"Report visited from date {startDate.Date} - {endDate.Date}",
                data
            });
        }

        [HttpPost("top-view-product")]
        public async Task<IActionResult> GetReportTopViewProduct([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int top = 5)
        {
            var data = await _reportService.ReportProductView(startDate, endDate, top);
            return Ok(new
            {
                message = $"Report top ${top} view product from date {startDate.Date} - {endDate.Date}",
                data
            });
        }


    }
}
