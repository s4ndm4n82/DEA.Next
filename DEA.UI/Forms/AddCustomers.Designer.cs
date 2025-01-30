namespace DEA.UI
{
    partial class AddCustomers
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            cusOn = new RadioButton();
            cusOff = new RadioButton();
            cusNameLbl = new Label();
            cusNameTxt = new TextBox();
            cusUnameLbl = new Label();
            grpBoxCustomer = new GroupBox();
            custFtwoValTxt = new TextBox();
            cusFtwoValLbl = new Label();
            cusStatusGrp = new GroupBox();
            cusDelMethodCombo = new ComboBox();
            cusDelMethodLbl = new Label();
            cusDomainTxt = new TextBox();
            cusDomainLbl = new Label();
            cusDocExtList = new CheckedListBox();
            cusDocExtensionsLbl = new Label();
            custFtwoNameTxt = new TextBox();
            cusFtwoNameLbl = new Label();
            cusFonValTxt = new TextBox();
            cusFoneValLbl = new Label();
            cusFonNameTxt = new TextBox();
            cusFoneNameLbl = new Label();
            cusDocencTxt = new TextBox();
            cusDocEncoLbl = new Label();
            cusMaxBatchTxt = new TextBox();
            cusMaxBatchLbl = new Label();
            cusDocIdTxt = new TextBox();
            cusDocIdLbl = new Label();
            cusTempIdTxt = new TextBox();
            cusTempKeyLbl = new Label();
            cusProjIdTxt = new TextBox();
            cusProjectLbl = new Label();
            cusQueuTxt = new TextBox();
            cusQueueLbl = new Label();
            cusApiTokenTxt = new TextBox();
            cusTokeLbl = new Label();
            cusUnameTxt = new TextBox();
            ftpDetails = new GroupBox();
            ftpRemoveFileCombo = new GroupBox();
            ftpRemoveOff = new RadioButton();
            ftpRemoveOn = new RadioButton();
            ftpSubPathTxt = new TextBox();
            ftpSubPathLbl = new Label();
            ftpMoveToSubGrp = new GroupBox();
            ftpMoveToSubOn = new RadioButton();
            ftpMoveToSubOff = new RadioButton();
            ftpLoopGrp = new GroupBox();
            ftpLoopOff = new RadioButton();
            ftpLoopOn = new RadioButton();
            ftpMainPathTxt = new TextBox();
            ftpMainPathLbl = new Label();
            ftpPortTxt = new TextBox();
            ftpPortLbl = new Label();
            ftpPasswordTxt = new MaskedTextBox();
            ftpPasswordLbl = new Label();
            ftpUserNameTxt = new TextBox();
            ftpUserNameLbl = new Label();
            ftpHostTxt = new TextBox();
            ftpHostlbl = new Label();
            ftpProfileCombo = new ComboBox();
            ftpProfileLbl = new Label();
            ftpTypCombo = new ComboBox();
            ftpTypeLbl = new Label();
            emlDetailsGrp = new GroupBox();
            emlSendSubjectGrp = new GroupBox();
            emlSndSubjectOff = new RadioButton();
            emlSndSubjectOn = new RadioButton();
            emlSendEmailGrp = new GroupBox();
            emlSenAdressOff = new RadioButton();
            emlSenAdressOn = new RadioButton();
            emlInboxPathTxt = new TextBox();
            emlInboxPathLbl = new Label();
            emlAddressTxt = new TextBox();
            emlEmailLbl = new Label();
            cmdBoxGrp = new GroupBox();
            btnSave = new Button();
            btnReset = new Button();
            btnCancel = new Button();
            toolTipGlobal = new ToolTip(components);
            grpBoxCustomer.SuspendLayout();
            cusStatusGrp.SuspendLayout();
            ftpDetails.SuspendLayout();
            ftpRemoveFileCombo.SuspendLayout();
            ftpMoveToSubGrp.SuspendLayout();
            ftpLoopGrp.SuspendLayout();
            emlDetailsGrp.SuspendLayout();
            emlSendSubjectGrp.SuspendLayout();
            emlSendEmailGrp.SuspendLayout();
            cmdBoxGrp.SuspendLayout();
            SuspendLayout();
            // 
            // cusOn
            // 
            cusOn.AutoSize = true;
            cusOn.Location = new Point(6, 13);
            cusOn.Name = "cusOn";
            cusOn.Size = new Size(67, 19);
            cusOn.TabIndex = 1;
            cusOn.TabStop = true;
            cusOn.Text = "Enabled";
            cusOn.UseVisualStyleBackColor = true;
            // 
            // cusOff
            // 
            cusOff.AutoSize = true;
            cusOff.Location = new Point(79, 13);
            cusOff.Name = "cusOff";
            cusOff.Size = new Size(70, 19);
            cusOff.TabIndex = 2;
            cusOff.TabStop = true;
            cusOff.Text = "Disabled";
            cusOff.UseVisualStyleBackColor = true;
            // 
            // cusNameLbl
            // 
            cusNameLbl.AutoSize = true;
            cusNameLbl.Location = new Point(6, 58);
            cusNameLbl.Name = "cusNameLbl";
            cusNameLbl.Size = new Size(97, 15);
            cusNameLbl.TabIndex = 3;
            cusNameLbl.Text = "Customer Name:";
            // 
            // cusNameTxt
            // 
            cusNameTxt.Location = new Point(6, 76);
            cusNameTxt.Name = "cusNameTxt";
            cusNameTxt.Size = new Size(280, 23);
            cusNameTxt.TabIndex = 4;
            // 
            // cusUnameLbl
            // 
            cusUnameLbl.AutoSize = true;
            cusUnameLbl.Location = new Point(6, 102);
            cusUnameLbl.Name = "cusUnameLbl";
            cusUnameLbl.Size = new Size(123, 15);
            cusUnameLbl.TabIndex = 5;
            cusUnameLbl.Text = "Customer User Name:";
            // 
            // grpBoxCustomer
            // 
            grpBoxCustomer.Controls.Add(custFtwoValTxt);
            grpBoxCustomer.Controls.Add(cusFtwoValLbl);
            grpBoxCustomer.Controls.Add(cusStatusGrp);
            grpBoxCustomer.Controls.Add(cusDelMethodCombo);
            grpBoxCustomer.Controls.Add(cusDelMethodLbl);
            grpBoxCustomer.Controls.Add(cusDomainTxt);
            grpBoxCustomer.Controls.Add(cusDomainLbl);
            grpBoxCustomer.Controls.Add(cusDocExtList);
            grpBoxCustomer.Controls.Add(cusDocExtensionsLbl);
            grpBoxCustomer.Controls.Add(custFtwoNameTxt);
            grpBoxCustomer.Controls.Add(cusFtwoNameLbl);
            grpBoxCustomer.Controls.Add(cusFonValTxt);
            grpBoxCustomer.Controls.Add(cusFoneValLbl);
            grpBoxCustomer.Controls.Add(cusFonNameTxt);
            grpBoxCustomer.Controls.Add(cusFoneNameLbl);
            grpBoxCustomer.Controls.Add(cusDocencTxt);
            grpBoxCustomer.Controls.Add(cusDocEncoLbl);
            grpBoxCustomer.Controls.Add(cusMaxBatchTxt);
            grpBoxCustomer.Controls.Add(cusMaxBatchLbl);
            grpBoxCustomer.Controls.Add(cusDocIdTxt);
            grpBoxCustomer.Controls.Add(cusDocIdLbl);
            grpBoxCustomer.Controls.Add(cusTempIdTxt);
            grpBoxCustomer.Controls.Add(cusTempKeyLbl);
            grpBoxCustomer.Controls.Add(cusProjIdTxt);
            grpBoxCustomer.Controls.Add(cusProjectLbl);
            grpBoxCustomer.Controls.Add(cusQueuTxt);
            grpBoxCustomer.Controls.Add(cusQueueLbl);
            grpBoxCustomer.Controls.Add(cusApiTokenTxt);
            grpBoxCustomer.Controls.Add(cusTokeLbl);
            grpBoxCustomer.Controls.Add(cusUnameTxt);
            grpBoxCustomer.Controls.Add(cusUnameLbl);
            grpBoxCustomer.Controls.Add(cusNameTxt);
            grpBoxCustomer.Controls.Add(cusNameLbl);
            grpBoxCustomer.Location = new Point(12, 12);
            grpBoxCustomer.Name = "grpBoxCustomer";
            grpBoxCustomer.Size = new Size(603, 552);
            grpBoxCustomer.TabIndex = 6;
            grpBoxCustomer.TabStop = false;
            grpBoxCustomer.Text = "Customer Details";
            // 
            // custFtwoValTxt
            // 
            custFtwoValTxt.Location = new Point(315, 172);
            custFtwoValTxt.Name = "custFtwoValTxt";
            custFtwoValTxt.Size = new Size(280, 23);
            custFtwoValTxt.TabIndex = 34;
            // 
            // cusFtwoValLbl
            // 
            cusFtwoValLbl.AutoSize = true;
            cusFtwoValLbl.Location = new Point(315, 154);
            cusFtwoValLbl.Name = "cusFtwoValLbl";
            cusFtwoValLbl.Size = new Size(145, 15);
            cusFtwoValLbl.TabIndex = 33;
            cusFtwoValLbl.Text = "Customer Field Two Value:";
            // 
            // cusStatusGrp
            // 
            cusStatusGrp.Controls.Add(cusOn);
            cusStatusGrp.Controls.Add(cusOff);
            cusStatusGrp.Location = new Point(6, 19);
            cusStatusGrp.Name = "cusStatusGrp";
            cusStatusGrp.Size = new Size(174, 38);
            cusStatusGrp.TabIndex = 9;
            cusStatusGrp.TabStop = false;
            cusStatusGrp.Text = "Customer Status:";
            // 
            // cusDelMethodCombo
            // 
            cusDelMethodCombo.FormattingEnabled = true;
            cusDelMethodCombo.Items.AddRange(new object[] { "FTP", "EMAIL" });
            cusDelMethodCombo.Location = new Point(315, 260);
            cusDelMethodCombo.Name = "cusDelMethodCombo";
            cusDelMethodCombo.Size = new Size(179, 23);
            cusDelMethodCombo.TabIndex = 32;
            cusDelMethodCombo.Text = "Select Delivery Method ...";
            // 
            // cusDelMethodLbl
            // 
            cusDelMethodLbl.AutoSize = true;
            cusDelMethodLbl.Location = new Point(315, 242);
            cusDelMethodLbl.Name = "cusDelMethodLbl";
            cusDelMethodLbl.Size = new Size(211, 15);
            cusDelMethodLbl.TabIndex = 31;
            cusDelMethodLbl.Text = "Customer Document Delivery Method:";
            // 
            // cusDomainTxt
            // 
            cusDomainTxt.Location = new Point(315, 216);
            cusDomainTxt.Name = "cusDomainTxt";
            cusDomainTxt.Size = new Size(280, 23);
            cusDomainTxt.TabIndex = 30;
            // 
            // cusDomainLbl
            // 
            cusDomainLbl.AutoSize = true;
            cusDomainLbl.Location = new Point(315, 198);
            cusDomainLbl.Name = "cusDomainLbl";
            cusDomainLbl.Size = new Size(128, 15);
            cusDomainLbl.TabIndex = 29;
            cusDomainLbl.Text = "Customer Api Domain:";
            // 
            // cusDocExtList
            // 
            cusDocExtList.FormattingEnabled = true;
            cusDocExtList.Items.AddRange(new object[] { "Select All", "Pdf", "Jpg", "Jpeg", "Tif", "Tiff", "Csv" });
            cusDocExtList.Location = new Point(315, 304);
            cusDocExtList.Name = "cusDocExtList";
            cusDocExtList.Size = new Size(179, 130);
            cusDocExtList.TabIndex = 28;
            // 
            // cusDocExtensionsLbl
            // 
            cusDocExtensionsLbl.AutoSize = true;
            cusDocExtensionsLbl.Location = new Point(315, 286);
            cusDocExtensionsLbl.Name = "cusDocExtensionsLbl";
            cusDocExtensionsLbl.Size = new Size(179, 15);
            cusDocExtensionsLbl.TabIndex = 27;
            cusDocExtensionsLbl.Text = "Customer Document Extentions:";
            // 
            // custFtwoNameTxt
            // 
            custFtwoNameTxt.Location = new Point(315, 125);
            custFtwoNameTxt.Name = "custFtwoNameTxt";
            custFtwoNameTxt.Size = new Size(280, 23);
            custFtwoNameTxt.TabIndex = 26;
            // 
            // cusFtwoNameLbl
            // 
            cusFtwoNameLbl.AutoSize = true;
            cusFtwoNameLbl.Location = new Point(315, 107);
            cusFtwoNameLbl.Name = "cusFtwoNameLbl";
            cusFtwoNameLbl.Size = new Size(149, 15);
            cusFtwoNameLbl.TabIndex = 25;
            cusFtwoNameLbl.Text = "Customer Field Two Name:";
            // 
            // cusFonValTxt
            // 
            cusFonValTxt.Location = new Point(315, 81);
            cusFonValTxt.Name = "cusFonValTxt";
            cusFonValTxt.Size = new Size(280, 23);
            cusFonValTxt.TabIndex = 24;
            // 
            // cusFoneValLbl
            // 
            cusFoneValLbl.AutoSize = true;
            cusFoneValLbl.Location = new Point(315, 63);
            cusFoneValLbl.Name = "cusFoneValLbl";
            cusFoneValLbl.Size = new Size(146, 15);
            cusFoneValLbl.TabIndex = 23;
            cusFoneValLbl.Text = "Customer Field One Value:";
            // 
            // cusFonNameTxt
            // 
            cusFonNameTxt.Location = new Point(315, 37);
            cusFonNameTxt.Name = "cusFonNameTxt";
            cusFonNameTxt.Size = new Size(280, 23);
            cusFonNameTxt.TabIndex = 22;
            // 
            // cusFoneNameLbl
            // 
            cusFoneNameLbl.AutoSize = true;
            cusFoneNameLbl.Location = new Point(315, 19);
            cusFoneNameLbl.Name = "cusFoneNameLbl";
            cusFoneNameLbl.Size = new Size(150, 15);
            cusFoneNameLbl.TabIndex = 21;
            cusFoneNameLbl.Text = "Customer Field One Name:";
            // 
            // cusDocencTxt
            // 
            cusDocencTxt.Location = new Point(6, 519);
            cusDocencTxt.Name = "cusDocencTxt";
            cusDocencTxt.Size = new Size(123, 23);
            cusDocencTxt.TabIndex = 20;
            cusDocencTxt.Text = "UTF-8";
            // 
            // cusDocEncoLbl
            // 
            cusDocEncoLbl.AutoSize = true;
            cusDocEncoLbl.Location = new Point(6, 501);
            cusDocEncoLbl.Name = "cusDocEncoLbl";
            cusDocEncoLbl.Size = new Size(174, 15);
            cusDocEncoLbl.TabIndex = 19;
            cusDocEncoLbl.Text = "Customer Document Encoding:";
            // 
            // cusMaxBatchTxt
            // 
            cusMaxBatchTxt.Location = new Point(6, 475);
            cusMaxBatchTxt.Name = "cusMaxBatchTxt";
            cusMaxBatchTxt.Size = new Size(123, 23);
            cusMaxBatchTxt.TabIndex = 18;
            cusMaxBatchTxt.Text = "1";
            // 
            // cusMaxBatchLbl
            // 
            cusMaxBatchLbl.AutoSize = true;
            cusMaxBatchLbl.Location = new Point(6, 457);
            cusMaxBatchLbl.Name = "cusMaxBatchLbl";
            cusMaxBatchLbl.Size = new Size(147, 15);
            cusMaxBatchLbl.TabIndex = 17;
            cusMaxBatchLbl.Text = "Customer Max. Batch Size:";
            // 
            // cusDocIdTxt
            // 
            cusDocIdTxt.Location = new Point(6, 431);
            cusDocIdTxt.Name = "cusDocIdTxt";
            cusDocIdTxt.Size = new Size(280, 23);
            cusDocIdTxt.TabIndex = 16;
            // 
            // cusDocIdLbl
            // 
            cusDocIdLbl.AutoSize = true;
            cusDocIdLbl.Location = new Point(6, 413);
            cusDocIdLbl.Name = "cusDocIdLbl";
            cusDocIdLbl.Size = new Size(134, 15);
            cusDocIdLbl.TabIndex = 15;
            cusDocIdLbl.Text = "Customer Document Id:";
            // 
            // cusTempIdTxt
            // 
            cusTempIdTxt.Location = new Point(6, 387);
            cusTempIdTxt.Name = "cusTempIdTxt";
            cusTempIdTxt.Size = new Size(280, 23);
            cusTempIdTxt.TabIndex = 14;
            // 
            // cusTempKeyLbl
            // 
            cusTempKeyLbl.AutoSize = true;
            cusTempKeyLbl.Location = new Point(6, 369);
            cusTempKeyLbl.Name = "cusTempKeyLbl";
            cusTempKeyLbl.Size = new Size(135, 15);
            cusTempKeyLbl.TabIndex = 13;
            cusTempKeyLbl.Text = "Customer Template Key:";
            // 
            // cusProjIdTxt
            // 
            cusProjIdTxt.Location = new Point(6, 343);
            cusProjIdTxt.Name = "cusProjIdTxt";
            cusProjIdTxt.Size = new Size(280, 23);
            cusProjIdTxt.TabIndex = 12;
            // 
            // cusProjectLbl
            // 
            cusProjectLbl.AutoSize = true;
            cusProjectLbl.Location = new Point(6, 325);
            cusProjectLbl.Name = "cusProjectLbl";
            cusProjectLbl.Size = new Size(115, 15);
            cusProjectLbl.TabIndex = 11;
            cusProjectLbl.Text = "Customer Project Id:";
            // 
            // cusQueuTxt
            // 
            cusQueuTxt.Location = new Point(6, 299);
            cusQueuTxt.Name = "cusQueuTxt";
            cusQueuTxt.Size = new Size(123, 23);
            cusQueuTxt.TabIndex = 10;
            // 
            // cusQueueLbl
            // 
            cusQueueLbl.AutoSize = true;
            cusQueueLbl.Location = new Point(6, 281);
            cusQueueLbl.Name = "cusQueueLbl";
            cusQueueLbl.Size = new Size(100, 15);
            cusQueueLbl.TabIndex = 9;
            cusQueueLbl.Text = "Customer Queue:";
            // 
            // cusApiTokenTxt
            // 
            cusApiTokenTxt.Location = new Point(6, 164);
            cusApiTokenTxt.Multiline = true;
            cusApiTokenTxt.Name = "cusApiTokenTxt";
            cusApiTokenTxt.ScrollBars = ScrollBars.Vertical;
            cusApiTokenTxt.Size = new Size(280, 114);
            cusApiTokenTxt.TabIndex = 8;
            // 
            // cusTokeLbl
            // 
            cusTokeLbl.AutoSize = true;
            cusTokeLbl.Location = new Point(6, 146);
            cusTokeLbl.Name = "cusTokeLbl";
            cusTokeLbl.Size = new Size(110, 15);
            cusTokeLbl.TabIndex = 7;
            cusTokeLbl.Text = "Customer Api Toke:";
            // 
            // cusUnameTxt
            // 
            cusUnameTxt.Location = new Point(6, 120);
            cusUnameTxt.Name = "cusUnameTxt";
            cusUnameTxt.Size = new Size(280, 23);
            cusUnameTxt.TabIndex = 6;
            // 
            // ftpDetails
            // 
            ftpDetails.Controls.Add(ftpRemoveFileCombo);
            ftpDetails.Controls.Add(ftpSubPathTxt);
            ftpDetails.Controls.Add(ftpSubPathLbl);
            ftpDetails.Controls.Add(ftpMoveToSubGrp);
            ftpDetails.Controls.Add(ftpLoopGrp);
            ftpDetails.Controls.Add(ftpMainPathTxt);
            ftpDetails.Controls.Add(ftpMainPathLbl);
            ftpDetails.Controls.Add(ftpPortTxt);
            ftpDetails.Controls.Add(ftpPortLbl);
            ftpDetails.Controls.Add(ftpPasswordTxt);
            ftpDetails.Controls.Add(ftpPasswordLbl);
            ftpDetails.Controls.Add(ftpUserNameTxt);
            ftpDetails.Controls.Add(ftpUserNameLbl);
            ftpDetails.Controls.Add(ftpHostTxt);
            ftpDetails.Controls.Add(ftpHostlbl);
            ftpDetails.Controls.Add(ftpProfileCombo);
            ftpDetails.Controls.Add(ftpProfileLbl);
            ftpDetails.Controls.Add(ftpTypCombo);
            ftpDetails.Controls.Add(ftpTypeLbl);
            ftpDetails.Location = new Point(621, 12);
            ftpDetails.Name = "ftpDetails";
            ftpDetails.Size = new Size(608, 296);
            ftpDetails.TabIndex = 7;
            ftpDetails.TabStop = false;
            ftpDetails.Text = "Ftp Details";
            // 
            // ftpRemoveFileCombo
            // 
            ftpRemoveFileCombo.Controls.Add(ftpRemoveOff);
            ftpRemoveFileCombo.Controls.Add(ftpRemoveOn);
            ftpRemoveFileCombo.Location = new Point(317, 234);
            ftpRemoveFileCombo.Name = "ftpRemoveFileCombo";
            ftpRemoveFileCombo.Size = new Size(203, 53);
            ftpRemoveFileCombo.TabIndex = 24;
            ftpRemoveFileCombo.TabStop = false;
            ftpRemoveFileCombo.Text = "Ftp Remove Files From Ftp:";
            // 
            // ftpRemoveOff
            // 
            ftpRemoveOff.AutoSize = true;
            ftpRemoveOff.Location = new Point(79, 22);
            ftpRemoveOff.Name = "ftpRemoveOff";
            ftpRemoveOff.Size = new Size(70, 19);
            ftpRemoveOff.TabIndex = 1;
            ftpRemoveOff.TabStop = true;
            ftpRemoveOff.Text = "Disabled";
            ftpRemoveOff.UseVisualStyleBackColor = true;
            // 
            // ftpRemoveOn
            // 
            ftpRemoveOn.AutoSize = true;
            ftpRemoveOn.Location = new Point(6, 22);
            ftpRemoveOn.Name = "ftpRemoveOn";
            ftpRemoveOn.Size = new Size(67, 19);
            ftpRemoveOn.TabIndex = 0;
            ftpRemoveOn.TabStop = true;
            ftpRemoveOn.Text = "Enabled";
            ftpRemoveOn.UseVisualStyleBackColor = true;
            // 
            // ftpSubPathTxt
            // 
            ftpSubPathTxt.Enabled = false;
            ftpSubPathTxt.Location = new Point(317, 202);
            ftpSubPathTxt.Name = "ftpSubPathTxt";
            ftpSubPathTxt.Size = new Size(280, 23);
            ftpSubPathTxt.TabIndex = 23;
            // 
            // ftpSubPathLbl
            // 
            ftpSubPathLbl.AutoSize = true;
            ftpSubPathLbl.Location = new Point(317, 184);
            ftpSubPathLbl.Name = "ftpSubPathLbl";
            ftpSubPathLbl.Size = new Size(110, 15);
            ftpSubPathLbl.TabIndex = 22;
            ftpSubPathLbl.Text = "Ftp Sub Folder Path";
            // 
            // ftpMoveToSubGrp
            // 
            ftpMoveToSubGrp.Controls.Add(ftpMoveToSubOn);
            ftpMoveToSubGrp.Controls.Add(ftpMoveToSubOff);
            ftpMoveToSubGrp.Location = new Point(317, 128);
            ftpMoveToSubGrp.Name = "ftpMoveToSubGrp";
            ftpMoveToSubGrp.Size = new Size(203, 53);
            ftpMoveToSubGrp.TabIndex = 21;
            ftpMoveToSubGrp.TabStop = false;
            ftpMoveToSubGrp.Text = "Ftp Move To Sub Folder:";
            // 
            // ftpMoveToSubOn
            // 
            ftpMoveToSubOn.AutoSize = true;
            ftpMoveToSubOn.Location = new Point(6, 22);
            ftpMoveToSubOn.Name = "ftpMoveToSubOn";
            ftpMoveToSubOn.Size = new Size(67, 19);
            ftpMoveToSubOn.TabIndex = 18;
            ftpMoveToSubOn.TabStop = true;
            ftpMoveToSubOn.Text = "Enabled";
            ftpMoveToSubOn.UseVisualStyleBackColor = true;
            // 
            // ftpMoveToSubOff
            // 
            ftpMoveToSubOff.AutoSize = true;
            ftpMoveToSubOff.Location = new Point(79, 22);
            ftpMoveToSubOff.Name = "ftpMoveToSubOff";
            ftpMoveToSubOff.Size = new Size(70, 19);
            ftpMoveToSubOff.TabIndex = 19;
            ftpMoveToSubOff.TabStop = true;
            ftpMoveToSubOff.Text = "Disabled";
            ftpMoveToSubOff.UseVisualStyleBackColor = true;
            // 
            // ftpLoopGrp
            // 
            ftpLoopGrp.Controls.Add(ftpLoopOff);
            ftpLoopGrp.Controls.Add(ftpLoopOn);
            ftpLoopGrp.Location = new Point(317, 66);
            ftpLoopGrp.Name = "ftpLoopGrp";
            ftpLoopGrp.Size = new Size(203, 56);
            ftpLoopGrp.TabIndex = 20;
            ftpLoopGrp.TabStop = false;
            ftpLoopGrp.Text = "Ftp Loop Through Sub Folders:";
            // 
            // ftpLoopOff
            // 
            ftpLoopOff.AutoSize = true;
            ftpLoopOff.Location = new Point(79, 22);
            ftpLoopOff.Name = "ftpLoopOff";
            ftpLoopOff.Size = new Size(70, 19);
            ftpLoopOff.TabIndex = 14;
            ftpLoopOff.TabStop = true;
            ftpLoopOff.Text = "Disabled";
            ftpLoopOff.UseVisualStyleBackColor = true;
            // 
            // ftpLoopOn
            // 
            ftpLoopOn.AutoSize = true;
            ftpLoopOn.Location = new Point(6, 22);
            ftpLoopOn.Name = "ftpLoopOn";
            ftpLoopOn.Size = new Size(67, 19);
            ftpLoopOn.TabIndex = 13;
            ftpLoopOn.TabStop = true;
            ftpLoopOn.Text = "Enabled";
            ftpLoopOn.UseVisualStyleBackColor = true;
            // 
            // ftpMainPathTxt
            // 
            ftpMainPathTxt.Location = new Point(317, 37);
            ftpMainPathTxt.Name = "ftpMainPathTxt";
            ftpMainPathTxt.Size = new Size(280, 23);
            ftpMainPathTxt.TabIndex = 16;
            // 
            // ftpMainPathLbl
            // 
            ftpMainPathLbl.AutoSize = true;
            ftpMainPathLbl.Location = new Point(317, 19);
            ftpMainPathLbl.Name = "ftpMainPathLbl";
            ftpMainPathLbl.Size = new Size(120, 15);
            ftpMainPathLbl.TabIndex = 15;
            ftpMainPathLbl.Text = "Ftp Main Folder Path:";
            // 
            // ftpPortTxt
            // 
            ftpPortTxt.Location = new Point(6, 257);
            ftpPortTxt.Name = "ftpPortTxt";
            ftpPortTxt.Size = new Size(123, 23);
            ftpPortTxt.TabIndex = 11;
            // 
            // ftpPortLbl
            // 
            ftpPortLbl.AutoSize = true;
            ftpPortLbl.Location = new Point(6, 239);
            ftpPortLbl.Name = "ftpPortLbl";
            ftpPortLbl.Size = new Size(52, 15);
            ftpPortLbl.TabIndex = 10;
            ftpPortLbl.Text = "Ftp Port:";
            // 
            // ftpPasswordTxt
            // 
            ftpPasswordTxt.Location = new Point(6, 213);
            ftpPasswordTxt.Name = "ftpPasswordTxt";
            ftpPasswordTxt.PasswordChar = '•';
            ftpPasswordTxt.Size = new Size(280, 23);
            ftpPasswordTxt.TabIndex = 9;
            // 
            // ftpPasswordLbl
            // 
            ftpPasswordLbl.AutoSize = true;
            ftpPasswordLbl.Location = new Point(6, 195);
            ftpPasswordLbl.Name = "ftpPasswordLbl";
            ftpPasswordLbl.Size = new Size(80, 15);
            ftpPasswordLbl.TabIndex = 8;
            ftpPasswordLbl.Text = "Ftp Password:";
            // 
            // ftpUserNameTxt
            // 
            ftpUserNameTxt.Location = new Point(6, 169);
            ftpUserNameTxt.Name = "ftpUserNameTxt";
            ftpUserNameTxt.Size = new Size(280, 23);
            ftpUserNameTxt.TabIndex = 7;
            // 
            // ftpUserNameLbl
            // 
            ftpUserNameLbl.AutoSize = true;
            ftpUserNameLbl.Location = new Point(6, 151);
            ftpUserNameLbl.Name = "ftpUserNameLbl";
            ftpUserNameLbl.Size = new Size(88, 15);
            ftpUserNameLbl.TabIndex = 6;
            ftpUserNameLbl.Text = "Ftp User Name:";
            // 
            // ftpHostTxt
            // 
            ftpHostTxt.Location = new Point(6, 125);
            ftpHostTxt.Name = "ftpHostTxt";
            ftpHostTxt.Size = new Size(280, 23);
            ftpHostTxt.TabIndex = 5;
            // 
            // ftpHostlbl
            // 
            ftpHostlbl.AutoSize = true;
            ftpHostlbl.Location = new Point(6, 107);
            ftpHostlbl.Name = "ftpHostlbl";
            ftpHostlbl.Size = new Size(107, 15);
            ftpHostlbl.TabIndex = 4;
            ftpHostlbl.Text = "Ftp Server Address:";
            // 
            // ftpProfileCombo
            // 
            ftpProfileCombo.FormattingEnabled = true;
            ftpProfileCombo.Items.AddRange(new object[] { "profileepe", "profilepxe", "profilefsv" });
            ftpProfileCombo.Location = new Point(6, 81);
            ftpProfileCombo.Name = "ftpProfileCombo";
            ftpProfileCombo.Size = new Size(174, 23);
            ftpProfileCombo.TabIndex = 3;
            // 
            // ftpProfileLbl
            // 
            ftpProfileLbl.AutoSize = true;
            ftpProfileLbl.Location = new Point(6, 63);
            ftpProfileLbl.Name = "ftpProfileLbl";
            ftpProfileLbl.Size = new Size(64, 15);
            ftpProfileLbl.TabIndex = 2;
            ftpProfileLbl.Text = "Ftp Profile:";
            // 
            // ftpTypCombo
            // 
            ftpTypCombo.FormattingEnabled = true;
            ftpTypCombo.Items.AddRange(new object[] { "FTP", "SFTP", "FTPS" });
            ftpTypCombo.Location = new Point(6, 37);
            ftpTypCombo.Name = "ftpTypCombo";
            ftpTypCombo.Size = new Size(174, 23);
            ftpTypCombo.TabIndex = 1;
            ftpTypCombo.Text = "Select The Ftp Type ...";
            // 
            // ftpTypeLbl
            // 
            ftpTypeLbl.AutoSize = true;
            ftpTypeLbl.Location = new Point(6, 19);
            ftpTypeLbl.Name = "ftpTypeLbl";
            ftpTypeLbl.Size = new Size(54, 15);
            ftpTypeLbl.TabIndex = 0;
            ftpTypeLbl.Text = "Ftp Type:";
            // 
            // emlDetailsGrp
            // 
            emlDetailsGrp.Controls.Add(emlSendSubjectGrp);
            emlDetailsGrp.Controls.Add(emlSendEmailGrp);
            emlDetailsGrp.Controls.Add(emlInboxPathTxt);
            emlDetailsGrp.Controls.Add(emlInboxPathLbl);
            emlDetailsGrp.Controls.Add(emlAddressTxt);
            emlDetailsGrp.Controls.Add(emlEmailLbl);
            emlDetailsGrp.Location = new Point(621, 311);
            emlDetailsGrp.Name = "emlDetailsGrp";
            emlDetailsGrp.Size = new Size(608, 148);
            emlDetailsGrp.TabIndex = 8;
            emlDetailsGrp.TabStop = false;
            emlDetailsGrp.Text = "Email Details";
            // 
            // emlSendSubjectGrp
            // 
            emlSendSubjectGrp.Controls.Add(emlSndSubjectOff);
            emlSendSubjectGrp.Controls.Add(emlSndSubjectOn);
            emlSendSubjectGrp.Location = new Point(317, 73);
            emlSendSubjectGrp.Name = "emlSendSubjectGrp";
            emlSendSubjectGrp.Size = new Size(203, 48);
            emlSendSubjectGrp.TabIndex = 5;
            emlSendSubjectGrp.TabStop = false;
            emlSendSubjectGrp.Text = "Email Send Subject:";
            // 
            // emlSndSubjectOff
            // 
            emlSndSubjectOff.AutoSize = true;
            emlSndSubjectOff.Location = new Point(79, 22);
            emlSndSubjectOff.Name = "emlSndSubjectOff";
            emlSndSubjectOff.Size = new Size(70, 19);
            emlSndSubjectOff.TabIndex = 1;
            emlSndSubjectOff.TabStop = true;
            emlSndSubjectOff.Text = "Disabled";
            emlSndSubjectOff.UseVisualStyleBackColor = true;
            // 
            // emlSndSubjectOn
            // 
            emlSndSubjectOn.AutoSize = true;
            emlSndSubjectOn.Location = new Point(6, 22);
            emlSndSubjectOn.Name = "emlSndSubjectOn";
            emlSndSubjectOn.Size = new Size(67, 19);
            emlSndSubjectOn.TabIndex = 0;
            emlSndSubjectOn.TabStop = true;
            emlSndSubjectOn.Text = "Enabled";
            emlSndSubjectOn.UseVisualStyleBackColor = true;
            // 
            // emlSendEmailGrp
            // 
            emlSendEmailGrp.Controls.Add(emlSenAdressOff);
            emlSendEmailGrp.Controls.Add(emlSenAdressOn);
            emlSendEmailGrp.Location = new Point(317, 19);
            emlSendEmailGrp.Name = "emlSendEmailGrp";
            emlSendEmailGrp.Size = new Size(203, 48);
            emlSendEmailGrp.TabIndex = 4;
            emlSendEmailGrp.TabStop = false;
            emlSendEmailGrp.Text = "Email Send Address:";
            // 
            // emlSenAdressOff
            // 
            emlSenAdressOff.AutoSize = true;
            emlSenAdressOff.Location = new Point(79, 22);
            emlSenAdressOff.Name = "emlSenAdressOff";
            emlSenAdressOff.Size = new Size(70, 19);
            emlSenAdressOff.TabIndex = 1;
            emlSenAdressOff.TabStop = true;
            emlSenAdressOff.Text = "Disabled";
            emlSenAdressOff.UseVisualStyleBackColor = true;
            // 
            // emlSenAdressOn
            // 
            emlSenAdressOn.AutoSize = true;
            emlSenAdressOn.Location = new Point(6, 22);
            emlSenAdressOn.Name = "emlSenAdressOn";
            emlSenAdressOn.Size = new Size(67, 19);
            emlSenAdressOn.TabIndex = 0;
            emlSenAdressOn.TabStop = true;
            emlSenAdressOn.Text = "Enabled";
            emlSenAdressOn.UseVisualStyleBackColor = true;
            // 
            // emlInboxPathTxt
            // 
            emlInboxPathTxt.Location = new Point(6, 81);
            emlInboxPathTxt.Name = "emlInboxPathTxt";
            emlInboxPathTxt.Size = new Size(280, 23);
            emlInboxPathTxt.TabIndex = 3;
            // 
            // emlInboxPathLbl
            // 
            emlInboxPathLbl.AutoSize = true;
            emlInboxPathLbl.Location = new Point(6, 63);
            emlInboxPathLbl.Name = "emlInboxPathLbl";
            emlInboxPathLbl.Size = new Size(99, 15);
            emlInboxPathLbl.TabIndex = 2;
            emlInboxPathLbl.Text = "Email Inbox Path:";
            // 
            // emlAddressTxt
            // 
            emlAddressTxt.Location = new Point(6, 37);
            emlAddressTxt.Name = "emlAddressTxt";
            emlAddressTxt.Size = new Size(280, 23);
            emlAddressTxt.TabIndex = 1;
            // 
            // emlEmailLbl
            // 
            emlEmailLbl.AutoSize = true;
            emlEmailLbl.Location = new Point(6, 19);
            emlEmailLbl.Name = "emlEmailLbl";
            emlEmailLbl.Size = new Size(84, 15);
            emlEmailLbl.TabIndex = 0;
            emlEmailLbl.Text = "Email Address:";
            // 
            // cmdBoxGrp
            // 
            cmdBoxGrp.Controls.Add(btnSave);
            cmdBoxGrp.Controls.Add(btnReset);
            cmdBoxGrp.Controls.Add(btnCancel);
            cmdBoxGrp.Location = new Point(621, 464);
            cmdBoxGrp.Name = "cmdBoxGrp";
            cmdBoxGrp.Size = new Size(608, 100);
            cmdBoxGrp.TabIndex = 9;
            cmdBoxGrp.TabStop = false;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(480, 35);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(117, 42);
            btnSave.TabIndex = 2;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(357, 35);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(117, 42);
            btnReset.TabIndex = 1;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(232, 35);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(119, 42);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // AddCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1242, 576);
            Controls.Add(cmdBoxGrp);
            Controls.Add(emlDetailsGrp);
            Controls.Add(ftpDetails);
            Controls.Add(grpBoxCustomer);
            Name = "AddCustomers";
            Text = "Add Customers";
            grpBoxCustomer.ResumeLayout(false);
            grpBoxCustomer.PerformLayout();
            cusStatusGrp.ResumeLayout(false);
            cusStatusGrp.PerformLayout();
            ftpDetails.ResumeLayout(false);
            ftpDetails.PerformLayout();
            ftpRemoveFileCombo.ResumeLayout(false);
            ftpRemoveFileCombo.PerformLayout();
            ftpMoveToSubGrp.ResumeLayout(false);
            ftpMoveToSubGrp.PerformLayout();
            ftpLoopGrp.ResumeLayout(false);
            ftpLoopGrp.PerformLayout();
            emlDetailsGrp.ResumeLayout(false);
            emlDetailsGrp.PerformLayout();
            emlSendSubjectGrp.ResumeLayout(false);
            emlSendSubjectGrp.PerformLayout();
            emlSendEmailGrp.ResumeLayout(false);
            emlSendEmailGrp.PerformLayout();
            cmdBoxGrp.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        public RadioButton cusOn;
        public RadioButton cusOff;
        public Label cusNameLbl;
        public TextBox cusNameTxt;
        public Label cusUnameLbl;
        public GroupBox grpBoxCustomer;
        public Label cusTokeLbl;
        public TextBox cusUnameTxt;
        public TextBox cusApiTokenTxt;
        public Label cusQueueLbl;
        public TextBox cusDocIdTxt;
        public Label cusDocIdLbl;
        public TextBox cusTempIdTxt;
        public Label cusTempKeyLbl;
        public TextBox cusProjIdTxt;
        public Label cusProjectLbl;
        public TextBox cusQueuTxt;
        public TextBox cusMaxBatchTxt;
        public Label cusMaxBatchLbl;
        public Label cusDocEncoLbl;
        public TextBox cusDocencTxt;
        public TextBox cusFonValTxt;
        public Label cusFoneValLbl;
        public TextBox cusFonNameTxt;
        public Label cusFoneNameLbl;
        public TextBox custFtwoNameTxt;
        public Label cusFtwoNameLbl;
        public CheckedListBox cusDocExtList;
        public Label cusDocExtensionsLbl;
        public Label cusDelMethodLbl;
        public TextBox cusDomainTxt;
        public Label cusDomainLbl;
        public ComboBox cusDelMethodCombo;
        public GroupBox ftpDetails;
        public ComboBox ftpTypCombo;
        public Label ftpTypeLbl;
        public Label ftpProfileLbl;
        public Label ftpHostlbl;
        public MaskedTextBox ftpPasswordTxt;
        public Label ftpPasswordLbl;
        public TextBox ftpUserNameTxt;
        public Label ftpUserNameLbl;
        public TextBox ftpHostTxt;
        public RadioButton ftpLoopOff;
        public RadioButton ftpLoopOn;
        public TextBox ftpPortTxt;
        public Label ftpPortLbl;
        public RadioButton ftpMoveToSubOn;
        public TextBox ftpMainPathTxt;
        public Label ftpMainPathLbl;
        public RadioButton ftpMoveToSubOff;
        public GroupBox ftpLoopGrp;
        public GroupBox ftpMoveToSubGrp;
        public TextBox ftpSubPathTxt;
        public Label ftpSubPathLbl;
        public GroupBox ftpRemoveFileCombo;
        public RadioButton ftpRemoveOff;
        public RadioButton ftpRemoveOn;
        public GroupBox emlDetailsGrp;
        public GroupBox emlSendEmailGrp;
        public RadioButton emlSenAdressOff;
        public RadioButton emlSenAdressOn;
        public TextBox emlInboxPathTxt;
        public Label emlInboxPathLbl;
        public TextBox emlAddressTxt;
        public Label emlEmailLbl;
        public GroupBox emlSendSubjectGrp;
        public RadioButton emlSndSubjectOff;
        public RadioButton emlSndSubjectOn;
        public GroupBox cusStatusGrp;
        public GroupBox cmdBoxGrp;
        public Button btnSave;
        public Button btnReset;
        public Button btnCancel;
        public ToolTip toolTipGlobal;
        public ComboBox ftpProfileCombo;
        public TextBox custFtwoValTxt;
        public Label cusFtwoValLbl;
    }
}