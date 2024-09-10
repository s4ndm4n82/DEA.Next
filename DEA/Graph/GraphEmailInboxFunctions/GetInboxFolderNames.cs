namespace DEA.Next.Graph.GraphEmailInboxFunctions
{
    /// <summary>
    /// Get the email folder path and extracts the inbox child folder names from it.
    /// </summary>
    internal class GetInboxFolderNames
    {
        private readonly string[] inboxNames;
        private int inboxCurrentIndex;

        /// <summary>
        /// Takes the entire email folder path and then splits it by '/'. And recorst the index each time it is called.
        /// </summary>
        /// <param name="inboxPath">Inbox path</param>
        public GetInboxFolderNames(string inboxPath)
        {
            if (inboxPath.Contains('\\'))
            {
                inboxPath = inboxPath.Replace("\\", "/");
            }

            inboxNames = inboxPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            inboxCurrentIndex = 0;
        }

        /// <summary>
        /// When called it will return the next inbox name. While incrementing index by one each time.
        /// </summary>
        /// <returns></returns>
        public string GetNextInboxName()
        {
            if (inboxCurrentIndex < inboxNames.Length)
            {
                return inboxNames[inboxCurrentIndex++];
            }

            return null;
        }
    }
}
