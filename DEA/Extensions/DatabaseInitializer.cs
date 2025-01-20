using DEA.Next.Data;
using DEA.Next.HelperClasses.OtherFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WriteLog;

namespace DEA.Next.Extensions;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        try
        {
            var context = services.GetRequiredService<DataContext>();

            // Apply any pending migrations.
            await context.Database.MigrateAsync();

            // Seed the database with initial data. If the data already exists, skip the seeding process.
            if (!context.CustomerDetails.Any()) await Seed.SeedData(context);

            if (!context.CustomerDetails.Any())
            {
                WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 0);
                return;
            }

            WriteLogClass.WriteToLog(1, "Starting download process ....", 1);

            var processStartupFunctions = services.GetService<ProcessStartupFunctionsClass>();

            if (processStartupFunctions != null) await ProcessStartupFunctionsClass.StartupProcess();

            //RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0, $"Exception at seed data: {e.Message} ....", 0);
            throw;
        }
    }
}