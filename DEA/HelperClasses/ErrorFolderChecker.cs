using FindFolder;

namespace ErrorFolderChecker
{
    public class ErrorFolderCheckerClass
    {
        public static (IEnumerable<DirectoryInfo>, DirectoryInfo) ErrorFolderChecker()
        {
            DirectoryInfo errorFolderPath = FindFoldersClass.FindFolder("Error");
            IEnumerable<DirectoryInfo> subFolderList = errorFolderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

            return (subFolderList,errorFolderPath);
        }
    }
}