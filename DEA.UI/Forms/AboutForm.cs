using System.Reflection;
using System.Runtime.Versioning;

namespace DEA.UI.Forms
{
    [SupportedOSPlatform("windows")]
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            LoadAssemblyInfo();
        }

        private void LoadAssemblyInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version?.ToString() ?? "N/A";

            abtVersion.Text = $"Version: {version}";
        }
    }
}
