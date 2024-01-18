using WriteLog;
using FolderFunctions;
using RunTimedFunctions;
using ProcessSartupFunctions;
using ErrorFolderChecker;
using DisplayLogoClass;
using AppConfigReader;
using VersionIncrementerClass;
using DEA.Next.HelperClasses.InternetLineChecker;

// DEA.Next
// ~~~~~~~~
// TODO 1: Rewrite the code to match Graph v5.0.0. +

// Increments the version number
VersionIncrementer.IncrementVersion();
// Displays the logo
DisplayLogo.Logo();

// Checks and creates the main folders that used by the app.
FolderFunctionsClass.CheckFolders(null!);
// Get the set amount of allowed error folders.
AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
int maxErrorFolders = jsonData.ProgramSettings.MaxErrorFolders;
// Checks how many folders are in the error folder.
int errorFolderItemCount = ErrorFolderCheckerClass.ErrorFolderChecker().Item1.Count();
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
    await ProcessStartupFunctionsClass.StartupProcess();
    // Start the timed processes deacleaner.
    RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");
    Environment.Exit(0);
}
else
{
    WriteLogClass.WriteToLog(0, "No working internet connection. Exiting ....", 0);
    Environment.Exit(0);
}