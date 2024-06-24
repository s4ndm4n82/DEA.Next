using Microsoft.VisualBasic.FileIO;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using static DownloadFtpFilesClass.FtpFilesDownload;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal class ReadFileContent
    {
        public static async Task<int> StartReadingFileContent(string filePath,
                                                              List<FtpFileInfo> downloadFileList,
                                                              int clientId)
        {
            UserConfigSetter.Customerdetail jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            int lineCount = 0;

            foreach (FtpFileInfo fileName in downloadFileList)
            {
                string filePathToRead = Path.Combine(filePath, fileName.FileName);
                using TextFieldParser fileParser = new(filePathToRead);
                fileParser.TextFieldType = FieldType.Delimited;
                fileParser.SetDelimiters(jsonData.SetDelimiter);
                fileParser.TrimWhiteSpace = true;

                string[] headerFields = fileParser.ReadFields() ?? Array.Empty<string>();

                List<Dictionary<string, string>> data = new();

                while (!fileParser.EndOfData)
                {
                    string[] lineItems = fileParser.ReadFields() ?? Array.Empty<string>();

                    Dictionary<string, string> dataRow = new();

                    for (int i = 0; i < headerFields.Length; i++)
                    {
                        dataRow.Add(headerFields[i], lineItems[i]);
                    }

                    data.Add(dataRow);
                    lineCount++;

                    if (lineCount == jsonData.NumberOfLinesToRead)
                    {
                        DislplayData(data);
                        data.Clear();
                        lineCount = 0;
                    }
                }
            }
            return -1;
        }

        private static void DislplayData(List<Dictionary<string, string>> data)
        {
            foreach (var item in data)
            {
                Console.WriteLine(item);
            }
        }
    }
}
