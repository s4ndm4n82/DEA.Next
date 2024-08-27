using System.Text.RegularExpressions;
using DEA.Next.HelperClasses.ConfigFileFunctions;
using Microsoft.IdentityModel.Tokens;
using WriteLog;

namespace DEAMailer.MainClasses
{
    internal static class CreateEmailClass
    {
        private class EmailInformation
        {
            public int CustomerId { get; set; }
            public int FileCount { get; set; }
            public int FolderCount { get; set; }
        }

        public static bool StartCreatingEmail(DirectoryInfo folderPath, IEnumerable<DirectoryInfo> folderList)
        {
            try
            {
                var detailsArray = Array.Empty<string>();

                EmailInformation emailInformation = new()
                {
                    FolderCount = folderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly).Count()
                };

                foreach (var subFolderPath in folderList)
                {
                    const string regexPattern = @"ID_(\d+)";
                    var matchValue = Regex.Match(subFolderPath.Name, regexPattern);

                    if (matchValue.Success)
                    {
                        emailInformation.CustomerId = int.Parse(matchValue.Groups[1].Value);
                        emailInformation.FileCount = subFolderPath.EnumerateFiles("*.*", SearchOption.AllDirectories).Count();

                        var jsonData = UserConfigRetriever.RetrieveUserConfigById(emailInformation.CustomerId);
                        var clientDetails = jsonData.Result;

                        if (!clientDetails.ClientName.IsNullOrEmpty())
                        {
                            // Resize the array to accomodate the new line
                            Array.Resize(ref detailsArray, detailsArray.Length + 1);

                            // [^1] is equal to [detailsArray.Length - 1]. Which add the value to the last index of the array.
                            detailsArray[^1] = " - Client " + clientDetails.ClientName + " has " + emailInformation.FileCount + " files in the error folder.";
                        }
                    }
                    else
                    {
                        WriteLogClass.WriteToLog(0,"Customer ID selection from folder returned empty.", 0);
                    }
                }

                if (detailsArray.Length != 0)
                {
                    return CreatEmailBody(detailsArray, emailInformation.FolderCount);
                }
                else
                {
                    WriteLogClass.WriteToLog(0,"Client details array is empty.", 0);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,$"Exception at StartCreatingEmail under CreatEmailClass: {ex.Message}", 0);
            }
            return false;
        }

        private static bool CreatEmailBody(string[] detailsArray, int folderCount)
        {
            try
            {
                var folderDetails = string.Join(Environment.NewLine, detailsArray);
                var emailBody = $@"There a total of {folderCount} problem folder/s waiting in error DEA.Next folder.{Environment.NewLine}{Environment.NewLine}{folderDetails}";

                if(!folderDetails.IsNullOrEmpty())
                {
                    return EmailClass.EmailSenderHandler(emailBody);
                }
                else
                {
                    WriteLogClass.WriteToLog(1,"Folder details array was empty or set to null.", 1);
                    return false;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,$"Exception at CreatEmailBody CreatEmailClass: {ex.Message}", 0);
                return false;
            }
        }
    }
}
