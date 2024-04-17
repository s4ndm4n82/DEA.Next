using DEA.Next.FileOperations.TpsJsonStringClasses;
using Newtonsoft.Json;
using UserConfigRetriverClass;
using UserConfigSetterClass;

namespace DEA.Next.FileOperations.TpsJsonStringCreatorFunctions
{
    internal class MakeJsonRequestSendBodyTextFunction
    {
        public static async Task<int> MakeJsonRequestSendBodyTextAsync(string bodyText, int customerId)
        {
            UserConfigSetter.Customerdetail customerDetails = await UserConfigRetriver.RetriveUserConfigById(customerId);

            // Creaing the email field list to be added to the JSON request.
            List<TpsJsonSendBodyTextClass.Emailfieldlist> emailFieldList = MakeJsonRequestHelperClass.ReturnEmailFieldList(customerId, bodyText);

            // Creating the JSON request.
            TpsJsonSendBodyTextClass.TpsJsonSendBodyText tpsJsonRequest = new()
            {
                Token = customerDetails.Token,
                Username = customerDetails.UserName,
                TemplateKey = customerDetails.TemplateKey,
                Queue = customerDetails.Queue,
                ProjectID = customerDetails.ProjetID,
                EmailFieldList = emailFieldList
            };

            // Assigning the JSON request to a string. To be handed over to the REST API.
            string jsonResult = JsonConvert.SerializeObject(tpsJsonRequest, Formatting.Indented);

            return 0;
        }
    }
}
