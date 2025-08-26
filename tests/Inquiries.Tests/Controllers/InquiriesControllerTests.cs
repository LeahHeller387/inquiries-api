using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

using Inquiries.Api.Controllers;
using Inquiries.Api.Application.Interfaces;
using Inquiries.Api.Application.DTOs.Inquiries;

namespace Inquiries.Tests.Controllers;

public class InquiriesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOkWithList()
    {
        var svc = new Mock<IInquiryService>();
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<InquiryDto> {
               new InquiryDto { Id = 1, Name = "א" },
               new InquiryDto { Id = 2, Name = "ב" }
           });

        var controller = new InquiriesController(svc.Object);
        var result = await controller.GetAll(CancellationToken.None);

        var ok = result.Result as OkObjectResult;
        ok.Should().NotBeNull();
        (ok!.Value as IEnumerable<InquiryDto>).Should().NotBeNull();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithId()
    {
        var svc = new Mock<IInquiryService>();
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateInquiryDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(123);

        var controller = new InquiriesController(svc.Object);
        var dto = new CreateInquiryDto { Name = "ג", Phone = "050-0000000", Email = "g@g", DepartmentIds = new(){1} };

        var result = await controller.Create(dto, CancellationToken.None);

        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.RouteValues!["id"].Should().Be(123);
    }
}
