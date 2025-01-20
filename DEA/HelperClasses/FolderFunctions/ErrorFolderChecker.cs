using DEA.Next.HelperClasses.OtherFunctions;
using FindFolder;

namespace ErrorFolderChecker;

public class ErrorFolderCheckerClass
{
    /// <summary>
    /// Search and returns the error folder path.
    /// </summary>
    /// <returns>Return the path</returns>
    public static (IEnumerable<DirectoryInfo>, DirectoryInfo) ErrorFolderChecker()
    {
        DirectoryInfo errorFolderPath = FindFoldersClass.FindFolder(MagicWords.Error);
        IEnumerable<DirectoryInfo> subFolderList = errorFolderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

        return (subFolderList,errorFolderPath);
    }
}