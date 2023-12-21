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
            // Get the executed file name.
            string processFileName = Path.ChangeExtension(processName, ".exe").ToLower();

            // Get the 'ErrorCheckInterval' value from the JSON file.
            double timeItnterval = TimeSpan.FromMinutes(GetJsonFileData().ErrorCheckInterval).TotalMinutes;

            // Get the 'LogsDeleteAfter' value from the JSON file
            int dateInterval = GetJsonFileData().LogsDeleteAfter;

            // Check if it's time to run the process.
            if (GetTimeDifference() < timeItnterval)
            {
                return false;                
            }

            // Run the process.
            if (!RunProcess(GetWorkingDir(), processFileName))
            {
                return false;
            }

            // Update the config file.
            if (!AppConfigUpdaterClass.UpdateConfigFile(DateTime.Now.ToString("t"), null))
            {
                return false;
            }

            // Check if it's time to delete the logs.
            if (GetDateDifference() < dateInterval)
            {
                return false;
            }

            // Run the process.
            if (!RunProcess(GetWorkingDir(), processFileName))
            {
                return false;
            }

            // Update the config file.
            if (!AppConfigUpdaterClass.UpdateConfigFile(null, DateTime.Now.ToString("d")))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get the time difference between the current time and the previous run time.
        /// </summary>
        /// <returns>Return the total time in minutes.</returns>
        private static double GetTimeDifference()
        {
            // Parse the previous run time as a TimeSpan.
            TimeSpan previousRunTime = TimeSpan.Parse(GetJsonFileData().PreviousRunTime);

            // Combine the current date with the previous run time TimeSpan
            DateTime previousRunDateTime = DateTime.Today.Add(previousRunTime);

            // Get the current DateTime
            DateTime currentRunTime = DateTime.Now;

            // If the previous run time is greater than the current time, it means the previous run was yesterday.
            if (previousRunDateTime > currentRunTime)
            {
                // Subtract a day from the previous run DateTime to get the correct DateTime value.
                previousRunDateTime = previousRunDateTime.AddDays(-1);
            }

            // Get the difference between the current time and the previous run time.
            TimeSpan timeDifference = currentRunTime - previousRunDateTime;

            // Get the difference in minutes.
            double timeDifferenceInMiuntes = timeDifference.TotalMinutes;

            return timeDifferenceInMiuntes;
        }

        /// <summary>
        /// Get the date difference between the current date and the previous run date.
        /// </summary>
        /// <returns>Total dates</returns>
        private static double GetDateDifference()
        {
            // Parse the 'PreviousRunDate' from the JSON file into a DateTime object,
            DateTime previousRunDate = DateTime.Parse(GetJsonFileData().PreviousRunDate);

            // Get the current date without the time component.
            DateTime currentRunDate = DateTime.Today;

            // Calculate the date difference by subtracting the previous run date from the current date.
            TimeSpan dateDifference = currentRunDate - previousRunDate;

            // Get the date difference in days as a double value
            double dateDifferenceInDays = dateDifference.TotalDays;

            return dateDifferenceInDays;
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
