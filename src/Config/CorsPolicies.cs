namespace Inquiries.Api.Config;

public static class CorsPolicies
{
    public const string Default = "Default";

    public static IServiceCollection AddDefaultCors(this IServiceCollection services)
    {
        services.AddCors(o =>
            o.AddPolicy(Default, p =>
                p.WithOrigins("http://localhost:5173", "http://localhost:4200")
                 .AllowAnyHeader()
                 .AllowAnyMethod()));
        return services;
    }
}
