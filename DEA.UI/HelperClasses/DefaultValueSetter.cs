using DEA.UI.Forms;

namespace DEA.UI.HelperClasses
{
    internal class DefaultValueSetter
    {
        public static void SetDefaultValues(Form form)
        {
            switch (form)
            {
                case AddCustomers addCustomersForm:
                    SetDefaultValuesAddCustomers(addCustomersForm);
                    break;
                case EditCustomersList editCustomersListForm:
                    SetDefaultValuesEditCustomersList(editCustomersListForm);
                    break;
                case EditCustomerForm editCustomerForm:
                    SetDefaultValuesEditCustomersForm(editCustomerForm);
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

            // Setting default values for the radio buttons
            form.cusOn.Checked = true;
            form.ftpLoopOff.Checked = true;
            form.ftpMoveToSubOff.Checked = true;
            form.ftpRemoveOn.Checked = true;
            form.emlSenAdressOff.Checked = true;
            form.emlSndSubjectOff.Checked = true;
            form.emlSndBodyOff.Checked = true;
        }

        private static void SetDefaultValuesEditCustomersList(EditCustomersList form)
        {
            // Setting default values for the radio buttons
            form.searchCusId.Checked = true;
        }

        private static void SetDefaultValuesEditCustomersForm(EditCustomerForm form)
        {
            // Setting the default value for the FTP profile combo box
            form.ftpProfileComboEdFrm.SelectedIndex = 0;

            // Setting default values for the radio buttons
            form.cusOnEdFrm.Checked = true;
            form.ftpLoopOffEdFrm.Checked = true;
            form.ftpMoveToSubOffEdFrm.Checked = true;
            form.ftpRemoveOnEdFrm.Checked = true;
            form.emlSenAdressOffEdFrm.Checked = true;
            form.emlSndSubjectOffEdFrm.Checked = true;
        }

        private static void SetDefaultValuesRemoveCustomers(RemoveCustomers form)
        {
            // Setting default values for the radio buttons
            form.rmSearchId.Checked = true;
        }
    }
}
