using AppConfigReader;
using AppConfigUpdater;
using System.Diagnostics;
using WriteLog;

namespace RunTimedFunctions
{
    /// <summary>
    /// This is used to run other executable files like DEAMailer or DEACleaner. This will read the config file and run the said executable files according to the set intervals.
    /// </summary>
    internal class RunTimedFunctionsClass
    {
        /// <summary>
        /// Runs the error folder checker (DEAMailer.exe) which will check the error folder for files.
        /// If there's anyfiles in the folder an email will be sent to configured emails informing them that there's files in the errrs folder that need to be manually cleared.
        /// </summary>
        /// <returns>Returns a bool value as true or false.</returns>
        public static bool CallDeaMailer()
        {
            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;
            double previousTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime).TotalMinutes;
            double timeNow = TimeSpan.Parse(DateTime.Now.ToString("t")).TotalMinutes;

            double timeDiff = timeNow - previousTime;

            if (timeDiff >= timeItnterval)
            {
                AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), null);
                RunProcess(GetWorkingDir(), "DEAMailer.exe");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calls the log folder cleaner (DEACleaner.exe) which will run periodically and cleans the log foler if the files are older than configured dates.
        /// </summary>
        /// <returns>Returns a bool value as true or false</returns>
        public static bool CallDeaCleaner()
        {
            int dateInterval = GetJsonFileData().LogsDeleteAfter;
            DateTime previousRunDate = DateTime.Parse(GetJsonFileData().PreviousRunDate);
            DateTime dateNow = DateTime.Parse(DateTime.Now.ToString("d"));

            TimeSpan dateDiff = dateNow - previousRunDate;

            if (dateDiff.TotalDays >= dateInterval)
            {
                AppConfigUpdaterClass.UpdateConfigFile(null, dateNow.ToString("d"));
                RunProcess(GetWorkingDir(), "DEACleaner.exe");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads and load the jason file data to be edited from the above functions
        /// </summary>
        /// <returns>The loaded json data from the config file.</returns>
        private static AppConfigReaderClass.Timingsettings GetJsonFileData()
        {
            AppConfigReaderClass.AppSettingsRoot jsonData = AppConfigReaderClass.ReadAppDotConfig();
            AppConfigReaderClass.Timingsettings timeSettings = jsonData.TimingSettings;

            return timeSettings;
        }

        /// <summary>
        /// This would run the executables from another project.
        /// </summary>
        /// <param name="workingDirPath"></param>
        /// <param name="exeFileName"></param>
        private static void RunProcess(string workingDirPath, string exeFileName)
        {
            ProcessStartInfo processInfo = new()
            {
                FileName = Path.Combine(workingDirPath, exeFileName),
                RedirectStandardOutput = true, // Redirects the standard output of the process.
                UseShellExecute = false, // Makes the process launch in it's own windows and to have it's own arguments, if set to true.
                CreateNoWindow = true, // This stops the external process from creating a window.
            };

            // Creating the process.
            Process startProcess = new()
            {
                StartInfo = processInfo,
                EnableRaisingEvents = true // Enables Exited event to be raised.
            };

            startProcess.OutputDataReceived += (reciver, d) =>
            {
                /*if (!string.IsNullOrEmpty(d.Data))
                {*/
                    Console.WriteLine("Output: {0}", d.Data);
                //}
            };

            startProcess.Exited += (sender, e) =>
            {
                //WriteLogClass.WriteToLog(1, $"{Path.GetFileNameWithoutExtension(exeFileName)} has exited.", 1);
                Console.WriteLine($"{Path.GetFileNameWithoutExtension(exeFileName)} has exited.");
            };

            try
            {
                startProcess.Start();
                startProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at starting process function: {ex.Message}", 0);
            }
        }

        private static void StartProcess_Exited(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the current working directory.
        /// </summary>
        /// <returns>Return the file path as a string value.</returns>
        private static string GetWorkingDir()
        {
            string currentWorkingDir = Environment.CurrentDirectory;

            return currentWorkingDir;
        }
    }
}
