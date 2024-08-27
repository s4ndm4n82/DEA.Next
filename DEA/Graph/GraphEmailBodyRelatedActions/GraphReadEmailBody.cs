using DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using GetRecipientEmail;
using Microsoft.Graph;
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
            int result = 0;

            try
            {
                int loopCount = 0;

                UserConfigSetter.Emaildetails emailDetails = await UserConfigRetriever.RetrieveEmailConfigById(clientId);

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
                                             MessageSubject = x.Subject,
                                             MessageBody = x.Body.Content
                                         })
                                         .ToList();

                foreach (MessageDetails messageDetail in messageDetails)
                {
                    string toEmail = await GetRecipientEmailClass.GetRecipientEmail(requestBuilder, messageDetail.MessageId);                    

                    if (emailDetails.EmailList.Any(y => y == toEmail))
                    {
                        string emailSubject = messageDetail.MessageSubject;
                        WriteLogClass.WriteToLog(1, $"Reading email {messageDetail.MessageSubject} ....", 1);

                        result = await MakeJsonRequestSendBodyTextFunction.MakeJsonRequestSendBodyTextAsync(requestBuilder,
                                                                                                            messageDetail.MessageId,
                                                                                                            messageDetail.MessageSubject,
                                                                                                            messageDetail.MessageBody,
                                                                                                            clientId);

                        loopCount++;
                    }
                }

                if (messageDetails.Count == loopCount)
                {
                    WriteLogClass.WriteToLog(1, "Email body sent to system successfully ....", 2);
                    return result;
                }

                WriteLogClass.WriteToLog(1, "Email body not sent to system ....", 2);
                return result;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Read Email Body: {ex.Message}....", 0);
                return result;
            }
        }

        private class MessageDetails
        {
            public string MessageId { get; set; }
            public string MessageSubject { get; set; }
            public string MessageBody { get; set; }
        }
    }
}
