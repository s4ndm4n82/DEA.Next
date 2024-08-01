using System.Diagnostics;
using ConnectFtp;
using ConnectFtps;
using ConnectSftp;
using DEA.Next.HelperClasses.OtherFunctions;
using Renci.SshNet;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.HelperClasses.FolderFunctions;

public static class FolderCleanerLines
{
    public interface IFtpClient
    {
        Task<object> ConnectAsync(int clientId);
    }

    public class AsyncFtpClient : IFtpClient
    {
        public async Task<object> ConnectAsync(int clientId)
        {
            var jsonFtpData = await UserConfigRetriver.RetriveFtpConfigById(clientId);
            
            var ftpClient = await ConnectFtpClass.ConnectFtp(jsonFtpData.FtpProfile,
                jsonFtpData.FtpHostName,
                jsonFtpData.FtpUser,
                jsonFtpData.FtpPassword,
                jsonFtpData.FtpPort);
            
            return ftpClient;
        }
    }
    
    public class AsyncFtpsClient : IFtpClient
    {
        public async Task<object> ConnectAsync(int clientId)
        {
            var jsonFtpData = await UserConfigRetriver.RetriveFtpConfigById(clientId);
            
            var ftpClient = await ConnectFtpsClass.ConnectFtps(jsonFtpData.FtpProfile,
                jsonFtpData.FtpHostName,
                jsonFtpData.FtpUser,
                jsonFtpData.FtpPassword,
                jsonFtpData.FtpPort);
            
            return ftpClient;
        }
    }
    
    public class AsyncSftpClient : IFtpClient
    {
        public async Task<object> ConnectAsync(int clientId)
        {
            var jsonFtpData = await UserConfigRetriver.RetriveFtpConfigById(clientId);
            
            var ftpClient = await ConnectSftpClass.ConnectSftp(jsonFtpData.FtpHostName,
                jsonFtpData.FtpUser,
                jsonFtpData.FtpPassword,
                jsonFtpData.FtpPort);
            
            return ftpClient;
        }
    }
    
    public static async Task<bool> RemoveUploadedFilesLinesAsync(string localeFile)
    {
        try
        {
            await Task.Run(() =>
            {
                if (File.Exists(localeFile)) File.Delete(localeFile); // Remove the uploaded files.
            });
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Folder cleaner lines: {ex.Message}", 0);
            return false;
        }

        return true;
    }

    public static async Task<bool> RemoveMainFileAsync(string localFilePath, string mainFileName)
    {
        var pathRoot = Path.GetDirectoryName(localFilePath);
        if (pathRoot == null) return false;
        var mainFilePath = Path.Combine(pathRoot, mainFileName);

        try
        {
            await using var stream = File.Open(mainFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            // Main data file will be removed when it's not in use
            File.Delete(mainFilePath);
            WriteLogClass.WriteToLog(1, $"\"{mainFileName}\" file deleted successfully ....", 1);
            return true;
        }
        catch (IOException ex)
        {
            // If the file is in use, close any processes that are using it and then delete the file
            var processes = Process.GetProcessesByName("explorer");
            foreach (var process in processes)
            {
                if (process.MainModule?.FileName == null) return false;

                if (!process.MainModule.FileName.Contains(mainFilePath)) continue;
                WriteLogClass.WriteToLog(1, $"Killing process \"{process.ProcessName}\" ....", 1);
                process.Kill();
            }

            File.Delete(mainFilePath);
            WriteLogClass.WriteToLog(1, $"\"{mainFileName}\" file deleted successfully ....", 1);
            return true;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Folder cleaner CSV file delete: {ex.Message}", 0);
            return false;
        }
    }

    public static async Task<bool> DeleteLocalHoldFolderAsync(string localFolderPath)
    {
        try
        {
            return await Task.Run(() =>
            {
                var isFolderEmpty = !Directory.GetFiles(localFolderPath).Any();
            
                if (!isFolderEmpty)
                {
                    WriteLogClass.WriteToLog(1, $"Local hold folder is not empty ....", 1);
                    return false;
                }
            
                if (Directory.Exists(localFolderPath))
                {
                    Directory.Delete(localFolderPath, true);
                    WriteLogClass.WriteToLog(1, 
                        $"Local hold folder \"{Path.GetFileName(localFolderPath)}\" deleted successfully ....",
                        1);
                    return true;
                }
                
                WriteLogClass.WriteToLog(1, $"Local hold folder was not deleted ....", 1);
                return false; 
            });
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at Folder cleaner local hold folder: {ex.Message}", 0);
            return false;
        }
        
    }

    public static async Task<bool> RemoveDataFileFromFtpAsync(string mainFileName, int clientId)
    {
        var ftpClient = await CreateFtpClientAsync(clientId);;
        
        return true;
    }

    private static async Task<IFtpClient> CreateFtpClientAsync(int clientId)
    {
        var jsonFtpData = await UserConfigRetriver.RetriveFtpConfigById(clientId);

        switch (jsonFtpData.FtpType.ToLower())
        {
            case MagicWords.ftp:
                return new AsyncFtpClient();
            case MagicWords.ftps:
                return new AsyncFtpsClient();
            case MagicWords.sftp:
                return new AsyncSftpClient();
            default:
                WriteLogClass.WriteToLog(0, $"Ftp type {jsonFtpData.FtpType} not found.", 1);
                throw new ArgumentException($@"Ftp type {jsonFtpData.FtpType} not found.", nameof(jsonFtpData.FtpType));
        }
    }
}