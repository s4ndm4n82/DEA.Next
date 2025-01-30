namespace DEA.UI
{
    partial class EditCustomersList
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
            toolTipGlobal = new ToolTip(components);
            searchGrp = new GroupBox();
            btnEditCustomerSearch = new Button();
            cusEditSearchTxt = new TextBox();
            searchProjectId = new RadioButton();
            searchCusName = new RadioButton();
            searchCusId = new RadioButton();
            grdEditCustomer = new DataGridView();
            btnEditCutomerReset = new Button();
            btnEditCustomerCancel = new Button();
            searchGrp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grdEditCustomer).BeginInit();
            SuspendLayout();
            // 
            // searchGrp
            // 
            searchGrp.Controls.Add(btnEditCustomerCancel);
            searchGrp.Controls.Add(btnEditCutomerReset);
            searchGrp.Controls.Add(btnEditCustomerSearch);
            searchGrp.Controls.Add(cusEditSearchTxt);
            searchGrp.Controls.Add(searchProjectId);
            searchGrp.Controls.Add(searchCusName);
            searchGrp.Controls.Add(searchCusId);
            searchGrp.Location = new Point(12, 12);
            searchGrp.Name = "searchGrp";
            searchGrp.Size = new Size(1217, 68);
            searchGrp.TabIndex = 14;
            searchGrp.TabStop = false;
            searchGrp.Text = "Search Customer";
            // 
            // btnEditCustomerSearch
            // 
            btnEditCustomerSearch.Location = new Point(842, 15);
            btnEditCustomerSearch.Name = "btnEditCustomerSearch";
            btnEditCustomerSearch.Size = new Size(119, 42);
            btnEditCustomerSearch.TabIndex = 4;
            btnEditCustomerSearch.Text = "Search";
            btnEditCustomerSearch.UseVisualStyleBackColor = true;
            // 
            // cusEditSearchTxt
            // 
            cusEditSearchTxt.Location = new Point(304, 26);
            cusEditSearchTxt.Name = "cusEditSearchTxt";
            cusEditSearchTxt.Size = new Size(532, 23);
            cusEditSearchTxt.TabIndex = 3;
            // 
            // searchProjectId
            // 
            searchProjectId.AutoSize = true;
            searchProjectId.Location = new Point(102, 27);
            searchProjectId.Name = "searchProjectId";
            searchProjectId.Size = new Size(75, 19);
            searchProjectId.TabIndex = 2;
            searchProjectId.TabStop = true;
            searchProjectId.Text = "Project Id";
            searchProjectId.UseVisualStyleBackColor = true;
            // 
            // searchCusName
            // 
            searchCusName.AutoSize = true;
            searchCusName.Location = new Point(186, 27);
            searchCusName.Name = "searchCusName";
            searchCusName.Size = new Size(112, 19);
            searchCusName.TabIndex = 1;
            searchCusName.TabStop = true;
            searchCusName.Text = "Customer Name";
            searchCusName.UseVisualStyleBackColor = true;
            // 
            // searchCusId
            // 
            searchCusId.AutoSize = true;
            searchCusId.Location = new Point(6, 27);
            searchCusId.Name = "searchCusId";
            searchCusId.Size = new Size(90, 19);
            searchCusId.TabIndex = 0;
            searchCusId.TabStop = true;
            searchCusId.Text = "Customer Id";
            searchCusId.UseVisualStyleBackColor = true;
            // 
            // grdEditCustomer
            // 
            grdEditCustomer.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grdEditCustomer.Location = new Point(12, 86);
            grdEditCustomer.Name = "grdEditCustomer";
            grdEditCustomer.Size = new Size(1217, 478);
            grdEditCustomer.TabIndex = 15;
            // 
            // btnEditCutomerReset
            // 
            btnEditCutomerReset.Location = new Point(967, 15);
            btnEditCutomerReset.Name = "btnEditCutomerReset";
            btnEditCutomerReset.Size = new Size(119, 42);
            btnEditCutomerReset.TabIndex = 5;
            btnEditCutomerReset.Text = "Reset";
            btnEditCutomerReset.UseVisualStyleBackColor = true;
            // 
            // btnEditCustomerCancel
            // 
            btnEditCustomerCancel.Location = new Point(1092, 15);
            btnEditCustomerCancel.Name = "btnEditCustomerCancel";
            btnEditCustomerCancel.Size = new Size(119, 42);
            btnEditCustomerCancel.TabIndex = 6;
            btnEditCustomerCancel.Text = "Cancel";
            btnEditCustomerCancel.UseVisualStyleBackColor = true;
            // 
            // EditCustomersList
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1242, 576);
            Controls.Add(grdEditCustomer);
            Controls.Add(searchGrp);
            Name = "EditCustomersList";
            Text = "List Customers";
            searchGrp.ResumeLayout(false);
            searchGrp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grdEditCustomer).EndInit();
            ResumeLayout(false);
        }

        #endregion
        public ToolTip toolTipGlobal;
        public GroupBox searchGrp;
        public RadioButton searchCusId;
        public RadioButton searchCusName;
        public TextBox cusEditSearchTxt;
        public RadioButton searchProjectId;
        public Button btnEditCustomerSearch;
        public DataGridView grdEditCustomer;
        public Button btnEditCustomerCancel;
        public Button btnEditCutomerReset;
    }
}