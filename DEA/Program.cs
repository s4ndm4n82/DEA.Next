using DEA;
using WriteLog;
using FolderCleaner;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using FtpFunctions;
using UserConfigReader;
using SftpFunctions; // keep it untill implimentation.
using System.Drawing.Text;


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
// TODO 3: Make a FTP/SFTP/FTPS component.
// TODO 4: Make it posible to download files from and FTP server.
// TODO 5: Change the download from location according to what is set in the above config file.
// TODO 6: Keep the files in a holding folder and then convert them into Base64 string.
// TODO 7: Make DEA send the POST url to the TPM REST API.
// TODO 8: Get the success signal and then delete the file. If not keep the file and send an error mail or message.

// Aplication title just for fun.

WriteLogClass.WriteToLog(3, "Connecting to FTP Server ....", "FTP");

var jsonData = UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();

var ftpClients = jsonData.CustomerDetails!.Where(ftpc => ftpc.FileDeliveryMethod!.ToUpper() == "FTP");
var emailClients = jsonData.CustomerDetails!.Where(emailc => emailc.FileDeliveryMethod!.ToLower() == "email");

/*foreach (var ftpClient in ftpClients)
{
    if (ftpClient.FtpDetails!.FtpType!.ToUpper() == "FTP" || ftpClient.FtpDetails!.FtpType!.ToUpper() == "FTPS")
    {
        await FtpFunctionsClass.GetFtpFiles(ftpClient.id);
    }
    /*else // Awating to be implimented. Will be added when needed.
    {
        await SftpFunctionsClass.GetSftpFiles(client.id);
    }
}*/

foreach (var emailClient in emailClients)
{
   await GraphHelper.InitializGetAttachment(emailClient.id);
}

/*
// Check for the attachment download folder and the log folder. Then creates the folders if they're missing.
GraphHelper.CheckFolders("none");
WriteLogClass.WriteToLog(3, "Checking main folders ....");

// Clean the main download folder.
FolderCleanerClass.GetFolders(GraphHelper.CheckFolders("Download"));
*/