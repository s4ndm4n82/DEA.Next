using WriteLog;
using FolderFunctions;
using RunTimedFunctions;
using ProcessSartupFunctions;
using ErrorFolderChecker;


// DEA.Next
// ~~~~~~~~
// TODO 1: Rewrite the code to match Graph v5.0.0. +

// Aplication title just for fun.

FolderFunctionsClass.CheckFolders(null!);

if (!ErrorFolderCheckerClass.ErrorFolderChecker().Item1.Any())
{
    WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
    await ProcessStartupFunctionsClass.StartupProcess();
}
else
{
    RunTimedFunctionsClass.CallDeaTimedProcesses("deamailer");
    WriteLogClass.WriteToLog(1, "Error folder is not empty. Check and empty the error folder before continuing ....", 1);
}

RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");