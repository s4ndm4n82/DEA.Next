using DEA.Next.Data;

namespace DEA.UI.HelperClasses
{
    public class CustomerRemovalHelper(DataContext context)
    {
        private readonly DataContext _context = context;

        public void RemoveSelectedCustomers(DataGridView grdRemoveCustomers)
        {
            var selectedRows = grdRemoveCustomers.SelectedRows;

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer to remove.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
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

            _context.SaveChanges();
        }
    }
}
