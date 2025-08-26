using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Inquiries.Api.Application.DTOs.Reports;
using Inquiries.Api.Application.Interfaces;

namespace Inquiries.Api.Application.Services;

public class ReportServiceDb : IReportService
{
    private readonly IConfiguration _cfg;
    public ReportServiceDb(IConfiguration cfg) => _cfg = cfg;

    public async Task<IEnumerable<MonthlyReportItemDto>> GetMonthlyAsync(int? year, int? month, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var y = year ?? now.Year;
        var m = month ?? now.Month;

        var cs = _cfg.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'Default'.");

        await using var conn = new SqlConnection(cs);
        return await conn.QueryAsync<MonthlyReportItemDto>(
            "dbo.GetMonthlyInquiryReport",
            new { Year = y, Month = m },
            commandType: CommandType.StoredProcedure);
    }
}
