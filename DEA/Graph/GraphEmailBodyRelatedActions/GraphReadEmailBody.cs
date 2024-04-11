using GetRecipientEmail;
using Microsoft.Graph;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailBodyRelatedActions
{
    internal class GraphReadEmailBody
    {
        public static async Task<int> ReadEmailBodyAsync(IMailFolderRequestBuilder requestBuilder,
                                                         int maxEmails,
                                                         int clientId)
        {
            try
            {
                UserConfigSetter.Emaildetails emailDetails = await UserConfigRetriver.RetriveEmailConfigById(clientId);

                IMailFolderMessagesCollectionPage messages = await requestBuilder
                                                                  .Messages
                                                                  .Request()
                                                                  .Select("body")
                                                                  .Top(maxEmails)
                                                                  .GetAsync();

                List<MessageDetails> messageDetails = messages
                                         .Where(x => x.Body.ContentType == BodyType.Html)
                                         .Select(x => new MessageDetails
                                         {
                                             MessageId = x.Id,
                                             MessageBody = x.Body.Content
                                         })
                                         .ToList();

                foreach (MessageDetails messageDetail in messageDetails)
                {
                    string toEmail = await GetRecipientEmailClass.GetRecipientEmail(requestBuilder, messageDetail.MessageId);
                    if (emailDetails.EmailList.Any(y => y == toEmail))
                    {
                        await Console.Out.WriteLineAsync(messageDetail.MessageBody);
                    }
                    
                    WriteLogClass.WriteToLog(1, "Email list end ....", 1);
                    break;
                }
                return 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, "Exception at Read Email Body ....", 0);
            }
            return 0;
        }

        private class MessageDetails
        {
            public string MessageId { get; set; }
            public string MessageBody { get; set; }
        }
    }
}
