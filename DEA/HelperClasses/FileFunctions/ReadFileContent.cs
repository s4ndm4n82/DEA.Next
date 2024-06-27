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
                fileParser.SetDelimiters(jsonData.ReadContentSettings.SetDelimiter);
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

                    if (data.Count < jsonData.ReadContentSettings.NumberOfLinesToRead || lineCount >= jsonData.ReadContentSettings.NumberOfLinesToRead)
                    {
                        // If there are fewer lines read than the specified number or lineCount exceeds the specified number, process the available data

                        if (data.Count < jsonData.ReadContentSettings.NumberOfLinesToRead)
                        {
                            await CreatePdfFile.StartCreatePdfFile(data, filePath, fileName.FileName, clientId);
                        }
                        else
                        {
                            int remainingLines = data.Count - jsonData.ReadContentSettings.NumberOfLinesToRead;
                            var remainingData = data.Skip(jsonData.ReadContentSettings.NumberOfLinesToRead).ToList();
                            await CreatePdfFile.StartCreatePdfFile(remainingData, filePath, fileName.FileName, clientId);
                        }

                        data.Clear();
                        lineCount = 0;
                    }
                }
            }
            return -1;
        }
    }
}
