using WriteLog;
using FtpFunctions;
using UserConfigReader;
using FolderFunctions;
using ProcessStatusMessageSetter;
using GraphHelper;
using RunTimedFunctions;
using ProcessSartupFunctions;


// DEA.Next
// ~~~~~~~~
// TODO 1: Rewrite the code to match Graph v5.0.0. +

// Aplication title just for fun.

WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
FolderFunctionsClass.CheckFolders(null!);

Console.WriteLine(RunTimedFunctionsClass.CallDeaCleaner());
Console.WriteLine(RunTimedFunctionsClass.CallDeaMailer());

await ProcessStartupFunctionsClass.StartupProcess();