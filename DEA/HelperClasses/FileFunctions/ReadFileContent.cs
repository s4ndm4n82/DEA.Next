using System.Text;
using Microsoft.Graph;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using static DownloadFtpFilesClass.FtpFilesDownload;
using File = System.IO.File;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal static class ReadFileContent
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
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            var batchSize = jsonData.ReadContentSettings.NumberOfLinesToRead;
            
            
            try
            {
                foreach (var fileName in downloadFileList)
                {
                    var trigger = fileName.FileName
                        .Contains(jsonData.ReadContentSettings.ReadByLineTrigger, StringComparison.OrdinalIgnoreCase);

                    if (trigger) batchSize = 1;
                    
                    var setId = MakeSetId();
                    
                    var data = await ReadFileData(filePath,
                        fileName.FileName,
                        jsonData);

                    if (data.Count == 0)
                    {
                        WriteLogClass.WriteToLog(0, "No data to create the pdf file ....", 1);
                        return -1;
                    }

                    if (!await ProcessDataInBatches(data,
                            filePath,
                            fileName.FileName,
                            setId,
                            batchSize,
                            clientId))
                    {
                        WriteLogClass.WriteToLog(0, "File data processing failed ....", 1);
                        return -1;
                    }
                    WriteLogClass.WriteToLog(1, "File data processed successfully ....", 1);
                    return 1;
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
                    var filePathToRead = Path.Combine(filePath, fileName);

                    // Open the file stream and reader to read the file.
                    using FileStream fileStream = new(filePathToRead, FileMode.Open, FileAccess.Read);
                    using StreamReader reader = new(fileStream);

                    List<Dictionary<string, string>> data = new();
                    // Split the header fields based on the delimiter provided in the user configuration.
                    var headerFields = reader.ReadLine()?.Split(jsonData.ReadContentSettings.SetDelimiter) ?? Array.Empty<string>();

                    while (!reader.EndOfStream)
                    {
                        // Split each line of data based on the delimiter.
                        var lineItems = reader.ReadLine()?.Split(jsonData.ReadContentSettings.SetDelimiter) ?? Array.Empty<string>();

                        Dictionary<string, string> dataRow = new();

                        // Map each header field to its corresponding data item in the row.
                        for (var i = 0; i < headerFields.Length; i++)
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
        /// <param name="setId">The set ID of the PDF file.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        private static async Task<bool> ProcessDataInBatches(List<Dictionary<string, string>> data,
            string filePath,
            string fileName,
            string setId,
            int batchSize,
            int clientId)
        {
            var loopCount = 0;
            var allBatchesSuccessful = true;
            var lastItem = false;
            
            try
            {
                // Process data in batches
                while (loopCount < data.Count)
                {
                    var batchData = data.Skip(loopCount).Take(batchSize).ToList();
                    
                    if (loopCount + batchSize >= data.Count)
                    {
                        lastItem = true;
                    }

                    // Start creating PDF files for the batch data
                    var batchSuccessful = await CreatePdfFile.StartCreatePdfFile(batchData,
                        filePath,
                        fileName,
                        setId,
                        lastItem,
                        clientId);
                    
                    if (!batchSuccessful)
                    {
                        allBatchesSuccessful = false;
                        break;
                    }

                    loopCount += batchSize;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during batch processing
                WriteLogClass.WriteToLog(0, $"Exception at process data in batch: {ex.Message}", 0);
                allBatchesSuccessful = false;
            }

            return allBatchesSuccessful;
        }
        
        /// <summary>
        /// Generates a unique Set ID based on the current date and random characters.
        /// </summary>
        /// <returns>The generated Set ID string.</returns>
        private static string MakeSetId()
        {
            // Get the current date and time
            var now = DateTime.Now;
            var nowString = now.ToString("yyyyMMddHHmm");

            // Generate random characters
            Random random = new();
            StringBuilder builtString = new(4);

            for (var i = 0; i < 4; i++)
            {
                var randomChar = (char)random.Next(65, 91); // Random ASCII characters from A to Z
                builtString.Append(randomChar);
            }

            return string.Concat(nowString, builtString.ToString());
        }
    }
}
