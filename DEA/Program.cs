using WriteLog;
using FolderFunctions;
using RunTimedFunctions;
using ProcessSartupFunctions;
using ErrorFolderChecker;
using DisplayLogoClass;
using AppConfigReader;
using VersionIncrementerClass;

// DEA.Next
// ~~~~~~~~
// TODO 1: Rewrite the code to match Graph v5.0.0. +

// Aplication title just for fun.

VersionIncrementer.IncrementVersion();

DisplayLogo.Logo();

FolderFunctionsClass.CheckFolders(null!);

AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
int maxErrorFolders = jsonData.ProgramSettings.MaxErrorFolders;
int errorFolderItemCount = ErrorFolderCheckerClass.ErrorFolderChecker().Item1.Count();

if (errorFolderItemCount > maxErrorFolders)
{
    RunTimedFunctionsClass.CallDeaTimedProcesses("deamailer");
    WriteLogClass.WriteToLog(2, $"Error folder contains {errorFolderItemCount} folders. Check and empty the error folder ....", 1);
}

WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
await ProcessStartupFunctionsClass.StartupProcess();

RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");