using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace DEA.UI.HelperClasses
{
    [SupportedOSPlatform("windows")]
    internal static class FormValidator
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

        public static bool ValidateCustomerApiToken(TextBox text, ErrorProvider errorProvider)
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

        public static bool ValidateCustomerMaxBatch(TextBox text, ErrorProvider errorProvider)
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

        public static bool ValidateCustomerDocumentEncoding(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Document encoding is required. Set is as UTF-8.");
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
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer domain is required.");
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

        public static bool ValidateCustomerExtensions(CheckedListBox checkedListBox, ErrorProvider errorProvider)
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

        public static bool ValidateCustomerFtpType(ComboBox combo, ErrorProvider errorProvider)
        {
            if (combo.SelectedIndex == -1)
            {
                errorProvider.SetError(combo, "Customer FTP type is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpProfile(ComboBox combo, ErrorProvider errorProvider)
        {
            if (combo.SelectedIndex == -1)
            {
                errorProvider.SetError(combo, "Customer FTP profile is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpHost(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer FTP host is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpUser(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer FTP user name is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpPassword(MaskedTextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer FTP password is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpPort(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer FTP port is required.");
                return false;
            }
            else if (!int.TryParse(text.Text, out _))
            {
                errorProvider.SetError(text, "Customer FTP port must be a number.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpMainPath(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer FTP path is required.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerFtpSubPath(TextBox text,
            RadioButton radioButton,
            ErrorProvider errorProvider)
        {
            if (radioButton.Checked && text.Enabled)
            {
                if (string.IsNullOrWhiteSpace(text.Text))
                {
                    errorProvider.SetError(text, "Customer FTP sub path is required.");
                    return false;
                }
                else
                {
                    errorProvider.Clear();
                    return true;
                }
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerEmail(TextBox text, ErrorProvider errorProvider)
        {
            var regex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer email is required.");
                return false;
            }
            else if (!Regex.IsMatch(text.Text, regex))
            {
                errorProvider.SetError(text, "Invalid email format.");
                return false;
            }
            else
            {
                errorProvider.Clear();
                return true;
            }
        }

        public static bool ValidateCustomerEmailInboxPath(TextBox text, ErrorProvider errorProvider)
        {
            if (string.IsNullOrWhiteSpace(text.Text))
            {
                errorProvider.SetError(text, "Customer email inbox path is required.");
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