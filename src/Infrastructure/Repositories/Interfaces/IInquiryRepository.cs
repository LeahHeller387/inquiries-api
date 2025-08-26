using Inquiries.Api.Domain.Entities;

namespace Inquiries.Api.Infrastructure.Repositories.Interfaces;

public interface IInquiryRepository
{
    Task<int> CreateAsync(Inquiry entity, CancellationToken ct = default);
    Task<Inquiry?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyCollection<Inquiry>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(Inquiry entity, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
