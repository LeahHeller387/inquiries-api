using Inquiries.Api.Domain.Entities;
using Inquiries.Api.Infrastructure.Ef;
using Inquiries.Api.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Inquiries.Api.Infrastructure.Repositories.Ef;

public class EfInquiryRepository : IInquiryRepository
{
    private readonly AppDbContext _db;
    public EfInquiryRepository(AppDbContext db) => _db = db;

    public async Task<int> CreateAsync(Inquiry e, CancellationToken ct = default)
    {
        var ef = new InquiryEf
        {
            Name = e.Name, Phone = e.Phone, Email = e.Email,
            Description = e.Description, CreatedAtUtc = DateTime.UtcNow
        };
        foreach (var depId in e.DepartmentIds.Distinct())
            ef.InquiryDepartments.Add(new InquiryDepartmentEf { DepartmentId = depId });

        _db.Inquiries.Add(ef);
        await _db.SaveChangesAsync(ct);
        return ef.Id;
    }

    public async Task<Inquiry?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ef = await _db.Inquiries.Include(i => i.InquiryDepartments)
                                    .FirstOrDefaultAsync(i => i.Id == id, ct);
        return ef is null ? null : ToDomain(ef);
    }

    public async Task<IReadOnlyCollection<Inquiry>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _db.Inquiries.Include(i => i.InquiryDepartments)
                                      .OrderBy(i => i.Id).ToListAsync(ct);
        return list.Select(ToDomain).ToList();
    }

    public async Task UpdateAsync(Inquiry e, CancellationToken ct = default)
    {
        var ef = await _db.Inquiries.Include(i => i.InquiryDepartments)
                                    .FirstOrDefaultAsync(i => i.Id == e.Id, ct)
                 ?? throw new KeyNotFoundException($"Inquiry {e.Id} not found.");

        ef.Name = e.Name; ef.Phone = e.Phone; ef.Email = e.Email; ef.Description = e.Description;

        _db.InquiryDepartments.RemoveRange(ef.InquiryDepartments);
        ef.InquiryDepartments = e.DepartmentIds.Distinct()
            .Select(id => new InquiryDepartmentEf { InquiryId = ef.Id, DepartmentId = id }).ToList();

        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var ef = await _db.Inquiries.FindAsync([id], ct);
        if (ef is null) return;
        _db.Inquiries.Remove(ef);
        await _db.SaveChangesAsync(ct);
    }

    private static Inquiry ToDomain(InquiryEf ef) => new()
    {
        Id = ef.Id,
        Name = ef.Name,
        Phone = ef.Phone,
        Email = ef.Email,
        Description = ef.Description,
        CreatedAtUtc = ef.CreatedAtUtc,
        DepartmentIds = ef.InquiryDepartments.Select(x => x.DepartmentId).ToList()
    };
}
