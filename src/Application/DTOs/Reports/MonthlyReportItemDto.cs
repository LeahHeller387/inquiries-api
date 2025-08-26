namespace Inquiries.Api.Application.DTOs.Reports;

public class MonthlyReportItemDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = default!;
    public int CurrentMonthCount { get; set; }
    public int PrevMonthCount { get; set; }
    public int SameMonthLastYearCount { get; set; }
}
