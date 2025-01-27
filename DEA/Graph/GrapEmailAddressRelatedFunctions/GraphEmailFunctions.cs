using System.Net.Mail;
using DEA.Next.Graph.ResourceFiles;
using Microsoft.Graph;
using WriteLog;

namespace GraphEmailFunctions;

/// <summary>
///     Mainly contains all the function for email actions like sending emails and forwarding errored emails.
/// </summary>
internal class GraphEmailFunctionsClass
{
    /// <summary>
    ///     Get's all needed details from the received email. So, the process can forward the email to the appropriate sender.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <param name="clientEmail"></param>
    /// <param name="attachmentStatus"></param>
    /// <returns></returns>
    public static async Task<(bool, string)> EmailForwarder(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        string clientEmail,
        int attachmentStatus)
    {
        try
        {
            // Get message details
            var messageDetails = await GetEmailMessageDetails(requestBuilder, messageId);

            // Get sender details
            var fromName = messageDetails.From.EmailAddress.Name;
            var fromEmail = messageDetails.From.EmailAddress.Address;
            var replyEmail = ExtractReplyEmail(messageDetails);

            // Check for null or whitespace
            if (string.IsNullOrEmpty(fromName) && string.IsNullOrEmpty(fromEmail) && string.IsNullOrEmpty(replyEmail))
                return (false, "From name, email or reply email cannot be null or whitespace.");

            // Forward the email
            var returnResult = await SendForwardEmail(requestBuilder,
                fromName,
                fromEmail,
                clientEmail,
                messageId,
                attachmentStatus);
            // Return the result and reply email
            return (returnResult, replyEmail);
        }
        catch (Exception ex)
        {
            return (false, $"Exception error thrown: {ex.Message}");
        }
    }

    /// <summary>
    ///     Get's the email message details.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <returns>Returns all the email message details.</returns>
    private static async Task<Message> GetEmailMessageDetails(IMailFolderRequestBuilder requestBuilder,
        string messageId)
    {
        Message messagesDetails;
        try
        {
            messagesDetails = await requestBuilder.Messages[$"{messageId}"].Request().GetAsync();
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at GetEmailMessageDetails: {ex.Message}", 0);
            throw;
        }

        return messagesDetails;
    }

    /// <summary>
    ///     Extracts the reply email.
    /// </summary>
    /// <param name="messageDetails"></param>
    /// <returns>Return the extracted reply email.</returns>
    private static string ExtractReplyEmail(Message messageDetails)
    {
        return messageDetails.ToRecipients
            .Select(e => e.EmailAddress.Address)
            .FirstOrDefault(email => email.Contains("@efakturamottak.no")) ?? string.Empty;
    }

    /// <summary>
    ///     Forwards the email and flags the email to be moved to the error folder.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="fromName"></param>
    /// <param name="fromEmail"></param>
    /// <param name="recipientEmail"></param>
    /// <param name="messageId"></param>
    /// <param name="attachmentStatus"></param>
    /// <returns></returns>
    private static async Task<bool> SendForwardEmail(IMailFolderRequestBuilder requestBuilder,
        string fromName,
        string fromEmail,
        string recipientEmail,
        string messageId,
        int attachmentStatus)
    {
        // Validate the parameters
        if (!ValidDateParameters(fromName, fromEmail, recipientEmail, messageId)) return false;

        // Create the body of the mail
        var mailBody = CreateMailBody(attachmentStatus, recipientEmail);

        // List of recipients emails.
        var recipients = GetRecipientEmail(fromName, fromEmail);

        // Send the email
        return await SendEmailAsync(requestBuilder,
                recipients,
                messageId,
                mailBody)
            .ConfigureAwait(false);
    }

    /// <summary>
    ///     Validate the parameters. Sent to EmailForwarder.
    /// </summary>
    /// <param name="senderName"></param>
    /// <param name="senderEmail"></param>
    /// <param name="recipientEmail"></param>
    /// <param name="messageId"></param>
    /// <returns>Return true or false according to the results.</returns>
    private static bool ValidDateParameters(string senderName, string senderEmail, string recipientEmail,
        string messageId)
    {
        // Validate the parameters
        if (string.IsNullOrEmpty(senderName) || string.IsNullOrEmpty(senderEmail) ||
            string.IsNullOrEmpty(recipientEmail) || string.IsNullOrEmpty(messageId))
        {
            WriteLogClass.WriteToLog(0, "One of the parameters is null or whitespace", 0);
            return false;
        }

        // Validate the email
        if (IsValidEmail(senderEmail) && IsValidEmail(recipientEmail)) return true;
        WriteLogClass.WriteToLog(0, "One of the parameters is not a valid email", 0);
        return false;
    }

    /// <summary>
    ///     Validate the email and return true or false
    /// </summary>
    /// <param name="email"></param>
    /// <returns>Return true if email is legit,</returns>
    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    ///     Creates the forward email message body.
    /// </summary>
    /// <param name="attachmentStatus"></param>
    /// <param name="recipientEmail"></param>
    /// <returns>Return the email body.</returns>
    private static string CreateMailBody(int attachmentStatus, string recipientEmail)
    {
        try
        {
            var creatingMailBody = attachmentStatus != 1
                ? EmailMessageBodyText.EmailTemplateWithoutAttachment
                : EmailMessageBodyText.EmailTemplateInvalidAttachment;

            var mailBody = string.Format(creatingMailBody, recipientEmail);

            return mailBody;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at CreateMailBody: {ex.Message}", 0);
            return string.Empty;
        }
    }

    /// <summary>
    ///     Send the email back to the sender.
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="recipientEmail"></param>
    /// <param name="messageId"></param>
    /// <param name="mailBody"></param>
    /// <returns>Return tru or false.</returns>
    private static async Task<bool> SendEmailAsync(IMailFolderRequestBuilder requestBuilder,
        List<Recipient> recipientEmail,
        string messageId,
        string mailBody)
    {
        try
        {
            await requestBuilder
                .Messages[$"{messageId}"]
                .Forward(recipientEmail, null, mailBody)
                .Request()
                .PostAsync(); // Send the message
            return true;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SendEmailAsync: {ex.Message}", 0);
            return false;
        }
    }

    /// <summary>
    ///     Create the recipient email list.
    /// </summary>
    /// <param name="senderName"></param>
    /// <param name="senderEmail"></param>
    /// <returns>Returns the recipient email.</returns>
    private static List<Recipient> GetRecipientEmail(string senderName, string senderEmail)
    {
        List<Recipient> recipientEmails = new()
        {
            new Recipient
            {
                EmailAddress = new EmailAddress
                {
                    Name = senderName,
                    Address = senderEmail
                }
            }
        };
        return recipientEmails;
    }
}