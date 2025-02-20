using System.Text.RegularExpressions;
using WriteLog;

namespace DEA.UI.Versioning;

public static partial class VersionIncrementerUi
{
    public static void IncrementVersion()
    {
        try
        {
            // Get the current working directory
            var workingDirectory = Environment.CurrentDirectory;
            // Get the app root directory
            var appRootDirectory = workingDirectory.Split("\\bin")[0];
            // Get the path to the AssemblyInfo.cs file
            var assemplyInfoFilePath = Path.Combine(appRootDirectory, "Versioning", "AssemblyInfo.cs");

            // Check if the file exists
            if (!File.Exists(assemplyInfoFilePath))
            {
                return;
            }

            // Read the AssemblyInfo.cs file
            string assemblyInfo = File.ReadAllText(assemplyInfoFilePath);
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

    private static (int, int) ExtractAssemblyNumbers(string assemblyInfo)
    {
        var buildNumber = 0;
        var revisionNumber = 0;

        try
        {
            // Extract the current version number.
            var matchNumber = MyRegex().Match(assemblyInfo);

            if (!matchNumber.Success) return (buildNumber, revisionNumber);
            // Extract the version numbers.
            var versionNumbers = matchNumber.Value
                .Trim('(', ')', '"')
                .Split('.')
                .Skip(1)
                .ToArray();
            buildNumber = int.Parse(versionNumbers[1]);
            revisionNumber = int.Parse(versionNumbers[2]);
            // Return the version numbers
            return (buildNumber, revisionNumber);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at ExtractAssemblyNumbers: {ex.Message}", 0);
            return (buildNumber, revisionNumber);
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
                UpdateBuildAndRevisionNumber(assemblyInfo, assemblyInfoFilePath, buildNumber, revisionNumber);
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
            var updatedAssemblyInfo = MyRegex1().Replace(assemblyInfo, m =>
            {
                var newMinorNumber = int.Parse(m.Groups[1].Value) + 1;
                return $"AssemblyVersion(\"1.{newMinorNumber}.0.0\")"; // Adjust the version format as needed
            });

            // Write the updated content back to AssemblyInfo.cs
            File.WriteAllText(assemblyInfoFilePath, updatedAssemblyInfo);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at updateMinorNumber: {ex.Message}", 0);
        }
    }

    private static void UpdateBuildAndRevisionNumber(string assemblyInfo,
        string assemblyInfoFilePath,
        int buildNumber,
        int revisionNumber)
    {
        try
        {
            // Update the build and revision numbers
            var updatedAssemblyInfo = MyRegex2().Replace(assemblyInfo, m =>
            {
                return $"AssemblyVersion(\"1.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            updatedAssemblyInfo = MyRegex3().Replace(updatedAssemblyInfo, m =>
            {
                return $"AssemblyFileVersion(\"1.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            updatedAssemblyInfo = MyRegex4().Replace(updatedAssemblyInfo, m =>
            {
                return $"AssemblyInformationalVersion(\"1.0.{buildNumber}.{revisionNumber}\")"; // Adjust the version format as needed
            });

            // Write the updated content back to AssemblyInfo.cs
            File.WriteAllText(assemblyInfoFilePath, updatedAssemblyInfo);
        }
        catch (Exception ex)
        {
            WriteLogClass.WriteToLog(0, $"Error at updateBuildAndRevisionNumber: {ex.Message}", 0);
        }
    }

    [GeneratedRegex("""AssemblyVersion\("\d+\.\d+\.\d+\.\d+"\)""")]
    private static partial Regex MyRegex();
    [GeneratedRegex("""AssemblyVersion\("\d+\.\d+\.(\d+)\.\d+"\)""")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("""AssemblyVersion\("\d+\.\d+\.(\d+)\.\d+"\)""")]
    private static partial Regex MyRegex2();
    [GeneratedRegex("""AssemblyFileVersion\("\d+\.\d+\.\d+\.\d+"\)""")]
    private static partial Regex MyRegex3();
    [GeneratedRegex("""AssemblyInformationalVersion\("\d+\.\d+\.\d+\.\d+"\)""")]
    private static partial Regex MyRegex4();
}