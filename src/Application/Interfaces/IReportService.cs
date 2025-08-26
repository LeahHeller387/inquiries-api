using Inquiries.Api.Application.DTOs.Reports;

namespace Inquiries.Api.Application.Interfaces;

public interface IReportService
{
    Task<IEnumerable<MonthlyReportItemDto>> GetMonthlyAsync(int? year, int? month, CancellationToken ct = default);
}
