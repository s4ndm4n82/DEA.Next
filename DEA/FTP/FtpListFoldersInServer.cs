using ConnectFtp;
using FluentFTP;
using UserConfigReader;
using WriteLog;

namespace FtpListFoldersInServerClass
{
    internal class FtpListFoldersInServer
    {
        public static void CheckFtpFolders()
        {
            GetFtpFolderList();
        }

        private static UserConfigReaderClass.Ftpdetails GetFtpDetails()
        {
            UserConfigReaderClass.CustomerDetailsObject jsonDate = UserConfigReaderClass.ReadUserDotConfig <UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail customerDetails  = jsonDate.CustomerDetails.FirstOrDefault(cid => cid.Id == 1);
            UserConfigReaderClass.Ftpdetails ftpDetails = customerDetails.FtpDetails;

            return ftpDetails;
        }

        public static async void GetFtpFolderList()
        {
            try
            {
                FtpClient conn = new(GetFtpDetails().FtpHostName, GetFtpDetails().FtpUser, GetFtpDetails().FtpPassword);

                using (conn)
                {
                    conn.Connect();
                    foreach(FtpListItem names in conn.GetListing(GetFtpDetails().FtpMainFolder))
                    {
                        await Console.Out.WriteLineAsync(names.FullName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at GetFtpFolderList: {ex.Message}", 0);
            }
        }
    }
}
