using AppConfigReader;
using DEA.Next.FileOperations.TpsFileFunctions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.PdfCreation;

public static class CreatePdfLineProcess
{
    /// <summary>
    /// Creates a PDF file for each item in the data list and sends the data to a web service asynchronously.
    /// </summary>
    /// <param name="data">The list of data items to create PDF files for.</param>
    /// <param name="outputPath">The path where the PDF files will be saved.</param>
    /// <param name="mainFileName">The name of the main file.</param>
    /// <param name="setId">The set ID.</param>
    /// <param name="lastItem">Indicates whether this is the last item in the batch.</param>
    /// <param name="clientId">The client ID.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the operation was successful.</returns>
    public static async Task<bool> CreatPdfBatchByLine(List<Dictionary<string, string>>? data,
        string outputPath,
        string mainFileName,
        string setId,
        bool lastItem,
        int clientId)
    {
        try
        {
            // Initialize the result variable to -1
            var result = -1;

            // Get the directory path from the output path
            var filePath = Path.GetDirectoryName(outputPath);

            // Check if the file path is null
            if (filePath == null) return false;

            // Define the margin constant
            const int margin = 10;

            // Retrieve the user configuration data by client ID
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the main field list, main fields to skip, output file extension, and generated field name from the user configuration data
            var mainFieldList = jsonData.ReadContentSettings.MainFieldNameList;
            var mainFieldsToSkip = jsonData.ReadContentSettings.MainFieldToSkip;
            var outputFileExtension = string.Concat('.', jsonData.ReadContentSettings.OutputFileExtension);
            var generatedFieldName = jsonData.ReadContentSettings.GeneratedField;

            // Get the upload delay time from app config
            var appJsonData = AppConfigReaderClass.ReadAppDotConfig();
            var uploadDelay = appJsonData.ProgramSettings.UploadDelayTime;
            
            // Define the PDF main field list
            var pdfMainFieldList = new[]
            {
                "Generated Date", "Generated Time", "File Name", "Set Id"
            };

            // Define the PDF main field list values
            var pdfMainFieldListValues = new[]
            {
                DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"),
                Path.GetFileNameWithoutExtension(mainFileName), setId
            };

            // Check if the data is null or empty
            if (data is null || !data.Any())
            {
                // Log a message indicating that there is no data to create the PDF file
                WriteLogClass.WriteToLog(0, "No data to create the pdf file ....", 1);
                return false;
            }

            // Create a new data list by calling the MakeNewDataListLine method
            var newDataList = await PdfCreationHelperClass.MakeNewDataListLine(data,
                mainFieldList,
                mainFieldsToSkip,
                generatedFieldName);

            // Get the new invoice number from the new data list
            var newInvoiceNumber = newDataList
                .Where(dataItem => dataItem.ContainsKey(generatedFieldName))
                .Select(dataItem => dataItem[generatedFieldName])
                .FirstOrDefault();


            foreach (var newDataItem in newDataList)
            {
                // Create a new Document
                Document document = new();

                // Add a default style to the document main fields
                var mainFieldStyle = document.Styles.AddStyle("MainFieldStyle", "Normal");
                mainFieldStyle.Font.Size = Unit.FromPoint(16);
                mainFieldStyle.Font.Bold = true;
                mainFieldStyle.Font.Name = "Times New Roman";

                // Add a default style to the document values
                var valuesStyle = document.Styles.AddStyle("ValuesStyle", "Normal");
                valuesStyle.Font.Size = Unit.FromPoint(16);
                valuesStyle.Font.Bold = false;
                valuesStyle.Font.Name = "Times New Roman";

                // Add a section to the document
                var section = document.AddSection();

                // Set page width, height, and orientation
                section.PageSetup.PageWidth = Unit.FromPoint(1754);
                section.PageSetup.PageHeight = Unit.FromPoint(1240);
                section.PageSetup.Orientation = Orientation.Landscape;

                // Add header
                var pageHeader = section.Headers.Primary.AddParagraph();
                pageHeader.Format.Alignment = ParagraphAlignment.Right;
                pageHeader.Format.Font.Bold = true;
                pageHeader.Format.Font.Size = Unit.FromPoint(8);
                pageHeader.AddText("Page ");
                pageHeader.AddPageField();
                pageHeader.AddLineBreak();

                // Add empty space before the text
                section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(100);

                // Add static text at the top
                foreach (var (fieldName, fieldValue) in pdfMainFieldList.Zip(pdfMainFieldListValues, Tuple.Create))
                {
                    var paragraph = section.AddParagraph();
                    paragraph.AddFormattedText(fieldName + ": ", mainFieldStyle.Name);
                    paragraph.AddTab();
                    paragraph.AddFormattedText(fieldValue, valuesStyle.Name);
                    paragraph.AddLineBreak();
                    section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(margin);
                }

                // Add dynamic text from newDataItem
                foreach (var (fieldName, fieldValue) in newDataItem)
                {
                    if (mainFieldsToSkip.Contains(fieldName)) continue;
                    var paragraph = section.AddParagraph();
                    paragraph.AddFormattedText(fieldName + ": ", mainFieldStyle.Name);
                    paragraph.AddTab();
                    paragraph.AddFormattedText(fieldValue, valuesStyle.Name);
                    paragraph.AddLineBreak();
                    section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(margin);
                }

                // Add footer
                var footer = section.Footers.Primary.AddParagraph();
                footer.AddText("Created by DEA.NEXT upload system");
                footer.Format.Font.Size = 8;
                footer.Format.Alignment = ParagraphAlignment.Right;
                footer.Format.Font.Bold = true;
                footer.Format.Font.Color = Colors.DarkGray;
                footer.Format.Borders.Top = new Border
                    { Color = Colors.Black, Style = BorderStyle.Single, Width = Unit.FromPoint(0.5) };
                
                // Checking if the new invoice number is null
                if (newInvoiceNumber == null)
                {
                    // Write a log message indicating that the invoice number is null
                    WriteLogClass.WriteToLog(0, "Invoice number is null in Line process ....", 1);
                    return false;
                }
                
                // Saves the document to a PDF file and returns a tuple indicating whether the operation was successful
                // and the new invoice number.
                var saveResult = await PdfCreationHelperClass.SaveDocumentToPdf(document,
                    outputPath,
                    outputFileExtension);

                if (!saveResult.Item1 || string.IsNullOrEmpty(newInvoiceNumber))
                {
                    WriteLogClass.WriteToLog(1,
                        saveResult.Item1 ? "Invoice number is null in Line process ...."
                            : "Pdf file creation unsuccessful ....",
                        1);
                    return false;
                }
                
                // Send the data to the web service asynchronously
                result = await SendToWebServiceWithLines.SendToWebServiceAsync(newDataList,
                    newInvoiceNumber,
                    mainFileName,
                    saveResult.Item2,
                    setId,
                    lastItem,
                    clientId);
                
                // Add a delay to avoid overloading the web service
                await Task.Delay(uploadDelay);
            }

            // Check if the process is not the last item, return the opposite of lastItem
            if (!lastItem) return !lastItem;

            // Switch based on the result of sending data to the web service
            switch (result)
            {
                case 1:
                    // Log a success message if all data was uploaded successfully
                    WriteLogClass.WriteToLog(1, "All data uploaded successfully ....", 4);
                    break;
                default:
                    // Log a failure message if data upload was unsuccessful
                    WriteLogClass.WriteToLog(1, "Data uploaded unsuccessfully ....", 4);
                    break;
            }

            // Call the method to remove files after upload and return the result asynchronously
            return await PdfCreationHelperClass.RemoveFilesAfterUpload(outputPath, mainFileName, clientId);
        }
        catch (Exception e)
        {
            WriteLogClass.WriteToLog(0, $"Exception at process data in batch line: {e.Message}", 0);
            return false;
        }
    }
}