using DEA.Next.Data;
using DEA.Next.HelperClasses.OtherFunctions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RunTimedFunctions;
using WriteLog;

namespace DEA.Next.Extensions;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        try
        {
            // Get the DataContext service and apply any pending migrations.
            var context = services.GetRequiredService<DataContext>();
            await context.Database.MigrateAsync();

            // Seed the database with initial data. If the data already exists, skip the seeding process.
            if (!context.CustomerDetails.Any()) await Seed.SeedData(context);

            if (!context.CustomerDetails.Any())
            {
                WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 0);
                return;
            }

            // Log that the download process is starting.
            WriteLogClass.WriteToLog(1, "Starting download process ....", 1);

            // Get the ProcessStartupFunctionsClass service.
            var processStartupFunctions = services.GetService<ProcessStartupFunctionsClass>();

            // If the service is available, start the process.
            if (processStartupFunctions != null) await ProcessStartupFunctionsClass.StartupProcess();

            // Call the DEA cleaner timed processes.
            RunTimedFunctionsClass.CallDeaTimedProcesses("deacleaner");
        }
        catch (Exception e)
        {
            // Log any exceptions that occur during the migration or seeding process.
            WriteLogClass.WriteToLog(0, $"Exception at seed data: {e.Message} ....", 0);
            throw;
        }
    }
}