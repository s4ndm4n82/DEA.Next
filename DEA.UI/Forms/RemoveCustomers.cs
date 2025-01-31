using DEA.Next.Data;
using DEA.UI.HelperClasses;

namespace DEA.UI
{
    public partial class RemoveCustomers : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;

        public RemoveCustomers(DataContext context)
        {
            InitializeComponent();
            _conttext = context;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();

            // Initialize the controls
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Set the default values
            DefaultValueSetter.SetDefaultValues(this);

            // Initializing tool tips
            InitalizeToolTips();
        }

        private void InitalizeToolTips()
        {
            _toolTipHelper.SetToolTip(rmSearchId, "Use the customer ID to search.");
            _toolTipHelper.SetToolTip(rmSearchName, "Use the customer name to search.");
            _toolTipHelper.SetToolTip(rmSearchProjId, "Use the Project ID to search.");
            _toolTipHelper.SetToolTip(rmSearchTxt, "Enter search text.");
            _toolTipHelper.SetToolTip(btnRmSearch, "Search for a customer.");
            _toolTipHelper.SetToolTip(rmBtnRemove, "Remove a customer.");
            _toolTipHelper.SetToolTip(rmBtnCancel, "Cancel.");
            _toolTipHelper.SetToolTip(rmBtnReset, "Reset the form.");
        }
    }
}
