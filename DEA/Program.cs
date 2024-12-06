using AppConfigReader;
using DEA.Next.Classes;
using DEA.Next.Data;
using DEA.Next.Extensions;
using DEA.Next.HelperClasses.InternetLineChecker;
using DisplayLogoClass;
using ErrorFolderChecker;
using FolderFunctions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProcessSartupFunctions;
using RunTimedFunctions;
using VersionIncrementerClass;
using WriteLog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration.AddJsonFile(@".\Config\appsettings.json").Build());

var app = builder.Build();

var dbContext = app.Services.GetService<DataContext>();
if (dbContext != null) builder.Services.AddSingleton(dbContext);

// Increments the version number
VersionIncrementer.IncrementVersion();
// Displays the logo
DisplayLogo.Logo();

// Checks and creates the main folders that used by the app.
FolderFunctionsClass.CheckFolders(null!);
// Get the set amount of allowed error folders.
var jsonData = AppConfigReaderClass.ReadAppDotConfig();
var maxErrorFolders = jsonData.ProgramSettings.MaxErrorFolders;
// Checks how many folders are in the error folder.
var errorFolderItemCount = ErrorFolderCheckerClass.ErrorFolderChecker().Item1.Count();
// If there are more than the set amount of allowed error folders then call the dea timed processes.
if (errorFolderItemCount > maxErrorFolders)
{
    RunTimedFunctionsClass.CallDeaTimedProcesses("deamailer");
    WriteLogClass.WriteToLog(2, $"Error folder contains {errorFolderItemCount} folders. Check and empty the error folder ....", 1);
}

if (await InternetLineChecker.InternetLineCheckerAsync())
{
    WriteLogClass.WriteToLog(1, "Working internet connection found ....", 1);
    // Start the download process
    WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
    var processStartupFunctions = app.Services.GetService<ProcessStartupFunctionsClass>();
    if (processStartupFunctions != null) await processStartupFunctions.StartupProcess();
    // Start the timed processes deacleaner.
    RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");
    Environment.Exit(0);
}
else
{
    WriteLogClass.WriteToLog(0, "No working internet connection. Exiting ....", 0);
    Environment.Exit(0);
}

var appLife = app.Services.GetService<IHostApplicationLifetime>();
appLife.ApplicationStopping.Register(() =>
{
    dbContext.Dispose();
});