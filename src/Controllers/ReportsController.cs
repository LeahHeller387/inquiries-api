using Inquiries.Api.Application.Interfaces;
using Inquiries.Api.Application.DTOs.Reports;

using Microsoft.AspNetCore.Mvc;

namespace Inquiries.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpGet("monthly")]
    public async Task<ActionResult<IEnumerable<MonthlyReportItemDto>>> GetMonthly([FromQuery] int? year, [FromQuery] int? month, CancellationToken ct)
    {
        var data = await _service.GetMonthlyAsync(year, month, ct);
        return Ok(data);
    }
}
