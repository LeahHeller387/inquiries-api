using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using Inquiries.Api.Controllers;
using Inquiries.Api.Application.Interfaces;
using Inquiries.Api.Application.DTOs.Reports;

namespace Inquiries.Tests.Controllers;

public class ReportsControllerTests
{
    [Fact]
    public async Task Monthly_ReturnsOk_WithData()
    {
        var svc = new Mock<IReportService>();
        svc.Setup(s => s.GetMonthlyAsync(2025, 8, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<MonthlyReportItemDto> {
               new MonthlyReportItemDto { DepartmentId = 1, DepartmentName = "כללי", CurrentMonthCount = 5, PrevMonthCount = 3, SameMonthLastYearCount = 2 }
           });

        var controller = new ReportsController(svc.Object);
        var result = await controller.GetMonthly(2025, 8, CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        (ok!.Value as IEnumerable<MonthlyReportItemDto>).Should().NotBeNull();
    }
}
