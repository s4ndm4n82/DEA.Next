using CreatEmail;
using FindFolder;

namespace ErrorFolderChecker
{
    internal class ErrorFolderCheckerClass
    {
        public static bool ErrorFolderChecker()
        {
            DirectoryInfo errorFolderPath = FindFoldersClass.FindFolder("Error");
            IEnumerable<DirectoryInfo> subFolderList = errorFolderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

            if (subFolderList.Any())
            {
                CreateEmailClass.StartCreatingEmail(errorFolderPath, subFolderList);
                return true;
            }
            return false;
        }
    }
}