using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using static DownloadFtpFilesClass.FtpFilesDownload;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal class ReadFileContent
    {
        /// <summary>
        /// Starts reading the content of the file based on the specified parameters.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="downloadFileList">The list of files to download.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        /// <returns>1 if the file content reading process is successful, -1 otherwise.</returns>
        public static async Task<int> StartReadingFileContent(string filePath,
                                                               List<FtpFileInfo> downloadFileList,
                                                               int clientId)
        {
            UserConfigSetter.Customerdetail jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            int batchSize = jsonData.ReadContentSettings.NumberOfLinesToRead;

            try
            {
                foreach (FtpFileInfo fileName in downloadFileList)
                {
                    List<Dictionary<string, string>> data = await ReadFileData(filePath,
                                                                               fileName.FileName,
                                                                               jsonData);

                    if (data.Count == 0)
                    {
                        WriteLogClass.WriteToLog(0, "No data to create the pdf file.", 1);
                        return -1;
                    }

                    await ProcessDataInBatches(data, batchSize, filePath, fileName.FileName, clientId);

                    // TODO 1: Add code to upload the generated PDF files and the line data to TPS.
                    // TODO 2: Add code to delete the generated PDF files.
                    // TODO 3: Add code to delete the downloaded files.
                }
                return 1;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at reading file content: {ex.Message}", 0);
                return -1;
            }
        }

        /// <summary>
        /// Reads data from a file located at the specified filePath and fileName.
        /// Parses the data based on the delimiter provided in the user configuration.
        /// Returns a list of dictionaries where each dictionary represents a row of data.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="fileName">The name of the file to read.</param>
        /// <param name="jsonData">User configuration data containing parsing settings.</param>
        /// <returns>A task representing the asynchronous operation that returns a list of dictionaries.</returns>
        private static async Task<List<Dictionary<string, string>>> ReadFileData(string filePath,
                                                                                 string fileName,
                                                                                 UserConfigSetter.Customerdetail jsonData)
        {
            try
            {
                return await Task.Run(() =>
                {
                    // Combine the filePath and fileName to get the full path to read the file.
                    string filePathToRead = Path.Combine(filePath, fileName);

                    // Open the file stream and reader to read the file.
                    using FileStream fileStream = new(filePathToRead, FileMode.Open, FileAccess.Read);
                    using StreamReader reader = new(fileStream);

                    List<Dictionary<string, string>> data = new();
                    // Split the header fields based on the delimiter provided in the user configuration.
                    string[] headerFields = reader.ReadLine()?.Split(jsonData.ReadContentSettings.SetDelimiter) ?? Array.Empty<string>();

                    while (!reader.EndOfStream)
                    {
                        // Split each line of data based on the delimiter.
                        string[] lineItems = reader.ReadLine()?.Split(jsonData.ReadContentSettings.SetDelimiter) ?? Array.Empty<string>();

                        Dictionary<string, string> dataRow = new();

                        // Map each header field to its corresponding data item in the row.
                        for (int i = 0; i < headerFields.Length; i++)
                        {
                            dataRow.Add(headerFields[i], lineItems[i]);
                        }

                        data.Add(dataRow);
                    }

                    return data;
                });
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during the file reading process.
                WriteLogClass.WriteToLog(0, $"Exception at read file data: {ex.Message}", 0);
                return new List<Dictionary<string, string>>();
            }
        }

        /// <summary>
        /// Process data in batches and create PDF files for each batch.
        /// </summary>
        /// <param name="data">The list of dictionaries containing the data to be processed.</param>
        /// <param name="batchSize">The size of each batch to be processed.</param>
        /// <param name="filePath">The path where the PDF files will be stored.</param>
        /// <param name="fileName">The name of the file to be created.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        private static async Task ProcessDataInBatches(List<Dictionary<string, string>> data,
                                                        int batchSize,
                                                        string filePath,
                                                        string fileName,
                                                        int clientId)
        {
            int loopCount = 0;
            try
            {
                // Process data in batches
                while (loopCount < data.Count)
                {
                    var batchData = data.Skip(loopCount).Take(batchSize).ToList();

                    // Start creating PDF files for the batch data
                    await CreatePdfFile.StartCreatePdfFile(batchData, filePath, fileName, clientId);

                    loopCount += batchSize;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during batch processing
                WriteLogClass.WriteToLog(0, $"Exception at process data in batch: {ex.Message}", 0);
            }
        }
    }
}
