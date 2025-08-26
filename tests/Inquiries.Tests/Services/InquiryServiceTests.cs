using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using System.Linq;

using Inquiries.Api.Application.Services;
using Inquiries.Api.Application.DTOs.Inquiries;
using Inquiries.Api.Infrastructure.Repositories.Interfaces;
using Inquiries.Api.Domain.Entities;

namespace Inquiries.Tests.Services;

public class InquiryServiceTests
{
    [Fact]
    public async Task CreateAsync_WithValidDepartments_ReturnsNewId_AndCallsRepository()
    {
        var repo = new Mock<IInquiryRepository>();
        var deptRepo = new Mock<IDepartmentRepository>();

        deptRepo.Setup(d => d.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        deptRepo.Setup(d => d.ExistsAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        repo.Setup(r => r.CreateAsync(It.IsAny<Inquiry>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var svc = new InquiryService(repo.Object, deptRepo.Object);

        var dto = new CreateInquiryDto
        {
            Name = "לאה",
            Phone = "050-1234567",
            Email = "lea@example.com",
            DepartmentIds = new() { 1, 3 },
            Description = "בדיקה"
        };

        var id = await svc.CreateAsync(dto, CancellationToken.None);

        id.Should().Be(100);
        repo.Verify(r => r.CreateAsync(It.IsAny<Inquiry>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithUnknownDepartment_Throws()
    {
        var repo = new Mock<IInquiryRepository>();
        var deptRepo = new Mock<IDepartmentRepository>();
        deptRepo.Setup(d => d.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var svc = new InquiryService(repo.Object, deptRepo.Object);

        var dto = new CreateInquiryDto
        {
            Name = "רות",
            Phone = "050-5555555",
            Email = "rut@example.com",
            DepartmentIds = new() { 99 },
            Description = "ולידציה"
        };

        var act = async () => await svc.CreateAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<System.Exception>();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsList()
    {
        var repo = new Mock<IInquiryRepository>();
        var deptRepo = new Mock<IDepartmentRepository>();
        var svc = new InquiryService(repo.Object, deptRepo.Object);

        repo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[]
            {
                new Inquiry { Id = 1, Name = "א", Phone = "1", Email = "a@a", DepartmentIds = new(){1} },
                new Inquiry { Id = 2, Name = "ב", Phone = "2", Email = "b@b", DepartmentIds = new(){2} },
            });

        var list = await svc.GetAllAsync(CancellationToken.None);

        var arr = list.ToList();
        arr.Should().HaveCount(2);
        arr[0].Id.Should().Be(1);
        arr[1].Name.Should().Be("ב");
    }
}
