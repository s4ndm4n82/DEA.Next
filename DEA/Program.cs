using WriteLog;
using FtpFunctions;
using UserConfigReader;
using FolderFunctions;
using ProcessStatusMessageSetter;
using GraphHelper;
using RunTimedFunctions;


// DEA.Next
// ~~~~~~~~
// TODO 1: Rewrite the code to match Graph v5.0.0. +

// Aplication title just for fun.

WriteLogClass.WriteToLog(1, "Starting download process ....", 1);
FolderFunctionsClass.CheckFolders(null!);

Console.WriteLine(RunTimedFunctionsClass.CallDeaCleaner());

int emailResult = 0;
int ftpResult = 0;

// User cpmfig reader.
UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

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
        if ((ftpClient.FtpDetails!.FtpType!.ToLower() == "ftp" || ftpClient.FtpDetails!.FtpType!.ToLower() == "ftps") && ftpClient.CustomerStatus == 1)
        {
            ftpResult = await FtpFunctionsClass.GetFtpFiles(ftpClient.Id);
        }
        else
        {
            // Awating to be implimented. Will be added when needed.
            //SftpFunctionsClass.GetSftpFiles(ftpClient.id);
        }
    }
}

// Email download loop.
if (emailClientCount > 0)
{
    foreach (var emailClient in emailClients)
    {
        if (emailClient.CustomerStatus == 1)
        {
            emailResult = await GraphHelperClass.InitializGetAttachment(emailClient.Id);
        }        
    }
}

WriteLogClass.WriteToLog(ProcessStatusMessageSetterClass.SetMessageTypeMain(emailResult, ftpResult), $"{ProcessStatusMessageSetterClass.SetProcessStatusMain(emailResult, ftpResult)}\n", 1);