using GraphHelper;
using WriteLog;
using FolderCleaner;
using FtpFunctions;
using UserConfigReader;
using FolderFunctions;

// DEA.Next
// ~~~~~~~~
// TODO 1: Impliment error file handler.

// Aplication title just for fun.

WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
FolderFunctionsClass.CheckFolders(null!);

int emailResult = 0;
int ftpResult = 0;

// User cpmfig reader.
UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

// Array contaning all the FTP user details.
IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients = jsonData.CustomerDetails!.Where(ftpc => ftpc.FileDeliveryMethod!.ToLower() == "ftp");
int ftpClientCount = ftpClients.Count(); // FTP client count.

// Array contaning all the Email user details.
IEnumerable<UserConfigReaderClass.Customerdetail> emailClients = jsonData.CustomerDetails!.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == "email");
int emailClientCount = emailClients.Count(); // Email client count.

// FTP download loop.
if (ftpClientCount > 0)
{
    foreach (var ftpClient in ftpClients)
    {
        if (ftpClient.FtpDetails!.FtpType!.ToLower() == "ftp" || ftpClient.FtpDetails!.FtpType!.ToLower() == "ftps")
        {
            ftpResult = await FtpFunctionsClass.GetFtpFiles(ftpClient.id);
        }
        /*else
        {
            // Awating to be implimented. Will be added when needed.
            SftpFunctionsClass.GetSftpFiles(ftpClient.id);
        }*/
    }
}

// Email download loop.
if (emailClientCount > 0)
{
    foreach (var emailClient in emailClients)
    {
        emailResult = await GraphHelperClass.InitializGetAttachment(emailClient.id);
    }
}

// Selects the correct status message for the log.
// Using ternary operator which stands for below pesudocode.
// IF emailResult = 1 and ftpResult = 1 SET "Process completed successfully ...."
// ELSE IF emailResult = 2 and ftpResult = 2 SET "Process completed with issues ...."
// ELSE SET "Process terminated due to errors ...."
string logMsg = (emailResult == 1 || emailResult == 1) && (ftpResult == 4 || ftpResult == 4) ? "Process completed successfully ...." :
                emailResult == 2 && ftpResult == 2 ? "Process completed with issues ...." :
                emailResult == 1 && (ftpResult == 0 || ftpResult == 4) ? $"Email process completed .... FTP code {ftpResult} ...." :
                emailResult == 2 && (ftpResult == 0 || ftpResult == 4) ? $"Email process ended with errors .... FTP code {ftpResult} ...." :
                ftpResult == 1 && (emailResult == 0 || emailResult == 4) ? $"FTP process completed .... Email code {emailResult} ...." :
                ftpResult == 2 && (emailResult == 0 || emailResult == 4) ? $"FTP process ended with errors .... Email code {emailResult} ...." :
                "Process terminated due to errors ....";

// Select the correct log type.
// Using ternary operator which stands for below pesudocode.
// IF emailResult = 1 and ftpResult = 1 OR emailResult = 2 && ftpResult = 2 SET 1
// ELSE SET 0
int msgType = (emailResult == 1 || emailResult == 4) && (ftpResult == 1 || ftpResult == 4) || (emailResult == 2 && ftpResult == 2) ? 1 : 0;

WriteLogClass.WriteToLog(msgType, $"{logMsg}\n", 1);