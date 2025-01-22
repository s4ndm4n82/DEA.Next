namespace DEA.UI.HelperClasses
{
    internal class DefaultValueSetter
    {
        public void SetDefaultValues(Form form)
        {
            switch (form)
            {
                case AddCustomers addCustomersForm:
                    SetDefaultValuesAddCustomers(addCustomersForm);
                    break;
                case EditCustomers editCustomersForm:
                    SetDefaultValuesEditCustomers(editCustomersForm);
                    break;
                case RemoveCustomers removeCustomersForm:
                    SetDefaultValuesRemoveCustomers(removeCustomersForm);
                    break;
                default:
                    throw new ArgumentException("Unsupported form type", nameof(form));
            }
        }

        private static void SetDefaultValuesAddCustomers(AddCustomers form)
        {
            // Setting the default value for the FTP profile combo box
            form.ftpProfileCombo.SelectedIndex = 0;

            // Setting default values for the checkboxes
            form.cusOn.Checked = true;
            form.ftpLoopOff.Checked = true;
            form.ftpMoveToSubOff.Checked = true;
            form.ftpRemoveOn.Checked = true;
            form.emlSenAdressOff.Checked = true;
            form.emlSndSubjectOff.Checked = true;
        }

        private static void SetDefaultValuesEditCustomers(EditCustomers form)
        {
            // Setting the default value for the FTP profile combo box
            form.ftpEditProfileCombo.SelectedIndex = 0;

            // Setting default values for the checkboxes
            form.cusEditOn.Checked = true;
            form.ftpEditLoopOff.Checked = true;
            form.ftpEditMoveToSubOff.Checked = true;
            form.ftpEditRemoveOn.Checked = true;
            form.emlEditSenAdressOff.Checked = true;
            form.emlEditSndSubjectOff.Checked = true;
        }

        private static void SetDefaultValuesRemoveCustomers(RemoveCustomers form)
        {
            // Add logic for default values for RemoveCustomers
        }
    }
}
