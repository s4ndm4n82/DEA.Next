using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.FileOperations.TpsServerResponseFunctions;
using Newtonsoft.Json;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal static class MakeJsonRequestLinesFunction
    {
        public static async Task<int> MakeJsonRequestBatch(List<Dictionary<string, string>>? data,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListBatch(mainFileName,
                    setId,
                    clientId);
                
                var jsonRequest = await CreatTheJsonRequestBatch(localFilePath,
                    jsonData,
                    data,
                    fieldsList);
                
                var result = await SendFilesToApiLines.SendFilesToApiAsync(jsonRequest,
                    localFilePath,
                    clientId);

                if (!result) return await TapsServerOnFailLines.ServerOnFailLinesAsync(localFilePath,
                    setId,
                    clientId);
                
                if (File.Exists(localFilePath))
                    return await TpsServerOnSuccessLines.ServerOnSuccessLinesAsync(localFilePath);
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return -1;
            }
            return -1;
        }

        private static async Task<string> CreatTheJsonRequestBatch(string localFile,
            UserConfigSetter.Customerdetail jsonData,
            List<Dictionary<string, string>>? data,
            TpsJsonLinesUploadString.Fields[] fieldsList)
        {
            try
            {
                var localFileList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnFilesListBatch(localFile));
                var tablesList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnTableListBatch(data));

                var tpsJsonRequest = new TpsJsonLinesUploadString.TpsJsonLinesUploadObject
                {
                    Token = jsonData.Token,
                    Username = jsonData.UserName,
                    TemplateKey = jsonData.TemplateKey,
                    Queue = jsonData.Queue,
                    ProjectId = jsonData.ProjetID,
                    Fields = fieldsList,
                    Tables = tablesList, 
                    Files = localFileList
                };
                
                var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);
                
                return jsonResult;
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at lines Json serialization: {e.Message}", 0);
                return string.Empty;
            }
        }

        public static async Task<int> MakeJsonRequestByLine(List<string> valuesList,
            string newInvoiceNumber,
            string mainFileName,
            string localFilePath,
            string setId,
            int clientId)
        {
            var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);
            
            var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListLines(valuesList,
                newInvoiceNumber,
                mainFileName,
                setId,
                clientId);
            
            var jsonRequest = await CreatTheJsonRequestLines(localFilePath,
                jsonData,
                fieldsList);
            
            var result = await SendFilesToApiLines.SendFilesToApiAsync(jsonRequest,
                localFilePath,
                clientId);
            
            if (!result) return await TapsServerOnFailLines.ServerOnFailLinesAsync(localFilePath,
                setId,
                clientId);
            
            if (File.Exists(localFilePath))
                return await TpsServerOnSuccessLines.ServerOnSuccessLinesAsync(localFilePath);
            
            return -1;
        }
        
        private static async Task<string> CreatTheJsonRequestLines(string localFile,
            UserConfigSetter.Customerdetail jsonData,
            TpsJsonLinesUploadString.Fields[] fieldList)
        {
            try
            {
                var localFileList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnFilesListBatch(localFile));
                
                var tpsJsonRequest = new TpsJsonLinesUploadString.TpsJsonLinesUploadObject
                {
                    Token = jsonData.Token,
                    Username = jsonData.UserName,
                    TemplateKey = jsonData.TemplateKey,
                    Queue = jsonData.Queue,
                    ProjectId = jsonData.ProjetID,
                    Fields = fieldList,
                    Tables = Array.Empty<TpsJsonLinesUploadString.Tables>(), 
                    Files = localFileList
                };
                
                var jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);
                
                return jsonResult;
            }
            catch (Exception e)
            {
                WriteLogClass.WriteToLog(0, $"Exception at lines Json serialization: {e.Message}", 0);
                return string.Empty;
            }
        }
    }
}
