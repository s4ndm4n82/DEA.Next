using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using WriteLog;

namespace MetaFileReaderWriter
{
    internal class MetaFileReaderWriterClass
    {
        public class MetaFileReaderWriterObject
        {
            public List<ProcessDetail>? ProcessDetails { get; set; }
        }

        public class ProcessDetail
        {
            public int? ClientID { get; set; }
            public string? FtpPath { get; set; }
            public string? RecivedEmail { get; set; }
            public string? DownloadStatus { get; set; }
            public string? UploadStatus { get; set; }
            public List<string>? FileList { get; set; }

        }

        public static T MetaReader<T>(string metaFilePath)
        {
            T? returnData = default;
            try
            {
                using StreamReader fileData = new(metaFilePath);
                string dataString = fileData.ReadToEnd();
                return returnData = JsonConvert.DeserializeObject<T>(dataString)!;
            }
            catch(Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Excpetion at Meta file reader: {ex.Message}", 1);
                return returnData!;
            }
        }

        public static bool MetaWriter(string metaFilePath, int clientID, string ftpPath, string recivedEmail, string downloadaStatus, string uploadStatus, string[] localFileList)
        {
            try
            {
                List<string> localFiles = new();

                foreach (var file in localFileList)
                {
                    localFiles.Add(file);
                }

                MetaFileReaderWriterObject metaFiles = new MetaFileReaderWriterObject();

                MetaFileReaderWriterObject metaData = new()
                {
                    ProcessDetails = new List<ProcessDetail>
                    {
                        new ProcessDetail
                        {
                            ClientID = clientID,
                            FtpPath = ftpPath,
                            RecivedEmail = recivedEmail,
                            DownloadStatus = downloadaStatus,
                            UploadStatus = uploadStatus,
                            FileList = localFiles
                        }

                    }
                };

                string jsonString = JsonConvert.SerializeObject(metaData);

                Console.WriteLine(jsonString);

                string latFolderName = metaFilePath.Split(Path.DirectorySeparatorChar).Last();
                string fileExtention = ".json";
                string fileNemStart = "Meta_";
                string metaFileName = string.Concat(fileNemStart, latFolderName, fileExtention);
                string metaFullPath = Path.Combine(metaFilePath, metaFileName);

                if (!string.IsNullOrEmpty(jsonString))
                {
                    File.WriteAllText(metaFullPath, jsonString);
                }

                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(3, $"Exception at meta file writer: {ex.Message}", 1);
                return false;
            }
        }
    }
}
