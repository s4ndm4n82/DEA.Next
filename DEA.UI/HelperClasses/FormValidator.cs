using System.Text.RegularExpressions;

namespace DEA.UI.HelperClasses
{
    public static class FormValidator
    {
        public static bool ValidateCustomerName(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer name is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerUserName(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer user name is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValudateCustomerApitoken(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer API token is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerQueue(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer queue is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerPassword(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer password is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerBatchSize(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer batch size is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerDocumentPath(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer document path is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerDomain(TextBox text, ErrorProvider errorProvider)
        {
            var regex = @"^https:\/\/.*\/Import\?$";
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer domain is required.");
                return false;
            }
            else if (!Regex.IsMatch(text.Text, regex))
            {
                errorProvider.SetError(text, "Invalid URL format. Expected format: https://<domain>/<path>/Import?");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerDeliveryMethod(ComboBox combo, ErrorProvider errorProvider)
        {
            if (combo.SelectedIndex == -1)
            {
                errorProvider.SetError(combo, "Customer delivery method is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerExtenstion(CheckedListBox checkedListBox, ErrorProvider errorProvider)
        {
            if (checkedListBox.CheckedItems.Count == 0)
            {
                errorProvider.SetError(checkedListBox, "At least one extension is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }
    }
}