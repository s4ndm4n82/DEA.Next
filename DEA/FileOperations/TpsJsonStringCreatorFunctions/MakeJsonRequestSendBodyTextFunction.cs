using DEA.Next.FileOperations.TpsFileUploadFunctions;
using DEA.Next.FileOperations.TpsJsonStringClasses;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.Graph;
using Newtonsoft.Json;
using UserConfigSetterClass;
using WriteLog;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestSendBodyTextFunction
    {
        public static async Task<int> MakeJsonRequestSendBodyTextAsync(IMailFolderRequestBuilder requestBuilder,
                                                                       string messageId,
                                                                       string messageSubject,
                                                                       string messageBody,
                                                                       int customerId)
        {
            try
            {
                UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriever.RetrieveUserConfigById(customerId);

                // Creaing the email field list to be added to the JSON request.
                List<TpsJsonSendBodyTextClass.Emailfieldlist> emailFieldList = MakeJsonRequestHelperClass.ReturnEmailFieldList(customerId, messageBody);

                // Creating the JSON request.
                TpsJsonSendBodyTextClass.TpsJsonSendBodyText tpsJsonRequest = new()
                {
                    Token = customerDetails.Token,
                    Username = customerDetails.UserName,
                    TemplateKey = customerDetails.TemplateKey,
                    Queue = customerDetails.Queue,
                    ProjectID = customerDetails.ProjectId,
                    EmailFieldList = emailFieldList
                };

                // Assigning the JSON request to a string. To be handed over to the REST API.
                string jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

                return await SendBodyTextToRestApi.SendBodyTextToRestAsync(requestBuilder,
                                                                           messageId,
                                                                           messageSubject,
                                                                           jsonResult,
                                                                           customerId);
            } 
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at MakeJsonRequestSendBodyTextAsync: {ex.Message}", 0);
                return 0;
            }
        }
    }
}
