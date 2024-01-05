using WriteLog;
using Microsoft.Graph;
using System.Text.RegularExpressions;

namespace GetRecipientEmail
{
    internal class GetRecipientEmailClass
    {
        /// <summary>
        /// Get the recipient email from the "InternetMessageHeaders".
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="MessageID"></param>
        /// <returns></returns>
        public static async Task<string> GetRecipientEmail(IMailFolderRequestBuilder requestBuilder,
                                                           string MessageID)
        {
            try
            {
                // Check for null or whitespace.
                if (requestBuilder == null)
                {
                    WriteLogClass.WriteToLog(0, "Main folder ID cannot be null or whitespace", 0);
                    return string.Empty;
                }

                // Get message details.
                Message emailMessages = await requestBuilder
                                              .Messages[$"{MessageID}"]
                                              .Request()
                                              .Select(eml => new
                                              {
                                                  eml.InternetMessageHeaders
                                              })
                                              .GetAsync();

                if (!emailMessages.InternetMessageHeaders.Any())
                {
                    WriteLogClass.WriteToLog(0, "InternetMessageHeaders is null ....", 0);
                    return "";
                }

                // RegEx to get the recipient email.
                Regex emailRegExPattern = new(@"[0-9a-z]+@efakturamottak\.no", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

                // Get the recipient email.
                string recipientEmail = emailMessages
                                       .InternetMessageHeaders
                                       .SelectMany(header => emailRegExPattern.Matches(header.Value))
                                       .Where(match => match.Success)
                                       .Select(match => match.Value.ToLower())
                                       .FirstOrDefault(email => email != null);

                if (string.IsNullOrEmpty(recipientEmail))
                {
                    WriteLogClass.WriteToLog(0, "Recipient email is empty ....", 0);
                    return string.Empty;
                }

                return recipientEmail;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at getting recipient email: {ex.Message}", 0);
                return string.Empty;
            }
        }
    }
}
