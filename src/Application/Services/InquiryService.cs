using Inquiries.Api.Application.DTOs.Inquiries;
using Inquiries.Api.Application.Interfaces;
using Inquiries.Api.Application.Mapping;
using Inquiries.Api.Infrastructure.Repositories.Interfaces;

namespace Inquiries.Api.Application.Services;

public class InquiryService : IInquiryService
{
    private readonly IInquiryRepository _repo;
    private readonly IDepartmentRepository _deptRepo;

    public InquiryService(IInquiryRepository repo, IDepartmentRepository deptRepo)
    {
        _repo = repo;
        _deptRepo = deptRepo;
    }

    public async Task<int> CreateAsync(CreateInquiryDto dto, CancellationToken ct = default)
    {
        await ValidateDepartments(dto.DepartmentIds, ct);
        var entity = dto.ToEntity();
        return await _repo.CreateAsync(entity, ct);
    }

    public async Task<InquiryDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetByIdAsync(id, ct);
        return e?.ToDto();
    }

    public async Task<IEnumerable<InquiryDto>> GetAllAsync(CancellationToken ct = default)
        => (await _repo.GetAllAsync(ct)).Select(x => x.ToDto());

    public async Task UpdateAsync(int id, UpdateInquiryDto dto, CancellationToken ct = default)
    {
        await ValidateDepartments(dto.DepartmentIds, ct);
        var current = await _repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException($"Inquiry {id} not found.");
        current.Apply(dto);
        await _repo.UpdateAsync(current, ct);
    }

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);

    private async Task ValidateDepartments(IEnumerable<int> ids, CancellationToken ct)
    {
        foreach (var depId in ids.Distinct())
            if (!await _deptRepo.ExistsAsync(depId, ct))
                throw new ArgumentException($"Department {depId} does not exist.");
    }
}
