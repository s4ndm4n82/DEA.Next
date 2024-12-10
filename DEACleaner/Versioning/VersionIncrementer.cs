using System.Text.RegularExpressions;
using WriteLog;

namespace VersionIncrementerClass;

public static class VersionIncrementer
{
    public static void IncrementVersion()
    {
        try
        {
            // Get the current working directory
            string workingDirectory = Environment.CurrentDirectory;
            // Get the app root directory
            string appRootDirectory = workingDirectory.Split("\\bin")[0];
            // Get the path to the AssemblyInfo.cs file
            string assemplyInfoFilePath = Path.Combine(appRootDirectory, "Versioning", "AssemblyInfo.cs");
            // Read the AssemblyInfo.cs file
            string assemblyInfo = File.ReadAllText(assemplyInfoFilePath);
            // Check if the file exists
            if (!File.Exists(assemplyInfoFilePath))
            {
                return;
            }
            // Update the numbers
            UpdateNumbers(assemblyInfo,
                assemplyInfoFilePath,
                ExtractAssemblyNumbers(assemblyInfo).Item1,
                ExtractAssemblyNumbers(assemblyInfo).Item2);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at IncrementVersion: {ex.Message}", 0);
        }
    }

    private static (int,int) ExtractAssemblyNumbers(string assemblyInfo)
    {
        int buildNumber = 0;
        int revisionNumber = 0;

        try
        {
            // Extract the current version number.
            Match matchNumber = Regex.Match(assemblyInfo, @"AssemblyVersion\(""\d+\.\d+\.\d+\.\d+""\)");

            if (matchNumber.Success)
            {
                // Extract the version numbers.
                string[] versionNumbers = matchNumber.Value
                    .Trim('(', ')', '"')
                    .Split('.')
                    .Skip(1)
                    .ToArray();
                buildNumber = int.Parse(versionNumbers[1]);
                revisionNumber = int.Parse(versionNumbers[2]);                    
            }
            // Return the version numbers
            return (buildNumber, revisionNumber);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at ExtractAssemblyNumbers: {ex.Message}", 0);
            return (buildNumber,revisionNumber);
        }
    }
    private static void UpdateNumbers(string assemblyInfo,
        string assemblyInfoFilePath,
        int buildNumber,
        int revisionNumber)
    {
        try
        {
            // Increment the revision number.
            revisionNumber++;
            // Check if the revision number is greater than 1000
            // If it is, increment the build number and reset the revision number.
            if (revisionNumber == 1000)
            {
                buildNumber++;
                revisionNumber = 0;
            }
            // Update the numbers
            if (buildNumber == 10)
            {
                UpdateMinorNumber(assemblyInfo, assemblyInfoFilePath);
            }
            else
            {
                UpdateBuildAndRevisonNumber(assemblyInfo, assemblyInfoFilePath, buildNumber, revisionNumber);
            }
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at updateMinorNumber: {ex.Message}", 0);
        }
    }

    private static void UpdateMinorNumber(string assemblyInfo, string assemblyInfoFilePath)
    {
        try
        {
            // Increment the minor number and reset the build number
            string updatedAssemblyInfo = Regex.Replace(assemblyInfo, @"AssemblyVersion\(""\d+\.\d+\.(\d+)\.\d+""\)", m =>
            {
                int newMinorNumber = int.Parse(m.Groups[1].Value) + 1;
                return $"AssemblyVersion(\"2.{newMinorNumber}.0.0\")"; // Adjust the version format as needed
            });

            // Write the updated content back to AssemblyInfo.cs
            File.WriteAllText(assemblyInfoFilePath, updatedAssemblyInfo);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at updateMinorNumber: {ex.Message}", 0);
        }
    }

    private static void UpdateBuildAndRevisonNumber(string assemblyInfo,
        string assemblyInfoFilePath,
        int buildNumber,
        int revisionNumber)
    {
        try
        {
            // Update the build and revision numbers
            string updatedAssemblyInfo = Regex.Replace(assemblyInfo, @"AssemblyVersion\(""\d+\.\d+\.(\d+)\.\d+""\)", m =>
            {
                return $"AssemblyVersion(\"2.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            updatedAssemblyInfo = Regex.Replace(updatedAssemblyInfo, @"AssemblyFileVersion\(""\d+\.\d+\.\d+\.\d+""\)", m =>
            {
                return $"AssemblyFileVersion(\"2.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            updatedAssemblyInfo = Regex.Replace(updatedAssemblyInfo, @"AssemblyInformationalVersion\(""\d+\.\d+\.\d+\.\d+""\)", m =>
            {
                return $"AssemblyInformationalVersion(\"2.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            // Write the updated content back to AssemblyInfo.cs
            File.WriteAllText(assemblyInfoFilePath, updatedAssemblyInfo);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at updateBuildAndRevisonNumber: {ex.Message}", 0);
        }
    }
}