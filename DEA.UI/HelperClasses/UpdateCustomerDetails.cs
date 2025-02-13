using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.UI.Forms;

namespace DEA.UI.HelperClasses
{
    internal class UpdateCustomerDetails(DataContext context)
    {
        private readonly DataContext _context = context;

        public bool UpdateCustomerData(EditCustomerForm form, CustomerDetails customer)
        {
            try
            {
                if (customer != null)
                {
                    // Update the existing customer details directly
                    customer.CustomerName = form.cusNameEdFrmTxt.Text;
                    customer.UserName = form.cusUnameEdFrmTxt;
                    customer.Token = form.cusApiTokenEdFrmTxt;
                    customer.Queue = form.cusQueuEdFrmTxt;
                }

                // Save the changes
                var result = _context.SaveChanges();

                // Show a success message
                if (result > 0)
                {
                    MessageBox
                        .Show("Customer details updated successfully.",
                        "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                {
                    MessageBox
                        .Show("Failed to update customer details.",
                        "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
                else
            {
                MessageBox
                    .Show("Customer details not found.",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
            catch (Exception ex)
            {
                // Handle the exception (log it, rethrow it, etc.)
                MessageBox
                    .Show($"An error occurred while updating the customer details: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
}
    }
}
