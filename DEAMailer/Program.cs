using CreatEmail;
using ErrorFolderChecker;

if (ErrorFolderCheckerClass.ErrorFolderChecker().Item1.Any())
{
    CreateEmailClass.StartCreatingEmail(ErrorFolderCheckerClass.ErrorFolderChecker().Item2, ErrorFolderCheckerClass.ErrorFolderChecker().Item1);
}