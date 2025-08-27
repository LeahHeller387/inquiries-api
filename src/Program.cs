using Inquiries.Api.Application.Interfaces;
using Inquiries.Api.Application.Services;
using Inquiries.Api.Middleware;

using Inquiries.Api.Infrastructure.Ef;
using Inquiries.Api.Infrastructure.Repositories.Ef;
using Inquiries.Api.Infrastructure.Repositories.Interfaces;
using Inquiries.Api.Infrastructure.Setup;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173", "http://localhost:4200")
     .AllowAnyHeader()
     .AllowAnyMethod()));

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IInquiryRepository, EfInquiryRepository>();
builder.Services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();
builder.Services.AddScoped<IInquiryService, InquiryService>();
builder.Services.AddScoped<IReportService, ReportServiceDb>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();

    if (!await db.Departments.AnyAsync())
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Departments ON;");

            db.Departments.AddRange(
                new DepartmentEf { Id = 1, Name = "כללי" },
                new DepartmentEf { Id = 2, Name = "תפעול" },
                new DepartmentEf { Id = 3, Name = "מערכות מידע" }
            );
            await db.SaveChangesAsync();

            await db.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Departments OFF;");
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
    await DemoDataSeeder.SeedAsync(db);

    await SqlServerObjects.EnsureStoredProcAsync(cfg, env);

}

app.Run();
