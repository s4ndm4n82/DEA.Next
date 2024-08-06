using DEA.Next.HelperClasses.PdfCreation;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal static class CreatePdfFile
    {
        /// <summary>
        /// Start the process of creating a PDF file.
        /// </summary>
        /// <param name="data">List of dictionaries containing data for the PDF file.</param>
        /// <param name="downloadFilePath">The path where the PDF file will be downloaded.</param>
        /// <param name="mainFileName">The main file name of the PDF file.</param>
        /// <param name="setId">The set ID of the PDF file.</param>
        /// <param name="lastItem">True if this is the last item in the data list, false otherwise.</param>
        /// <param name="clientId">The client ID for retrieving user configuration.</param>
        /// <returns>True if the PDF file creation process is successful, false otherwise.</returns>
        public static async Task<bool> StartCreatePdfFile(List<Dictionary<string, string>>? data,
                                                           string downloadFilePath,
                                                           string mainFileName,
                                                           string setId,
                                                           bool lastItem,
                                                           int clientId)
        {
            // Read the user config file.
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the output filename
            var outputFileName = PdfCreationHelperClass.MakeOutPutFileName(mainFileName, jsonData.ReadContentSettings.OutputFileExtension);

            // Get the output path.
            var outputPath = Path.Combine(downloadFilePath, outputFileName);

            var fileB2BTrue = mainFileName
                .Contains(jsonData.ReadContentSettings.ReadByLineTrigger, StringComparison.OrdinalIgnoreCase);

            if (data != null && jsonData.ReadContentSettings is { MakeUploadFile: true } && !fileB2BTrue)
                return await CreatePdfBatchProcess.CreatPdfBatch(data,
                    outputPath,
                    mainFileName,
                    setId,
                    lastItem,
                    clientId);
            
            if (data != null && jsonData.ReadContentSettings is { MakeUploadFile: true } && fileB2BTrue)
                return await CreatePdfLineProcess.CreatPdfBatchByLine(data,
                    outputPath,
                    mainFileName,
                    setId,
                    lastItem,
                    clientId);

            WriteLogClass.WriteToLog(1, "No data to create the pdf file ....", 1);
            return false;
        }
    }
}
