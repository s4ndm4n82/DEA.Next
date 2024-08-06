using DEA.Next.FileOperations.TpsFileFunctions;
using DEA.Next.HelperClasses.FolderFunctions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using WriteLog;

namespace DEA.Next.HelperClasses.PdfCreation;

public class PdfCreationHelperClass
{
    /// <summary>
    /// This function generates an output file name based on the main file name and output file extension.
    /// </summary>
    /// <param name="mainFileName">The name of the main file.</param>
    /// <param name="outputFileExtension">The extension of the output file.</param>
    /// <returns>The generated output file name.</returns>
    public static string MakeOutPutFileName(string mainFileName, string outputFileExtension)
    {
        // Get current date and time
        var now = DateTime.Now;
        var dateTimeString = now.ToString("yyyyMMdd_HHmmss");

        // Main file name
        var mainFileNameOnly = Path.GetFileNameWithoutExtension(mainFileName);

        // Creating the output filename
        return string.Concat(mainFileNameOnly, "_", dateTimeString, ".", outputFileExtension.ToLower()).Replace(" ", "_");
    }
    
    public static async Task<List<Dictionary<string, string>>> MakeNewDataListLine(
            List<Dictionary<string, string>>? data,
            string[] mainFieldNames,
            string[] mainFieldsToSkip,
            string fieldToGenerate)
        {
            var invDate = string.Empty;
            var invNum = string.Empty;
            var invWaterAccount = string.Empty;
            var newData = new List<Dictionary<string, string>>();
            
            try
            {
                return await Task.Run(() =>
                {
                    if (data == null)
                    {
                        WriteLogClass.WriteToLog(0, "Data array is null ....", 1);
                        return new List<Dictionary<string, string>>();
                    }
                    
                    foreach (var item in data)
                    {
                        var filteredItem = new Dictionary<string, string>();
                    
                        for (var i = 0; i < item.Count; i++)
                        {
                            var key = i.ToString();
                            switch (key)
                            {
                                case "2":
                                    invNum = item[key];
                                    break;
                                
                                case "3":
                                    invWaterAccount = item[key];
                                    break;
                            
                                case "4":
                                    invDate = item[key];
                                    break;
                            }
                        }
                    
                        if (!string.IsNullOrEmpty(invNum)
                            && !string.IsNullOrEmpty(invDate)
                            && mainFieldNames.Contains(fieldToGenerate,
                                StringComparer.OrdinalIgnoreCase))
                        {
                            filteredItem[fieldToGenerate] = $"{invNum}+{invWaterAccount}+{invDate}";
                        }
                    
                        for (var i = 0; i < mainFieldNames.Length && i < item.Count; i++)
                        {
                            var fieldName = mainFieldNames[i];
                            if (!mainFieldsToSkip.Contains(fieldName)
                                && !string
                                    .Equals(fieldName, fieldToGenerate,
                                        StringComparison.OrdinalIgnoreCase))
                            {
                                filteredItem.Add(fieldName, item[i.ToString()]);
                            }
                        }
                    
                        newData.Add(filteredItem);
                    }
                
                    return newData;
                });
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at make new data list batch: {e.Message}", 0);
                return new List<Dictionary<string, string>>();
            }
        }
    
