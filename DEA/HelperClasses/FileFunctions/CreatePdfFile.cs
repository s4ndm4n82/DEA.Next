using DEA.Next.HelperClasses.PdfCreation;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.FileFunctions
{
    internal static class CreatePdfFile
    {
        /// <summary>
        /// Starts the process of creating a PDF file from a list of dictionaries.
        /// </summary>
        /// <param name="data">The list of dictionaries containing the data to be written to the PDF file.</param>
        /// <param name="downloadFilePath">The path where the PDF file will be saved.</param>
        /// <param name="mainFileName">The name of the main file.</param>
        /// <param name="setId">The ID of the set.</param>
        /// <param name="lastItem">A boolean indicating whether this is the last item in the batch.</param>
        /// <param name="loopCount">The number of times the loop has been executed.</param>
        /// <param name="clientId">The ID of the client.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the PDF file was successfully created.</returns>
        public static async Task<bool> StartCreatePdfFile(List<Dictionary<string, string>>? data,
            string downloadFilePath,
            string mainFileName,
            string setId,
            bool lastItem,
            int fileNameSequence,
            int clientId)
        {
            // Read the user config file.
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

            // Get the output filename
            var outputFileName =
                PdfCreationHelperClass.MakeOutPutFileName(mainFileName,
                    jsonData.ReadContentSettings.OutputFileExtension);

            // Get the output path.
            var outputPath = Path.Combine(downloadFilePath, outputFileName);

            // Check if the main filename contains the trigger string for B2B files.
            var fileB2BTrue = !string.IsNullOrEmpty(jsonData.ReadContentSettings.ReadByLineTrigger)
                              && mainFileName.Contains(jsonData.ReadContentSettings.ReadByLineTrigger,
                                  StringComparison.OrdinalIgnoreCase);

            // If there is data to process and the user config indicates that an upload file should be created,
            // and the main filename does not contain the trigger string for B2B files,
            // then create the PDF file using the batch process.
            if (data != null && jsonData.ReadContentSettings is { MakeUploadFile: true } && !fileB2BTrue)
                return await CreatePdfBatchProcess.CreatPdfBatch(data,
                    outputPath,
                    mainFileName,
                    setId,
                    lastItem,
                    fileNameSequence,
                    clientId);

            // If there is data to process and the user config indicates that an upload file should be created,
            // and the main filename contains the trigger string for B2B files,
            // then create the PDF file using the line process.
            if (data != null && jsonData.ReadContentSettings is { MakeUploadFile: true } && fileB2BTrue)
                return await CreatePdfLineProcess.CreatPdfBatchByLine(data,
                    outputPath,
                    mainFileName,
                    setId,
                    lastItem,
                    clientId);

            // If there is no data to process, write a log message and return false.
            WriteLogClass.WriteToLog(1, "No data to create the pdf file ....", 1);
            return false;
        }
    }
}
