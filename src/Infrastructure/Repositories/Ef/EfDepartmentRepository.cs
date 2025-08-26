using Inquiries.Api.Domain.Entities;
using Inquiries.Api.Infrastructure.Ef;
using Inquiries.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inquiries.Api.Infrastructure.Repositories.Ef;

public class EfDepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _db;
    public EfDepartmentRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken ct = default)
        => (await _db.Departments.OrderBy(d => d.Id).ToListAsync(ct))
           .Select(d => new Department { Id = d.Id, Name = d.Name }).ToList();

    public async Task<Department?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var d = await _db.Departments.FindAsync([id], ct);
        return d is null ? null : new Department { Id = d.Id, Name = d.Name };
    }

    public Task<bool> ExistsAsync(int id, CancellationToken ct = default)
        => _db.Departments.AnyAsync(d => d.Id == id, ct);
}
