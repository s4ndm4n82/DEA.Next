using WriteLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        public static void GetFolders(string folderPath)
        {
            var filePath = Directory.GetParent(Path.GetDirectoryName(folderPath)!);

            if (Directory.Exists(filePath!.FullName))
            {
                WriteLogClass.WriteToLog(3, $"Cleaning download folder ....", string.Empty);

                string[] folderList = Directory.GetDirectories(filePath.FullName, "*.*", SearchOption.AllDirectories);

                DeleteFolders(folderList);
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Folder path does not exsits ....", string.Empty);
            }
        }

        private static void DeleteFolders(string[] folderList)
        {
            foreach (string folder in folderList)
            {
                if (Directory.Exists(folder))
                {
                    if (Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length > 0)
                    {
                        try
                        {
                            var RmvdFolderName = folder.Split(Path.DirectorySeparatorChar).Last();
                            Directory.Delete(folder, true);
                            WriteLogClass.WriteToLog(3, $"Folder {RmvdFolderName} .... deleted", string.Empty);
                        }
                        catch (IOException ex)
                        {
                            WriteLogClass.WriteToLog(2, $"Exception at folder delete: {ex.Message}", string.Empty);
                        }

                    }
                }
            }
        }
    }
}
