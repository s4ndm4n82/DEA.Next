using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.UI.Forms;
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

            // Add even handler for DatagridView CellDoubleClick event
            grdEditCustomer.CellDoubleClick += GrdEditCustomer_CellDoubleClick;

            // Add even handler for DatagridView CellContentClick event
            grdEditCustomer.CellValueChanged += GrdEditCustomer_CellValueChanged;
        }

        private void InitializeControls()
        {
            // Set the default values
            DefaultValueSetter.SetDefaultValues(this);

            // Initializing tool tips
            InitalizeToolTips();

            // Load the customer data
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            // Get the customers
            var customers = GetCustomers();
            // Bind the customers to the grid
            grdEditCustomer.DataSource = customers;

            // Hide the ApiKey column
            grdEditCustomer.Columns["Token"].Visible = false;
            grdEditCustomer.Columns["FtpDetails"].Visible = false;
            grdEditCustomer.Columns["EmailDetails"].Visible = false;
            grdEditCustomer.Columns["CreatedDate"].Visible = false;
            grdEditCustomer.Columns["ModifiedDate"].Visible = false;

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

        private List<CustomerDetails> GetCustomers()
        {
            // Get the customers from the database
            return [.. _conttext.CustomerDetails];
        }

        private void GrdEditCustomer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Get the selected customer ID
                var selectedRow = grdEditCustomer.Rows[e.RowIndex];
                var customerId = (Guid)selectedRow.Cells["Id"].Value;

                // Open the edit customer form
                var editCustomerForm = new EditCustomerForm(_conttext, customerId);
                editCustomerForm.ShowDialog();
            }
        }

        private void GrdEditCustomer_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
                    // Create a new customer object with updated status
                    var updatedCustomer = new CustomerDetails
                    {
                        Id = customer.Id,
                        Status = newStatus,
                        CustomerName = customer.CustomerName,
                        UserName = customer.UserName,
                        Token = customer.Token,
                        Queue = customer.Queue,
                        ProjectId = customer.ProjectId,
                        TemplateKey = customer.TemplateKey,
                        DocumentId = customer.DocumentId,
                        DocumentEncoding = customer.DocumentEncoding,
                        MaxBatchSize = customer.MaxBatchSize,
                        FieldOneValue = customer.FieldOneValue,
                        FieldOneName = customer.FieldOneName,
                        FieldTwoValue = customer.FieldTwoValue,
                        FieldTwoName = customer.FieldTwoName,
                        Domain = customer.Domain,
                        FileDeliveryMethod = customer.FileDeliveryMethod,
                        DocumentDetails = customer.DocumentDetails,
                        FtpDetails = customer.FtpDetails,
                        EmailDetails = customer.EmailDetails,
                        CreatedDate = customer.CreatedDate,
                        ModifiedDate = customer.ModifiedDate
                    };

                    // Update the customer in the context
                    _conttext.Entry(customer).CurrentValues.SetValues(updatedCustomer);
                    var result = _conttext.SaveChanges();

                    if (result > 0)
                    {
                        MessageBox.Show("Customer status updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the customer status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

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
