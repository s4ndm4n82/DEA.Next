using DEA;
using WriteLog;
using FolderCleaner;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using FtpFunctions;
using UserConfigReader;
using SftpFunctions; // keep it untill implimentation.
using System.Drawing.Text;
using FolderFunctions;


// DEA old
// ~~~~~~~
// TODO 1: Brake the main graph functions into smaller set of chuncks.
// TODO 2: Change the usage of DEA.conf to app.conf (But I don't think it's needed. Bcause I use app.conf to stroe some very important data set.).
// TODO 3: Make the metadata file have the same name as the pdf or attachment file and Remove .pdf extention.
// TODO 4: Make the attachmetn download loop more efficiant. <-- done.
// TODO 5: Check the error folder mover. <-- Working and done.
// TODO 6: Stramline the code.
// TODO 7: Write summeries.
// TODO 8: Create a way to move files to error folder and forward the mail if the file attachments are not accepted.
// TODO 9: Make a internet connection checker.

// DEA.Next
// ~~~~~~~~
// TODO 1: Create an XML config file (use app.config).
// TODO 2: Create an XML reader toload the settings from the config file.
// TODO 3: Make a FTP/SFTP/FTPS component. <-- Done and working
// TODO 4: Make it posible to download files from and FTP server. <-- Done and working
// TODO 5: Change the download from location according to what is set in the above config file. <-- Done and working
// TODO 6: Keep the files in a holding folder and then convert them into Base64 string. <-- Done and working
// TODO 7: Make DEA send the POST url to the TPM REST API.<-- Done and working
// TODO 8: Get the success signal and then delete the file. If not keep the file and send an error mail or message. <-- Done and working

// Aplication title just for fun.

WriteLogClass.WriteToLog(3, "Starting download process ....", 1);

FolderFunctionsClass.CheckFolders(null!);

HandleErrorFiles.HandleErrorFilesClass.MoveFilesToErrorFolder("G:\\Users\\S4NDM4N\\Development\\Repos\\s4ndm4n82\\DEA.Next\\DEA\\bin\\Debug\\net6.0\\Download\\FTPFiles\\98766543210", 2);

Thread.Sleep(1000000000);

int ftpLoopCount = 0;
int emlLoopCount = 0;
bool result = false;

UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
IEnumerable<UserConfigReaderClass.Customerdetail> ftpClients = jsonData.CustomerDetails!.Where(ftpc => ftpc.FileDeliveryMethod!.ToUpper() == "FTP");
IEnumerable<UserConfigReaderClass.Customerdetail> emailClients = jsonData.CustomerDetails!.Where(emailc => emailc.FileDeliveryMethod!.ToUpper() == "EMAIL");

if (ftpClients.Any())
{
    foreach (var ftpClient in ftpClients)
    {
        ftpLoopCount++;

        if (ftpClient.FtpDetails!.FtpType!.ToUpper() == "FTP" || ftpClient.FtpDetails!.FtpType!.ToUpper() == "FTPS")
        {
            result = await FtpFunctionsClass.GetFtpFiles(ftpClient.id);
        }
        /*else
        {
            // Awating to be implimented. Will be added when needed.
            SftpFunctionsClass.GetSftpFiles(ftpClient.id);
        }*/
    }
}

/*if (emailClients.Any())
{
    foreach (var emailClient in emailClients)
    {
        emlLoopCount++;

        await GraphHelper.InitializGetAttachment(emailClient.id);
    }
}*/
/*
if (ftpLoopCount == ftpClients.Count() && result)
{
    WriteLogClass.WriteToLog(3, "Process completed successfully ... ", 1);
}
else
{
    WriteLogClass.WriteToLog(3, "Process exited with errors ... ", 1);
}*/
