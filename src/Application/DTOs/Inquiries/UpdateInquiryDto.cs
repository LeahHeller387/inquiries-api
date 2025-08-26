using System.ComponentModel.DataAnnotations;

namespace Inquiries.Api.Application.DTOs.Inquiries;

public class UpdateInquiryDto
{
    [Required, StringLength(120)]
    public string Name { get; set; } = default!;

    [Required, StringLength(40)]
    public string Phone { get; set; } = default!;

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; set; } = default!;

    [Required, MinLength(1)]
    public List<int> DepartmentIds { get; set; } = new();

    [StringLength(1000)]
    public string? Description { get; set; }
}
