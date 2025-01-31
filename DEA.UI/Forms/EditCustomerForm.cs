using DEA.Next.Data;
using DEA.UI.HelperClasses;

namespace DEA.UI.Forms
{
    public partial class EditCustomerForm : Form
    {
        private readonly DataContext _conttext;
        private readonly ToolTipHelper _toolTipHelper;
        private readonly DefaultValueSetter _defaultValueSetter;

        public EditCustomerForm(DataContext context)
        {
            InitializeComponent();

            // Initializing the context
            _conttext = context;
            _toolTipHelper = new ToolTipHelper();
            _defaultValueSetter = new DefaultValueSetter();

            // Initialize the controls
            InitializeControls();
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
            _toolTipHelper.SetToolTip(btnCancelEdFrm, "Close the form without saving.");
            _toolTipHelper.SetToolTip(btnResetEdFrm, "Reset the form.");
            _toolTipHelper.SetToolTip(btnSaveEdFrm, "Save the form.");
        }
    }
}
