using System.Reflection;
using WriteLog;

namespace DEA.Next.Versioning;

internal class DisplayLogo
{
    public static void Logo()
    {
        var programVersion = Assembly.GetExecutingAssembly().GetName().Version;
        var programName = Assembly.GetExecutingAssembly().GetName().Name;

        if (programName == null || programVersion == null) return;
        ShowLogo(programName);
        ShowLogo(programVersion.ToString());

        WriteLogClass.WriteToLog(1, $"{programName} .... v{programVersion} ....", 1);
    }

    private static void ShowLogo(string textToSet)
    {            
        var windowWidth = Console.WindowWidth;
        var windowPadding = (windowWidth - textToSet.Length) / 2;
        Console.WriteLine(new string(' ', windowPadding) + textToSet);
    }
}