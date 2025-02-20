using DEA.Next.Data;
using System.Runtime.Versioning;

namespace DEA.UI.HelperClasses
{
    [SupportedOSPlatform("windows")]
    public class CustomerRemovalHelper(DataContext context)
    {
        private readonly DataContext _context = context;

        public bool RemoveSelectedCustomers(DataGridView grdRemoveCustomers)
        {
            try
            {
                var selectedRows = grdRemoveCustomers.SelectedRows;

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a customer to remove.",
                        "Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return false;
                }

                foreach (DataGridViewRow selectedRow in selectedRows)
                {
                    var customerId = selectedRow.Cells["Id"].Value;
                    var customer = _context.CustomerDetails.Find(customerId);

                    if (customer != null)
                    {
                        _context.CustomerDetails.Remove(customer);
                    }
                }

                var result = _context.SaveChanges();

                if (result > 0)
                {
                    MessageBox.Show("Customer details removed successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return true;
                }

                MessageBox.Show("Failed to remove the customer details.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exxception error thrown while removing cutomer: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                throw;
            }
        }
    }
}
