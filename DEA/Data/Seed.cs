using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;
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
                
                switch (customer.FileDeliveryMethod.ToLower())
                {
                    case MagicWords.Ftp:
                        if (customer.FtpDetails != null)
                        {
                            var ftpDetails = new FtpDetails
                            {
                                FtpType = customer.FtpDetails.FtpType,
                                FtpProfile = customer.FtpDetails.FtpProfile,
                                FtpHost = customer.FtpDetails.FtpHost,
                                FtpUser = customer.FtpDetails.FtpUser,
                                FtpPassword = customer.FtpDetails.FtpPassword,
                                FtpPort = customer.FtpDetails.FtpPort,
                                FtpFolderLoop = customer.FtpDetails.FtpFolderLoop,
                                FtpMoveToSubFolder = customer.FtpDetails.FtpMoveToSubFolder,
                                FtpMainFolder = customer.FtpDetails.FtpMainFolder,
                                FtpSubFolder = customer.FtpDetails.FtpSubFolder,
                                FtpRemoveFiles = customer.FtpDetails.FtpRemoveFiles,
                                CustomerDetailsId = customer.Id
                            };

                            context.FtpDetails.Add(ftpDetails);
                        }

                        break;

                    case MagicWords.Email:
                        if (customer.EmailDetails != null)
                        {
                            var emailDetails = new EmailDetails
                            {
                                Email = customer.EmailDetails.Email,
                                EmailInboxPath = customer.EmailDetails.EmailInboxPath,
                                SendSubject = customer.EmailDetails.SendSubject,
                                SendEmail = customer.EmailDetails.SendEmail,
                                CustomerDetailsId = customer.Id
                            };

                            context.EmailDetails.Add(emailDetails);
                        }

                        break;
                    
                    default:
                        WriteLogClass.WriteToLog(0, "File delivery method not found ....", 1);
                        break;
                }
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