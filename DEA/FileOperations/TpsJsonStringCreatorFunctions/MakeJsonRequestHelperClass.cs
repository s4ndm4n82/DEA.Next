using DEA.Next.FileOperations.TpsJsonStringClasses;
using TpsJsonProjectUploadString;
using UserConfigRetriverClass;
using static UserConfigSetterClass.UserConfigSetter;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestHelperClass
    {
        public static List<TpsJsonProjectUploadStringClass.FieldList> ReturnIdFieldList(int customerId,
                                                                                        string clientOrgNo)
        {
            Customerdetail customerDetails = UserConfigRetriver.RetriveUserConfigById(customerId).Result;

            // Creating the field list to be added to the Json request.
            List<TpsJsonProjectUploadStringClass.FieldList> idField = new()
            {
                new TpsJsonProjectUploadStringClass.FieldList() { Name = customerDetails.ClientIdField, Value = clientOrgNo }
            };

            if (!string.IsNullOrWhiteSpace(customerDetails.ClientIdField2) && string.IsNullOrWhiteSpace(customerDetails.IdField2Value))
            {
                idField.Add(new TpsJsonProjectUploadStringClass.FieldList() { Name = customerDetails.ClientIdField2, Value = clientOrgNo });
            }

            if (!string.IsNullOrWhiteSpace(customerDetails.ClientIdField2) && !string.IsNullOrWhiteSpace(customerDetails.IdField2Value))
            {
                idField.Add(new TpsJsonProjectUploadStringClass.FieldList() { Name = customerDetails.ClientIdField2, Value = customerDetails.IdField2Value });
            }

            return idField;
        }

        public static List<TpsJsonLinesUploadString.Fields> ReturnIdFieldListLines(string mainFileName,
            string setId,
            int clientId)
        {
            var jsonData = UserConfigRetriver.RetriveUserConfigById(clientId).Result;

            List<TpsJsonLinesUploadString.Fields> mainField = new()
            {
                new TpsJsonLinesUploadString.Fields() { Name = jsonData.ClientIdField, Value = mainFileName},
                new TpsJsonLinesUploadString.Fields() { Name = jsonData.ClientIdField2, Value = setId}
            };
            
            return mainField;
        }

        public static List<TpsJsonProjectUploadStringClass.FileList> ReturnFileList(string[] filesToSend)
        {
            // Creating the file list to be added to the Json request.
            List<TpsJsonProjectUploadStringClass.FileList> jsonFileList = new();
            foreach (var file in filesToSend)
            {
                jsonFileList.Add(new TpsJsonProjectUploadStringClass.FileList() { Name = Path.GetFileName(file), Data = Convert.ToBase64String(File.ReadAllBytes(file)) });
            }

            return jsonFileList;
        }

        public static List<TpsJsonSendBodyTextClass.Emailfieldlist> ReturnEmailFieldList(int customerId, string bodyText)
        {
            Customerdetail customerDetails = UserConfigRetriver.RetriveUserConfigById(customerId).Result;
            List<Emailfieldlist> emailFieldNames = customerDetails.EmailDetails.EmailFieldList.Where(fname => fname.FieldName != "FieldId").ToList();
            List<TpsJsonSendBodyTextClass.Emailfieldlist> emailFieldList = new();

            // Creating the list with the email body text.
            foreach (Emailfieldlist emailFieldName in emailFieldNames)
            {
                emailFieldList.Add(new TpsJsonSendBodyTextClass.Emailfieldlist() { Name = emailFieldName.FieldName, Value = bodyText });
            }

            return emailFieldList;
        }
    }
}
