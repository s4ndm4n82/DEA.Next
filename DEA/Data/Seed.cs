using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WriteLog;

namespace DEA.Next.Data;

public class Seed
{
    private class CustomerConfig
    {
        public required List<CustomerDetails> CustomerDetails { get; set; }
    }

    public static async Task SeedData(DataContext context)
    {
        if (await context.CustomerDetails.AnyAsync()) return;

        using var customerStream = new StreamReader("./Config/CustomerConfig.json");
        var customerData = await customerStream.ReadToEndAsync();
        var settings = new JsonSerializerSettings
        {
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            DefaultValueHandling = DefaultValueHandling.Populate
        };

        try
        {
            var customerConfigs = JsonConvert.DeserializeObject<CustomerConfig>(customerData, settings);

            if (customerConfigs == null)
            {
                WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 1);
                return;
            }

            var customerDetails = customerConfigs.CustomerDetails;

            foreach (var customer in customerDetails)
            {
                context.CustomerDetails.Add(customer);
            }

            await context.SaveChangesAsync();
        }
        catch (JsonSerializationException ex)
        {
            WriteLogClass.WriteToLog(0,
                "Error deserializing customer configuration: " + ex.Message,
                0);
            
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0,
                "An exception error occurred: " + ex.Message,
                0);
        }
    }
}