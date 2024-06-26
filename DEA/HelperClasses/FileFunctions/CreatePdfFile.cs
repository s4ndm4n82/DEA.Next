using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using UserConfigRetriverClass;
using UserConfigSetterClass;
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

            CreatingTheFile(data, outputPath);

            return false;
        }

        private static bool CreatingTheFile(List<Dictionary<string, string>> data, string outputPath)
        {
            try
            {
                Document document = new();
                var section = document.AddSection();

                section.PageSetup.PageWidth = Unit.FromPoint(1754);
                section.PageSetup.PageHeight = Unit.FromPoint(1240);
                section.PageSetup.Orientation = Orientation.Landscape;
                section.VerticalAlignment = VerticalAlignment.Center;

                var table = section.AddTable();
                table.Borders.Visible = true;

                foreach (var header in data[0].Keys)
                {
                    table.AddColumn(Unit.FromPoint(100));
                }

                var headerRow = table.AddRow();

                for (int i = 0; i < data[0].Keys.Count; i++)
                {
                    headerRow.Cells[i].AddParagraph(data[0].Keys.ElementAt(i));
                }

                for (int i = 0; i < data.Count; i++)
                {
                    var row = table.AddRow();
                    for (int j = 0; j < data[i].Count; j++)
                    {
                        row.Cells[j].AddParagraph(data[i].ElementAt(j).Value);
                    }
                }

                // Save the document to a PDF file
                PdfDocumentRenderer renderer = new()
                {
                    Document = document
                };
                renderer.RenderDocument();
                renderer.PdfDocument.Save(outputPath);

                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at creating the pdf file: {ex.Message}", 0);
                return false;
            }
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
