using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.UI.Forms;
using DEA.UI.HelperClasses;
using System.Runtime.Versioning;

namespace DEA.UI
{
    [SupportedOSPlatform("windows")]
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

            // Add even handler for DatagridView CellDoubleClick event
            grdEditCustomer.CellDoubleClick += GrdEditCustomer_CellDoubleClick;

            // Add even handler for DatagridView CellContentClick event
            grdEditCustomer.CellValueChanged += GrdEditCustomer_CellValueChanged;

            // Set the selection mode to FullRowSelect
            grdEditCustomer.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Set the auto size mode for columns
            grdEditCustomer.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            btnEditCustomerSearch.Click += BtnEditCustomerSearch_Click;

            // Register the click event for the reset button
            btnEditCutomerReset.Click += BtnEditCutomerReset_Click;

            // Register the click event for the cancel button
            btnEditCustomerCancel.Click += BtnEditCustomerCancel_Click;
        }

        public void LoadCustomerData()
        {
            // Get the customers
            var customers = GetCustomers();
            // Bind the customers to the grid
            grdEditCustomer.DataSource = customers;

            // Load the column settings
            GridColumnSettings();
        }

        private List<CustomerDetails> GetCustomers()
        {
            // Get the customers from the database
            return [.. _conttext.CustomerDetails];
        }

        private void GridColumnSettings()
        {
            // Hide the ApiKey column
            grdEditCustomer.Columns["Token"].Visible = false;
            grdEditCustomer.Columns["FtpDetails"].Visible = false;
            grdEditCustomer.Columns["EmailDetails"].Visible = false;
            grdEditCustomer.Columns["CreatedDate"].Visible = false;
            grdEditCustomer.Columns["ModifiedDate"].Visible = false;
            grdEditCustomer.Columns["TemplateKey"].Visible = false;
            grdEditCustomer.Columns["DocumentId"].Visible = false;
            grdEditCustomer.Columns["DocumentEncoding"].Visible = false;
            grdEditCustomer.Columns["MaxBatchSize"].Visible = false;
            grdEditCustomer.Columns["FieldOneName"].Visible = false;
            grdEditCustomer.Columns["FieldOneValue"].Visible = false;
            grdEditCustomer.Columns["FieldTwoName"].Visible = false;
            grdEditCustomer.Columns["FieldTwoValue"].Visible = false;
            grdEditCustomer.Columns["Domain"].Visible = false;
            grdEditCustomer.Columns["FileDeliveryMethod"].Visible = false;

            // Make the columns read only
            grdEditCustomer.Columns["Id"].ReadOnly = true;
            grdEditCustomer.Columns["CustomerName"].ReadOnly = true;
            grdEditCustomer.Columns["UserName"].ReadOnly = true;
            grdEditCustomer.Columns["Queue"].ReadOnly = true;
            grdEditCustomer.Columns["ProjectId"].ReadOnly = true;
            grdEditCustomer.Columns["TemplateKey"].ReadOnly = true;
            grdEditCustomer.Columns["DocumentId"].ReadOnly = true;
            grdEditCustomer.Columns["DocumentEncoding"].ReadOnly = true;
            grdEditCustomer.Columns["MaxBatchSize"].ReadOnly = true;
            grdEditCustomer.Columns["FieldOneName"].ReadOnly = true;
            grdEditCustomer.Columns["FieldOneValue"].ReadOnly = true;
            grdEditCustomer.Columns["FieldTwoName"].ReadOnly = true;
            grdEditCustomer.Columns["FieldTwoValue"].ReadOnly = true;
            grdEditCustomer.Columns["Domain"].ReadOnly = true;
            grdEditCustomer.Columns["FileDeliveryMethod"].ReadOnly = true;
        }

        // Event handler for the DataGridView CellDoubleClick event
        private void GrdEditCustomer_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected customer ID
                var selectedRow = grdEditCustomer.Rows[e.RowIndex];
                var customerId = (Guid)selectedRow.Cells["Id"].Value;

                // Open the edit customer form
                var editCustomerForm = new EditCustomerForm(_conttext, customerId, this);
                editCustomerForm.ShowDialog();
            }
        }

        // Event handler for the DataGridView CellValueChanged event
        private void GrdEditCustomer_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && grdEditCustomer.Columns[e.ColumnIndex].Name == "Status")
            {
                // Get the selected customer ID
                var selectedRow = grdEditCustomer.Rows[e.RowIndex];
                var customerId = (Guid)selectedRow.Cells["Id"].Value;

                // Get the selected customer
                var newStatus = (bool)selectedRow.Cells["Status"].Value;

                // Update the customer status in the database
                var customer = _conttext.CustomerDetails.Find(customerId);
                if (customer != null)
                {
                    customer.Status = newStatus;
                    customer.ModifiedDate = DateTime.UtcNow;

                    var result = _conttext.SaveChanges();

                    if (result > 0)
                    {
                        MessageBox.Show("Customer status updated successfully.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the customer status.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Event handler for the search button click event
        private void BtnEditCustomerSearch_Click(object? sender, EventArgs e)
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

        // Event handler for the reset button click event
        private void BtnEditCutomerReset_Click(object? sender, EventArgs e)
        {
            cusEditSearchTxt.Text = string.Empty;
            searchCusId.Checked = true;
            LoadCustomerData();
        }

        // Add the nullable reference type annotations to the event handler parameters
        private void BtnEditCustomerCancel_Click(object? sender, EventArgs e)
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
            var searchText = cusEditSearchTxt.Text.Trim();
            var searchType = searchCusId.Checked ? "Id" :
                searchProjectId.Checked ? "ProjectId" :
                searchCusName.Checked ? "CustomerName" : string.Empty;

            if (string.IsNullOrEmpty(searchType))
            {
                MessageBox.Show("Please select a search type.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var customers = GetCustomers();
            var filteredCustomers = SearchMethods.SearchCustomers(customers, searchText, searchType);

            grdEditCustomer.DataSource = filteredCustomers;
        }

        // Method to initialize the tool tips
        private void InitalizeToolTips()
        {
            _toolTipHelper.SetToolTip(searchCusId, "Use the customer ID to search.");
            _toolTipHelper.SetToolTip(searchProjectId, "Use the Project ID to search.");
            _toolTipHelper.SetToolTip(searchCusName, "Use the customer name to search.");
            _toolTipHelper.SetToolTip(cusEditSearchTxt, "Enter search text.");
            _toolTipHelper.SetToolTip(btnEditCustomerSearch, "Search for a customer.");
            _toolTipHelper.SetToolTip(btnEditCutomerReset, "Reset the form.");
            _toolTipHelper.SetToolTip(btnEditCustomerCancel, "Cancel the form.");
        }
    }
}
