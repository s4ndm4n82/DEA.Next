using DEA.Next.Graph.ResourceFiles;
using Microsoft.Graph;
using System.Diagnostics.CodeAnalysis;
using WriteLog;

namespace GraphEmailFunctions
{
    /// <summary>
    /// Mainly contains all the function for email actions like sending emails and forwarding errored emails.
    /// </summary>
    internal class GraphEmailFunctionsClass
    {
        /// <summary>
        /// Get's all needed details from the recived email. So, the process can forward the email to the appropriate sender.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <param name="clientEmail"></param>
        /// <param name="atnStatus"></param>
        /// <returns></returns>
        public static async Task<(bool, string)> EmailForwarder([NotNull] GraphServiceClient graphClient,
                                                                string mainFolderId,
                                                                string subFolderId1,
                                                                string subFolderId2,
                                                                string messageId,
                                                                string clientEmail,
                                                                int atnStatus)
        {            
            bool returnResult;

            // Check for null or whitespace/
            if (string.IsNullOrEmpty(mainFolderId))
            {
                return (false, "Main folder ID cannot be null or whitespace.");
            }

            try
            {
                // Get message details
                Message messageDetails = await GetEmailMessageDetails(graphClient,
                                                                      clientEmail,
                                                                      mainFolderId,
                                                                      subFolderId1,
                                                                      subFolderId2,
                                                                      messageId);
                // Return if message details is null
                if (messageDetails == null)
                {
                    return (false, "Failed to retrieve message details.");
                }

                // Get sender details
                string fromName = messageDetails.From.EmailAddress.Name; 
                string fromEmail = messageDetails.From.EmailAddress.Address;
                string replyEmail = ExtractReplyEmail(messageDetails);

                // Check for null or whitespace
                if (string.IsNullOrEmpty(fromName) && string.IsNullOrEmpty(fromEmail) && string.IsNullOrEmpty(replyEmail))
                {
                    return (false, "From name, email or reply email cannot be null or whitespace.");
                }

                // Forward the email
                returnResult = await SendForwardEmail(graphClient,
                                                      mainFolderId,
                                                      subFolderId1,
                                                      subFolderId2,
                                                      fromName,
                                                      fromEmail,
                                                      replyEmail,
                                                      clientEmail,
                                                      messageId,
                                                      atnStatus);
                // Return the result and reply email
                return (returnResult, replyEmail);
            }
            catch (Exception ex)
            {   
                return (false, $"Exception error thrown: {ex.Message}");
            }
        }

        /// <summary>
        /// Get's the email message details.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="clientEmail"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="messageId"></param>
        /// <returns>Returns all the email message details.</returns>
        private static async Task<Message> GetEmailMessageDetails([NotNull] GraphServiceClient graphClient,
                                                                  string clientEmail,
                                                                  string mainFolderId,
                                                                  string subFolderId1,
                                                                  string subFolderId2,
                                                                  string messageId)
        {
            List<string> subInboxFolders = new() { mainFolderId, subFolderId1, subFolderId2 };
            subInboxFolders.RemoveAll(string.IsNullOrWhiteSpace); // Remove all empty strings

            if (!subInboxFolders.Any())
            {
                WriteLogClass.WriteToLog(0, "Inbox folder list cannot be empty ....", 0);
                return null;
            }

            IMailFolderRequestBuilder requestBuilder = graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"];

            foreach (string subInboxFolder in subInboxFolders)
            {
                requestBuilder = requestBuilder.ChildFolders[subInboxFolder];
            }

            Message messagesDetails;
            try
            {
                messagesDetails = await requestBuilder.Messages[$"{messageId}"].Request().GetAsync();
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at GetEmailMessageDetails: {ex.Message}", 0);
                return null;
            }

            return messagesDetails;
        }

        /// <summary>
        /// Extracts the reply email.
        /// </summary>
        /// <param name="messageDetails"></param>
        /// <returns>Return the extracted reply email.</returns>
        private static string ExtractReplyEmail(Message messageDetails)
        {
            return messageDetails.ToRecipients
                              .Select(e => e.EmailAddress.Address)
                              .FirstOrDefault(email => email.Contains("@efakturamottak.no"));
        }

