namespace DEA.Next.Graph.GraphEmailInboxFunctions
{
    internal class GetInboxFolderNames
    {
        private readonly string[] inboxNames;
        private int inboxCurrentIndex;

        public GetInboxFolderNames(string inboxPath)
        {
            if (inboxPath.Contains('\\'))
            {
                inboxPath = inboxPath.Replace("\\", "/");
            }

            inboxNames = inboxPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            inboxCurrentIndex = 0;
        }

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