    public static async Task<List<Dictionary<string, string>>> MakeNewDataListBatch(
            List<Dictionary<string, string>>? data,
            string[] lineFieldNames,
            string[] lineFieldsToSkip,
            string fieldToGenerate)
        {
            var invDate = string.Empty;
            var invNum = string.Empty;
            var newData = new List<Dictionary<string, string>>();
            
            try
            {
                return await Task.Run(() =>
                {
                    if (data == null)
                    {
                        WriteLogClass.WriteToLog(0, "Data array is null ....", 1);
                        return new List<Dictionary<string, string>>();
                    }
                    
                    foreach (var item in data)
                    {
                        var filteredItem = new Dictionary<string, string>();
                    
                        for (var i = 0; i < item.Count; i++)
                        {
                            var key = i.ToString();
                            switch (key)
                            {
                                case "2":
                                    invNum = item[key];
                                    break;
                            
                                case "4":
                                    invDate = item[key];
                                    break;
                            }
                        }
                    
                        if (!string.IsNullOrEmpty(invNum)
                            && !string.IsNullOrEmpty(invDate)
                            && lineFieldNames.Contains(fieldToGenerate,
                                StringComparer.OrdinalIgnoreCase))
                        {
                            filteredItem[fieldToGenerate] = $"{invNum}+{invDate}";
                        }
                    
                        for (var i = 0; i < lineFieldNames.Length && i < item.Count; i++)
                        {
                            var fieldName = lineFieldNames[i];
                            if (!lineFieldsToSkip.Contains(fieldName)
                                && !string
                                    .Equals(fieldName, fieldToGenerate,
                                        StringComparison.OrdinalIgnoreCase))
                            {
                                filteredItem.Add(fieldName, item[i.ToString()]);
                            }
                        }
                    
                        newData.Add(filteredItem);
                    }
                
                    return newData;
                });
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at make new data list batch: {e.Message}", 0);
                return new List<Dictionary<string, string>>();
            }
        }

        public static async Task<bool> RemoveFilesAfterUpload(string outputPath, string mainFileName, int clientId)
        {
            try
            {
                var lastDirectoryName = Path.GetDirectoryName(outputPath);

                if (string.IsNullOrEmpty(lastDirectoryName))
                {
                    WriteLogClass.WriteToLog(0, "The local last directory path is null or empty ....", 1);
                    return false;
                }

                WriteLogClass.WriteToLog(1, "Deleting local files and folders ....", 1);

                var removeMainFileTask = FolderCleanerLines.RemoveMainFileAsync(outputPath, mainFileName);
                var deleteLocalHoldFolderTask =
                    FolderCleanerLines.DeleteLocalHoldFolderAsync(lastDirectoryName);

                await Task.WhenAll(removeMainFileTask, deleteLocalHoldFolderTask);

                if (removeMainFileTask.Result && deleteLocalHoldFolderTask.Result)
                {
                    return await FolderCleanerLines.RemoveDataFileFromFtpAsync(mainFileName, clientId);
                }
                
                return false;
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at remove files after upload: {e.Message}", 0);
                return false;
            }
        }

        public static async Task<Tuple<bool,string>> SaveDocumentToPdf(Document document,
            string outputPath,
            string outputFileExtension,
            string newInvoiceNumber)
        {
            // Save the document to a PDF file
            var directoryPath = Path.GetDirectoryName(outputPath);

            // Check if the directory path is null or empty
            if (string.IsNullOrEmpty(directoryPath))
            {
                // Write a log message if the directory path is null or empty
                WriteLogClass.WriteToLog(1, "The output directory path is null or empty ....", 1);
                return Tuple.Create(false, string.Empty);
            }

            // Generate a new file name by concatenating the file name without extension and the output file extension
            var newFileName = string.Concat(Path.GetFileNameWithoutExtension(outputPath), outputFileExtension);

            // Generate a new output path by combining the directory path and the new file name
            var newOutputPath = Path.Combine(directoryPath, newFileName);

            // Create a PDF document renderer and render the document
            PdfDocumentRenderer renderer = new()
            {
                Document = document
            };
            renderer.RenderDocument();

            // Save the rendered PDF document to the new output path
            await Task.Run(() =>
            {
                renderer.PdfDocument.Save(newOutputPath);
            });

            // Check if the PDF file exists
            if (!File.Exists(newOutputPath)) return Tuple.Create(false, string.Empty);

            // Write a log message indicating that the PDF file was created successfully
            WriteLogClass.WriteToLog(1, "Pdf file created successfully ....", 1);
            return Tuple.Create(true, newOutputPath);
        }
}