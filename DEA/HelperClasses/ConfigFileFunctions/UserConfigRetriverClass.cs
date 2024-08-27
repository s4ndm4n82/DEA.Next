using UserConfigSetterClass;

namespace DEA.Next.HelperClasses.ConfigFileFunctions
{
    public class UserConfigRetriever
    {
        public static async Task<UserConfigSetter.Customerdetail> RetrieveUserConfigById(int? cid)
        {
            if (cid == 0)
            {
                throw new ArgumentException("Customer ID cannot be 0");
            }

            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);

            return customerData;
        }

        public static async Task<UserConfigSetter.Ftpdetails> RetrieveFtpConfigById(int cid)
        {
            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetter.Ftpdetails ftpData = customerData.FtpDetails;

            return ftpData;
        }

        public static async Task<UserConfigSetter.Emaildetails> RetrieveEmailConfigById(int cid)
        {
            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetter.Emaildetails emailData = customerData.EmailDetails;

            return emailData;
        }
        
        public static async Task<UserConfigSetter.Customerdetail[]> RetrieveAllUserConfig()
        {
            UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();

            return jsonAllData.CustomerDetails;
        }

        public static async Task<UserConfigSetter.Ftpdetails[]> RetrieveAllFtpConfig()
        {
            UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail[] customerDetails = jsonAllData.CustomerDetails;
            UserConfigSetter.Ftpdetails[] ftpdetails = customerDetails.Select(details => details.FtpDetails).ToArray();

            return ftpdetails;
        }

        public static async Task RetrieveUserConfigJson(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
