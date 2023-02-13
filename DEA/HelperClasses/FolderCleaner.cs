using WriteLog;

namespace FolderCleaner
{
    internal class FolderCleanerClass
    {
        public static bool GetFolders(string folderPath)
        {
            var filePath = Directory.GetParent(Path.GetDirectoryName(folderPath)!);

            if (Directory.Exists(filePath!.FullName))
            {
                WriteLogClass.WriteToLog(3, $"Cleaning download folder ....", string.Empty);

                string[] folderList = Directory.GetDirectories(filePath.FullName, "*.*", SearchOption.AllDirectories);

                if (DeleteFolders(folderList))
                {
                    return true;
                }
            }
            else
            {
                WriteLogClass.WriteToLog(3, "Folder path does not exsits ....", string.Empty);
            }
            return false;
        }

        private static bool DeleteFolders(string[] folderList)
        {
            int loopCount = 0;
            
            foreach (string folder in folderList)
            {
                loopCount++;

                if (Directory.Exists(folder))
                {
                    if (Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories).Length > 0)
                    {
                        try
                        {
                            var RmvdFolderName = folder.Split(Path.DirectorySeparatorChar).Last();
                            Directory.Delete(folder, true);
                        }
                        catch (IOException ex)
                        {
                            WriteLogClass.WriteToLog(2, $"Exception at folder delete: {ex.Message}", string.Empty);
                        }
                    }
                }
            }

            if (loopCount == folderList.Length)
            {
                WriteLogClass.WriteToLog(3, $"Removed {folderList.Length} folder from download folder ....", string.Empty);
                return true;
            }

            return false;
        }
    }
}
