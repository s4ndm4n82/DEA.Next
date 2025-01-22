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
            dataGridView1 = new DataGridView();
            rmSearchUsing = new GroupBox();
            rmSearchName = new RadioButton();
            rmSearchProjId = new RadioButton();
            rmSearchId = new RadioButton();
            btnRmIdSearch = new Button();
            rmSerchIdTxt = new TextBox();
            rmBtnRemove = new Button();
            rmBtnReset = new Button();
            rmBtnCancel = new Button();
            grpRemove.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            rmSearchUsing.SuspendLayout();
            SuspendLayout();
            // 
            // grpRemove
            // 
            grpRemove.Controls.Add(rmBtnCancel);
            grpRemove.Controls.Add(rmBtnReset);
            grpRemove.Controls.Add(rmBtnRemove);
            grpRemove.Controls.Add(dataGridView1);
            grpRemove.Controls.Add(rmSearchUsing);
            grpRemove.Controls.Add(btnRmIdSearch);
            grpRemove.Controls.Add(rmSerchIdTxt);
            grpRemove.Location = new Point(12, 12);
            grpRemove.Name = "grpRemove";
            grpRemove.Size = new Size(1218, 552);
            grpRemove.TabIndex = 0;
            grpRemove.TabStop = false;
            grpRemove.Text = "Customer Remove";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 110);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(1206, 376);
            dataGridView1.TabIndex = 3;
            // 
            // rmSearchUsing
            // 
            rmSearchUsing.Controls.Add(rmSearchName);
            rmSearchUsing.Controls.Add(rmSearchProjId);
            rmSearchUsing.Controls.Add(rmSearchId);
            rmSearchUsing.Location = new Point(453, 22);
            rmSearchUsing.Name = "rmSearchUsing";
            rmSearchUsing.Size = new Size(299, 53);
            rmSearchUsing.TabIndex = 2;
            rmSearchUsing.TabStop = false;
            rmSearchUsing.Text = "Search Using:";
            // 
            // rmSearchName
            // 
            rmSearchName.AutoSize = true;
            rmSearchName.Location = new Point(183, 22);
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
            rmSearchProjId.Location = new Point(102, 22);
            rmSearchProjId.Name = "rmSearchProjId";
            rmSearchProjId.Size = new Size(75, 19);
            rmSearchProjId.TabIndex = 1;
            rmSearchProjId.TabStop = true;
            rmSearchProjId.Text = "Project Id";
            rmSearchProjId.UseVisualStyleBackColor = true;
            // 
            // rmSearchId
            // 
            rmSearchId.AutoSize = true;
            rmSearchId.Location = new Point(6, 22);
            rmSearchId.Name = "rmSearchId";
            rmSearchId.Size = new Size(90, 19);
            rmSearchId.TabIndex = 0;
            rmSearchId.TabStop = true;
            rmSearchId.Text = "Customer Id";
            rmSearchId.UseVisualStyleBackColor = true;
            // 
            // btnRmIdSearch
            // 
            btnRmIdSearch.Location = new Point(803, 81);
            btnRmIdSearch.Name = "btnRmIdSearch";
            btnRmIdSearch.Size = new Size(75, 23);
            btnRmIdSearch.TabIndex = 1;
            btnRmIdSearch.Text = "Search";
            btnRmIdSearch.UseVisualStyleBackColor = true;
            // 
            // rmSerchIdTxt
            // 
            rmSerchIdTxt.Location = new Point(375, 81);
            rmSerchIdTxt.Name = "rmSerchIdTxt";
            rmSerchIdTxt.Size = new Size(422, 23);
            rmSerchIdTxt.TabIndex = 0;
            rmSerchIdTxt.Text = "Search Using Customer Id ...";
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
            // rmBtnReset
            // 
            rmBtnReset.Location = new Point(972, 499);
            rmBtnReset.Name = "rmBtnReset";
            rmBtnReset.Size = new Size(117, 42);
            rmBtnReset.TabIndex = 5;
            rmBtnReset.Text = "Reset";
            rmBtnReset.UseVisualStyleBackColor = true;
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
            // RemoveCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1242, 576);
            Controls.Add(grpRemove);
            Name = "RemoveCustomers";
            Text = "RemoveCustomers";
            grpRemove.ResumeLayout(false);
            grpRemove.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            rmSearchUsing.ResumeLayout(false);
            rmSearchUsing.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpRemove;
        private Button btnRmIdSearch;
        private TextBox rmSerchIdTxt;
        private GroupBox rmSearchUsing;
        private RadioButton rmSearchName;
        private RadioButton rmSearchProjId;
        private RadioButton rmSearchId;
        private DataGridView dataGridView1;
        private Button rmBtnCancel;
        private Button rmBtnReset;
        private Button rmBtnRemove;
    }
}