using PdfSharp.Pdf;
using PdfSharp.Drawing;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using PdfSharp.UniversalAccessibility.Drawing;
using Azure;
using WriteLog;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal class CreatePdfFile
    {
        public static async Task<bool> StartCreatePdfFile(List<Dictionary<string, string>> data,
                                                         string downloadFilePath,
                                                         string mainFileName,
                                                         int clientId)
        {
            // Read the user config file.
            UserConfigSetter.Customerdetail jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the output filename
            string outputFileName = MakeOutPutFileName(mainFileName, jsonData.ReadContentSettings.FileExtension);

            // Get the output path.
            string outputPath = Path.Combine(downloadFilePath, outputFileName);

            if (data.Count == 0)
            {
                WriteLogClass.WriteToLog(1, "No data to create the pdf file.", 1);
                return false;
            }

            await CreatingTheFile(data, outputPath);

            return false;
        }

        private static async Task<bool> CreatingTheFile(List<Dictionary<string, string>> data, string outputPath)
        {
            try
            {
                // Creating the new PDF file
                PdfDocument document = new();

                // Adding a new page
                PdfPage newPage = document.AddPage();

                // Getting the graphics from the page
                XGraphics gfx = XGraphics.FromPdfPage(newPage);

                // Creating the pdf font
                XFont headerFont = new("Aria", 10, XFontStyleEx.Bold);
                XFont lineFont = new("Aria", 8, XFontStyleEx.Regular);

                // Define page dimensions
                double pageWidth = newPage.Width.Point;
                double pageHeight = newPage.Height.Point;

                // Define table dimensions
                double tableWidth = 11 * 100;
                double tableHeight = data.Count * 20;

                // Calculate the starting position for the table to center it vertically
                double tableXPosition = (pageWidth - tableWidth) / 2;
                double tableYPosition = (pageHeight - tableHeight) / 2;

                // Defaning column width and row height
                double columnWidth = 100;
                double rowHeight = 20;

                // Darwing the table headers
                foreach (string header in data[0].Keys)
                {
                    gfx.DrawString(header, headerFont, XBrushes.Black, tableXPosition, tableYPosition);
                    tableXPosition += columnWidth;
                }

                tableWidth += rowHeight;

                // Drawing the table lines
                foreach (var row in data)
                {
                    tableXPosition = (pageWidth - tableWidth) / 2;
                    foreach (string value in row.Values)
                    {
                        gfx.DrawString(value, lineFont, XBrushes.Black, tableXPosition, tableYPosition);

                        // Drawing the veritical lines
                        gfx.DrawLine(XPens.Black, tableXPosition, tableYPosition, tableXPosition, tableYPosition + rowHeight);
                        tableYPosition += rowHeight;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at creating the pdf file: {ex.Message}", 0);
                return false;
            }

            return false;
        }

        private static string MakeOutPutFileName(string mainFileName, string outputFileExrention)
        {
            // Get current date and time
            DateTime now = DateTime.Now;
            string dateTimeString = now.ToString("yyyyMMdd_HHmmss");

            // Main file name
            string mainFilnameOnly = Path.GetFileNameWithoutExtension(mainFileName);

            // Creating the outout filename
            return string.Concat(mainFilnameOnly, "_", dateTimeString, ".", outputFileExrention.ToLower()).Replace(" ", "");
        }
    }
}
