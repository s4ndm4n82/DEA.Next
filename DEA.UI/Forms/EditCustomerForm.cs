using DEA.Next.Data;
using DEA.Next.Entities;
using DEA.UI.HelperClasses;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Versioning;

namespace DEA.UI.Forms
{
    [SupportedOSPlatform("windows")]
    public partial class EditCustomerForm : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;
        private readonly UpdateCustomerDetails _updateCustomerDetails;
        private readonly FormFunctionHelper _formFunctionHelper;
        private readonly EditCustomersList _editCustomersList;
        private readonly Guid _customerId;
        private CustomerDetails? _customerDetails;

        public EditCustomerForm(DataContext context, Guid customerId, EditCustomersList editCustomersList)
        {
            InitializeComponent();

            // Initializing the context
            _conttext = context;
            _customerId = customerId;
            _editCustomersList = editCustomersList;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();
            _formFunctionHelper = new FormFunctionHelper();
            _updateCustomerDetails = new UpdateCustomerDetails(context);

            // Initialize the controls
            InitializeControls();

            // Load the customer details
            LoadCustomerData(customerId);

            // Handle FTP move to subfolder option change
            ftpMoveToSubOnEdFrm.CheckedChanged += (sender, e) =>
                FormFunctionHelper.HandleFtpSubPathChanges(ftpMoveToSubOnEdFrm, ftpSubPathEdFrmTxt);

            ftpMoveToSubOffEdFrm.CheckedChanged += (sender, e) =>
                FormFunctionHelper.HandleFtpSubPathChanges(ftpMoveToSubOnEdFrm, ftpSubPathEdFrmTxt);

            // Handles the item events
            cusDocExtListEdFrm.ItemCheck += _formFunctionHelper.CheckBoxListHandler;
        }

        private void InitializeControls()
        {
            // Set the default values
            DefaultValueSetter.SetDefaultValues(this);

            // Set diabled fields on load
            cusStatusEdFrmGrp.Enabled = false;

            // Initializing tool tips
            InitalizeToolTips();

            // Register the SelectedIndexChanged event for the delivery method combo box
            cusDelMethodEdFrmCombo.SelectedIndexChanged += CusDelMethodEdFrmCombo_SelectedIndexChanged;

            // Register the CheckedChanged event for the save button
            btnSaveEdFrm.Click += BtnSaveEdFrm_CheckedChanged;

            // Register the click event for the reset button
            btnResetEdFrm.Click += BtnResetEdFrm_Click;

            // Register the click event for the cancel button
            btnCancelEdFrm.Click += BtnCancelEdFrm_Click;
        }

        private void LoadCustomerData(Guid customerId)
        {
            // Load the customer details along with the related entities
            _customerDetails = _conttext.CustomerDetails
                .Include(cd => cd.FtpDetails)
                .Include(cd => cd.EmailDetails)
                .Include(cd => cd.DocumentDetails)
                .FirstOrDefault(cd => cd.Id == customerId)
                ?? throw new InvalidOperationException("Customer not found.");

            if (_customerDetails != null)
            {
                // Bind the customer details to the form
                BindDataToForms.BindEditCustomerFormData(this, _customerDetails);
                FormFunctionHelper.ToggleDetailsFields(ftpDetailsEdFrm,
                    emlDetailsEdFrmGrp,
                    ftpSubPathEdFrmTxt,
                    _customerDetails.FileDeliveryMethod);
            }
        }

