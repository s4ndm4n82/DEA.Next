using DEA.Next.Classes;
using DEA.Next.Data;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.Interfaces;
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
        services.AddScoped<IUserConfigRepository, CustomerDetailsRepository>();
        
        var repository = services.BuildServiceProvider().GetService<IUserConfigRepository>();
        if (repository != null) UserConfigRetriever.Initialize(repository);

        return services;
    }
}