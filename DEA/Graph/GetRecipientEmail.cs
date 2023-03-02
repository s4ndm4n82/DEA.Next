using WriteLog;
using Microsoft.Graph;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace GetRecipientEmail
{
    internal class GetRecipientEmailClass
    {
        public static string GetRecipientEmail(GraphServiceClient graphClient, string SubFolderId1, string SubFolderId2, string SubFolderId3, string MessageID, string _Email)
        {
            MailAddress rEmail = null!;
            IEnumerable<InternetMessageHeader> ToEmails;
            Task<Message> GetToEmail;

            try
            {
                if (string.IsNullOrEmpty(SubFolderId3) && string.IsNullOrEmpty(SubFolderId2))
                {
                    GetToEmail = graphClient.Users[$"{_Email}"].MailFolders["Inbox"]
                            .ChildFolders[$"{SubFolderId1}"]
                            .Messages[$"{MessageID}"]
                            .Request()
                            .Select(eml => new
                            {
                                eml.InternetMessageHeaders
                            })
                            .GetAsync();
                }
                else if (string.IsNullOrEmpty(SubFolderId3))
                {
                    GetToEmail = graphClient.Users[$"{_Email}"].MailFolders["Inbox"]
                            .ChildFolders[$"{SubFolderId1}"]
                            .ChildFolders[$"{SubFolderId2}"]
                            .Messages[$"{MessageID}"]
                            .Request()
                            .Select(eml => new
                            {
                                eml.InternetMessageHeaders
                            })
                            .GetAsync();
                }
                else
                {
                    GetToEmail = graphClient.Users[$"{_Email}"].MailFolders["Inbox"]
                            .ChildFolders[$"{SubFolderId1}"]
                            .ChildFolders[$"{SubFolderId2}"]
                            .ChildFolders[$"{SubFolderId3}"]
                            .Messages[$"{MessageID}"]
                            .Request()
                            .Select(eml => new
                            {
                                eml.InternetMessageHeaders
                            })
                            .GetAsync();
                }

                ToEmails = GetToEmail.Result.InternetMessageHeaders.Where(adrs => adrs.Value.ToLower().Contains("@efakturamottak.no"));

                foreach (var ToEmail in ToEmails)
                {
                    if (!string.IsNullOrEmpty(ToEmail.Value))
                    {
                        string RegExString = @"[0-9a-z]+@efakturamottak\.no";
                        Regex RecivedEmail = new(RegExString, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
                        var ExtractedEmail = RecivedEmail.Match(ToEmail.Value.ToLower());

                        if (ExtractedEmail.Success)
                        {
                            rEmail = new(ExtractedEmail.Value.ToLower().Replace(" ",""));
                            WriteLogClass.WriteToLog(1, $"Recipient email {rEmail} extracted ...", 2);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at getting recipient email: {ex.Message}", 0);
            }

            return rEmail.User;
        }
    }
}
