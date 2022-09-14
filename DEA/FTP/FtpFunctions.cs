using WriteLog;
using ConnectFtp;
using FluentFTP;

namespace FtpFunctions
{
    internal class FtpFunctionsClass
    {
        public static async void ListDirectories()
        {
            using (var FtpConnect = await ConnectFtpClass.ConnectFtp())
            {   
                foreach (FtpListItem item in await FtpConnect.GetListing("/TestFolder/Test FTP Root/Test Dir"))
                {
                    WriteLogClass.WriteToLog(3, $"File {item.FullName}");
                }
            }
        }
    }
}
