using UserConfigSetterClass;

namespace UserConfigRetriverClass
{
    internal class UserConfigRetriver
    {
        public static async Task<UserConfigSetter.Customerdetail> RetriveUserConfigById(int? cid)
        {
            if (cid == 0)
            {
                throw new ArgumentException("Customer ID cannot be 0");
            }

            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);

            return customerData;
        }

        public static async Task<UserConfigSetter.Ftpdetails> RetriveFtpConfigById(int cid)
        {
            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetter.Ftpdetails ftpData = customerData.FtpDetails;

            return ftpData;
        }

        public static async Task<UserConfigSetter.Emaildetails> RetriveEmailConfigById(int cid)
        {
            UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetter.Emaildetails emailData = customerData.EmailDetails;

            return emailData;
        }
        
        public static async Task<UserConfigSetter.Customerdetail[]> RetriveAllUserConfig()
        {
            UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();

            return jsonAllData.CustomerDetails;
        }

        public static async Task<UserConfigSetter.Ftpdetails[]> RetriveAllFtpConfig()
        {
            UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetter.Customerdetail[] customerDetails = jsonAllData.CustomerDetails;
            UserConfigSetter.Ftpdetails[] ftpdetails = customerDetails.Select(details => details.FtpDetails).ToArray();

            return ftpdetails;
        }

        public static async Task RetriveUserConfigJson(int clientId)
        {
            throw new NotImplementedException();
        }
    }
}
