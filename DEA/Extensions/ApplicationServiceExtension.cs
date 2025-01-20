using DEA.Next.Classes;
using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.OtherFunctions;
using DEA.Next.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DEA.Next.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            opt.UseNpgsql(config.GetConnectionString("DefaultConnection"));
        }, ServiceLifetime.Scoped);
        
        services.AddScoped<CustomerDataClass>();
        services.AddScoped<IUserConfigRepository, CustomerDetailsRepository>();

        services.AddSingleton<CustomerDetails>();

        services.AddTransient<ProcessStartupFunctionsClass>();

        var serviceProvider = services.BuildServiceProvider(); 
        var repository = serviceProvider.GetService<IUserConfigRepository>();
        if (repository != null) UserConfigRetriever.Initialize(repository);

        return services;
    }
}