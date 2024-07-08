using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.FileOperations.TpsServerResponseFunctions;
using Newtonsoft.Json;
using UserConfigRetriverClass;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestLinesFunction
    {
        public static async Task<int> MakeJsonRequestLines(List<Dictionary<string, string>> data,
            string mainFileName,
            string setId,
            int clientId,
            string[] localFileList)
        {
            try
            {
                var jsonData = await UserConfigRetriver.RetriveUserConfigById(clientId);

                var fieldsList = MakeJsonRequestHelperClass.ReturnIdFieldListLines(mainFileName,
                    setId,
                    clientId);

                var allRessultsTrue = true;
                foreach (var localFile in localFileList)
                {
                   var jsonRequest = await CreatTheJsonRequestLines(localFile, jsonData, data, fieldsList);
                   var result = await SendFilestoApiLines.SendFilesToApiLinesAsync(jsonRequest, clientId);

                   if (result) continue;
                   allRessultsTrue = false;
                   break;

                }

                if (allRessultsTrue)
                {
                    var folderPath = Path.GetDirectoryName(localFileList[0]);
                    if (folderPath != null)
                        return await TpsServerOnSuccessLines.ServerOnSuccessLinesAsync(folderPath, localFileList);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at Json serialization: {ex.Message}", 0);
                return -1;
            }
            return -1;
        }

        private static async Task<string> CreatTheJsonRequestLines(string localFile,
            UserConfigSetter.Customerdetail jsonData,
            List<Dictionary<string, string>> data,
            TpsJsonLinesUploadString.Fields[] fieldsList)
        {
            try
            {
                var localFileList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnFilesListLines(localFile));
                var tablesList = await Task.Run(() => MakeJsonRequestHelperClass.ReturnTableListLines(data));

                var tpsJsonRequest = new TpsJsonLinesUploadString.TpsJsonLinesUploadObject
                {
                    Token = jsonData.Token,
                    Username = jsonData.UserName,
                    TemplateKey = jsonData.TemplateKey,
                    Queue = jsonData.Queue,
                    ProjectID = jsonData.ProjetID,
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
                throw;
            }
        }
    }
}
