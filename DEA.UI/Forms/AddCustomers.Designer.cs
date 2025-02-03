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
            cusOn = new RadioButton();
            cusOff = new RadioButton();
            cusNameLbl = new Label();
            cusNameTxt = new TextBox();
            cusUnameLbl = new Label();
            cusDetailsGrp = new GroupBox();
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
            ftpDetailsGrp = new GroupBox();
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
            cusDetailsGrp.SuspendLayout();
            cusStatusGrp.SuspendLayout();
            ftpDetailsGrp.SuspendLayout();
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
            cusNameTxt.TabIndex = 3;
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
            // cusDetailsGrp
            // 
            cusDetailsGrp.Controls.Add(custFtwoValTxt);
            cusDetailsGrp.Controls.Add(cusFtwoValLbl);
            cusDetailsGrp.Controls.Add(cusStatusGrp);
            cusDetailsGrp.Controls.Add(cusDelMethodCombo);
            cusDetailsGrp.Controls.Add(cusDelMethodLbl);
            cusDetailsGrp.Controls.Add(cusDomainTxt);
            cusDetailsGrp.Controls.Add(cusDomainLbl);
            cusDetailsGrp.Controls.Add(cusDocExtList);
            cusDetailsGrp.Controls.Add(cusDocExtensionsLbl);
            cusDetailsGrp.Controls.Add(custFtwoNameTxt);
            cusDetailsGrp.Controls.Add(cusFtwoNameLbl);
            cusDetailsGrp.Controls.Add(cusFonValTxt);
            cusDetailsGrp.Controls.Add(cusFoneValLbl);
            cusDetailsGrp.Controls.Add(cusFonNameTxt);
            cusDetailsGrp.Controls.Add(cusFoneNameLbl);
            cusDetailsGrp.Controls.Add(cusDocencTxt);
            cusDetailsGrp.Controls.Add(cusDocEncoLbl);
            cusDetailsGrp.Controls.Add(cusMaxBatchTxt);
            cusDetailsGrp.Controls.Add(cusMaxBatchLbl);
            cusDetailsGrp.Controls.Add(cusDocIdTxt);
            cusDetailsGrp.Controls.Add(cusDocIdLbl);
            cusDetailsGrp.Controls.Add(cusTempIdTxt);
            cusDetailsGrp.Controls.Add(cusTempKeyLbl);
            cusDetailsGrp.Controls.Add(cusProjIdTxt);
            cusDetailsGrp.Controls.Add(cusProjectLbl);
            cusDetailsGrp.Controls.Add(cusQueuTxt);
            cusDetailsGrp.Controls.Add(cusQueueLbl);
            cusDetailsGrp.Controls.Add(cusApiTokenTxt);
            cusDetailsGrp.Controls.Add(cusTokeLbl);
            cusDetailsGrp.Controls.Add(cusUnameTxt);
            cusDetailsGrp.Controls.Add(cusUnameLbl);
            cusDetailsGrp.Controls.Add(cusNameTxt);
            cusDetailsGrp.Controls.Add(cusNameLbl);
            cusDetailsGrp.Location = new Point(12, 12);
            cusDetailsGrp.Name = "cusDetailsGrp";
            cusDetailsGrp.Size = new Size(603, 552);
            cusDetailsGrp.TabIndex = 44;
            cusDetailsGrp.TabStop = false;
            cusDetailsGrp.Text = "Customer Details";
            // 
            // custFtwoValTxt
            // 
            custFtwoValTxt.Location = new Point(315, 172);
            custFtwoValTxt.Name = "custFtwoValTxt";
            custFtwoValTxt.Size = new Size(280, 23);
            custFtwoValTxt.TabIndex = 16;
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
            cusStatusGrp.TabIndex = 0;
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
            cusDelMethodCombo.TabIndex = 18;
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
            cusDomainTxt.TabIndex = 17;
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
            cusDocExtList.TabIndex = 19;
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
            custFtwoNameTxt.TabIndex = 15;
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
            cusFonValTxt.TabIndex = 14;
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
            cusFonNameTxt.TabIndex = 13;
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
            cusDocencTxt.TabIndex = 12;
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
            cusMaxBatchTxt.TabIndex = 10;
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
            cusDocIdTxt.TabIndex = 9;
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
            cusTempIdTxt.TabIndex = 8;
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
            cusProjIdTxt.TabIndex = 7;
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
            cusQueuTxt.TabIndex = 6;
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
            cusApiTokenTxt.TabIndex = 5;
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
            cusUnameTxt.TabIndex = 4;
            // 
            // ftpDetailsGrp
            // 
            ftpDetailsGrp.Controls.Add(ftpRemoveFileCombo);
            ftpDetailsGrp.Controls.Add(ftpSubPathTxt);
            ftpDetailsGrp.Controls.Add(ftpSubPathLbl);
            ftpDetailsGrp.Controls.Add(ftpMoveToSubGrp);
            ftpDetailsGrp.Controls.Add(ftpLoopGrp);
            ftpDetailsGrp.Controls.Add(ftpMainPathTxt);
            ftpDetailsGrp.Controls.Add(ftpMainPathLbl);
            ftpDetailsGrp.Controls.Add(ftpPortTxt);
            ftpDetailsGrp.Controls.Add(ftpPortLbl);
            ftpDetailsGrp.Controls.Add(ftpPasswordTxt);
            ftpDetailsGrp.Controls.Add(ftpPasswordLbl);
            ftpDetailsGrp.Controls.Add(ftpUserNameTxt);
            ftpDetailsGrp.Controls.Add(ftpUserNameLbl);
            ftpDetailsGrp.Controls.Add(ftpHostTxt);
            ftpDetailsGrp.Controls.Add(ftpHostlbl);
            ftpDetailsGrp.Controls.Add(ftpProfileCombo);
            ftpDetailsGrp.Controls.Add(ftpProfileLbl);
            ftpDetailsGrp.Controls.Add(ftpTypCombo);
            ftpDetailsGrp.Controls.Add(ftpTypeLbl);
            ftpDetailsGrp.Location = new Point(621, 12);
            ftpDetailsGrp.Name = "ftpDetailsGrp";
            ftpDetailsGrp.Size = new Size(608, 296);
            ftpDetailsGrp.TabIndex = 45;
            ftpDetailsGrp.TabStop = false;
            ftpDetailsGrp.Text = "Ftp Details";
            // 
            // ftpRemoveFileCombo
            // 
            ftpRemoveFileCombo.Controls.Add(ftpRemoveOff);
            ftpRemoveFileCombo.Controls.Add(ftpRemoveOn);
            ftpRemoveFileCombo.Location = new Point(317, 234);
            ftpRemoveFileCombo.Name = "ftpRemoveFileCombo";
            ftpRemoveFileCombo.Size = new Size(203, 53);
            ftpRemoveFileCombo.TabIndex = 53;
            ftpRemoveFileCombo.TabStop = false;
            ftpRemoveFileCombo.Text = "Ftp Remove Files From Ftp:";
            // 
            // ftpRemoveOff
            // 
            ftpRemoveOff.AutoSize = true;
            ftpRemoveOff.Location = new Point(79, 22);
            ftpRemoveOff.Name = "ftpRemoveOff";
            ftpRemoveOff.Size = new Size(70, 19);
            ftpRemoveOff.TabIndex = 33;
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
            ftpRemoveOn.TabIndex = 32;
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
            ftpSubPathTxt.TabIndex = 31;
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
            ftpMoveToSubGrp.TabIndex = 51;
            ftpMoveToSubGrp.TabStop = false;
            ftpMoveToSubGrp.Text = "Ftp Move To Sub Folder:";
            // 
            // ftpMoveToSubOn
            // 
            ftpMoveToSubOn.AutoSize = true;
            ftpMoveToSubOn.Location = new Point(6, 22);
            ftpMoveToSubOn.Name = "ftpMoveToSubOn";
            ftpMoveToSubOn.Size = new Size(67, 19);
            ftpMoveToSubOn.TabIndex = 29;
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
            ftpMoveToSubOff.TabIndex = 30;
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
            ftpLoopGrp.TabIndex = 50;
            ftpLoopGrp.TabStop = false;
            ftpLoopGrp.Text = "Ftp Loop Through Sub Folders:";
            // 
            // ftpLoopOff
            // 
            ftpLoopOff.AutoSize = true;
            ftpLoopOff.Location = new Point(79, 22);
            ftpLoopOff.Name = "ftpLoopOff";
            ftpLoopOff.Size = new Size(70, 19);
            ftpLoopOff.TabIndex = 28;
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
            ftpLoopOn.TabIndex = 27;
            ftpLoopOn.TabStop = true;
            ftpLoopOn.Text = "Enabled";
            ftpLoopOn.UseVisualStyleBackColor = true;
            // 
            // ftpMainPathTxt
            // 
            ftpMainPathTxt.Location = new Point(317, 37);
            ftpMainPathTxt.Name = "ftpMainPathTxt";
            ftpMainPathTxt.Size = new Size(280, 23);
            ftpMainPathTxt.TabIndex = 26;
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
            ftpPortTxt.TabIndex = 25;
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
            ftpPasswordTxt.TabIndex = 24;
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
            ftpUserNameTxt.TabIndex = 23;
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
            ftpHostTxt.TabIndex = 22;
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
            ftpProfileCombo.TabIndex = 21;
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
            ftpTypCombo.TabIndex = 20;
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
            emlDetailsGrp.TabIndex = 46;
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
            emlSendSubjectGrp.TabIndex = 55;
            emlSendSubjectGrp.TabStop = false;
            emlSendSubjectGrp.Text = "Email Send Subject:";
            // 
            // emlSndSubjectOff
            // 
            emlSndSubjectOff.AutoSize = true;
            emlSndSubjectOff.Location = new Point(79, 22);
            emlSndSubjectOff.Name = "emlSndSubjectOff";
            emlSndSubjectOff.Size = new Size(70, 19);
            emlSndSubjectOff.TabIndex = 39;
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
            emlSndSubjectOn.TabIndex = 38;
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
            emlSendEmailGrp.TabIndex = 54;
            emlSendEmailGrp.TabStop = false;
            emlSendEmailGrp.Text = "Email Send Address:";
            // 
            // emlSenAdressOff
            // 
            emlSenAdressOff.AutoSize = true;
            emlSenAdressOff.Location = new Point(79, 22);
            emlSenAdressOff.Name = "emlSenAdressOff";
            emlSenAdressOff.Size = new Size(70, 19);
            emlSenAdressOff.TabIndex = 37;
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
            emlSenAdressOn.TabIndex = 36;
            emlSenAdressOn.TabStop = true;
            emlSenAdressOn.Text = "Enabled";
            emlSenAdressOn.UseVisualStyleBackColor = true;
            // 
            // emlInboxPathTxt
            // 
            emlInboxPathTxt.Location = new Point(6, 81);
            emlInboxPathTxt.Name = "emlInboxPathTxt";
            emlInboxPathTxt.Size = new Size(280, 23);
            emlInboxPathTxt.TabIndex = 35;
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
            emlAddressTxt.TabIndex = 34;
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
            cmdBoxGrp.TabIndex = 47;
            cmdBoxGrp.TabStop = false;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(480, 35);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(117, 42);
            btnSave.TabIndex = 40;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            btnReset.Location = new Point(357, 35);
            btnReset.Name = "btnReset";
            btnReset.Size = new Size(117, 42);
            btnReset.TabIndex = 41;
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(232, 35);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(119, 42);
            btnCancel.TabIndex = 43;
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
            Controls.Add(ftpDetailsGrp);
            Controls.Add(cusDetailsGrp);
            Name = "AddCustomers";
            Text = "Add Customers";
            cusDetailsGrp.ResumeLayout(false);
            cusDetailsGrp.PerformLayout();
            cusStatusGrp.ResumeLayout(false);
            cusStatusGrp.PerformLayout();
            ftpDetailsGrp.ResumeLayout(false);
            ftpDetailsGrp.PerformLayout();
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
        public GroupBox cusDetailsGrp;
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
        public GroupBox ftpDetailsGrp;
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
        public ComboBox ftpProfileCombo;
        public TextBox custFtwoValTxt;
        public Label cusFtwoValLbl;
    }
}