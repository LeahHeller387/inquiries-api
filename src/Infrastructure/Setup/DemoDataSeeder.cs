using Inquiries.Api.Infrastructure.Ef;
using Microsoft.EntityFrameworkCore;

namespace Inquiries.Api.Infrastructure.Setup;

internal static class DemoDataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        var nowUtc = DateTime.UtcNow;
        var curStart = new DateTime(nowUtc.Year, nowUtc.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var prevStart = curStart.AddMonths(-1);
        var prevEnd   = curStart;

        var lyStart = curStart.AddYears(-1);
        var lyEnd   = lyStart.AddMonths(1);

        var hasPrev = await db.Inquiries.AnyAsync(i => i.CreatedAtUtc >= prevStart && i.CreatedAtUtc < prevEnd, ct);
        var hasLy   = await db.Inquiries.AnyAsync(i => i.CreatedAtUtc >= lyStart   && i.CreatedAtUtc < lyEnd,   ct);

        var depIds = await db.Departments.Select(d => d.Id).ToListAsync(ct);
        if (depIds.Count == 0) return;

        if (!hasPrev)
            await InsertRangeAsync(db, depIds, prevStart, prevEnd, basePerDept: 6, ct, varyByDepartment: true);

        if (!hasLy)
            await InsertRangeAsync(db, depIds, lyStart, lyEnd, basePerDept: 4, ct, varyByDepartment: true);
    }

    private static async Task InsertRangeAsync(
        AppDbContext db,
        IEnumerable<int> depIds,
        DateTime from, DateTime to,
        int basePerDept,
        CancellationToken ct,
        bool varyByDepartment)
    {
        var totalDays = Math.Max(1, (to - from).Days); 

        foreach (var depId in depIds)
        {
            var perDept = basePerDept;
            if (varyByDepartment)
            {
                perDept = Math.Max(1, basePerDept - 2 + (depId % 5));
            }

            var offsets = GenerateDayOffsets(depId, from, totalDays, perDept);

            int index = 0;
            foreach (var off in offsets)
            {
                index++;
                var dt = from.AddDays(off)
                          .AddHours((depId * 7 + index * 3) % 24)
                          .AddMinutes((depId * 11 + index * 5) % 60);

                var inquiry = new InquiryEf
                {
                    CreatedAtUtc = dt,
                    Name        = $"Dept {depId} – Person {index}",
                    Email       = $"dept{depId}+{from:yyyyMM}.{index}@example.local",
                    Phone       = $"050-{(depId * 13 + index * 7) % 1000:000}-{(depId * 17 + index * 9) % 10000:0000}",
                    Description = $"Seed for dep {depId} in window {from:yyyy-MM} (#{index})"
                };

                db.Inquiries.Add(inquiry);
                await db.SaveChangesAsync(ct); 

                db.InquiryDepartments.Add(new InquiryDepartmentEf
                {
                    InquiryId = inquiry.Id,
                    DepartmentId = depId
                });
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private static IEnumerable<int> GenerateDayOffsets(int depId, DateTime from, int totalDays, int count)
    {
        if (totalDays <= 1)
            return new int[] { 0 };

        var need = Math.Min(count, totalDays);
        var set = new HashSet<int>();

        long state = depId * 1103515245L + from.Month * 12345L + from.Year * 17L + 73L;

        while (set.Count < need)
        {
            state = (1664525L * state + 1013904223L) & 0x7fffffff;
            var off = (int)(state % totalDays);
            if (off >= totalDays) off = totalDays - 1;
            set.Add(off);
        }

        var result = set.ToList();
        result.Sort();
        return result;
    }
}
