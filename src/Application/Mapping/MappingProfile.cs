using Inquiries.Api.Application.DTOs.Inquiries;
using Inquiries.Api.Domain.Entities;

namespace Inquiries.Api.Application.Mapping;

public static class InquiryMapping
{
    public static Inquiry ToEntity(this CreateInquiryDto dto) => new()
    {
        Name = dto.Name.Trim(),
        Phone = dto.Phone.Trim(),
        Email = dto.Email.Trim(),
        DepartmentIds = dto.DepartmentIds.Distinct().ToList(),
        Description = dto.Description
    };

    public static void Apply(this Inquiry entity, UpdateInquiryDto dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Phone = dto.Phone.Trim();
        entity.Email = dto.Email.Trim();
        entity.DepartmentIds = dto.DepartmentIds.Distinct().ToList();
        entity.Description = dto.Description;
    }

    public static InquiryDto ToDto(this Inquiry e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Phone = e.Phone,
        Email = e.Email,
        DepartmentIds = e.DepartmentIds.ToList(),
        Description = e.Description,
        CreatedAtUtc = e.CreatedAtUtc
    };
}
