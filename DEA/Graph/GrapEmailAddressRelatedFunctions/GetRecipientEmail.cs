using System.Text.RegularExpressions;
using Microsoft.Graph;
using WriteLog;

namespace GetRecipientEmail;

internal partial class GetRecipientEmailClass
{
    /// <summary>
    ///     Get the recipient email from the "InternetMessageHeaders".
    /// </summary>
    /// <param name="requestBuilder"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public static async Task<string> GetRecipientEmail(IMailFolderRequestBuilder requestBuilder,
        string messageId,
        string msgName)
    {
        try
        {
            Console.Write(msgName);
            // Get message details.
            var emailMessages = await requestBuilder
                .Messages[messageId]
                .Request()
                .Select("InternetMessageHeaders")
                .GetAsync()
                .ConfigureAwait(false);

            if (!emailMessages.InternetMessageHeaders.Any())
            {
                WriteLogClass.WriteToLog(0, "InternetMessageHeaders is null ....", 0);
                return "";
            }

            // RegEx to get the recipient email.
            var emailRegExPattern = MyRegex();

            // Get the recipient email.
            var recipientEmail = emailMessages
                .InternetMessageHeaders
                .SelectMany(header => emailRegExPattern.Matches(header.Value))
                .Where(match => match.Success)
                .Select(match => match.Value.ToLower())
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(recipientEmail)) return recipientEmail;
            WriteLogClass.WriteToLog(0, "Recipient email is empty ....", 0);
            return string.Empty;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at getting recipient email: {ex.Message}", 0);
            return string.Empty;
        }
    }

    [GeneratedRegex(@"[0-9a-z]+@efakturamottak\.no", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex MyRegex();
}