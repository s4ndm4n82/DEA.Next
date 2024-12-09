using DEA.Next.HelperClasses.OtherFunctions;

namespace ProcessStatusMessageSetter
{
    internal enum ProcessStatusMain
    {
        CompletedSuccessfully,
        CompletedWithIssues,
        EmailProcessCompleted,
        EmailProcessEndedWithErrors,
        TerminatedDueToErrors,
        EmailWithTooManyReplies
    }

    internal enum ProcessStatusOther
    {
        Success,
        FtpUploadFailed,
        FtpDownloadFailed,
        FolderEmpty,     
        EmailUploadFailed,
        EmailDownloadFailed,
        Unsuccess,
        TooManyReplies,
        FtpSubUploadFailed
    }

    internal class ProcessStatusMessageSetterClass
    {
        public static string SetProcessStatusMain(int emailResult, int ftpResult)
        {
            var processStatus = GetProcessStatusMain(emailResult, ftpResult);
            var processStatusMessage = GetStatusProcessMessageMain(processStatus, ftpResult);

            return processStatusMessage;
        }

        private static ProcessStatusMain GetProcessStatusMain(int emailResult, int ftpResult)
        {
            return emailResult switch
            {
                // TODO 2: Have to refine this logic again. Seems I've to break this into another one to get the FTP code message.
                1 or 4 when ftpResult is 1 or 4 => ProcessStatusMain.CompletedSuccessfully,
                2 when ftpResult == 2 => ProcessStatusMain.CompletedWithIssues,
                1 when ftpResult is 0 or 2 or 4 => ProcessStatusMain.EmailProcessCompleted,
                2 when ftpResult is 0 or 1 or 4 => ProcessStatusMain.EmailProcessEndedWithErrors,
                5 => ProcessStatusMain.EmailWithTooManyReplies,
                _ => ProcessStatusMain.TerminatedDueToErrors
            };
        }

        private static string GetFtpStatus(int ftpResult)
        {
            return ftpResult switch
            {
                1 => "FTP download completed",
                2 or 3 => "FTP process completed with issues",
                4 => "FTP folder empty",
                _ => "Process terminated due to errors"
            };
        }

        private static string GetStatusProcessMessageMain(ProcessStatusMain processStatus, int ftpResult)
        {
            return processStatus switch
            {
                ProcessStatusMain.CompletedSuccessfully => "Process completed successfully ....",
                ProcessStatusMain.CompletedWithIssues => "Process completed with issues ....",
                ProcessStatusMain.EmailProcessCompleted => $"Email process completed .... {GetFtpStatus(ftpResult)} ....",
                ProcessStatusMain.EmailProcessEndedWithErrors => $"Email process ended with errors.... {GetFtpStatus(ftpResult)} ....",
                ProcessStatusMain.EmailWithTooManyReplies => "Process completed .... Email with too many replies ....",
                ProcessStatusMain.TerminatedDueToErrors => "Process terminated due to errors ....",
                _ => string.Empty,
            };
        }

        public static int SetMessageTypeMain(int emailResult, int ftpResult)
        {
            var msgTyp = 0;
            var emailSuccessful = emailResult is 1 or 4;
            var ftpSuccessful = ftpResult is 1 or 4;
            var successfulWithErrors = emailResult == 2 || ftpResult == 2;

            if (emailSuccessful && ftpSuccessful || successfulWithErrors)
            {
                msgTyp = 1;
            }

            return msgTyp;
        }

        public static string SetProcessStatusOther(int statusResult, string processType)
        {
            var processStatus = GetProcessStatusOther(statusResult, processType);
            var processStatusMessage = GetStatusProcessMessageOther(processStatus);
            return processStatusMessage;
        }

        private static ProcessStatusOther GetProcessStatusOther(int statusResult, string processType)
        {
            return (statusResult, processType) switch
            {
                (1, MagicWords.ftp) or (1, MagicWords.email) => ProcessStatusOther.Success,
                (2, MagicWords.ftp) => ProcessStatusOther.FtpUploadFailed,
                (2, MagicWords.email) => ProcessStatusOther.EmailUploadFailed,
                (3, MagicWords.ftp) => ProcessStatusOther.FtpDownloadFailed,
                (3, MagicWords.email) => ProcessStatusOther.EmailDownloadFailed,
                (4, MagicWords.ftp) or (4, MagicWords.email) => ProcessStatusOther.FolderEmpty,
                (5, MagicWords.email) => ProcessStatusOther.TooManyReplies,
                (6, MagicWords.ftp) => ProcessStatusOther.FtpSubUploadFailed,
                _ => ProcessStatusOther.Unsuccess,
            };
        }

        private static string GetStatusProcessMessageOther(ProcessStatusOther processStatus)
        {
            return processStatus switch
            {
                ProcessStatusOther.Success => "Files sent to processing successfully .... ",
                ProcessStatusOther.FtpUploadFailed => "Uploading files failed. File moved to error folder ....,",
                ProcessStatusOther.EmailUploadFailed => "Uploading file/s didn't complete successfully. Moved files to error folder ....",
                ProcessStatusOther.FtpDownloadFailed => "File download failed ....",
                ProcessStatusOther.EmailDownloadFailed => "No attachment or file type not supported. Email moved to error and forwarded to sender ....",
                ProcessStatusOther.FolderEmpty => "Folder/Inbox empty ....",
                ProcessStatusOther.TooManyReplies => "Too many replies. Email moved to deleted items ....",
                ProcessStatusOther.FtpSubUploadFailed => "Uploading files failed. Local files removed ....",
                _ => "Operation failed ....",
            };
        }

        public static int SetMessageTypeOther(int statusResult)
        {
            var msgTyp = 0;
            var successful = statusResult is 1 or 3 or 4 or 5;
            var successfulWithErrors = statusResult == 2;

            if (successful || successfulWithErrors)
            {
                msgTyp = 1;
            }

            return msgTyp;
        }
    }
}
