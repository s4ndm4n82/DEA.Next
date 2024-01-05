using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class GraphDeleteMessages
    {
        public static async Task<bool> GraphDeleteMessagesAsync(IMailFolderRequestBuilder requestBuilder,
                                                                string conversationId)
        {
            IMailFolderMessagesCollectionPage messagesInConversation = await requestBuilder
                                                                             .Messages
                                                                             .Request()
                                                                             .Filter($"conversationId eq '{conversationId}'")
                                                                             .GetAsync();
            bool result = true;

            foreach (Message message in messagesInConversation)
            {
                try
                {
                    await requestBuilder.Messages[message.Id].Request().DeleteAsync();
                }
                catch (Exception ex)
                {
                    WriteLogClass.WriteToLog(0, $"Exception at deleting messages: {ex.Message}", 0);
                    result = false;
                    break;
                }
            }

            return result;
        }
    }
}
