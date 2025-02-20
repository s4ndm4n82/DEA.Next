using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Properties;
using WriteLog;

namespace DEA.Next.HelperClasses.Pdf;

public class CreateSamplePdf
{
    public static async Task<string> CreateSamplePdfWithWatermarkAsync(string subject)
    {
        // Create a new PDF document
        using var stream = new MemoryStream();
        // Initialize PDF writer
        var writer = new PdfWriter(stream);
        // Initialize PDF document
        var pdf = new PdfDocument(writer);
        // Initialize document
        var document = new Document(pdf);

        // Adding watermark to the PDF document
        await AddWatermarkToPdfAsync(pdf, subject);
        document.Close();

        // Return the PDF document as a byte array
        var pdfBytes = stream.ToArray();
        return Convert.ToBase64String(pdfBytes);
    }

    private static async Task AddWatermarkToPdfAsync(PdfDocument pdf, string watermarkText)
    {
        try
        {
            // Create a watermark}
            var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
            var pdfCanvas = new Canvas(pdf.AddNewPage(), pdf.GetDefaultPageSize());
            pdfCanvas.SetFont(font);
            pdfCanvas.SetFontSize(60);
            pdfCanvas.SetFontColor(ColorConstants.LIGHT_GRAY);
            pdfCanvas.ShowTextAligned(watermarkText,
                pdf.GetDefaultPageSize().GetWidth() / 2,
                pdf.GetDefaultPageSize().GetHeight() / 2,
                TextAlignment.CENTER,
                VerticalAlignment.MIDDLE,
                45);
            pdfCanvas.Close();
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at AddWatermarkToPdfAsync: {ex.Message}", 0);
            throw;
        }
    }
}