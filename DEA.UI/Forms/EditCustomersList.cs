using DEA.Next.Data;
using DEA.UI.HelperClasses;

namespace DEA.UI
{
    public partial class EditCustomersList : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;

        public EditCustomersList(DataContext context)
        {
            InitializeComponent();

            // Initializing the context
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
            _toolTipHelper.SetToolTip(searchCusId, "Use the customer ID to search.");
            _toolTipHelper.SetToolTip(searchProjectId, "Use the Project ID to search.");
            _toolTipHelper.SetToolTip(searchCusName, "Use the customer name to search.");
            _toolTipHelper.SetToolTip(cusEditSearchTxt, "Enter search text.");
            _toolTipHelper.SetToolTip(btnEditCustomerSearch, "Search for a customer.");
        }
    }
}
