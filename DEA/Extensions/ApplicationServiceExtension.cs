using DEA.Next.Classes;
using DEA.Next.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProcessSartupFunctions;

namespace DEA.Next.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        });
        
        services.AddScoped<CustomerDataClass>();
        services.AddTransient<ProcessStartupFunctionsClass>();

        return services;
    }
}