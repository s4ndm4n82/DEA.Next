using Microsoft.Graph;
using WriteLog;

namespace DEA.Next.Graph.GraphEmailActons
{
    internal class GraphDeleteMessages
    {
        public static async Task<bool> GraphMoveToDeletedItemsAsync(IMailFolderRequestBuilder requestBuilder,
                                                                string deletedItemsId,
                                                                IEnumerable<Message> allMessages)
        {   
            if (!allMessages.Any())
            {
                WriteLogClass.WriteToLog(0, "Conversation messages are empty ....", 0);
                return false;
            }

            bool result = true;

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
