using DEA.Next.FileOperations.TpsFileFunctions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.PdfCreation;

public static class CreatePdfBatchProcess
{
    /// <summary>
    /// Creates a PDF file from the provided data and saves it to the specified output path.
    /// </summary>
    /// <param name="data">The list of dictionaries containing the data to be included in the PDF.</param>
    /// <param name="outputPath">The path where the generated PDF file will be saved.</param>
    /// <param name="mainFileName">Name of the original CSV file.</param>
    /// <param name="setId">The set ID of the PDF file.</param>
    /// <param name="lastItem">True if this is the last item in the batch, false otherwise.</param>
    /// <param name="clientId">The client ID for retrieving user configuration.</param>
    /// <returns>True if the PDF file was created and saved successfully, false otherwise.</returns>
    public static async Task<bool> CreatPdfBatch(List<Dictionary<string, string>>? data,
        string outputPath,
        string mainFileName,
        string setId,
        bool lastItem,
        int clientId)
    {
        try
        {
            // Initialize result to -1
            var result = -1;

            // Set a margin constant
            const int margin = 10;

            // Retrieve user configuration data based on client ID
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the generated field name, line field names, and line fields to skip from user configuration
            var generatedFieldName = jsonData.ReadContentSettings.GeneratedField;
            var lineFieldNames = jsonData.ReadContentSettings.LineFieldNameList;
            var lineFieldsToSkip = jsonData.ReadContentSettings.LineFieldToSkip;

            // Define the main field list for the PDF
            var pdfMainFieldList = new[]
            {
                "Generated Date", "Generated Time", "File Name", "Set Id"
            };

            // Define the values for the main fields of the PDF
            var pdfMainFieldListValues = new[]
            {
                DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToString("HH:mm:ss"),
                Path.GetFileNameWithoutExtension(mainFileName), setId
            };

            // Filter line field names based on fields to skip
            var newHeaders = lineFieldNames
                .Where(lineFieldNameE => !lineFieldsToSkip
                    .Contains(lineFieldNameE)).ToList();

            // Create a new data list for PDF creation
            var newDataList = await PdfCreationHelperClass.MakeNewDataListBatch(data,
                lineFieldNames,
                lineFieldsToSkip,
                generatedFieldName);

            // Extract the new invoice number from the data list
            var newInvoiceNumber = newDataList
                .Where(dataItem => dataItem.ContainsKey(generatedFieldName))
                .Select(dataItem => dataItem[generatedFieldName])
                .FirstOrDefault();

            // Group data based on the generated field name
            var groupedData = newDataList
                .GroupBy(dataItem => dataItem.GetValueOrDefault(generatedFieldName))
                .Where(group => group.Key != null);

            // Convert grouped data to an array of grouping items
            var groupedDataItems = groupedData
                as IGrouping<string, Dictionary<string, string>>[] ?? groupedData.ToArray();

            foreach (var groupedDataItem in groupedDataItems)
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

                // Add a new section to the document
                var section = document.AddSection();

                // Set page width, height, and orientation
                section.PageSetup.PageWidth = Unit.FromPoint(1754);
                section.PageSetup.PageHeight = Unit.FromPoint(1240);
                section.PageSetup.Orientation = Orientation.Landscape;

                // Add header
                var pageHeader = section.Headers.Primary.AddParagraph();
                pageHeader.Format.Alignment = ParagraphAlignment.Right;
                pageHeader.Format.Font.Bold = true;
                pageHeader.Format.Font.Size = 8;
                pageHeader.AddText("Page ");
                pageHeader.AddPageField();
                pageHeader.AddLineBreak();

                // Add empty space before the text and table
                section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(100);

                // Add text before the table
                foreach (var (fieldName, fieldValue) in pdfMainFieldList.Zip(pdfMainFieldListValues, Tuple.Create))
                {
                    var paragraph = section.AddParagraph();
                    paragraph.AddFormattedText(fieldName + ": ", mainFieldStyle.Name);
                    paragraph.AddTab();
                    paragraph.AddFormattedText(fieldValue, valuesStyle.Name);
                    paragraph.AddLineBreak();
                    section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(margin);
                }

                // Add empty space before the table
                section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);

                // Add a table to the section
                var table = section.AddTable();
                table.Borders.Visible = true;

                if (data is null)
                {
                    WriteLogClass.WriteToLog(0, "Data array is null ....", 1);
                    return false;
                }

                // Add columns to the table based on keys in the first data row
                foreach (var unused in newHeaders
                             .Where(lineFieldName => !lineFieldsToSkip
                                 .Contains(lineFieldName)))
                {
                    var column = table.AddColumn(Unit.FromPoint(120));
                    column.Format.Alignment = ParagraphAlignment.Center;
                }

                // Add header row to the table
                var headerRow = table.AddRow();
                headerRow.Format.Alignment = ParagraphAlignment.Center;
                headerRow.Format.Font.Bold = true;
                headerRow.Format.Font.Size = 12;
                headerRow.HeadingFormat = true;

                // Populate the header row cells with the key values
                for (var i = 0; i < newHeaders.Count; i++)
                {
                    if (!lineFieldsToSkip.Contains(newHeaders[i]))
                        headerRow.Cells[i].AddParagraph(newHeaders[i]);
                }

                // Populate the table with data rows
                foreach (var dataItem in groupedDataItem)
                {
                    var row = table.AddRow();
                    for (var j = 0; j < dataItem.Count; j++)
                    {
                        row.Cells[j].AddParagraph(dataItem.ElementAt(j).Value);
                    }
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

                // Save the document to a PDF file
                PdfDocumentRenderer renderer = new()
                {
                    Document = document
                };
                renderer.RenderDocument();
                renderer.PdfDocument.Save(outputPath);

                var groupData = groupedDataItem.ToList();

                // Check if the PDF file exists
                if (!File.Exists(outputPath))
                {
                    continue;
                }
                WriteLogClass.WriteToLog(1, "Pdf file created successfully ....", 1);

                // Check if the invoice number is null or empty
                if (string.IsNullOrEmpty(newInvoiceNumber))
                {
                    WriteLogClass.WriteToLog(0, "Invoice number is null in Batch process ....", 1);
                    return false;
                }

                // Send the data to the web service
                result = await SendToWebServiceWithLines.SendToWebServiceAsync(groupData,
                    newInvoiceNumber,
                    mainFileName,
                    outputPath,
                    setId,
                    lastItem,
                    clientId);
            }

            if (!lastItem) return !lastItem;

            switch (result)
            {
                case 1:
                    WriteLogClass.WriteToLog(1, "All data uploaded successfully ....", 4);
                    break;
                default:
                    WriteLogClass.WriteToLog(1, "Data uploaded unsuccessfully ....", 4);
                    break;
            }

            return await PdfCreationHelperClass.RemoveFilesAfterUpload(outputPath, mainFileName, clientId);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at creating the pdf file: {ex.Message}", 0);
            return false;
        }
    }
}