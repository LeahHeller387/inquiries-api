using Inquiries.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inquiries.Api.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _repo;
    public DepartmentsController(IDepartmentRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var deps = await _repo.GetAllAsync();
        return Ok(deps.Select(d => new { d.Id, d.Name }));
    }
}
