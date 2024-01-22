using System.Reflection;
using WriteLog;
using DEA.Next.HelperClasses.OtherFunctions;

namespace FolderFunctions
{
    public class FolderFunctionsClass
    {
        /// <summary>
        /// Checks and creates the main folders that used by the app. And also returns the path for those folders when needed.
        /// </summary>
        /// <param name="folderSwitch"></param>
        /// <returns></returns>
        public static string CheckFolders(string folderSwitch)
        {
            string pathRootFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string[] folderNames = { MagicWords.download, MagicWords.attachments, MagicWords.ftpfiles, MagicWords.logs, MagicWords.error };
            string folderPath;
            string returnFolderPath = string.Empty;

            foreach (string folderName in folderNames)
            {
                if (folderName != MagicWords.attachments && folderName != MagicWords.ftpfiles)
                {
                    folderPath = Path.Combine(pathRootFolder!, folderName);
                }
                else
                {
                    folderPath = Path.Combine(pathRootFolder!, folderNames[0], folderName);
                }

                if (!Directory.Exists(folderPath))
                {
                    try
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    catch (Exception ex)
                    {
                        WriteLogClass.WriteToLog(0, $"Exception at main folder checker: {ex.Message}", 0);
                    }
                }
            }

            if (!string.IsNullOrEmpty(folderSwitch))
            {
                string folderName = folderNames.FirstOrDefault(fn =>
                                               fn.IndexOf(folderSwitch, StringComparison.CurrentCultureIgnoreCase) != -1);

                if (folderName != MagicWords.attachments && folderName != MagicWords.ftpfiles)
                {
                    returnFolderPath = Path.Combine(pathRootFolder!, folderName);
                }
                else
                {
                    returnFolderPath = Path.Combine(pathRootFolder!, folderNames[0], folderName);
                }

                return returnFolderPath;
            }

            return returnFolderPath;
        }
    }
}
