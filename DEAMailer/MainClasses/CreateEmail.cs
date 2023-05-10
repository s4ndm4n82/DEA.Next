

namespace CreatEmail
{
    internal class CreateEmailClass
    {
        public class EmailInfor
        {
            public string customerID { get; set; }
            public string fileCount { get; set; }
        }

        public static int StartCreatingEmail(DirectoryInfo folderPath)
        {
            IEnumerable<FileInfo> fileList = folderPath.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);
            foreach (FileInfo file in fileList)
            {
                Console.WriteLine(file);
            }
            return 0;
        }
    }
}
