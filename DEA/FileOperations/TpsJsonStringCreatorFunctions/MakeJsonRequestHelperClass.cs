using DEA.Next.FileOperations.TpsJsonStringClasses;
using TpsJsonProjectUploadString;
using UserConfigRetriverClass;
using WriteLog;
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
        
        // CSV file read processed as batches
        public static TpsJsonLinesUploadString.Fields[] ReturnIdFieldListBatch(string mainFileName,
            string setId,
            int clientId)
        {
            var jsonData = UserConfigRetriver.RetriveUserConfigById(clientId).Result;

            var mainField = new[]
            {
                new TpsJsonLinesUploadString.Fields() { Name = jsonData.ClientIdField, Value = setId },
                new TpsJsonLinesUploadString.Fields() { Name = jsonData.ClientIdField2, Value = mainFileName }
            };
            
            return mainField;
        }
        
        public static TpsJsonLinesUploadString.Files[] ReturnFilesListBatch(string fileToSend)
        {
            var jsonFileList = new[]
            {
                new TpsJsonLinesUploadString.Files()
                {
                    Name = Path.GetFileName(fileToSend),
                    Data = Convert.ToBase64String(File.ReadAllBytes(fileToSend))
                }
            };

            return jsonFileList;
        }

        public static TpsJsonLinesUploadString.Tables[] ReturnTableListBatch(List<Dictionary<string, string>>? data)
        {
            var tableList = new TpsJsonLinesUploadString.Tables[1];

            var rowsList = new List<TpsJsonLinesUploadString.Rows>();
            
            foreach (var rowData in data)
            {
                TpsJsonLinesUploadString.Rows rows = new()
                {
                    FieldsList = new TpsJsonLinesUploadString.Fields1[rowData.Count]
                };

                var fieldsIndex = 0;
                foreach (var fields1 in rowData.Select(fieldData => new TpsJsonLinesUploadString.Fields1()
                         {
                             Name = fieldData.Key,
                             Value = fieldData.Value
                         }))
                {
                    rows.FieldsList[fieldsIndex] = fields1;
                    fieldsIndex++;
                }
                
                rowsList.Add(rows);
            }
            tableList[0] = new TpsJsonLinesUploadString.Tables() { TableRows = rowsList.ToArray() };

            return tableList;
        }
        
        // CSV file read processed as lines
        public static TpsJsonLinesUploadString.Fields[] ReturnIdFieldListLines(List<string> valueList,
            string newInvoiceNumber,
            string mainFileName,
            string setId,
            int clientId)
        {
            var jsonData = UserConfigRetriver.RetriveUserConfigById(clientId).Result;
            var mainFieldNameList = jsonData.ReadContentSettings.MainFieldNameList;

            if (mainFieldNameList == null)
            {
                WriteLogClass.WriteToLog(1, "Main field name list is empty ....", 0);
                return Array.Empty<TpsJsonLinesUploadString.Fields>();
            }

            var mainField = new List<TpsJsonLinesUploadString.Fields>
            {
                new() { Name = jsonData.ClientIdField, Value = setId },
                new() { Name = jsonData.ClientIdField2, Value = mainFileName },
                new() { Name = mainFieldNameList[2], Value = newInvoiceNumber }
            };

            foreach (var (fieldName, fieldValue) in mainFieldNameList.Zip(valueList, (name, value) => (name, value)))
            {
                mainField.Add(new TpsJsonLinesUploadString.Fields() { Name = fieldName, Value = fieldValue });
            }
            
            return mainField.ToArray();
        }
    }
}
