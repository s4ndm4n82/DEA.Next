using System.Reflection;

namespace FindFolderClass
{
    internal class FindFolders
    {
        public static DirectoryInfo FindFolder()
        {
            DirectoryInfo folderList = new(Assembly.GetExecutingAssembly().Location);
            DirectoryInfo logFolderPath = folderList.GetDirectories("*.*", SearchOption.TopDirectoryOnly).FirstOrDefault(dn => dn.Name.ToLower().Equals("logs"));
            return logFolderPath;
        }        
    }
}
