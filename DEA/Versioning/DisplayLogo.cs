using System.Reflection;

namespace DisplayLogoClass
{
    internal class DisplayLogo
    {
        public static void Logo()
        {
            Version programVersion = Assembly.GetExecutingAssembly().GetName().Version;
            string programName = Assembly.GetExecutingAssembly().GetName().Name;

            ShowLogo(programName);
            ShowLogo(programVersion.ToString());
        }

        private static void ShowLogo(string textToSet)
        {            
            int windowWidth = Console.WindowWidth;
            int windowPadding = (windowWidth - textToSet.Length) / 2;
            Console.WriteLine(new string(' ', windowPadding) + textToSet);
        }
    }
}
