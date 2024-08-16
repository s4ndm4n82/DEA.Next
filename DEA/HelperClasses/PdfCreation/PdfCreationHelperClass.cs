using DEA.Next.HelperClasses.FolderFunctions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using WriteLog;

namespace DEA.Next.HelperClasses.PdfCreation;

public static class PdfCreationHelperClass
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

    /// <summary>
    /// This method generates a new list of dictionaries based on the input data.
    /// Each dictionary contains a subset of the keys from the input data, with some keys added or modified.
    /// </summary>
    /// <param name="data">The input data as a list of dictionaries. Each dictionary represents a row in the data.</param>
    /// <param name="mainFieldNames">An array of strings representing the names of the fields that should be included in the output data.</param>
    /// <param name="mainFieldsToSkip">An array of strings representing the names of the fields that should be skipped in the output data.</param>
    /// <param name="fieldToGenerate">The name of the field that should be generated in the output data.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new data as a list of dictionaries.</returns>
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
                    // If the input data is null, log a message and return an empty list.
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
                        // If the required fields are not empty and the field to generate is in the list of main fields,
                        // add a new key-value pair to the filtered item.
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
                            // If the field is not in the list of fields to skip and is not the field to generate,
                            // add a new key-value pair to the filtered item.
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
            // If an exception occurs, log the exception message and return an empty list.
            WriteLogClass.WriteToLog(0, $"Exception at make new data list batch: {e.Message}", 0);
            return new List<Dictionary<string, string>>();
        }
    }

    /// <summary>
    /// Creates a new batch of data by filtering and transforming the input data.
    /// </summary>
    /// <param name="data">The input data to process.</param>
    /// <param name="lineFieldNames">The names of the fields to include in the output data.</param>
    /// <param name="lineFieldsToSkip">The names of the fields to exclude from the output data.</param>
    /// <param name="fieldToGenerate">The name of the field to generate based on the input data.</param>
    /// <returns>A list of dictionaries representing the transformed data.</returns>
    public static async Task<List<Dictionary<string, string>>> MakeNewDataListBatch(
        List<Dictionary<string, string>>? data,
        string[] lineFieldNames,
        string[] lineFieldsToSkip,
        string fieldToGenerate)
    {
        // Initialize variables to store the invoice date and number
        var invDate = string.Empty;
        var invNum = string.Empty;
        var waterNum = string.Empty;
        var invoiceDescription = string.Empty;

        // Initialize an empty list to store the transformed data
        var newData = new List<Dictionary<string, string>>();

        try
        {
            // Process the input data in a separate thread
            return await Task.Run(() =>
            {
                // Check if the input data is null
                if (data == null)
                {
                    // Log an error message if the data is null
                    WriteLogClass.WriteToLog(0, "Data array is null ....", 1);
                    return new List<Dictionary<string, string>>();
                }

                // Iterate over each item in the input data
                foreach (var item in data)
                {
                    // Create a new dictionary to store the filtered data
                    var filteredItem = new Dictionary<string, string>();

                    // Iterate over each field in the item
                    for (var i = 0; i < item.Count; i++)
                    {
                        // Get the key of the current field
                        var key = i.ToString();

                        // Check if the key matches a specific value
                        switch (key)
                        {
                            case "2":
                                // Store the invoice number
                                invNum = item[key];
                                break;
                            
                            case "3":
                                // Store the water account number
                                waterNum = item[key];
                                break;

                            case "4":
                                // Store the invoice date
                                invDate = item[key];
                                break;
                            
                            case "9":
                                // Store the invoice description
                                invoiceDescription = item[key];
                                break;
                        }
                    }

                    // Check if the invoice number and date are not empty and the field to generate is in the line field names
                    if (!string.IsNullOrEmpty(invNum)
                        && !string.IsNullOrEmpty(invDate)
                        && lineFieldNames.Contains(fieldToGenerate, StringComparer.OrdinalIgnoreCase))
                    {
                        // Generate the field value based on the invoice number and date
                        filteredItem[fieldToGenerate] = $"{invNum}+{waterNum}+{invDate}+{invoiceDescription}";
                    }

                    // Iterate over each field in the line field names
                    for (var i = 0; i < lineFieldNames.Length && i < item.Count; i++)
                    {
                        // Get the field name
                        var fieldName = lineFieldNames[i];

                        // Check if the field name is not in the fields to skip and not equal to the field to generate
                        if (!lineFieldsToSkip.Contains(fieldName)
                            && !string.Equals(fieldName, fieldToGenerate, StringComparison.OrdinalIgnoreCase))
                        {
                            // Add the field to the filtered data
                            filteredItem.Add(fieldName, item[i.ToString()]);
                        }
                    }

                    // Add the filtered data to the new data list
                    newData.Add(filteredItem);
                }

                // Return the new data list
                return newData;
            });
        }
        catch (Exception e)
        {
            // Log an error message if an exception occurs
            WriteLogClass.WriteToLog(0, $"Exception at make new data list batch: {e.Message}", 0);
            return new List<Dictionary<string, string>>();
        }
    }

    /// <summary>
    /// Asynchronously removes files after uploading to FTP.
    /// </summary>
    /// <param name="outputPath">The path where the files were uploaded from.</param>
    /// <param name="mainFileName">The name of the main file.</param>
    /// <param name="clientId">The client ID for retrieving user configuration.</param>
    /// <returns>True if the files were successfully removed, false otherwise.</returns>
    public static async Task<bool> RemoveFilesAfterUpload(string outputPath, string mainFileName, int clientId)
    {
        try
        {
            // Get the last directory name from the output path.
            var lastDirectoryName = Path.GetDirectoryName(outputPath);

            // Check if the last directory name is null or empty.
            if (string.IsNullOrEmpty(lastDirectoryName))
            {
                WriteLogClass.WriteToLog(0, "The local last directory path is null or empty ....", 1);
                return false;
            }

            // Log the process of deleting local files and folders.
            WriteLogClass.WriteToLog(1, "Deleting local files and folders ....", 1);

            // Remove the main file asynchronously.
            var removeMainFileTask = FolderCleanerLines.RemoveMainFileAsync(outputPath, mainFileName);

            // Delete the local hold folder asynchronously.
            var deleteLocalHoldFolderTask = FolderCleanerLines.DeleteLocalHoldFolderAsync(lastDirectoryName);

            // Wait for both tasks to complete.
            await Task.WhenAll(removeMainFileTask, deleteLocalHoldFolderTask);

            // If both tasks were successful, remove the data file from FTP and return the result.
            if (removeMainFileTask.Result && deleteLocalHoldFolderTask.Result)
            {
                return await FolderCleanerLines.RemoveDataFileFromFtpAsync(mainFileName, clientId);
            }

            return false; // Return false if any of the tasks failed.
        }
        catch (Exception e)
        {
            // Log any exceptions that occur during the process.
            WriteLogClass.WriteToLog(0, $"Exception at remove files after upload: {e.Message}", 0);
            return false;
        }
    }

    /// <summary>
    /// Saves a document to a PDF file and returns a tuple indicating whether the operation was successful and the path of the saved PDF file.
    /// </summary>
    /// <param name="document">The document to save as a PDF.</param>
    /// <param name="outputPath">The path where the PDF file will be saved.</param>
    /// <param name="outputFileExtension">The extension of the output file.</param>
    /// <returns>A tuple containing a boolean indicating whether the operation was successful and the path of the saved PDF file.</returns>
    public static async Task<Tuple<bool, string>> SaveDocumentToPdf(Document document,
        string outputPath,
        string outputFileExtension)
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
        await Task.Run(() => { renderer.PdfDocument.Save(newOutputPath); });

        // Check if the PDF file exists
        if (!File.Exists(newOutputPath)) return Tuple.Create(false, string.Empty);

        // Write a log message indicating that the PDF file was created successfully
        WriteLogClass.WriteToLog(1, "Pdf file created successfully ....", 1);
        return Tuple.Create(true, newOutputPath);
    }
}