using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DEA.Next.Extensions;

/// <summary>
///     Provides extension methods for creating and configuring the application builder.
/// </summary>
public class ApplicationBuilderExtension
{
    /// <summary>
    ///     Creates and configures a new instance of <see cref="HostApplicationBuilder" />.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A configured <see cref="HostApplicationBuilder" /> instance.</returns>
    public static HostApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        // Create a new HostApplicationBuilder instance with the provided arguments.
        var builder = Host.CreateApplicationBuilder(args);

        // Add application services using the configuration from the appsettings.json file.
        builder.Services.AddApplicationServices(builder.Configuration.AddJsonFile(@".\Config\appsettings.json")
            .Build());

        // Return the configured builder.
        return builder;
    }
}