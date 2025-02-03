namespace DEA.UI.HelperClasses
{
    internal class ResetForms
    {
        public static void RestAddCustomersForm(AddCustomers form)
        {
            // Reset all the textboxes
            form.cusNameTxt.Text = string.Empty;
            form.cusUnameTxt.Text = string.Empty;
            form.cusApiTokenTxt.Text = string.Empty;
            form.cusQueuTxt.Text = string.Empty;
            form.cusProjIdTxt.Text = string.Empty;
            form.cusTempIdTxt.Text = string.Empty;
            form.cusDocIdTxt.Text = string.Empty;
            form.cusDocencTxt.Text = "UTF-8";
            form.cusMaxBatchTxt.Text = "1";
            form.cusFonNameTxt.Text = string.Empty;
            form.cusFonValTxt.Text = string.Empty;
            form.custFtwoNameTxt.Text = string.Empty;
            form.custFtwoValTxt.Text = string.Empty;
            form.cusDomainTxt.Text = string.Empty;
            form.ftpHostTxt.Text = string.Empty;
            form.ftpHostTxt.Enabled = false;
            form.ftpUserNameTxt.Text = string.Empty;
            form.ftpUserNameTxt.Enabled = false;
            form.ftpPasswordTxt.Text = string.Empty;
            form.ftpPasswordTxt.Enabled = false;
            form.ftpPortTxt.Text = string.Empty;
            form.ftpPortTxt.Enabled = false;
            form.ftpMainPathTxt.Text = string.Empty;
            form.ftpMainPathTxt.Enabled = false;
            form.ftpSubPathTxt.Text = string.Empty;
            form.ftpSubPathTxt.Enabled = false;
            form.emlAddressTxt.Text = string.Empty;
            form.emlAddressTxt.Enabled = false;
            form.emlInboxPathTxt.Text = string.Empty;
            form.emlInboxPathTxt.Enabled = false;

            // Reset all the comboboxes
            form.cusDelMethodCombo.SelectedIndex = -1;
            form.cusDelMethodCombo.Text = "Select Delivery Method ...";

            form.ftpTypCombo.SelectedIndex = -1;
            form.ftpTypCombo.Text = "Select The Ftp Type ...";
            form.ftpTypCombo.Enabled = false;

            form.ftpProfileCombo.SelectedIndex = 0;
            form.ftpProfileCombo.Enabled = false;

            // Reset all the radio buttons
            form.cusOn.Checked = true;
            form.cusOff.Checked = false;

            form.ftpLoopOn.Checked = false;
            form.ftpLoopOn.Enabled = false;

            form.ftpLoopOff.Checked = true;
            form.ftpLoopOff.Enabled = false;

            form.ftpMoveToSubOn.Checked = false;
            form.ftpMoveToSubOn.Enabled = false;

            form.ftpMoveToSubOff.Checked = true;
            form.ftpMoveToSubOff.Enabled = false;

            form.ftpRemoveOn.Checked = true;
            form.ftpRemoveOn.Enabled = false;

            form.ftpRemoveOff.Checked = false;
            form.ftpRemoveOff.Enabled = false;

            form.emlSenAdressOn.Checked = false;
            form.emlSenAdressOn.Enabled = false;

            form.emlSenAdressOff.Checked = true;
            form.emlSenAdressOff.Enabled = false;

            form.emlSndSubjectOn.Checked = false;
            form.emlSndSubjectOn.Enabled = false;

            form.emlSndSubjectOff.Checked = true;
            form.emlSndSubjectOff.Enabled = false;

            // Reset all the listboxes
            FormFunctionHelper.CheckAllItems(form.cusDocExtList);


            // Reset the error provider
            form._errorProvider.Clear();
        }
    }
}
