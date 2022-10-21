using UserConfigReader;
using WriteLog;

namespace FileFunctions
{
    internal class FileFunctionsClass
    {
        public static async Task<bool> SendToWebService(string filePath, int customerId)
        {
            UserConfigReaderClass.CustomerDetailsObject jsonData = await UserConfigReaderClass.ReadAppDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
            UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.id == customerId)!;

            string[] downloadedFiles = Directory.GetFiles(filePath);



            return true;
        }
    }
}
