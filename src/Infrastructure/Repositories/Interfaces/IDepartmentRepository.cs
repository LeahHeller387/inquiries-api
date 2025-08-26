using Inquiries.Api.Domain.Entities;

namespace Inquiries.Api.Infrastructure.Repositories.Interfaces;

public interface IDepartmentRepository
{
    Task<IReadOnlyCollection<Department>> GetAllAsync(CancellationToken ct = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> ExistsAsync(int id, CancellationToken ct = default);
}
