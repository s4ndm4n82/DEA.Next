using UserConfigSetterClass;

namespace UserConfigRetriverClass
{
    internal class UserConfigRetriver
    {
        public static async Task<UserConfigSetterClass.UserConfigSetter.Customerdetail> RetriveUserConfigById(int cid)
        {
            if (cid == 0)
            {
                throw new ArgumentException("Customer ID cannot be 0");
            }

            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetterClass.UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);

            return customerData;
        }

        public static async Task<UserConfigSetterClass.UserConfigSetter.Ftpdetails> RetriveFtpConfigById(int cid)
        {
            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetterClass.UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetterClass.UserConfigSetter.Ftpdetails ftpData = customerData.FtpDetails;

            return ftpData;
        }

        public static async Task<UserConfigSetterClass.UserConfigSetter.Emaildetails> RetriveEmailConfigById(int cid)
        {
            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonData = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetterClass.UserConfigSetter.Customerdetail customerData = jsonData.CustomerDetails.FirstOrDefault(id => id.Id == cid);
            UserConfigSetterClass.UserConfigSetter.Emaildetails emailData = customerData.EmailDetails;

            return emailData;
        }
        
        public static async Task<UserConfigSetterClass.UserConfigSetter.Customerdetail[]> RetriveAllUserConfig()
        {
            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();

            return jsonAllData.CustomerDetails;
        }

        public static async Task<UserConfigSetterClass.UserConfigSetter.Ftpdetails[]> RetriveAllFtpConfig()
        {
            UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject jsonAllData = await UserConfigSetterClass.UserConfigSetter.ReadUserDotConfigAsync<UserConfigSetterClass.UserConfigSetter.CustomerDetailsObject>();
            UserConfigSetterClass.UserConfigSetter.Customerdetail[] customerDetails = jsonAllData.CustomerDetails;
            UserConfigSetterClass.UserConfigSetter.Ftpdetails[] ftpdetails = customerDetails.Select(details => details.FtpDetails).ToArray();

            return ftpdetails;
        }
    }
}
