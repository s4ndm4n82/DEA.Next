using DEA.Next.FileOperations.TpsFileFunctions;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal class CreatePdfFile
    {
        /// <summary>
        /// Start the process of creating a PDF file.
        /// </summary>
        /// <param name="data">List of dictionaries containing data for the PDF file.</param>
        /// <param name="downloadFilePath">The path where the PDF file will be downloaded.</param>
        /// <param name="mainFileName">The main file name of the PDF file.</param>
        /// <param name="setId">The set ID of the PDF file.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        /// <returns>True if the PDF file creation process is successful, false otherwise.</returns>
        public static async Task<bool> StartCreatePdfFile(List<Dictionary<string, string>> data,
                                                           string downloadFilePath,
                                                           string mainFileName,
                                                           string setId,
                                                           int clientId)
        {
            // Read the user config file.
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the output filename
            var outputFileName = MakeOutPutFileName(mainFileName, jsonData.ReadContentSettings.OutputFileExtension);

            // Get the output path.
            var outputPath = Path.Combine(downloadFilePath, outputFileName);

            if (data.Any()) return await CreatingTheFile(data, outputPath, mainFileName, setId, clientId);
            
            WriteLogClass.WriteToLog(1, "No data to create the pdf file.", 1);
            return false;

        }

        /// <summary>
        /// Creates a PDF file from the provided data and saves it to the specified output path.
        /// </summary>
        /// <param name="data">The list of dictionaries containing the data to be included in the PDF.</param>
        /// <param name="outputPath">The path where the generated PDF file will be saved.</param>
        /// <param name="mainFileName">Name of the original CSV file.</param>
        /// <param name="setId">The set ID of the PDF file.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        /// <returns>True if the PDF file was created and saved successfully, false otherwise.</returns>
        private static async Task<bool> CreatingTheFile(List<Dictionary<string, string>> data,
                                            string outputPath,
                                            string mainFileName,
                                            string setId,
                                            int clientId)
        {
            try
            {
                // Create a new Document
                Document document = new();

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
                section.AddParagraph($"Generated Date: {DateTime.Now:yyyy-MM-dd}");
                section.AddParagraph($"Generated Time: {DateTime.Now:HH:mm:ss}");
                section.AddParagraph($"File Name: {Path.GetFileNameWithoutExtension(mainFileName)}");
                section.AddParagraph($"Set ID: {setId}");

                // Add empty space before the table
                section.AddParagraph().Format.SpaceAfter = Unit.FromPoint(10);

                // Add a table to the section
                var table = section.AddTable();
                table.Borders.Visible = true;

                // Add columns to the table based on keys in the first data row
                foreach (var header in data[0].Keys)
                {
                    table.AddColumn(Unit.FromPoint(100));
                }

                // Add header row to the table
                var headerRow = table.AddRow();
                headerRow.Format.Alignment = ParagraphAlignment.Center;
                headerRow.Format.Font.Bold = true;
                headerRow.Format.Font.Size = 12;
                headerRow.HeadingFormat = true;

                // Populate the header row cells with the key values
                for (var i = 0; i < data[0].Keys.Count; i++)
                {
                    headerRow.Cells[i].AddParagraph(data[0].Keys.ElementAt(i));
                }

                // Populate the table with data rows
                foreach (var t in data)
                {
                    var row = table.AddRow();
                    for (var j = 0; j < t.Count; j++)
                    {
                        row.Cells[j].AddParagraph(t.ElementAt(j).Value);
                    }
                }

                // Add footer
                var footer = section.Footers.Primary.AddParagraph();
                footer.AddText("Created by DEA.NEXT upload system");
                footer.Format.Font.Size = 8;
                footer.Format.Alignment = ParagraphAlignment.Right;
                footer.Format.Font.Bold = true;
                footer.Format.Font.Color = Colors.DarkGray;
                footer.Format.Borders.Top = new Border { Color = Colors.Black, Style = BorderStyle.Single, Width = Unit.FromPoint(0.5) };


                // Save the document to a PDF file
                PdfDocumentRenderer renderer = new()
                {
                    Document = document
                };
                renderer.RenderDocument();
                renderer.PdfDocument.Save(outputPath);

                if (File.Exists(outputPath))
                {
                    WriteLogClass.WriteToLog(1, "Pdf file created successfully ....", 1);
                    /*await SendToWebServiceWithLines.SendToWebServiceWithLinesAsync(data,
                        mainFileName,
                        outputPath,
                        setId,
                        clientId);*/
                    return true;
                }

                WriteLogClass.WriteToLog(0, "Pdf file not created successfully.", 0);
                return false;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at creating the pdf file: {ex.Message}", 0);
                return false;
            }
        }

        /// <summary>
        /// This function generates an output file name based on the main file name and output file extension.
        /// </summary>
        /// <param name="mainFileName">The name of the main file.</param>
        /// <param name="outputFileExtension">The extension of the output file.</param>
        /// <returns>The generated output file name.</returns>
        private static string MakeOutPutFileName(string mainFileName, string outputFileExtension)
        {
            // Get current date and time
            var now = DateTime.Now;
            var dateTimeString = now.ToString("yyyyMMdd_HHmmss");

            // Main file name
            var mainFilnameOnly = Path.GetFileNameWithoutExtension(mainFileName);

            // Creating the output filename
            return string.Concat(mainFilnameOnly, "_", dateTimeString, ".", outputFileExtension.ToLower()).Replace(" ", "_");
        }
    }
}
