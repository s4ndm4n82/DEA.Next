namespace DEA.UI
{
    partial class RemoveCustomers
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
            grpRemove = new GroupBox();
            searchOptions = new Label();
            rmSearchName = new RadioButton();
            rmSearchProjId = new RadioButton();
            rmBtnCancel = new Button();
            rmSearchId = new RadioButton();
            rmBtnReset = new Button();
            rmBtnRemove = new Button();
            dataGridView1 = new DataGridView();
            btnRmSearch = new Button();
            rmSearchTxt = new TextBox();
            grpRemove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // grpRemove
            // 
            grpRemove.Controls.Add(searchOptions);
            grpRemove.Controls.Add(rmSearchName);
            grpRemove.Controls.Add(rmSearchProjId);
            grpRemove.Controls.Add(rmBtnCancel);
            grpRemove.Controls.Add(rmSearchId);
            grpRemove.Controls.Add(rmBtnReset);
            grpRemove.Controls.Add(rmBtnRemove);
            grpRemove.Controls.Add(dataGridView1);
            grpRemove.Controls.Add(btnRmSearch);
            grpRemove.Controls.Add(rmSearchTxt);
            grpRemove.Location = new Point(12, 12);
            grpRemove.Name = "grpRemove";
            grpRemove.Size = new Size(1218, 552);
            grpRemove.TabIndex = 0;
            grpRemove.TabStop = false;
            grpRemove.Text = "Customer Remove";
            // 
            // searchOptions
            // 
            searchOptions.AutoSize = true;
            searchOptions.Location = new Point(397, 32);
            searchOptions.Name = "searchOptions";
            searchOptions.Size = new Size(78, 15);
            searchOptions.TabIndex = 7;
            searchOptions.Text = "Search Using:";
            // 
            // rmSearchName
            // 
            rmSearchName.AutoSize = true;
            rmSearchName.Location = new Point(658, 30);
            rmSearchName.Name = "rmSearchName";
            rmSearchName.Size = new Size(112, 19);
            rmSearchName.TabIndex = 2;
            rmSearchName.TabStop = true;
            rmSearchName.Text = "Customer Name";
            rmSearchName.UseVisualStyleBackColor = true;
            // 
            // rmSearchProjId
            // 
            rmSearchProjId.AutoSize = true;
            rmSearchProjId.Location = new Point(577, 30);
            rmSearchProjId.Name = "rmSearchProjId";
            rmSearchProjId.Size = new Size(75, 19);
            rmSearchProjId.TabIndex = 1;
            rmSearchProjId.TabStop = true;
            rmSearchProjId.Text = "Project Id";
            rmSearchProjId.UseVisualStyleBackColor = true;
            // 
            // rmBtnCancel
            // 
            rmBtnCancel.Location = new Point(849, 499);
            rmBtnCancel.Name = "rmBtnCancel";
            rmBtnCancel.Size = new Size(117, 42);
            rmBtnCancel.TabIndex = 6;
            rmBtnCancel.Text = "Cancel";
            rmBtnCancel.UseVisualStyleBackColor = true;
            // 
            // rmSearchId
            // 
            rmSearchId.AutoSize = true;
            rmSearchId.Location = new Point(481, 30);
            rmSearchId.Name = "rmSearchId";
            rmSearchId.Size = new Size(90, 19);
            rmSearchId.TabIndex = 0;
            rmSearchId.TabStop = true;
            rmSearchId.Text = "Customer Id";
            rmSearchId.UseVisualStyleBackColor = true;
            // 
            // rmBtnReset
            // 
            rmBtnReset.Location = new Point(972, 499);
            rmBtnReset.Name = "rmBtnReset";
            rmBtnReset.Size = new Size(117, 42);
            rmBtnReset.TabIndex = 5;
            rmBtnReset.Text = "Reset";
            rmBtnReset.UseVisualStyleBackColor = true;
            // 
            // rmBtnRemove
            // 
            rmBtnRemove.Location = new Point(1095, 499);
            rmBtnRemove.Name = "rmBtnRemove";
            rmBtnRemove.Size = new Size(117, 42);
            rmBtnRemove.TabIndex = 4;
            rmBtnRemove.Text = "Remove";
            rmBtnRemove.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 92);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1206, 394);
            dataGridView1.TabIndex = 3;
            // 
            // btnRmSearch
            // 
            btnRmSearch.Location = new Point(792, 44);
            btnRmSearch.Name = "btnRmSearch";
            btnRmSearch.Size = new Size(117, 42);
            btnRmSearch.TabIndex = 1;
            btnRmSearch.Text = "Search";
            btnRmSearch.UseVisualStyleBackColor = true;
            // 
            // rmSearchTxt
            // 
            rmSearchTxt.Location = new Point(364, 55);
            rmSearchTxt.Name = "rmSearchTxt";
            rmSearchTxt.Size = new Size(422, 23);
            rmSearchTxt.TabIndex = 0;
            // 
            // RemoveCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1242, 576);
            Controls.Add(grpRemove);
            Name = "RemoveCustomers";
            Text = "Remove Customers";
            grpRemove.ResumeLayout(false);
            grpRemove.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        public GroupBox grpRemove;
        public Button btnRmSearch;
        public TextBox rmSearchTxt;
        public DataGridView dataGridView1;
        public Button rmBtnCancel;
        public Button rmBtnReset;
        public Button rmBtnRemove;
        private Label searchOptions;
        public RadioButton rmSearchName;
        public RadioButton rmSearchProjId;
        public RadioButton rmSearchId;
    }
}