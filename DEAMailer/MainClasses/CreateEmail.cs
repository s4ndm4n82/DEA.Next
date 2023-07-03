using System.Text.RegularExpressions;
using Microsoft.IdentityModel.Tokens;
using WriteLog;
using UserConfigReader;
using Emailer;

namespace CreatEmail
{
    internal class CreateEmailClass
    {
        public class EmailInfor
        {
            public int CustomerID { get; set; }
            public int FileCount { get; set; }
            public int FolderCount { get; set; }
        }

        public static int StartCreatingEmail(DirectoryInfo folderPath, IEnumerable<DirectoryInfo> folderList)
        {
            try
            {
                string[] detailsArray = Array.Empty<string>();

                EmailInfor emailInfor = new()
                {
                    FolderCount = folderPath.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly).Count()
                };

                foreach (DirectoryInfo subFolderPath in folderList)
                {
                    string regexPattern = @"ID_(\d+)";
                    Match matchValue = Regex.Match(subFolderPath.Name, regexPattern);

                    if (matchValue.Success)
                    {
                        emailInfor.CustomerID = int.Parse(matchValue.Groups[1].Value);
                        emailInfor.FileCount = subFolderPath.EnumerateFiles("*.*", SearchOption.AllDirectories).Count();

                        UserConfigReaderClass.CustomerDetailsObject jsonData = UserConfigReaderClass.ReadUserDotConfig<UserConfigReaderClass.CustomerDetailsObject>();
                        UserConfigReaderClass.Customerdetail clientDetails = jsonData.CustomerDetails!.FirstOrDefault(cid => cid.Id == emailInfor.CustomerID)!;

                        if (!clientDetails.ClientName.IsNullOrEmpty())
                        {
                            // Resize the array to accomodate the new line
                            Array.Resize(ref detailsArray, detailsArray.Length + 1);

                            // [^1] is equal to [detailsArray.Length - 1]. Which add the value to the last index of the array.
                            detailsArray[^1] = " - Client " + clientDetails.ClientName + " has " + emailInfor.FileCount + " files in the error folder.";
                        }
                    }
                    else
                    {
                        WriteLogClass.WriteToLog(0,"Customer ID selection from folder returned empty.", 0);
                    }
                }

                if (detailsArray != null && detailsArray.Length != 0)
                {
                    CreatEmailBody(detailsArray, emailInfor.FolderCount);
                }
                else
                {
                    WriteLogClass.WriteToLog(0,"Client details array is empty.", 0);
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,$"Exeption at SartCreatingEmail under CreatEmailClass: {ex.Message}", 0);
            }
            return 0;
        }

        public static int CreatEmailBody(string[] detailsArray, int folderCount)
        {
            try
            {
                string folderDetails = string.Join(Environment.NewLine, detailsArray);
                string emailBody = $@"There a total of {folderCount} problem folder/s waiting in error DEA.Next folder.{Environment.NewLine}{Environment.NewLine}{folderDetails}";

                if(!folderDetails.IsNullOrEmpty())
                {
                    EmailerClass.EmailSenderHandler(emailBody);
                    return 1;
                }
                else
                {
                    WriteLogClass.WriteToLog(1,"Folder details array was empty or set to null.", 1);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0,$"Exeption at CreatEmailBody CreatEmailClass: {ex.Message}", 0);
                return 0;
            }            
        }
    }
}
