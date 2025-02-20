using DEA.Next.Extensions;
using DEA.Next.HelperClasses.InternetLineChecker;
using DEA.Next.Versioning;
using FolderFunctions;
using Microsoft.Extensions.DependencyInjection;
using VersionIncrementerClass;
using WriteLog;

// Create and configure the application builder.
var builder = ApplicationBuilderExtension.CreateApplicationBuilder(args);

// Build the application.
var app = builder.Build();

// Increments the version number
VersionIncrementer.IncrementVersion();

// Displays the logo
DisplayLogo.Logo();

// Checks and creates the main folders that used by the app.
FolderFunctionsClass.CheckFolders(null!);

if (await InternetLineChecker.InternetLineCheckerAsync())
{
    // Log that a working internet connection was found.
    WriteLogClass.WriteToLog(1, "Working internet connection found ....", 1);

    // Create a new scope for dependency injection.
    using var scope = app.Services.CreateScope();

    // Get the service provider from the scope.
    var services = scope.ServiceProvider;

    await DatabaseInitializer.InitializeAsync(services);

    // Exit the application.
    Environment.Exit(0);
}
else
{
    WriteLogClass.WriteToLog(0, "No working internet connection. Exiting ....", 0);
    Environment.Exit(0);
}