        /// <summary>
        /// Forwards the email and flags the email to be moved to the error folder.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="fromName"></param>
        /// <param name="fromEmail"></param>
        /// <param name="clientEmail"></param>
        /// <param name="inEmail"></param>
        /// <param name="messageId"></param>
        /// <param name="attachmentStatus"></param>
        /// <returns></returns>
        private static async Task<bool> SendForwardEmail([NotNull] GraphServiceClient graphClient,
                                                         string mainFolderId,
                                                         string subFolderId1,
                                                         string subFolderId2,
                                                         string fromName,
                                                         string fromEmail,
                                                         string clientEmail,
                                                         string recipientEmail,
                                                         string messageId,
                                                         int attachmentStatus)
        {
            // Validate the parameters
            if (!ValiDateParameters(fromName, fromEmail, recipientEmail, messageId))
            {
                return false;
            }

            // Create the body of the mail
            string mailBody = CreateMailBody(attachmentStatus, recipientEmail);
            
            // List of recipients emails.
            List<Recipient> recipients = GetRecipeintEmail(fromName, fromEmail);

            // Send the email
            return await SendEmailAsync(graphClient,
                                        mainFolderId,
                                        subFolderId1,
                                        subFolderId2,
                                        recipients,
                                        clientEmail,
                                        messageId,
                                        mailBody)
                         .ConfigureAwait(false);
        }

        /// <summary>
        /// Validate the parameters. Sent to EmailForwarder.
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <param name="recipientEmail"></param>
        /// <param name="messageId"></param>
        /// <returns>Return true or false according to the results.</returns>
        private static bool ValiDateParameters(string senderName, string senderEmail, string recipientEmail, string messageId)
        {
            // Validate the parameters
            if (string.IsNullOrEmpty(senderName) || string.IsNullOrEmpty(senderEmail) ||
                string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(messageId))
            {
                WriteLogClass.WriteToLog(0, "One of the parameters is null or whitespace", 0);
                return false;
            }
            // Validate the email
            if (!IsValidEmail(senderEmail) || !IsValidEmail(recipientEmail))
            {
                WriteLogClass.WriteToLog(0, "One of the parameters is not a valid email", 0);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validate the email and return true or false
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Return true if email is legit,</returns>
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates the forward email message body.
        /// </summary>
        /// <param name="attachmentStatus"></param>
        /// <param name="recipientEmail"></param>
        /// <returns>Return the email body.</returns>
        private static string CreateMailBody(int attachmentStatus, string recipientEmail)
        {
            string creatingMailBody = attachmentStatus != 1
                ? EmailMessageBodyText.EmailTemplateWithoutAttachment
                : EmailMessageBodyText.EmailTemplateInvalidAttachment;

            string mailBody = string.Format(creatingMailBody, recipientEmail);

            return mailBody;
        }

        /// <summary>
        /// Send the email back to the sender.
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="mainFolderId"></param>
        /// <param name="subFolderId1"></param>
        /// <param name="subFolderId2"></param>
        /// <param name="recipientEmail"></param>
        /// <param name="clientEmail"></param>
        /// <param name="messageId"></param>
        /// <param name="mailBody"></param>
        /// <returns>Return tru or false.</returns>
        private static async Task<bool> SendEmailAsync([NotNull] GraphServiceClient graphClient,
                                                                        string mainFolderId,
                                                                        string subFolderId1,
                                                                        string subFolderId2,
                                                                        List<Recipient> recipientEmail,
                                                                        string clientEmail,
                                                                        string messageId,
                                                                        string mailBody)
        {
            List<string> subInboxFolderIds = new() { mainFolderId, subFolderId1, subFolderId2 };
            subInboxFolderIds.RemoveAll(string.IsNullOrWhiteSpace); // Remove all empty strings

            if (!subInboxFolderIds.Any())
            {
                WriteLogClass.WriteToLog(0, "Inbox folder list cannot be empty ....", 0);
                return false;
            }

            IMailFolderRequestBuilder requestBuilder = graphClient.Users[$"{clientEmail}"].MailFolders["Inbox"];

            foreach (var folderId in subInboxFolderIds)
            {
                requestBuilder = requestBuilder.ChildFolders[folderId];
            }

            try
            {
                await requestBuilder
                      .Messages[$"{messageId}"]
                      .Forward(recipientEmail, null, mailBody)
                      .Request()
                      .PostAsync(); // Send the messag
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at SendEmailAsync: {ex.Message}", 0);
                return false;
            }
        }

        /// <summary>
        /// Create the recipient email list.
        /// </summary>
        /// <param name="senderName"></param>
        /// <param name="senderEmail"></param>
        /// <returns>Returns the recipient email.</returns>
        private static List<Recipient> GetRecipeintEmail(string senderName, string senderEmail)
        {
            List<Recipient> recipientEmails = new()
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Name = senderName,
                        Address = senderEmail,
                    }
                }
            };
            return recipientEmails;
        }
    }
}