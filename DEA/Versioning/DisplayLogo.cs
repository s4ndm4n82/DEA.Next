using System.Reflection;
using WriteLog;

namespace DisplayLogoClass;

internal class DisplayLogo
{
    public static void Logo()
    {
        Version programVersion = Assembly.GetExecutingAssembly().GetName().Version;
        string programName = Assembly.GetExecutingAssembly().GetName().Name;

        ShowLogo(programName);
        ShowLogo(programVersion.ToString());

        WriteLogClass.WriteToLog(1, $"{programName} .... v{programVersion} ....", 1);
    }

    private static void ShowLogo(string textToSet)
    {            
        int windowWidth = Console.WindowWidth;
        int windowPadding = (windowWidth - textToSet.Length) / 2;
        Console.WriteLine(new string(' ', windowPadding) + textToSet);
    }
}