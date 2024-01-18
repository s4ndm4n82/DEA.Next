using System.Reflection;

namespace FindFolder
{
    public class FindFoldersClass
    {
        /// <summary>
        /// Search and returns the folder path.
        /// </summary>
        /// <param name="folderName">Folder name that need to be located.</param>
        /// <returns>Return the folder path.</returns>
        public static DirectoryInfo FindFolder(string folderName)
        {
            DirectoryInfo folderList = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            DirectoryInfo logFolderPath = folderList.GetDirectories(".", SearchOption.TopDirectoryOnly).FirstOrDefault(dn => dn.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase));
            return logFolderPath;
        }
    }
}