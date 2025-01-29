using DEA.Next.Data;
using DEA.UI.HelperClasses;

namespace DEA.UI
{
    public partial class EditCustomers : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;

        public EditCustomers(DataContext context)
        {
            InitializeComponent();

            // Initalizing the context
            _conttext = context;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();

            // Initialize the controls
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Set the default values
            _defaultValueSetter.SetDefaultValues(this);

            // Initilizing tool tips
            InitalizeToolTips();
        }

        private void InitalizeToolTips()
        {
            _toolTipHelper.SetToolTip(searchCusId, "Use the customer ID to search.");
            _toolTipHelper.SetToolTip(searchProjectId, "Use the Project ID to search.");
            _toolTipHelper.SetToolTip(searchCusName, "Use the customer name to search.");
            _toolTipHelper.SetToolTip(cusEditSearchTxt, "Enter search text.");
            _toolTipHelper.SetToolTip(btnEditCustomerSearch, "Search for a customer.");
            _toolTipHelper.SetToolTip(cusEditStatusGrp, "Set the customer status.");
            _toolTipHelper.SetToolTip(cusEditOn, "Enable the cutomer configuration.");
            _toolTipHelper.SetToolTip(cusEditOff, "Disable the cutomer configuration.");
            _toolTipHelper.SetToolTip(cusEditNameTxt, "Set the customer name.");
            _toolTipHelper.SetToolTip(cusEditUnameTxt, "Set the customer TPS username.");
            _toolTipHelper.SetToolTip(cusEditApiTokenTxt, "Set the customer API token.");
            _toolTipHelper.SetToolTip(cusEditQueuTxt, "Set the customer queue number.");
            _toolTipHelper.SetToolTip(cusEditProjIdTxt, "Set the customer project ID.");
            _toolTipHelper.SetToolTip(cusEditTempIdTxt, "Set the customer template Key.");
            _toolTipHelper.SetToolTip(cusEditDocIdTxt, "Set the customer document ID.");
            _toolTipHelper.SetToolTip(cusEditMaxBatchTxt, "Set the customer maximum batch size.");
            _toolTipHelper.SetToolTip(cusEditDocencTxt, "Set the customer document encoding.");
            _toolTipHelper.SetToolTip(cusEditFonNameTxt, "Set the customer first default field name.");
            _toolTipHelper.SetToolTip(cusEditFonValTxt, "Set the customer first default field value.");
            _toolTipHelper.SetToolTip(custEditFtwoNameTxt, "Set the customer second default field name.");
            _toolTipHelper.SetToolTip(custEditFtwoValTxt, "Set the customer second default field value.");
            _toolTipHelper.SetToolTip(cusEditDelMethodCombo, "Set the customer file delivery method.");
            _toolTipHelper.SetToolTip(cusEditDocExtList, "Set the customer document extension list.");
            _toolTipHelper.SetToolTip(ftpEditTypCombo, "Set the customer FTP type.");
            _toolTipHelper.SetToolTip(ftpEditProfileCombo, "Set the customer FTP profile.");
            _toolTipHelper.SetToolTip(ftpEditHostTxt, "Set the customer FTP host.");
            _toolTipHelper.SetToolTip(ftpEditUserNameTxt, "Set the customer FTP user.");
            _toolTipHelper.SetToolTip(ftpEditPasswordTxt, "Set the customer FTP password.");
            _toolTipHelper.SetToolTip(ftpEditPortTxt, "Set the customer FTP port.");
            _toolTipHelper.SetToolTip(ftpEditMainPathTxt, "Set the customer main FTP directory.");
            _toolTipHelper.SetToolTip(ftpEditLoopOn, "Enable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpEditLoopOff, "Disable the customer FTP loop.");
            _toolTipHelper.SetToolTip(ftpEditMoveToSubOn, "Enable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpEditMoveToSubOff, "Disable the customer FTP move to sub directory.");
            _toolTipHelper.SetToolTip(ftpEditSubPathTxt, "Set the customer FTP sub directory.");
            _toolTipHelper.SetToolTip(ftpEditRemoveOn, "Enable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(ftpEditRemoveOff, "Disable the customer FTP remove file.");
            _toolTipHelper.SetToolTip(emlEditAddressTxt, "Set the customer email address.");
            _toolTipHelper.SetToolTip(emlEditInboxPathTxt, "Set the customer email inbox path.");
            _toolTipHelper.SetToolTip(emlEditSenAdressOn, "Enable the sending the email address.");
            _toolTipHelper.SetToolTip(emlEditSenAdressOff, "Disable the sending the email address.");
            _toolTipHelper.SetToolTip(emlEditSndSubjectOn, "Enable the sending the email subject.");
            _toolTipHelper.SetToolTip(emlEditSndSubjectOff, "Disable the sending the email subject.");
            _toolTipHelper.SetToolTip(btnEditCancel, "Close the form without saving.");
            _toolTipHelper.SetToolTip(btnEditReset, "Reset the form.");
            _toolTipHelper.SetToolTip(btnEditSave, "Save the form.");
        }
    }
}
