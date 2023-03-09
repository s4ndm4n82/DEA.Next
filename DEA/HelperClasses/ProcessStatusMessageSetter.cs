namespace ProcessStatusMessageSetter
{
    enum ProcessStatusMain
    {
        CompletedSuccessfully,
        CompletedWithIssues,
        EmailProcessCompleted,
        EmailProcessEndedWithErrors,
        FTPProcessCompleted,
        FTPProcessEndedWithErrors,
        TerminatedDueToErrors
    }

    enum ProcessStatusOther
    {
        Success,
        FtpUploadFailed,
        FtpDownloadFailed,
        FolderEmpty,     
        EmailUploadFailed,
        EmailDownloadFailed,
        Unsuccess
    }

    internal class ProcessStatusMessageSetterClass
    {
        public static string SetProcessStatusMain(int emailResult, int ftpResult)
        {
            ProcessStatusMain processStatus = GetProcessStatusMain(emailResult, ftpResult);
            string processStatusMessage = GetStatusProcessMessageMain(processStatus, emailResult, ftpResult);

            return processStatusMessage;
        }

        private static ProcessStatusMain GetProcessStatusMain(int emailResult, int ftpResult)
        {
            if ((emailResult == 1 || emailResult == 4) && (ftpResult == 1 || ftpResult == 4))
            {
                return ProcessStatusMain.CompletedSuccessfully;
            }
            else if (emailResult == 2 && ftpResult == 2)
            {
                return ProcessStatusMain.CompletedWithIssues;
            }
            else if (emailResult == 1 && (ftpResult == 0 || ftpResult == 4))
            {
                return ProcessStatusMain.EmailProcessCompleted;
            }
            else if (emailResult == 2 && (ftpResult == 0 || ftpResult == 4))
            {
                return ProcessStatusMain.EmailProcessEndedWithErrors;
            }
            else if (ftpResult == 1 && (emailResult == 0 || emailResult == 4))
            {
                return ProcessStatusMain.FTPProcessCompleted;
            }
            else if (ftpResult == 2 && (emailResult == 0 || emailResult == 4))
            {
                return ProcessStatusMain.FTPProcessEndedWithErrors;
            }
            else
            {
                return ProcessStatusMain.TerminatedDueToErrors;
            }
        }

        private static string GetStatusProcessMessageMain(ProcessStatusMain processStatus, int emailResult, int ftpResult)
        {
            return processStatus switch
            {
                ProcessStatusMain.CompletedSuccessfully => "Process completed successfully....",
                ProcessStatusMain.CompletedWithIssues => "Process completed with issues....",
                ProcessStatusMain.EmailProcessCompleted => $"Email process completed.... FTP code {ftpResult}....",
                ProcessStatusMain.EmailProcessEndedWithErrors => $"Email process ended with errors.... FTP code {ftpResult}....",
                ProcessStatusMain.FTPProcessCompleted => $"FTP process completed.... Email code {emailResult}....",
                ProcessStatusMain.FTPProcessEndedWithErrors => $"FTP process ended with errors.... Email code {emailResult}....",
                ProcessStatusMain.TerminatedDueToErrors => "Process terminated due to errors....",
                _ => string.Empty,
            };
        }

        public static int SetMessageTypeMain(int emailResult, int ftpResult)
        {
            int msgTyp = 0;
            bool emailSuccessful = emailResult == 1 || emailResult == 4;
            bool ftpSuccessful = ftpResult == 1 || ftpResult == 4;
            bool successfulWithErrors = emailResult == 2 || ftpResult == 2;

            if (emailSuccessful && ftpSuccessful || successfulWithErrors)
            {
                msgTyp = 1;
            }

            return msgTyp;
        }

        public static string SetProcessStatusOther(int statusResult, string processType)
        {
            ProcessStatusOther processStatus = GetProcessStatusOther(statusResult, processType);
            string processStatusMessage = GetStatusProcessMessageOther(processStatus, statusResult, processType);
            return processStatusMessage;
        }

        private static ProcessStatusOther GetProcessStatusOther(int statusResult, string processType)
        {
            return (statusResult, processType) switch
            {
                (1, "ftp") or (1, "email") => ProcessStatusOther.Success,
                (2, "ftp") => ProcessStatusOther.FtpUploadFailed,
                (2, "email") => ProcessStatusOther.EmailUploadFailed,
                (3, "ftp") => ProcessStatusOther.FtpDownloadFailed,
                (3, "email") => ProcessStatusOther.EmailDownloadFailed,
                (4, "ftp") or (4, "email") => ProcessStatusOther.FolderEmpty,
                _ => ProcessStatusOther.Unsuccess,
            };
        }

        private static string GetStatusProcessMessageOther(ProcessStatusOther processStatus, int statusResult, string processType)
        {
            return processStatus switch
            {
                ProcessStatusOther.Success => "Files sent to processing successfully .... ",
                ProcessStatusOther.FtpUploadFailed => "Uploading files failed. File moved to error ....,",
                ProcessStatusOther.EmailUploadFailed => "Uploading file/s didn't complete successfully. Moved files to error folder ....",
                ProcessStatusOther.FtpDownloadFailed => "File download failed ....",
                ProcessStatusOther.EmailDownloadFailed => "No attachment or file type not supported. Email moved to error and forwarded to sender ....",
                ProcessStatusOther.FolderEmpty => "Folder/Inbox empty ....",
                _ => "Operation failed ....",
            };
        }

        public static int SetMessageTypeOther(int statusResult)
        {
            int msgTyp = 0;
            bool successful = statusResult == 1 || statusResult == 3 || statusResult == 4;
            bool successfulWithErrors = statusResult == 2;

            if (successful || successfulWithErrors)
            {
                msgTyp = 1;
            }

            return msgTyp;
        }
    }
}
