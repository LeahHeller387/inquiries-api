using Microsoft.EntityFrameworkCore;

namespace Inquiries.Api.Infrastructure.Ef;

public class AppDbContext : DbContext
{
    public DbSet<InquiryEf> Inquiries => Set<InquiryEf>();
    public DbSet<DepartmentEf> Departments => Set<DepartmentEf>();
    public DbSet<InquiryDepartmentEf> InquiryDepartments => Set<InquiryDepartmentEf>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<DepartmentEf>().HasKey(x => x.Id);
        b.Entity<DepartmentEf>().Property(x => x.Name).IsRequired().HasMaxLength(100);

        b.Entity<InquiryEf>().HasKey(x => x.Id);
        b.Entity<InquiryEf>().Property(x => x.Name).IsRequired().HasMaxLength(120);
        b.Entity<InquiryEf>().Property(x => x.Phone).IsRequired().HasMaxLength(40);
        b.Entity<InquiryEf>().Property(x => x.Email).IsRequired().HasMaxLength(160);
        b.Entity<InquiryEf>().Property(x => x.Description).HasMaxLength(1000);
        b.Entity<InquiryEf>().Property(x => x.CreatedAtUtc).IsRequired();

        b.Entity<InquiryDepartmentEf>().HasKey(x => new { x.InquiryId, x.DepartmentId });
        b.Entity<InquiryDepartmentEf>()
          .HasOne(x => x.Inquiry).WithMany(x => x.InquiryDepartments).HasForeignKey(x => x.InquiryId);
        b.Entity<InquiryDepartmentEf>()
          .HasOne(x => x.Department).WithMany(x => x.InquiryDepartments).HasForeignKey(x => x.DepartmentId);
    }
}

public class InquiryEf
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<InquiryDepartmentEf> InquiryDepartments { get; set; } = new List<InquiryDepartmentEf>();
}

public class DepartmentEf
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<InquiryDepartmentEf> InquiryDepartments { get; set; } = new List<InquiryDepartmentEf>();
}

public class InquiryDepartmentEf
{
    public int InquiryId { get; set; }
    public InquiryEf Inquiry { get; set; } = default!;
    public int DepartmentId { get; set; }
    public DepartmentEf Department { get; set; } = default!;
}
