using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using TpsJsonProjectUploadString;
using static UserConfigSetterClass.UserConfigSetter;

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

        if (!string.IsNullOrWhiteSpace(customerDetails.FieldTwoName) && string.IsNullOrWhiteSpace(customerDetails.FieldTwoValue))
        {
            idField.Add(new TpsJsonProjectUploadStringClass.FieldList { Name = customerDetails.FieldTwoName, Value = clientOrgNo });
        }

        if (!string.IsNullOrWhiteSpace(customerDetails.FieldTwoName) && !string.IsNullOrWhiteSpace(customerDetails.FieldTwoValue))
        {
            idField.Add(new TpsJsonProjectUploadStringClass.FieldList() { Name = customerDetails.FieldTwoName, Value = customerDetails.FieldTwoValue });
        }

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
}