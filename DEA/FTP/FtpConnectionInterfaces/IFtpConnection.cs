using DownloadFtpFilesClass;
using FluentFTP;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using UserConfigRetriverClass;
using WriteLog;

namespace DEA.Next.FTP.FtpConnectionInterfaces;

public interface IFtpConnection : IDisposable
{
    Task ConnectAsync();
    Task DisconnectAsync();
    Task<IEnumerable<FtpListItem>> GetListingFtp(string path, int clientId);
    Task<IEnumerable<ISftpFile>> GetListingSftp(string path, int clientId);
    Task<bool> DeleteFileFtp(string ftpFilePath);
    Task<bool> DeleteFileSftp(string ftpFilePath);
    
}

public sealed class AsyncFtpConnection : IFtpConnection
{
    private readonly AsyncFtpClient _ftpClient;
    private bool _disposed;

    public AsyncFtpConnection(AsyncFtpClient ftpClient)
    {
        _ftpClient = ftpClient;
    }

    public async Task ConnectAsync()
    {
        await _ftpClient.Connect();
    }

    public async Task DisconnectAsync()
    {
        await _ftpClient.Disconnect();
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _ftpClient.Dispose();
        }

        _disposed = true;
    }
    
    ~AsyncFtpConnection()
    {
        Dispose(false);
    }
    
    public async Task<IEnumerable<FtpListItem>> GetListingFtp(string path, int clientId)
    {
        var jsonFtpData = await UserConfigRetriver.RetriveUserConfigById(clientId);
        var allowedFileExtensions = jsonFtpData.DocumentDetails.DocumentExtensions;
        
        var fileList = await _ftpClient.GetListing(path);
        return fileList
            .Where(f => f.Type == FtpObjectType.File
                        && allowedFileExtensions.
                            Any(ext => f.FullName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            .Select(f => new FtpListItem{ FullName = f.FullName});
    }
    
    public Task<IEnumerable<ISftpFile>> GetListingSftp(string path, int clientId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteFileFtp(string path)
    {
        try
        {
            await _ftpClient.DeleteFile(path);
            return true;
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at DeleteFileFtp: {ex.Message}", 0);
            return false;
        }
    }

    public Task<bool> DeleteFileSftp(string path)
    {
        throw new NotImplementedException();
    }
}

public sealed class SftpConnection : IFtpConnection
{
    private readonly SftpClient _sftpClient;
    private bool _disposed;

    public SftpConnection(SftpClient sftpClient)
    {
        _sftpClient = sftpClient;
    }

    public async Task ConnectAsync()
    {
        await _sftpClient.ConnectAsync(cancellationToken: default);
    }

    public async Task DisconnectAsync()
    {
        await Task.Run(() => {
            _sftpClient.Disconnect();
        });
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _sftpClient.Dispose();
        }

        _disposed = true;
    }
    
    ~SftpConnection()
    {
        Dispose(false);
    }
    
    public Task<IEnumerable<FtpListItem>> GetListingFtp(string path, int clientId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<IEnumerable<ISftpFile>> GetListingSftp(string path, int clientId)
    {
        var jsonFtpData = await UserConfigRetriver.RetriveUserConfigById(clientId);
        var allowedFileExtensions = jsonFtpData.DocumentDetails.DocumentExtensions;
        
        return await Task.Run(() => _sftpClient.ListDirectory(path)
            .Where(f => f.IsRegularFile && allowedFileExtensions
            .Any(ext => f.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase))));
    }
    
    public Task<bool> DeleteFileFtp(string path)
    {
        throw new NotImplementedException();
    }
    
    public Task<bool> DeleteFileSftp(string path)
    {
        try
        {
            _sftpClient.DeleteFile(path);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Exception at SFTP file deletion: {ex.Message}", 0);
            return Task.FromResult(false);
        }
    }
}