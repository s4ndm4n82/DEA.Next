using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;
using DEA.UI.Forms;

namespace DEA.UI.HelperClasses
{
    internal class UpdateCustomerDetails(DataContext context)
    {
        private readonly DataContext _context = context;

        public void UpdateCustomerData(EditCustomerForm form, CustomerDetails customer)
        {
            try
            {
                if (customer != null)
                {
                    // Create a new CustomerDetails object with updated details
                    var updatedCustomer = new CustomerDetails
                    {
                        Status = customer.Status,
                        CustomerName = form.cusNameEdFrmTxt.Text,
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
                        DocumentDetails = form.cusDocExtListEdFrm.CheckedItems.Cast<string>().Skip(1).Select(ext => new DocumentDetails { Extension = ext }).ToList(),
                        CreatedDate = customer.CreatedDate,
                        ModifiedDate = DateTime.Now // Update the modified date
                    };

                    // Update the customer in the context
                    _context.Entry(customer).CurrentValues.SetValues(updatedCustomer);

                    switch (customer.FileDeliveryMethod.ToString().ToLower())
                    {
                        case MagicWords.Ftp:
                            // Create a new FtpDetails object with updated details
                            var updatedFtpDetails = new FtpDetails
                            {
                                FtpType = form.ftpTypComboEdFrm.SelectedItem?.ToString()?.ToUpper() ?? "FTP",
                                FtpProfile = form.ftpProfileComboEdFrm.SelectedItem?.ToString() ?? "profileepe",
                                FtpHost = form.ftpHostEdFrmTxt.Text,
                                FtpUser = form.ftpUserNameEdFrmTxt.Text,
                                FtpPassword = form.ftpPasswordEdFrmTxt.Text,
                                FtpPort = int.Parse(string.IsNullOrEmpty(form.ftpPortEdFrmTxt.Text) ? "21" : form.ftpPortEdFrmTxt.Text),
                                FtpFolderLoop = form.Ftp,
                                FtpMoveToSubFolder = form.ftpMoveToSubEdFrmOn.Checked
                            };
                            // Update the FTP details in the context
                            _context.Entry(customer.FtpDetails).CurrentValues.SetValues(updatedFtpDetails);
                            break;
                        case MagicWords.Email:
                            // Create a new EmailDetails object with updated details
                            var updatedEmailDetails = new EmailDetails
                            {
                                Id = customer.EmailDetails.Id,
                                EmailType = customer.EmailDetails.EmailType,
                                EmailProfile = customer.EmailDetails.EmailProfile,
                                EmailHost = form.emlHostEdFrmTxt.Text,
                                EmailUser = form.emlUserNameEdFrmTxt.Text,
                                EmailPassword = form.emlPasswordEdFrmTxt.Text,
                                EmailPort = int.Parse(string.IsNullOrEmpty(form.emlPortEdFrmTxt.Text) ? "25" : form.emlPortEdFrmTxt.Text),
                                EmailFolderLoop = form.emlLoopEdFrmOn.Checked,
                                EmailMoveToSubFolder = form.emlMoveToSubEdFrmOn.Checked
                            };
                            // Update the email details in the context
                            _context.Entry(customer.EmailDetails).CurrentValues.SetValues(updatedEmailDetails);
                            break;
                    }

                    // Save the changes
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Handle the exception (log it, rethrow it, etc.)
            }
        }
    }
}
