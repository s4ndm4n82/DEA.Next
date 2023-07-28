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
        /// The main function that calls the two process to clean the log files folder and to check the error folder.
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool CallDeaTimedProcesses(string processName)
        {
            string processFileName = Path.ChangeExtension(processName, ".exe").ToLower();

            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;
            double previousTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime).TotalMinutes;
            double timeNow = TimeSpan.Parse(DateTime.Now.ToString("t")).TotalMinutes;
            double timeDiff = timeNow - previousTime;

            int dateInterval = GetJsonFileData().LogsDeleteAfter;
            DateTime previousRunDate = DateTime.Parse(GetJsonFileData().PreviousRunDate);
            DateTime dateNow = DateTime.Parse(DateTime.Now.ToString("d"));
            TimeSpan dateDiff = dateNow - previousRunDate;

            if (timeDiff >= timeItnterval)
            {
                if (RunProcess(GetWorkingDir(), processFileName))
                {
                    if (AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), null))
                    {
                        return true;
                    }
                }
            }

            if (dateDiff.TotalDays >= dateInterval)
            {
                if (RunProcess(GetWorkingDir(), processFileName))
                {
                    if (AppConfigUpdaterClass.UpdateConfigFile(null, dateNow.ToString("d")))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// This would run the executables from another project.
        /// </summary>
        /// <param name="workingDirPath"></param>
        /// <param name="exeFileName"></param>
        private static bool RunProcess(string workingDirPath, string exeFileName)
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

            startProcess.OutputDataReceived += (sender, d) =>
            {
                if (!string.IsNullOrEmpty(d.Data))
                {
                    Console.WriteLine($"{d.Data}");
                }
            };

            startProcess.Exited += (sender, e) =>
            {
                WriteLogClass.WriteToLog(1, $"{Path.GetFileNameWithoutExtension(exeFileName)} process ended.", 1);
            };

            try
            {
                startProcess.Start();
                startProcess.BeginOutputReadLine();
                startProcess.WaitForExit();
                return true;
            }
            catch (Exception ex)
            {
                WriteLogClass.WriteToLog(0, $"Exception at starting process function: {ex.Message}", 0);
                return false;
            }
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
    }
}
