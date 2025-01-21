using DEA.Next.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DEA.UI;

internal static class Program
{
    /// <summary>
    ///     The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        var builder = ApplicationBuilderExtension.CreateApplicationBuilder(args);
        var app = builder.Build();
        var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new StartupForm(services));
    }
}