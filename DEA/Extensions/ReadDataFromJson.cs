using DEA.Next.Entities;
using Newtonsoft.Json;
using WriteLog;

namespace DEA.Next.Extensions;

public class ReadDataFromJson
{
    private class CustomerConfig
    {
        public required List<CustomerDetails> CustomerDetails { get; set; }
        
        // Initialize the customer details. Gets around a compiler warning. 
        public void InitializeCustomerDetails(List<CustomerDetails> details)
        {
            CustomerDetails = details;
        }
    } 
    
/// <summary>
/// Reads customer data from a JSON configuration file.
/// </summary>
/// <returns>A list of CustomerDetails objects.</returns>
public static async Task<List<CustomerDetails>> ReadDataFromJsonConfig()
{
    try
    {
        // Open the JSON configuration file for reading.
        using var customerStream = new StreamReader("./Config/CustomerConfig.json");
        
        // Read the entire content of the file.
        var customerData = await customerStream.ReadToEndAsync();
        
        // Define JSON serializer settings to handle missing members, null values, etc.
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            DefaultValueHandling = DefaultValueHandling.Populate
        };
        
        // Deserialize the JSON data into a CustomerConfig object.
        var customerConfigs = JsonConvert.DeserializeObject<CustomerConfig>(customerData, settings);

        // Check if the deserialization was successful and return the customer details.
        if (customerConfigs != null) return customerConfigs.CustomerDetails;
    
        // Log an error if the customer configurations are empty.
        WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 1);
        return [];
    }
    catch (Exception e)
    {
        // Log any exceptions that occur during the reading process.
        WriteLogClass.WriteToLog(0,
            $"Exception at reading customer data: {e.Message} ....",
            0);
        throw;
    }
}
}