using System.Reflection;

namespace FindFolder
{
    public class FindFoldersClass
    {
        public static DirectoryInfo FindFolder(string folderName)
        {
            DirectoryInfo folderList = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            DirectoryInfo logFolderPath = folderList.GetDirectories(".", SearchOption.TopDirectoryOnly).FirstOrDefault(dn => dn.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase));
            return logFolderPath;
        }
    }
}