using DEA.Next.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WriteLog;

namespace DEA.Next.Data;

public class Seed
{
    public static async Task SeedData(DataContext context)
    {
        if (await context.CustomerDetails.AnyAsync()) return;
        
        using var customerStream = new StreamReader("./Config/CustomerConfig.json");
        var customerData = await customerStream.ReadToEndAsync();
        JsonConvert.DeserializeObject<List<CustomerDetails>>(customerData);
    }
}