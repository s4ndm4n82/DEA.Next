using DEA.Next.Data;
using DEA.UI.HelperClasses;

namespace DEA.UI
{
    public partial class AddCustomers : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;
        private readonly ErrorProvider _errorProvider;
        private readonly FormFunctionHelper _formFunctionHelper;

        public AddCustomers(DataContext context)
        {
            InitializeComponent();
            _conttext = context;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();
            _errorProvider = new ErrorProvider();
            _formFunctionHelper = new FormFunctionHelper();

            // Initialize the controls
            InitializeControls();

            // Button click events
            btnSave.Click += BtnSave_Click;

            // Handles the item events
            cusDocExtList.ItemCheck += _formFunctionHelper.CheckBoxListHandler;
        }

        private void InitializeControls()
        {
            // Set the default values
            DefaultValueSetter.SetDefaultValues(this);

            // Initializing tool tips
            InitalizeToolTips();
        }

        private void InitalizeToolTips()
        {
            _toolTipHelper.SetToolTip(cusStatusGrp, "Set the customer status.");
            _toolTipHelper.SetToolTip(cusOn, "Enable the customer configuration.");
            _toolTipHelper.SetToolTip(cusOff, "Disable the customer configuration.");
            _toolTipHelper.SetToolTip(cusNameTxt, "Set the customer name.");
            _toolTipHelper.SetToolTip(cusUnameTxt, "Set the customer TPS user name.");
            _toolTipHelper.SetToolTip(cusApiTokenTxt, "Set the customer API token.");
            _toolTipHelper.SetToolTip(cusQueuTxt, "Set the customer queue number.");
            _toolTipHelper.SetToolTip(cusProjIdTxt, "Set the customer project ID.");
            _toolTipHelper.SetToolTip(cusTempIdTxt, "Set the customer template Key.");
            _toolTipHelper.SetToolTip(cusDocIdTxt, "Set the customer document ID.");
            _toolTipHelper.SetToolTip(cusMaxBatchTxt, "Set the customer maximum batch size.");
            _toolTipHelper.SetToolTip(cusDocencTxt, "Set the customer document encoding.");
            _toolTipHelper.SetToolTip(cusFonNameTxt, "Set the customer first default field name.");
            _toolTipHelper.SetToolTip(cusFonValTxt, "Set the customer first default field value.");
            _toolTipHelper.SetToolTip(custFtwoNameTxt, "Set the customer second default field name.");
            _toolTipHelper.SetToolTip(custFtwoValTxt, "Set the customer second default field value.");
            _toolTipHelper.SetToolTip(cusDelMethodCombo, "Set the customer file delivery method.");
            _toolTipHelper.SetToolTip(cusDocExtList, "Set the customer document extension list.");
            _toolTipHelper.SetToolTip(ftpTypCombo, "Set the customer FTP type.");
            _toolTipHelper.SetToolTip(ftpProfileCombo, "Set the customer FTP profile.");
            _toolTipHelper.SetToolTip(ftpHostTxt, "Set the customer FTP host.");
            _toolTipHelper.SetToolTip(ftpUserNameTxt, "Set the customer FTP user.");
            _toolTipHelper.SetToolTip(ftpPasswordTxt, "Set the customer FTP password.");
            _toolTipHelper.SetToolTip(ftpPortTxt, "Set the customer FTP port.");
            _toolTipHelper.SetToolTip(ftpMainPathTxt, "Set the customer main FTP directory.");
            _toolTipHelper.SetToolTip(ftpLoopOn, "Enable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpLoopOff, "Disable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpMoveToSubOn, "Enable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpMoveToSubOff, "Disable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpSubPathTxt, "Set the customer FTP sub directory.");
            _toolTipHelper.SetToolTip(ftpRemoveOn, "Enable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(ftpRemoveOff, "Disable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(emlAddressTxt, "Set the customer email address.");
            _toolTipHelper.SetToolTip(emlInboxPathTxt, "Set the customer email inbox path.");
            _toolTipHelper.SetToolTip(emlSenAdressOn, "Enable the sending the email address.");
            _toolTipHelper.SetToolTip(emlSenAdressOff, "Disable the sending the email address.");
            _toolTipHelper.SetToolTip(emlSndSubjectOn, "Enable the sending the email subject.");
            _toolTipHelper.SetToolTip(emlSndSubjectOff, "Disable the sending the email subject.");
            _toolTipHelper.SetToolTip(btnCancel, "Close the form without saving.");
            _toolTipHelper.SetToolTip(btnReset, "Reset the form.");
            _toolTipHelper.SetToolTip(btnSave, "Save the form.");
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (sender is not Button) return;

            if (ValidateInputs())
            {
                // Save the customer
                SaveCustomer();
            }
        }

        private bool ValidateInputs()
        {
            var isValid = true;

            // Check if the required fields are filled
            isValid &= FormValidator.ValidateCustomerName(cusNameTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerUserName(cusUnameTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerApiToken(cusApiTokenTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerQueue(cusQueuTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerMaxBatch(cusMaxBatchTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerDocumentEncoding(cusDocencTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerDomain(cusDomainTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerDeliveryMethod(cusDelMethodCombo, _errorProvider);
            isValid &= FormValidator.ValidateCustomerExtensions(cusDocExtList, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpType(ftpTypCombo, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpProfile(ftpProfileCombo, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpHost(ftpHostTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpUser(ftpUserNameTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpPassword(ftpPasswordTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpPort(ftpPortTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpMainPath(ftpMainPathTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerFtpSubPath(ftpSubPathTxt, ftpMoveToSubOn, _errorProvider);
            isValid &= FormValidator.ValidateCustomerEmail(emlAddressTxt, _errorProvider);
            isValid &= FormValidator.ValidateCustomerEmailInboxPath(emlInboxPathTxt, _errorProvider);

            return isValid;
        }

        private void SaveCustomer()
        {
            // Add the save method here.
        }
    }
}
