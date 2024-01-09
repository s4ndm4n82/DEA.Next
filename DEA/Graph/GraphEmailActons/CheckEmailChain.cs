using GetMailFolderIds;
using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class CheckEmailChain
    {
        /// <summary>
        /// Check if the email without attachment has more than 1 reply.
        /// </summary>
        /// <param name="requestBuilder">Graph request.</param>
        /// <param name="messageId">Email message ID.</param>
        /// <param name="deletedItemsId">Deleted items folder ID.</param>
        /// <returns>Returns a bool value.</returns>
        public static async Task<bool> CheckEmailChainAsync(IMailFolderRequestBuilder requestBuilder,
                                                            string messageId,
                                                            string deletedItemsId)
        {
            // Get the message.
            Message message = await requestBuilder
                                .Messages[$"{messageId}"]
                                .Request()
                                .GetAsync();

            // Check if the message was found.
            if (message == null)
            {
                WriteLogClass.WriteToLog(0, "Message was not found ....", 0);
                return false;
            }

            // Get the conversation ID.
            string conversationId = message.ConversationId;

            // Check the number of messages in the conversation.
            return await GetConversationCountAsync(requestBuilder,
                                                   conversationId,
                                                   deletedItemsId);
            
        }

        /// <summary>
        /// Check the amount of messages in a conversation thread.
        /// </summary>
        /// <param name="requestBuilder">Graph request.</param>
        /// <param name="conversationId">Conversation ID.</param>
        /// <param name="deletedItemsId">Deleted items folder.</param>
        /// <returns>Returna a bool value.</returns>
        private static async Task<bool> GetConversationCountAsync(IMailFolderRequestBuilder requestBuilder,
                                                                  string conversationId,
                                                                  string deletedItemsId)
        {
            // Get all messages in the conversation. From clients child folders.
            IMailFolderMessagesCollectionPage messagesInConversation = await requestBuilder
                                                                             .Messages
                                                                             .Request()
                                                                             .Filter($"conversationId eq '{conversationId}'")
                                                                             .GetAsync();

            // Get the error folder ID.
            string errorFolderId = await GetMailFolderIdsClass.GetErrorFolderId(requestBuilder);

            // Get all messages in the error folder. Equal to the conversation ID.
            IMailFolderMessagesCollectionPage conversationsInErrorFolder = await requestBuilder
                                                                                 .ChildFolders[errorFolderId]
                                                                                 .Messages
                                                                                 .Request()
                                                                                 .Filter($"conversationId eq '{conversationId}'")
                                                                                 .GetAsync();

            // Get the total number of messages in the conversation.
            int totalMessagesInConversation = messagesInConversation.Count + conversationsInErrorFolder.Count;

            // Get all messages in the conversation and error folder.
            IEnumerable<Message> allMessages = messagesInConversation.Concat(conversationsInErrorFolder);

            // Check if the messages were found.
            if (!allMessages.Any())
            {
                WriteLogClass.WriteToLog(0, "MessagesInConversation was not found ....", 0);
                return false;
            }

            // Check if the conversation has more than 1 reply.
            if (totalMessagesInConversation > 1)
            {
                // If there's more than 1 reply, move the message to the Deleted Items folder.
                return await GraphDeleteMessages.GraphMoveToDeletedItemsAsync(requestBuilder, deletedItemsId, allMessages);
            }

            return false;
        }
    }
}