        private void CusDelMethodEdFrmCombo_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cusDelMethodEdFrmCombo.SelectedItem is string selectedMethod)
            {
                FormFunctionHelper.ToggleDetailsFields(ftpDetailsEdFrm,
                    emlDetailsEdFrmGrp,
                    ftpSubPathEdFrmTxt,
                    selectedMethod);
            }
        }

        private void BtnSaveEdFrm_CheckedChanged(object? sender, EventArgs e)
        {
            var result = _updateCustomerDetails.UpdateCustomerData(this, _customerDetails);

            if (result)
            {
                // Refresh the grid
                _editCustomersList.LoadCustomerData();

                // Close the form
                this.Close();
            }
        }

        private void BtnResetEdFrm_Click(object? sender, EventArgs e)
        {
            // Reload the customer data
            LoadCustomerData(_customerId);
        }

        private void BtnCancelEdFrm_Click(object? sender, EventArgs e)
        {
            // Close the form
            this.Close();
        }
        private void InitalizeToolTips()
        {
            _toolTipHelper.SetToolTip(cusStatusEdFrmGrp, "Set the customer status.");
            _toolTipHelper.SetToolTip(cusOnEdFrm, "Enable the customer configuration.");
            _toolTipHelper.SetToolTip(cusOffEdFrm, "Disable the customer configuration.");
            _toolTipHelper.SetToolTip(cusNameEdFrmTxt, "Set the customer name.");
            _toolTipHelper.SetToolTip(cusUnameEdFrmTxt, "Set the customer TPS user name.");
            _toolTipHelper.SetToolTip(cusApiTokenEdFrmTxt, "Set the customer API token.");
            _toolTipHelper.SetToolTip(cusQueuEdFrmTxt, "Set the customer queue number.");
            _toolTipHelper.SetToolTip(cusProjIdEdFrmTxt, "Set the customer project ID.");
            _toolTipHelper.SetToolTip(cusTempIdEdFrmTxt, "Set the customer template Key.");
            _toolTipHelper.SetToolTip(cusDocIdEdFrmTxt, "Set the customer document ID.");
            _toolTipHelper.SetToolTip(cusMaxBatchEdFrmTxt, "Set the customer maximum batch size.");
            _toolTipHelper.SetToolTip(cusDocencEdFrmTxt, "Set the customer document encoding.");
            _toolTipHelper.SetToolTip(cusFonNameEdFrmTxt, "Set the customer first default field name.");
            _toolTipHelper.SetToolTip(cusFonValEdFrmTxt, "Set the customer first default field value.");
            _toolTipHelper.SetToolTip(custFtwoNameEdFrmTxt, "Set the customer second default field name.");
            _toolTipHelper.SetToolTip(custFtwoValEdFrmTxt, "Set the customer second default field value.");
            _toolTipHelper.SetToolTip(cusDelMethodEdFrmCombo, "Set the customer file delivery method.");
            _toolTipHelper.SetToolTip(cusDocExtListEdFrm, "Set the customer document extension list.");
            _toolTipHelper.SetToolTip(ftpTypComboEdFrm, "Set the customer FTP type.");
            _toolTipHelper.SetToolTip(ftpProfileComboEdFrm, "Set the customer FTP profile.");
            _toolTipHelper.SetToolTip(ftpHostEdFrmTxt, "Set the customer FTP host.");
            _toolTipHelper.SetToolTip(ftpUserNameEdFrmTxt, "Set the customer FTP user.");
            _toolTipHelper.SetToolTip(ftpPasswordEdFrmTxt, "Set the customer FTP password.");
            _toolTipHelper.SetToolTip(ftpPortEdFrmTxt, "Set the customer FTP port.");
            _toolTipHelper.SetToolTip(ftpMainPathEdFrmTxt, "Set the customer main FTP directory.");
            _toolTipHelper.SetToolTip(ftpLoopOnEdFrm, "Enable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpLoopOffEdFrm, "Disable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpMoveToSubOnEdFrm, "Enable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpMoveToSubOffEdFrm, "Disable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpSubPathEdFrmTxt, "Set the customer FTP sub directory.");
            _toolTipHelper.SetToolTip(ftpRemoveOnEdFrm, "Enable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(ftpRemoveOffEdFrm, "Disable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(emlAddressEdFrmTxt, "Set the customer email address.");
            _toolTipHelper.SetToolTip(emlInboxPathEdFrmTxt, "Set the customer email inbox path.");
            _toolTipHelper.SetToolTip(emlSenAdressOnEdFrm, "Enable the sending the email address.");
            _toolTipHelper.SetToolTip(emlSenAdressOffEdFrm, "Disable the sending the email address.");
            _toolTipHelper.SetToolTip(emlSndSubjectOnEdFrm, "Enable the sending the email subject.");
            _toolTipHelper.SetToolTip(emlSndSubjectOffEdFrm, "Disable the sending the email subject.");
            _toolTipHelper.SetToolTip(emlSndBodyOnEdFrm, "Enable the sending the email body.");
            _toolTipHelper.SetToolTip(emlSndBodyOffEdFrm, "Disable the sending the email body.");
            _toolTipHelper.SetToolTip(btnCancelEdFrm, "Close the form without saving.");
            _toolTipHelper.SetToolTip(btnResetEdFrm, "Reset the form.");
            _toolTipHelper.SetToolTip(btnSaveEdFrm, "Save the form.");
        }
    }
}
