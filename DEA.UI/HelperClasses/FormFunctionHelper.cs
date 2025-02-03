using DEA.Next.HelperClasses.OtherFunctions;

namespace DEA.UI.HelperClasses
{
    internal class FormFunctionHelper
    {
        public void CheckBoxListHandler(object sender, ItemCheckEventArgs e)
        {
            if (sender is not CheckedListBox checkedListBox) return;

            // Unsubscribe from the ItemCheck event to avoid recursive calls
            checkedListBox.ItemCheck -= CheckBoxListHandler;

            // If the first item is checked then check everything else.
            if (e.Index == 0 && e.NewValue == CheckState.Checked)
            {
                // Iterate through the list and check everything
                for (int i = 1; i < checkedListBox.Items.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, true);
                }
            }
            else if (e.Index == 0 && e.NewValue == CheckState.Unchecked)
            {
                for (int i = 1; i < checkedListBox.Items.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, false);
                }
            }

            // Resubscribe to the ItemCheck event
            checkedListBox.ItemCheck += CheckBoxListHandler;
        }

        public static void CheckAllItems(CheckedListBox checkedListBox)
        {
            // Check the first item and trigger the event handler
            checkedListBox.SetItemChecked(0, true);
        }

        public static void DisableFieldsOnLoad(Control ftpDetailsGrp,
            Control emlDetailsGrp,
            Control ftpSubPathTxt)
        {
            // Disable the FTP details fields
            foreach (Control control in ftpDetailsGrp.Controls)
            {
                control.Enabled = false;
            }

            // Disable the email details fields
            foreach (Control control in emlDetailsGrp.Controls)
            {
                control.Enabled = false;
            }

            // Disable the FTP sub path text box
            ftpSubPathTxt.Enabled = false;
        }

        public static void HandleDeliveryMethodChanges(ComboBox cusDelMethodCombo,
            Control ftpDetails,
            Control emlDetails,
            Control ftpSubPathTxt)
        {
            var selectedMethod = cusDelMethodCombo.SelectedItem?.ToString();

            if (string.IsNullOrWhiteSpace(selectedMethod)) return;

            if (selectedMethod.Equals(MagicWords.Ftp, StringComparison.OrdinalIgnoreCase))
            {
                EnableFtpDetails(ftpDetails, ftpSubPathTxt);
                DisableEmailDetails(emlDetails);
            }
            else if (selectedMethod.Equals(MagicWords.Email, StringComparison.OrdinalIgnoreCase))
            {
                EnableEmailDetails(emlDetails);
                DisableFtpDetails(ftpDetails);
            }
        }

        private static void EnableFtpDetails(Control ftpDetails, Control ftpSubPathTxt)
        {
            foreach (Control control in ftpDetails.Controls)
            {
                if (control != ftpSubPathTxt)
                    control.Enabled = true;
            }
        }

        private static void DisableFtpDetails(Control ftpDetails)
        {
            foreach (Control control in ftpDetails.Controls)
            {
                control.Enabled = false;
            }
        }

        private static void EnableEmailDetails(Control emlDetails)
        {
            foreach (Control control in emlDetails.Controls)
            {
                control.Enabled = true;
            }
        }

        private static void DisableEmailDetails(Control emlDetails)
        {
            foreach (Control control in emlDetails.Controls)
            {
                control.Enabled = false;
            }
        }

        public static void HandleFtpSubPathChanges(RadioButton radioButton, TextBox textBox)
        {
            textBox.Enabled = radioButton.Checked;
        }
    }
}
