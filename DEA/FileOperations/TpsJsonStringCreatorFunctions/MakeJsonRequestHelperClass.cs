using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using DEA.Next.HelperClasses.Pdf;
using TpsJsonProjectUploadString;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions;

internal class MakeJsonRequestHelperClass
{
    public static async Task<List<TpsJsonProjectUploadStringClass.FieldList>> ReturnIdFieldList(Guid customerId,
        string clientOrgNo)
    {
        var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

        // Creating the field list to be added to the Json request.
        List<TpsJsonProjectUploadStringClass.FieldList> idField =
        [
            new() { Name = customerDetails.FieldOneName, Value = clientOrgNo }
        ];

        if (!string.IsNullOrWhiteSpace(customerDetails.FieldTwoName) &&
            string.IsNullOrWhiteSpace(customerDetails.FieldTwoValue))
            idField.Add(new TpsJsonProjectUploadStringClass.FieldList
                { Name = customerDetails.FieldTwoName, Value = clientOrgNo });

        if (!string.IsNullOrWhiteSpace(customerDetails.FieldTwoName) &&
            !string.IsNullOrWhiteSpace(customerDetails.FieldTwoValue))
            idField.Add(new TpsJsonProjectUploadStringClass.FieldList
                { Name = customerDetails.FieldTwoName, Value = customerDetails.FieldTwoValue });

        return idField;
    }

    public static List<TpsJsonProjectUploadStringClass.FileList> ReturnFileList(string[] filesToSend)
    {
        // Creating the file list to be added to the Json request.
        return filesToSend
            .Select(file => new TpsJsonProjectUploadStringClass.FileList
                { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) })
            .ToList();
    }

    public static async Task<List<TpsJsonSendBodyTextClass.FieldList>> ReturnEmailBodyFieldList(Guid customerId,
        string recipientEmail,
        string bodyText)
    {
        // Retrieve the customer details.
        var customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);
        var idFieldOneName = customerDetails.FieldOneName;
        var idFieldOneValue = customerDetails.FieldOneValue;
        var idFieldTwoName = customerDetails.FieldTwoName;
        var idFieldTwoValue = customerDetails.FieldTwoValue;


        List<TpsJsonSendBodyTextClass.FieldList> emailFieldList = [];

        if (string.IsNullOrEmpty(idFieldOneValue) &&
            string.IsNullOrEmpty(idFieldTwoValue) &&
            !string.IsNullOrEmpty(idFieldOneName) &&
            !string.IsNullOrEmpty(idFieldTwoName))
        {
            emailFieldList.Add(new TpsJsonSendBodyTextClass.FieldList
                { Name = idFieldOneName, Value = recipientEmail });
            emailFieldList.Add(new TpsJsonSendBodyTextClass.FieldList
                { Name = idFieldTwoName, Value = bodyText });
        }

        if (string.IsNullOrEmpty(idFieldOneValue) &&
            string.IsNullOrEmpty(idFieldTwoValue) &&
            !string.IsNullOrEmpty(idFieldOneName) &&
            string.IsNullOrEmpty(idFieldTwoName))
            emailFieldList.Add(new TpsJsonSendBodyTextClass.FieldList
                { Name = idFieldOneName, Value = bodyText });

        if (!string.IsNullOrEmpty(idFieldOneValue) &&
            string.IsNullOrEmpty(idFieldTwoValue) &&
            !string.IsNullOrEmpty(idFieldOneName) &&
            !string.IsNullOrEmpty(idFieldTwoName))
            emailFieldList.Add(new TpsJsonSendBodyTextClass.FieldList
                { Name = idFieldTwoName, Value = bodyText });

        return emailFieldList;
    }

    public static List<TpsJsonSendBodyTextClass.FileList> ReturnEmailBodyFileList(string subject)
    {
        // Creating an empty file to add to the JSON request.
        var fileName = subject + ".pdf";
        var documentBytes = CreateSamplePdf.CreateSamplePdfWithWatermarkAsync(subject).Result;
        return [new TpsJsonSendBodyTextClass.FileList { Name = fileName, Data = documentBytes }];
    }
}