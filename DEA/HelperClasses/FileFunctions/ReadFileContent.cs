using System.Text;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;
using static DownloadFtpFilesClass.FtpFilesDownload;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal static class ReadFileContent
    {
        /// <summary>
        /// Starts reading file content from a list of FTP files.
        /// </summary>
        /// <param name="filePath">The path to the files to be read.</param>
        /// <param name="downloadFileList">A list of FTP file information.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        /// <returns>A task representing the asynchronous operation that returns an integer indicating the result of the operation.</returns>
        public static async Task<int> StartReadingFileContent(string filePath,
            List<FtpFileInfo> downloadFileList,
            int clientId)
        {
            // Retrieve user configuration data by client ID
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Initialize batch size from user configuration
            var batchSize = jsonData.ReadContentSettings.NumberOfLinesToRead;

            try
            {
                // Iterate through each file in the download list
                foreach (var fileName in downloadFileList)
                {
                    // Check if the file name contains a trigger to read by line
                    var trigger = fileName.FileName.Contains(jsonData.ReadContentSettings.ReadByLineTrigger,
                        StringComparison.OrdinalIgnoreCase);

                    // If trigger is found, set batch size to 1
                    if (trigger) batchSize = 1;

                    // Generate a unique set ID
                    var setId = MakeSetId();

                    // Read file data from the specified file path and file name
                    var data = await ReadFileData(filePath, fileName.FileName, jsonData);

                    // Check if there is any data to process
                    if (data.Count == 0)
                    {
                        // Log a message indicating no data to create PDF file
                        WriteLogClass.WriteToLog(0, "No data to create the pdf file ....", 1);
                        return -1;
                    }

                    // Process data in batches
                    if (!await ProcessDataInBatches(data, filePath, fileName.FileName, setId, batchSize, clientId))
                    {
                        // Log a message indicating file data processing failed
                        WriteLogClass.WriteToLog(0, "File data processing failed ....", 1);
                        return -1;
                    }

                    // Log a message indicating file data processed successfully
                    WriteLogClass.WriteToLog(1, "File data processed successfully ....", 1);
                    return 1;
                }

                // If all files are processed successfully, return 1
                return 1;
            }
            catch (Exception ex)
            {
                // Log an exception message
                WriteLogClass.WriteToLog(0, $"Exception at reading file content: {ex.Message}", 0);
                return -1;
            }
        }

        /// <summary>
        /// Reads file data from a specified file path and returns it as a list of dictionaries.
        /// </summary>
        /// <param name="filePath">The path to the file to be read.</param>
        /// <param name="fileName">The name of the file to be read.</param>
        /// <param name="jsonData">The user configuration data.</param>
        /// <returns>A task representing the asynchronous operation that returns a list of dictionaries containing the file data.</returns>
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
                    // Skip the first line of the file (assuming it contains header information)
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        // Split each line of data based on the delimiter provided in the user configuration.
                        var lineItems = reader.ReadLine()?.Split(jsonData.ReadContentSettings.SetDelimiter) ??
                                        Array.Empty<string>();

                        Dictionary<string, string> dataRow = new();

                        // Map each data item to its corresponding header field in the row.
                        for (var i = 0; i < lineItems.Length; i++)
                        {
                            dataRow.Add(i.ToString(), lineItems[i]);
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
        /// Processes data in batches.
        /// </summary>
        /// <param name="data">The data to process.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="setId">The set ID.</param>
        /// <param name="batchSize">The size of each batch.</param>
        /// <param name="clientId">The client ID.</param>
        /// <returns>True if all batches are successful, false otherwise.</returns>
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
                    // Get the current batch of data
                    var batchData = data.Skip(loopCount).Take(batchSize).ToList();

                    // If this is the last batch, set the lastItem flag to true
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

                    // If the batch is not successful, set the allBatchesSuccessful flag to false and break the loop
                    if (!batchSuccessful)
                    {
                        allBatchesSuccessful = false;
                        break;
                    }

                    // Increment the loopCount to move to the next batch
                    loopCount += batchSize;
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions that occur during batch processing
                WriteLogClass.WriteToLog(0, $"Exception at process data in batch: {ex.Message}", 0);
                allBatchesSuccessful = false;
            }

            // Return the result of all batches
            return allBatchesSuccessful;
        }
        
        /// <summary>
        /// Generates a unique set ID by combining the current date and time with four random uppercase letters.
        /// </summary>
        /// <returns>A string representing the unique set ID.</returns>
        private static string MakeSetId()
        {
            // Get the current date and time
            var now = DateTime.Now;
            var nowString = now.ToString("yyyyMMddHHmm");

            // Generate random characters
            Random random = new();
            StringBuilder builtString = new(4);

            // Generate four random uppercase letters
            for (var i = 0; i < 4; i++)
            {
                var randomChar = (char)random.Next(65, 91); // Random ASCII characters from A to Z
                builtString.Append(randomChar);
            }

            // Concatenate the date and time with the random characters to create the set ID
            return string.Concat(nowString, builtString.ToString());
        }
    }
}
