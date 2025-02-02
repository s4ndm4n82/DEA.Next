using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;

namespace DEA.UI.HelperClasses
{
    internal class SaveCustomerData(DataContext context)
    {
        private readonly DataContext _context = context;

        public void SaveCustomerDetails(AddCustomers form)
        {
            try
            {
                // Generating a new GUID
                var cusId = Guid.NewGuid();

                // Creating a new CustomerDetails object
                var customerDetails = new CustomerDetails
                {
                    Id = cusId,
                    Status = form.cusOn.Checked,
                    CustomerName = form.cusNameTxt.Text.Trim(),
                    UserName = form.cusUnameTxt.Text.Trim(),
                    Token = form.cusApiTokenTxt.Text.Trim(),
                    Queue = int.Parse(form.cusQueuTxt.Text.Trim()),
                    ProjectId = form.cusProjIdTxt.Text.Trim(),
                    DocumentId = form.cusDocIdTxt.Text.Trim(),
                    DocumentEncoding = string.IsNullOrEmpty(form.cusDocencTxt.Text.Trim()) ? form.cusDocencTxt.Text.Trim() : "UTF-8",
                    MaxBatchSize = int.Parse(form.cusMaxBatchTxt.Text.Trim()),
                    FieldOneValue = form.cusFonNameTxt.Text.Trim(),
                    FieldOneName = form.cusFonNameTxt.Text.Trim(),
                    FieldTwoValue = form.custFtwoValTxt.Text.Trim(),
                    FieldTwoName = form.custFtwoNameTxt.Text.Trim(),
                    Domain = form.cusDomainTxt.Text.Trim(),
                    FileDeliveryMethod = form.cusDelMethodCombo.SelectedItem?.ToString()?.ToUpper() ?? "FTP",
                    DocumentDetails = form.cusDocExtList.CheckedItems.Cast<string>().Select(ext => new DocumentDetails { Extension = ext }).ToList(),
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow
                };

                // Adding CustomerDetails to the context
                _context.CustomerDetails.Add(customerDetails);

                switch (form.cusDelMethodCombo.SelectedItem?.ToString()?.ToLower())
                {
                    case MagicWords.Ftp:

                        // Creating a new FtpDetails object
                        var ftpDetails = new FtpDetails
                        {
                            Id = Guid.NewGuid(),
                            FtpType = form.ftpTypCombo.SelectedItem?.ToString()?.ToUpper() ?? "FTP",
                            FtpProfile = form.ftpProfileCombo.SelectedItem?.ToString() ?? "profileepe",
                            FtpHost = form.ftpHostTxt.Text.Trim(),
                            FtpUser = form.ftpUserNameTxt.Text.Trim(),
                            FtpPassword = form.ftpPasswordTxt.Text.Trim(),
                            FtpPort = int.Parse(string.IsNullOrEmpty(form.ftpPortTxt.Text.Trim()) ? "21" : form.ftpPortTxt.Text.Trim()),
                            FtpFolderLoop = form.ftpLoopOn.Checked,
                            FtpMoveToSubFolder = form.ftpMoveToSubOn.Checked,
                            FtpMainFolder = form.ftpMainPathTxt.Text.Trim(),
                            FtpSubFolder = form.ftpSubPathTxt.Text.Trim(),
                            FtpRemoveFiles = form.ftpRemoveOn.Checked,
                            CustomerDetailsId = cusId
                        };

                        // Adding FtpDetails to the context
                        _context.FtpDetails.Add(ftpDetails);
                        break;

                    case MagicWords.Email:

                        // Creating a new EmailDetails object
                        var emailDetails = new EmailDetails
                        {
                            Id = Guid.NewGuid(),
                            Email = form.emlAddressTxt.Text.Trim(),
                            EmailInboxPath = form.emlInboxPathTxt.Text.Trim(),
                            SendEmail = form.emlSenAdressOn.Checked,
                            SendSubject = form.emlSndSubjectOn.Checked,
                            CustomerDetailsId = cusId
                        };

                        // Adding EmailDetails to the context
                        _context.EmailDetails.Add(emailDetails);
                        break;

                    default:
                        MessageBox.Show("Invalid file delivery method", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }

                var saveResult = _context.SaveChanges();

                if (saveResult > 0)
                {
                    MessageBox.Show("Customer details saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save customer details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

        }
    }
}
