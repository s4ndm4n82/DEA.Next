using GetMailFolderIds;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class CheckEmailChain
    {
        public static async Task<bool> CheckEmailChainAsync(IMailFolderRequestBuilder requestBuilder,
                                                            string messageId,
                                                            string deletedItemsId)
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
            
            if (!await GetConversationCountAsync(requestBuilder, conversationId, deletedItemsId))
            {
                return false;
            }

            return true;
        }

        private static async Task<bool> GetConversationCountAsync(IMailFolderRequestBuilder requestBuilder,
                                                                  string conversationId,
                                                                  string deletedItemsId)
        {
            IMailFolderMessagesCollectionPage messagesInConversation = await requestBuilder
                                                                             .Messages
                                                                             .Request()
                                                                             .Filter($"conversationId eq '{conversationId}'")
                                                                             .GetAsync();

            string errorFolderId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);

            IMailFolderMessagesCollectionPage conversationsInErrorFolder = await requestBuilder
                                                                                 .ChildFolders[errorFolderId]
                                                                                 .Messages
                                                                                 .Request()
                                                                                 .Filter($"conversationId eq '{conversationId}'")
                                                                                 .GetAsync();

            int totalMessagesInConversation = messagesInConversation.Count + conversationsInErrorFolder.Count;

            IEnumerable<Message> allMessages = messagesInConversation.Concat(conversationsInErrorFolder);

            if (messagesInConversation == null)
            {
                WriteLogClass.WriteToLog(0, "MessagesInConversation was not found ....", 0);
                return false;
            }

            if (totalMessagesInConversation > 1)
            {
                await GraphDeleteMessages.GraphMoveToDeletedItemsAsync(requestBuilder, deletedItemsId, allMessages);
                return true;
            }

            return false;
        }
    }
}
