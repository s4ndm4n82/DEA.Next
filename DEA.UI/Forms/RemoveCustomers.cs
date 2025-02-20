using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.UI.HelperClasses;
using System.Runtime.Versioning;

namespace DEA.UI
{
    [SupportedOSPlatform("windows")]
    public partial class RemoveCustomers : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;
        private readonly CustomerRemovalHelper _customerRemovalHelper;

        public RemoveCustomers(DataContext context)
        {
            InitializeComponent();
            _conttext = context;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();
            _customerRemovalHelper = new CustomerRemovalHelper(context);

            // Initialize the controls
            InitializeControls();

            // Enanable multiple row selection
            grdRemoveCustomer.MultiSelect = true;

            // Set the selection mode to FullRowSelect
            grdRemoveCustomer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Set the auto size mode for columns
            grdRemoveCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void InitializeControls()
        {
            // Set the default values
            DefaultValueSetter.SetDefaultValues(this);

            // Initializing tool tips
            InitalizeToolTips();

            // Load the customer data
            LoadCustomerData();

            // Register the click event for the reset button
            btnRmSearch.Click += BtnRmSearch_Click;

            // Register the click event for the remove button
            rmBtnRemove.Click += RmButtonRemove_Click;

            // Register the click event for the reset button
            rmBtnReset.Click += RmBtnReset_Click;

            // Register the click event for the cancel button
            rmBtnCancel.Click += RmBtnCancel_Click;
        }

        private void LoadCustomerData()
        {
            // Get the customers
            var customers = GetCustomers();

            // Bind the customers to the grid
            grdRemoveCustomer.DataSource = customers;

            // Load the column settings
            GridColumnSettings();

        }

        private List<CustomerDetails> GetCustomers()
        {
            // Get the customers
            return [.. _conttext.CustomerDetails];
        }

        // Event handler for the search button click event
        private void BtnRmSearch_Click(object? sender, EventArgs e)
        {
            try
            {
                SearchCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RmButtonRemove_Click(object? sender, EventArgs e)
        {
            try
            {
                var result = MessageBox.Show("Are you sure you want remove the customer details?",
                "Remove Customer Details",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

                if (result == DialogResult.No)
                    return;

                var removeResult = _customerRemovalHelper.RemoveSelectedCustomers(grdRemoveCustomer);

                if (removeResult)
                    LoadCustomerData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RmBtnReset_Click(object? sender, EventArgs e)
        {
            // Clear the search text
            rmSearchTxt.Text = string.Empty;
            // Set the default search type
            rmSearchId.Checked = true;
            // Load the customer data
            LoadCustomerData();
        }

        private void RmBtnCancel_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to close the application?",
                "Exit The Application",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button1);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        // Method to search for customers
        private void SearchCustomers()
        {
            // Get the search text
            var searchText = rmSearchTxt.Text.Trim();
            var searchType = rmSearchId.Checked ? "Id" :
                rmSearchProjId.Checked ? "ProjectId" :
                rmSearchName.Checked ? "CustomerName" : string.Empty;

            if (string.IsNullOrEmpty(searchType))
            {
                MessageBox.Show("Please select a search type.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var customers = GetCustomers();
            var filteredCustomers = SearchMethods.SearchCustomers(customers, searchText, searchType);

            grdRemoveCustomer.DataSource = filteredCustomers;
        }

        private void GridColumnSettings()
        {
            // Hide the ApiKey column
            grdRemoveCustomer.Columns["Token"].Visible = false;
            grdRemoveCustomer.Columns["FtpDetails"].Visible = false;
            grdRemoveCustomer.Columns["EmailDetails"].Visible = false;
            grdRemoveCustomer.Columns["CreatedDate"].Visible = false;
            grdRemoveCustomer.Columns["ModifiedDate"].Visible = false;
            grdRemoveCustomer.Columns["TemplateKey"].Visible = false;
            grdRemoveCustomer.Columns["DocumentId"].Visible = false;
            grdRemoveCustomer.Columns["DocumentEncoding"].Visible = false;
            grdRemoveCustomer.Columns["MaxBatchSize"].Visible = false;
            grdRemoveCustomer.Columns["FieldOneName"].Visible = false;
            grdRemoveCustomer.Columns["FieldOneValue"].Visible = false;
            grdRemoveCustomer.Columns["FieldTwoName"].Visible = false;
            grdRemoveCustomer.Columns["FieldTwoValue"].Visible = false;
            grdRemoveCustomer.Columns["Domain"].Visible = false;
            grdRemoveCustomer.Columns["FileDeliveryMethod"].Visible = false;

            // Make the columns read only
            FormFunctionHelper.SetDataGridColumnsReadOnly(grdRemoveCustomer);
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
