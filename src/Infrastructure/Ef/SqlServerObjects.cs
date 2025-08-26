using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting; 
using System.Data;

namespace Inquiries.Api.Infrastructure.Ef;

public static class SqlServerObjects
{
    public static async Task EnsureStoredProcAsync(
        IConfiguration cfg,
        IHostEnvironment env,                   
        CancellationToken ct = default)
    {
        var cs = cfg.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Missing connection string 'Default'.");

        var sqlPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Scripts", "GetMonthlyInquiryReport.sql");

        if (!File.Exists(sqlPath))
            throw new FileNotFoundException($"SQL file not found: {sqlPath}");

        var createSp = await File.ReadAllTextAsync(sqlPath, ct); 

        await using var conn = new SqlConnection(cs);
        await conn.OpenAsync(ct);
        await using var cmd = new SqlCommand(createSp, conn) { CommandType = CommandType.Text };
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
