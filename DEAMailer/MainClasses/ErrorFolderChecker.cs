using CreatEmail;
using FindFolder;

namespace ErrorFolderChecker
{
    internal class ErrorFolderCheckerClass
    {
        public static int ErrorFolderChecker()
        {
            DirectoryInfo errorFolderPath = FindFoldersClass.FindFolder("error");
            IEnumerable<DirectoryInfo> subFolderList = errorFolderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly);

            if (subFolderList.Any())
            {
                CreateEmailClass.StartCreatingEmail(errorFolderPath);
            }
        }
    }
}