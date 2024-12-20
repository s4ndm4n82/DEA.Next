using DEA.Next.Entities;
using Newtonsoft.Json;
using WriteLog;

namespace DEA.Next.Extensions;

public class ReadDataFromJson
{
    private class CustomerConfig
    {
        public required List<CustomerDetails> CustomerDetails { get; set; }
        
        public void InitializeCustomerDetails(List<CustomerDetails> details)
        {
            CustomerDetails = details;
        }
    }
    
    public static async Task<List<CustomerDetails>> ReadDataFromJsonConfig()
    {
        try
        {
            using var customerStream = new StreamReader("./Config/CustomerConfig.json");
            var customerData = await customerStream.ReadToEndAsync();
            var settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                ObjectCreationHandling = ObjectCreationHandling.Replace,
                DefaultValueHandling = DefaultValueHandling.Populate
            };
            var customerConfigs = JsonConvert.DeserializeObject<CustomerConfig>(customerData, settings);

            if (customerConfigs != null) return customerConfigs.CustomerDetails;
        
            WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 1);
            return [];
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0,
                $"Exception at reading customer data: {e.Message} ....",
                0);
            throw;
        }
    }
}