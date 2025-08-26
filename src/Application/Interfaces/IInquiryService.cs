using Inquiries.Api.Application.DTOs.Inquiries;

namespace Inquiries.Api.Application.Interfaces;

public interface IInquiryService
{
    Task<int> CreateAsync(CreateInquiryDto dto, CancellationToken ct = default);
    Task<InquiryDto?> GetAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<InquiryDto>> GetAllAsync(CancellationToken ct = default);
    Task UpdateAsync(int id, UpdateInquiryDto dto, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
