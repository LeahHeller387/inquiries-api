namespace Inquiries.Api.Application.DTOs.Inquiries;

public class InquiryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public List<int> DepartmentIds { get; set; } = new();
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
