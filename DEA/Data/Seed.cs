using DEA.Next.Entities;
using DEA.Next.Extensions;
using DEA.Next.HelperClasses.OtherFunctions;
using WriteLog;

namespace DEA.Next.Data;

public static class Seed
{
    /// <summary>
    ///     Seeds the database with customer data from a JSON configuration file.
    /// </summary>
    /// <param name="context">The data context to use for database operations.</param>
    public static async Task SeedData(DataContext context)
    {
        // Read customer data from the JSON configuration file.
        var customerData = await ReadDataFromJson.ReadDataFromJsonConfig();

        // Check if the customer data is empty and log an error if it is.
        if (customerData.Count == 0)
        {
            WriteLogClass.WriteToLog(0, "Customer configurations can't be empty ...", 1);
            return;
        }

        try
        {
            // Iterate through each customer in the customer data.
            foreach (var customer in customerData)
            {
                // Generate a new GUID for the customer ID.
                customer.Id = Guid.NewGuid();

                // Create a new CustomerDetails object and populate it with customer data.
                var customerDetails = new CustomerDetails
                {
                    Id = customer.Id,
                    CustomerName = customer.CustomerName,
                    Status = customer.Status,
                    UserName = customer.UserName,
                    Token = customer.Token,
                    Queue = customer.Queue,
                    ProjectId = customer.ProjectId,
                    TemplateKey = customer.TemplateKey,
                    DocumentId = customer.DocumentId,
                    DocumentEncoding = customer.DocumentEncoding,
                    MaxBatchSize = customer.MaxBatchSize,
                    FieldOneValue = customer.FieldOneValue,
                    FieldOneName = customer.FieldOneName,
                    FieldTwoValue = customer.FieldTwoValue,
                    FieldTwoName = customer.FieldTwoName,
                    Domain = customer.Domain,
                    FileDeliveryMethod = customer.FileDeliveryMethod,
                    DocumentDetails = customer.DocumentDetails,
                    CreatedDate = customer.CreatedDate.Ticks != 0 ? customer.CreatedDate : DateTime.UtcNow,
                    ModifiedDate = customer.ModifiedDate.Ticks != 0 ? customer.ModifiedDate : DateTime.UtcNow
                };

                // Add the CustomerDetails object to the context.
                context.CustomerDetails.Add(customerDetails);

                // Handle the file delivery method for the customer.
                switch (customer.FileDeliveryMethod.ToLower())
                {
                    case MagicWords.Ftp:
                        // Check if FTP details are provided and log an error if they are not.
                        if (customer.FtpDetails == null)
                        {
                            WriteLogClass.WriteToLog(0, "FTP details can't be empty ...", 1);
                            return;
                        }

                        // Create a new FtpDetails object and populate it with FTP data.
                        var ftpDetails = new FtpDetails
                        {
                            Id = Guid.NewGuid(),
                            CustomerDetailsId = customer.Id,
                            FtpType = customer.FtpDetails.FtpType,
                            FtpProfile = customer.FtpDetails.FtpProfile,
                            FtpHost = customer.FtpDetails.FtpHost,
                            FtpPort = customer.FtpDetails.FtpPort,
                            FtpUser = customer.FtpDetails.FtpUser,
                            FtpPassword = customer.FtpDetails.FtpPassword,
                            FtpFolderLoop = customer.FtpDetails.FtpFolderLoop,
                            FtpMainFolder = customer.FtpDetails.FtpMainFolder,
                            FtpMoveToSubFolder = customer.FtpDetails.FtpMoveToSubFolder,
                            FtpSubFolder = customer.FtpDetails.FtpSubFolder,
                            FtpRemoveFiles = customer.FtpDetails.FtpRemoveFiles
                        };

                        // Add the FtpDetails object to the context.
                        context.FtpDetails.Add(ftpDetails);
                        break;

                    case MagicWords.Email:
                        // Check if Email details are provided and log an error if they are not.
                        if (customer.EmailDetails == null)
                        {
                            WriteLogClass.WriteToLog(0, "Email details can't be empty ...", 1);
                            return;
                        }

                        // Create a new EmailDetails object and populate it with email data.
                        var emailDetails = new EmailDetails
                        {
                            Id = Guid.NewGuid(),
                            CustomerDetailsId = customer.Id,
                            Email = customer.EmailDetails.Email,
                            EmailInboxPath = customer.EmailDetails.EmailInboxPath,
                            SendEmail = customer.EmailDetails.SendEmail,
                            SendSubject = customer.EmailDetails.SendSubject,
                            SendBody = customer.EmailDetails.SendBody
                        };

                        // Add the EmailDetails object to the context.
                        context.EmailDetails.Add(emailDetails);
                        break;

                    default:
                        // Log an error if the file delivery method is not recognized.
                        WriteLogClass.WriteToLog(0, "File delivery method can't be empty ...", 1);
                        break;
                }
            }

            // Save all changes to the database.
            var saveResult = await context.SaveChangesAsync();

            if (saveResult > 0)
            {
                // Log that the customer data was successfully seeded.
                WriteLogClass.WriteToLog(1, "Customer data seeded successfully ....", 1);

                // Rename the config file to prevent reseeding.
                RenameConfigFile();
            }
        }
        catch (Exception e)
        {
            // Log any exceptions that occur during the seeding process.
            WriteLogClass.WriteToLog(0, $"Exception at seed data: {e.Message} ....", 0);
            throw;
        }
    }

    private static void RenameConfigFile()
    {
        const string oldConfigFile = "./Config/CustomerConfig.json";
        var newConfigFile = $"./Config/CustomerConfig_{DateTime.Now:yyyyMMddHHmmss}.bkp";
        try
        {
            if (!File.Exists(oldConfigFile)) return;
            File.Move(oldConfigFile, newConfigFile);
            File.Delete(oldConfigFile);
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0, $"Exception at renaming config file: {e.Message} ....", 0);
        }
    }
}