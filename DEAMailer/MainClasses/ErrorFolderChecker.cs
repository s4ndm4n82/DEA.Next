using CreatEmail;
using FindFolder;

namespace ErrorFolderChecker
{
    internal class ErrorFolderCheckerClass
    {
        public static int ErrorFolderChecker()
        {
            DirectoryInfo errorFolderPath = FindFoldersClass.FindFolder("Error");
            IEnumerable<DirectoryInfo> subFolderList = errorFolderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

            if (subFolderList.Any())
            {
                CreateEmailClass.StartCreatingEmail(errorFolderPath, subFolderList);
                return 1;
            }
            return 0;
        }
    }
}