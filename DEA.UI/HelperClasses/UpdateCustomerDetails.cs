using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.Next.HelperClasses.OtherFunctions;
using DEA.UI.Forms;
using System.Runtime.Versioning;

namespace DEA.UI.HelperClasses
{
    [SupportedOSPlatform("windows")]
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
                    customer.Status = form.cusOnEdFrm.Checked;
                    customer.CustomerName = form.cusNameEdFrmTxt.Text;
                    customer.UserName = form.cusUnameEdFrmTxt.Text;
                    customer.Token = form.cusApiTokenEdFrmTxt.Text;
                    customer.Queue = int.Parse(form.cusQueuEdFrmTxt.Text);
                    customer.ProjectId = form.cusProjIdEdFrmTxt.Text;
                    customer.TemplateKey = form.cusTempIdEdFrmTxt.Text;
                    customer.DocumentId = form.cusDocIdEdFrmTxt.Text;
                    customer.DocumentEncoding = form.cusDocencEdFrmTxt.Text;
                    customer.MaxBatchSize = int.Parse(form.cusMaxBatchEdFrmTxt.Text);
                    customer.FieldOneName = form.cusFonNameEdFrmTxt.Text;
                    customer.FieldOneValue = form.cusFonValEdFrmTxt.Text;
                    customer.FieldTwoName = form.custFtwoNameEdFrmTxt.Text;
                    customer.FieldTwoValue = form.custFtwoValEdFrmTxt.Text;
                    customer.Domain = form.cusDomainEdFrmTxt.Text;
                    customer.FileDeliveryMethod = form.cusDelMethodEdFrmCombo
                        .SelectedItem?.ToString() ?? string.Empty;
                    customer.DocumentDetails = [.. form.cusDocExtListEdFrm
                        .CheckedItems.Cast<string>()
                        .Select(ext => new DocumentDetails { Extension = ext })];
                    customer.ModifiedDate = DateTime.UtcNow;

                    switch (customer.FileDeliveryMethod.ToLower())
                    {
                        case MagicWords.Ftp:
                            // Update the existing FTP details directly
                            if (customer.FtpDetails == null)
                            {
                                customer.FtpDetails = new FtpDetails
                                {
                                    FtpType = form.ftpTypComboEdFrm.SelectedItem?.ToString()?
                                    .ToUpper() ?? "FTP",
                                    FtpProfile = form.ftpProfileComboEdFrm.SelectedItem?
                                    .ToString() ?? "profileepe",
                                    FtpHost = form.ftpHostEdFrmTxt.Text,
                                    FtpUser = form.ftpUserNameEdFrmTxt.Text,
                                    FtpPassword = form.ftpPasswordEdFrmTxt.Text,
                                    FtpPort = int.Parse(string
                                    .IsNullOrEmpty(form.ftpPortEdFrmTxt.Text) ? "21" : form.ftpPortEdFrmTxt.Text),
                                    FtpFolderLoop = form.ftpLoopOnEdFrm.Checked,
                                    FtpMoveToSubFolder = form.ftpMoveToSubOnEdFrm.Checked,
                                    FtpMainFolder = form.ftpMainPathEdFrmTxt.Text.Trim(),
                                    FtpSubFolder = form.ftpSubPathEdFrmTxt.Text.Trim(),
                                    FtpRemoveFiles = form.ftpRemoveOnEdFrm.Checked
                                };
                            }
                            else
                            {
                                customer.FtpDetails.FtpType = form.ftpTypComboEdFrm.SelectedItem?
                                    .ToString()?.ToUpper() ?? "FTP";
                                customer.FtpDetails.FtpProfile = form.ftpProfileComboEdFrm.SelectedItem?
                                    .ToString() ?? "profileepe";
                                customer.FtpDetails.FtpHost = form.ftpHostEdFrmTxt.Text;
                                customer.FtpDetails.FtpUser = form.ftpUserNameEdFrmTxt.Text;
                                customer.FtpDetails.FtpPassword = form.ftpPasswordEdFrmTxt.Text;
                                customer.FtpDetails.FtpPort = int.Parse(string
                                    .IsNullOrEmpty(form.ftpPortEdFrmTxt.Text) ? "21" : form.ftpPortEdFrmTxt.Text);
                                customer.FtpDetails.FtpFolderLoop = form.ftpLoopOnEdFrm.Checked;
                                customer.FtpDetails.FtpMoveToSubFolder = form.ftpMoveToSubOnEdFrm.Checked;
                                customer.FtpDetails.FtpMainFolder = form.ftpMainPathEdFrmTxt.Text.Trim();
                                customer.FtpDetails.FtpSubFolder = form.ftpSubPathEdFrmTxt.Text.Trim();
                                customer.FtpDetails.FtpRemoveFiles = form.ftpRemoveOnEdFrm.Checked;
                            }

                            // Clean up the EmailDetails if switching from email to FTP
                            if (customer.EmailDetails != null)
                            {
                                _context.EmailDetails.Remove(customer.EmailDetails);
                                customer.EmailDetails = null;
                            }
                            break;

                        case MagicWords.Email:
                            // Update the existing email details directly
                            if (customer.EmailDetails == null)
                            {
                                customer.EmailDetails = new EmailDetails
                                {
                                    Email = form.emlAddressEdFrmTxt.Text.Trim(),
                                    EmailInboxPath = form.emlInboxPathEdFrmTxt.Text.Trim(),
                                    SendSubject = form.emlSndSubjectOnEdFrm.Checked,
                                    SendEmail = form.emlSenAdressOnEdFrm.Checked,
                                    SendBody = form.emlSndBodyOnEdFrm.Checked
                                };
                            }
                            else
                            {
                                customer.EmailDetails.Email = form.emlAddressEdFrmTxt.Text.Trim();
                                customer.EmailDetails.EmailInboxPath = form.emlInboxPathEdFrmTxt.Text.Trim();
                                customer.EmailDetails.SendSubject = form.emlSndSubjectOnEdFrm.Checked;
                                customer.EmailDetails.SendEmail = form.emlSenAdressOnEdFrm.Checked;
                                customer.EmailDetails.SendBody = form.emlSndBodyOnEdFrm.Checked;
                            }

                            // Clean up the FtpDetails if switching from FTP to email
                            if (customer.FtpDetails != null)
                            {
                                _context.FtpDetails.Remove(customer.FtpDetails);
                                customer.FtpDetails = null;
                            }
                            break;

                        default:
                            MessageBox
                                .Show("Invalid file delivery method selected.",
                                "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
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
                var error = !string.IsNullOrEmpty(ex.InnerException?.Message) ? ex.InnerException.Message : ex.Message;

                // Handle the exception (log it, rethrow it, etc.)
                MessageBox
                    .Show($"An error occurred while updating the customer details: {error}",
                    "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
    }
}
