using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class GraphDeleteMessages
    {
        /// <summary>
        /// If the conversation has more than 1 reply, move the message to deleted items
        /// </summary>
        /// <param name="requestBuilder"></param>
        /// <param name="deletedItemsId"></param>
        /// <param name="allMessages"></param>
        /// <returns>Returns a bool value.</returns>
        public static async Task<bool> GraphMoveToDeletedItemsAsync(IMailFolderRequestBuilder requestBuilder,
                                                                string deletedItemsId,
                                                                IEnumerable<Message> allMessages)
        {
            // Check if the conversation is not empty.
            if (!allMessages.Any())
            {
                WriteLogClass.WriteToLog(0, "Conversation messages are empty ....", 0);
                return false;
            }

            // Return result.
            bool result = true;

            // Move the message to deleted items.
            foreach (Message message in allMessages)
            {
                try
                {
                   await requestBuilder
                         .Messages[message.Id]
                         .Move(deletedItemsId)
                         .Request()
                         .PostAsync();
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
