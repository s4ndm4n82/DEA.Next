namespace DEA.UI.HelperClasses
{
    internal class ToolTipHelper
    {
        private readonly ToolTip _toolTip;

        public ToolTipHelper()
        {
            _toolTip = new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 1000,
                ReshowDelay = 500,
                ShowAlways = true
            };

        }

        public void SetToolTip(Control control, string tooText)
        {
            _toolTip.SetToolTip(control, tooText);
        }
    }
}
