using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class CheckEmailChain
    {
        public static async Task<bool> CheckEmailChainAsync(IMailFolderRequestBuilder requestBuilder,
                                                            string messageId,
                                                            int customerId)
        {
            Message message = await requestBuilder
                                .Messages[$"{messageId}"]
                                .Request()
                                .GetAsync();

            if (message == null)
            {
                WriteLogClass.WriteToLog(0, "Message was not found ....", 0);
                return false;
            }

            string conversationId = message.ConversationId;           
            
            if (!await GetConversationCountAsync(requestBuilder, conversationId))
            {
                return false;
            }

            if (!await GraphDeleteMessages.GraphDeleteMessagesAsync(requestBuilder, conversationId))
            {
                WriteLogClass.WriteToLog(0, "Messages were not deleted ....", 0);
                return false;
            }

            return true;
        }

        private static async Task<bool> GetConversationCountAsync(IMailFolderRequestBuilder requestBuilder,
                                                                  string conversationId)
        {
            IMailFolderMessagesCollectionPage messagesInConversation = await requestBuilder
                                                                             .Messages
                                                                             .Request()
                                                                             .Filter($"conversationId eq '{conversationId}'")
                                                                             .GetAsync();
            if (messagesInConversation == null)
            {
                WriteLogClass.WriteToLog(0, "MessagesInConversation was not found ....", 0);
                return false;
            }

            if (messagesInConversation.Count > 2)
            {
                return true;
            }

            return false;
        }
    }
}
