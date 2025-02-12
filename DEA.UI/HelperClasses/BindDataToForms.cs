using DEA.Next.Entities;
using DEA.UI.Forms;

namespace DEA.UI.HelperClasses
{
    class BindDataToForms
    {
        public static void BindEditCustomerFormData(EditCustomerForm form, CustomerDetails customerDetails)
        {
            // Bind customer details
            form.cusOnEdFrm.Checked = customerDetails.Status;
            form.cusNameEdFrmTxt.Text = customerDetails.CustomerName;
            form.cusUnameEdFrmTxt.Text = customerDetails.UserName;
            form.cusApiTokenEdFrmTxt.Text = customerDetails.Token;
            form.cusQueuEdFrmTxt.Text = customerDetails.Queue.ToString();
            form.cusProjIdEdFrmTxt.Text = customerDetails.ProjectId;
            form.cusTempIdEdFrmTxt.Text = customerDetails.TemplateKey;
            form.cusDocIdEdFrmTxt.Text = customerDetails.DocumentId;
            form.cusMaxBatchEdFrmTxt.Text = customerDetails.MaxBatchSize.ToString();
            form.cusDocencEdFrmTxt.Text = customerDetails.DocumentEncoding;
            form.cusFonNameEdFrmTxt.Text = customerDetails.FieldOneName;
            form.cusFonValEdFrmTxt.Text = customerDetails.FieldOneValue;
            form.custFtwoNameEdFrmTxt.Text = customerDetails.FieldTwoName;
            form.custFtwoValEdFrmTxt.Text = customerDetails.FieldTwoValue;
            form.cusDelMethodEdFrmCombo.SelectedItem = customerDetails.FileDeliveryMethod;

            if (customerDetails.DocumentDetails != null)
            {
                var docExtList = customerDetails.DocumentDetails
                    .Select(dd => dd.Extension.TrimStart('.')).ToList();

                // Iterate through the document extension list and check the items
                for (var i = 0; i < form.cusDocExtListEdFrm.Items.Count; i++)
                {
                    var item = form.cusDocExtListEdFrm.Items[i]?.ToString();
                    if (item != null && docExtList.Contains(item))
                    {
                        form.cusDocExtListEdFrm.SetItemChecked(i, true);
                    }
                }

                var allChecked = true;
                for (var i = 1; i < form.cusDocExtListEdFrm.Items.Count; i++)
                {
                    if (!form.cusDocExtListEdFrm.GetItemChecked(i))
                    {
                        allChecked = false;
                        break;
                    }
                }

                if (allChecked)
                {
                    form.cusDocExtListEdFrm.SetItemChecked(0, true);
                }
            }

            // Bind FTP details
            if (customerDetails.FtpDetails != null)
            {
                var ftpDetails = customerDetails.FtpDetails;
                form.ftpTypComboEdFrm.SelectedItem = ftpDetails.FtpType;
                form.ftpProfileComboEdFrm.SelectedItem = ftpDetails.FtpProfile;
                form.ftpHostEdFrmTxt.Text = ftpDetails.FtpHost;
                form.ftpUserNameEdFrmTxt.Text = ftpDetails.FtpUser;
                form.ftpPasswordEdFrmTxt.Text = ftpDetails.FtpPassword;
                form.ftpPortEdFrmTxt.Text = ftpDetails.FtpPort.ToString();
                form.ftpMainPathEdFrmTxt.Text = ftpDetails.FtpMainFolder;
                form.ftpLoopOnEdFrm.Checked = ftpDetails.FtpFolderLoop;
                form.ftpMoveToSubOnEdFrm.Checked = ftpDetails.FtpMoveToSubFolder;
                form.ftpSubPathEdFrmTxt.Text = ftpDetails.FtpSubFolder;
                form.ftpRemoveOnEdFrm.Checked = ftpDetails.FtpRemoveFiles;
            }

            // Bind email details
            if (customerDetails.EmailDetails != null)
            {
                var emailDetails = customerDetails.EmailDetails;
                form.emlAddressEdFrmTxt.Text = emailDetails.Email;
                form.emlInboxPathEdFrmTxt.Text = emailDetails.EmailInboxPath;
                form.emlSenAdressOnEdFrm.Checked = emailDetails.SendEmail;
                form.emlSndSubjectOnEdFrm.Checked = emailDetails.SendSubject;
                form.emlSndBodyOnEdFrm.Checked = emailDetails.SendBody;
            }
        }
    }
}
