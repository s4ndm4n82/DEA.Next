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
            dataContextBindingSource = new BindingSource(components);
            customerDetailsBindingSource = new BindingSource(components);
            dataContextBindingSource1 = new BindingSource(components);
            customerDetailsModelBindingSource = new BindingSource(components);
            cusStatusLbl = new Label();
            cusOn = new RadioButton();
            cusOff = new RadioButton();
            cusNameLbl = new Label();
            cusNameTxt = new TextBox();
            cusUnameLbl = new Label();
            grpBoxCustomer = new GroupBox();
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
            cusFoneNameLbl = new Label();
            cusFonNameVal = new TextBox();
            cusFoneValLbl = new Label();
            cusFonValTxt = new TextBox();
            cusFtwoNameLbl = new Label();
            custFtwoValTxt = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataContextBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)customerDetailsBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataContextBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)customerDetailsModelBindingSource).BeginInit();
            grpBoxCustomer.SuspendLayout();
            SuspendLayout();
            // 
            // dataContextBindingSource
            // 
            dataContextBindingSource.DataSource = typeof(Next.Data.DataContext);
            // 
            // customerDetailsBindingSource
            // 
            customerDetailsBindingSource.DataSource = typeof(Next.Entities.CustomerDetails);
            // 
            // dataContextBindingSource1
            // 
            dataContextBindingSource1.DataSource = typeof(Next.Data.DataContext);
            // 
            // customerDetailsModelBindingSource
            // 
            customerDetailsModelBindingSource.DataSource = typeof(Next.Models.CustomerDetailsModel);
            // 
            // cusStatusLbl
            // 
            cusStatusLbl.AutoSize = true;
            cusStatusLbl.Location = new Point(14, 19);
            cusStatusLbl.Name = "cusStatusLbl";
            cusStatusLbl.Size = new Size(97, 15);
            cusStatusLbl.TabIndex = 0;
            cusStatusLbl.Text = "Customer Status:";
            // 
            // cusOn
            // 
            cusOn.AutoSize = true;
            cusOn.Location = new Point(14, 36);
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
            cusOff.Location = new Point(87, 36);
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
            cusNameLbl.Location = new Point(14, 58);
            cusNameLbl.Name = "cusNameLbl";
            cusNameLbl.Size = new Size(97, 15);
            cusNameLbl.TabIndex = 3;
            cusNameLbl.Text = "Customer Name:";
            // 
            // cusNameTxt
            // 
            cusNameTxt.Location = new Point(14, 76);
            cusNameTxt.Name = "cusNameTxt";
            cusNameTxt.Size = new Size(457, 23);
            cusNameTxt.TabIndex = 4;
            // 
            // cusUnameLbl
            // 
            cusUnameLbl.AutoSize = true;
            cusUnameLbl.Location = new Point(14, 102);
            cusUnameLbl.Name = "cusUnameLbl";
            cusUnameLbl.Size = new Size(123, 15);
            cusUnameLbl.TabIndex = 5;
            cusUnameLbl.Text = "Customer User Name:";
            // 
            // grpBoxCustomer
            // 
            grpBoxCustomer.Controls.Add(custFtwoValTxt);
            grpBoxCustomer.Controls.Add(cusFtwoNameLbl);
            grpBoxCustomer.Controls.Add(cusFonValTxt);
            grpBoxCustomer.Controls.Add(cusFoneValLbl);
            grpBoxCustomer.Controls.Add(cusFonNameVal);
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
            grpBoxCustomer.Controls.Add(cusOff);
            grpBoxCustomer.Controls.Add(cusUnameLbl);
            grpBoxCustomer.Controls.Add(cusStatusLbl);
            grpBoxCustomer.Controls.Add(cusNameTxt);
            grpBoxCustomer.Controls.Add(cusOn);
            grpBoxCustomer.Controls.Add(cusNameLbl);
            grpBoxCustomer.Location = new Point(12, 12);
            grpBoxCustomer.Name = "grpBoxCustomer";
            grpBoxCustomer.Size = new Size(486, 827);
            grpBoxCustomer.TabIndex = 6;
            grpBoxCustomer.TabStop = false;
            grpBoxCustomer.Text = "Customer Details";
            // 
            // cusDocencTxt
            // 
            cusDocencTxt.Location = new Point(14, 519);
            cusDocencTxt.Name = "cusDocencTxt";
            cusDocencTxt.Size = new Size(123, 23);
            cusDocencTxt.TabIndex = 20;
            cusDocencTxt.Text = "UTF-8";
            // 
            // cusDocEncoLbl
            // 
            cusDocEncoLbl.AutoSize = true;
            cusDocEncoLbl.Location = new Point(14, 501);
            cusDocEncoLbl.Name = "cusDocEncoLbl";
            cusDocEncoLbl.Size = new Size(174, 15);
            cusDocEncoLbl.TabIndex = 19;
            cusDocEncoLbl.Text = "Customer Document Encoding:";
            // 
            // cusMaxBatchTxt
            // 
            cusMaxBatchTxt.Location = new Point(14, 475);
            cusMaxBatchTxt.Name = "cusMaxBatchTxt";
            cusMaxBatchTxt.Size = new Size(123, 23);
            cusMaxBatchTxt.TabIndex = 18;
            cusMaxBatchTxt.Text = "1";
            // 
            // cusMaxBatchLbl
            // 
            cusMaxBatchLbl.AutoSize = true;
            cusMaxBatchLbl.Location = new Point(14, 457);
            cusMaxBatchLbl.Name = "cusMaxBatchLbl";
            cusMaxBatchLbl.Size = new Size(147, 15);
            cusMaxBatchLbl.TabIndex = 17;
            cusMaxBatchLbl.Text = "Customer Max. Batch Size:";
            // 
            // cusDocIdTxt
            // 
            cusDocIdTxt.Location = new Point(14, 431);
            cusDocIdTxt.Name = "cusDocIdTxt";
            cusDocIdTxt.Size = new Size(457, 23);
            cusDocIdTxt.TabIndex = 16;
            // 
            // cusDocIdLbl
            // 
            cusDocIdLbl.AutoSize = true;
            cusDocIdLbl.Location = new Point(14, 413);
            cusDocIdLbl.Name = "cusDocIdLbl";
            cusDocIdLbl.Size = new Size(134, 15);
            cusDocIdLbl.TabIndex = 15;
            cusDocIdLbl.Text = "Customer Document Id:";
            // 
            // cusTempIdTxt
            // 
            cusTempIdTxt.Location = new Point(14, 387);
            cusTempIdTxt.Name = "cusTempIdTxt";
            cusTempIdTxt.Size = new Size(457, 23);
            cusTempIdTxt.TabIndex = 14;
            // 
            // cusTempKeyLbl
            // 
            cusTempKeyLbl.AutoSize = true;
            cusTempKeyLbl.Location = new Point(14, 369);
            cusTempKeyLbl.Name = "cusTempKeyLbl";
            cusTempKeyLbl.Size = new Size(135, 15);
            cusTempKeyLbl.TabIndex = 13;
            cusTempKeyLbl.Text = "Customer Template Key:";
            // 
            // cusProjIdTxt
            // 
            cusProjIdTxt.Location = new Point(14, 343);
            cusProjIdTxt.Name = "cusProjIdTxt";
            cusProjIdTxt.Size = new Size(457, 23);
            cusProjIdTxt.TabIndex = 12;
            // 
            // cusProjectLbl
            // 
            cusProjectLbl.AutoSize = true;
            cusProjectLbl.Location = new Point(14, 325);
            cusProjectLbl.Name = "cusProjectLbl";
            cusProjectLbl.Size = new Size(115, 15);
            cusProjectLbl.TabIndex = 11;
            cusProjectLbl.Text = "Customer Project Id:";
            // 
            // cusQueuTxt
            // 
            cusQueuTxt.Location = new Point(14, 299);
            cusQueuTxt.Name = "cusQueuTxt";
            cusQueuTxt.Size = new Size(123, 23);
            cusQueuTxt.TabIndex = 10;
            // 
            // cusQueueLbl
            // 
            cusQueueLbl.AutoSize = true;
            cusQueueLbl.Location = new Point(14, 281);
            cusQueueLbl.Name = "cusQueueLbl";
            cusQueueLbl.Size = new Size(100, 15);
            cusQueueLbl.TabIndex = 9;
            cusQueueLbl.Text = "Customer Queue:";
            // 
            // cusApiTokenTxt
            // 
            cusApiTokenTxt.Location = new Point(14, 164);
            cusApiTokenTxt.Multiline = true;
            cusApiTokenTxt.Name = "cusApiTokenTxt";
            cusApiTokenTxt.ScrollBars = ScrollBars.Vertical;
            cusApiTokenTxt.Size = new Size(457, 114);
            cusApiTokenTxt.TabIndex = 8;
            // 
            // cusTokeLbl
            // 
            cusTokeLbl.AutoSize = true;
            cusTokeLbl.Location = new Point(14, 146);
            cusTokeLbl.Name = "cusTokeLbl";
            cusTokeLbl.Size = new Size(110, 15);
            cusTokeLbl.TabIndex = 7;
            cusTokeLbl.Text = "Customer Api Toke:";
            // 
            // cusUnameTxt
            // 
            cusUnameTxt.Location = new Point(14, 120);
            cusUnameTxt.Name = "cusUnameTxt";
            cusUnameTxt.Size = new Size(457, 23);
            cusUnameTxt.TabIndex = 6;
            // 
            // cusFoneNameLbl
            // 
            cusFoneNameLbl.AutoSize = true;
            cusFoneNameLbl.Location = new Point(14, 545);
            cusFoneNameLbl.Name = "cusFoneNameLbl";
            cusFoneNameLbl.Size = new Size(150, 15);
            cusFoneNameLbl.TabIndex = 21;
            cusFoneNameLbl.Text = "Customer Field One Name:";
            // 
            // cusFonNameVal
            // 
            cusFonNameVal.Location = new Point(14, 563);
            cusFonNameVal.Name = "cusFonNameVal";
            cusFonNameVal.Size = new Size(457, 23);
            cusFonNameVal.TabIndex = 22;
            // 
            // cusFoneValLbl
            // 
            cusFoneValLbl.AutoSize = true;
            cusFoneValLbl.Location = new Point(14, 589);
            cusFoneValLbl.Name = "cusFoneValLbl";
            cusFoneValLbl.Size = new Size(146, 15);
            cusFoneValLbl.TabIndex = 23;
            cusFoneValLbl.Text = "Customer Field One Value:";
            // 
            // cusFonValTxt
            // 
            cusFonValTxt.Location = new Point(14, 607);
            cusFonValTxt.Name = "cusFonValTxt";
            cusFonValTxt.Size = new Size(457, 23);
            cusFonValTxt.TabIndex = 24;
            // 
            // cusFtwoNameLbl
            // 
            cusFtwoNameLbl.AutoSize = true;
            cusFtwoNameLbl.Location = new Point(14, 633);
            cusFtwoNameLbl.Name = "cusFtwoNameLbl";
            cusFtwoNameLbl.Size = new Size(149, 15);
            cusFtwoNameLbl.TabIndex = 25;
            cusFtwoNameLbl.Text = "Customer Field Two Name:";
            // 
            // custFtwoValTxt
            // 
            custFtwoValTxt.Location = new Point(14, 651);
            custFtwoValTxt.Name = "custFtwoValTxt";
            custFtwoValTxt.Size = new Size(457, 23);
            custFtwoValTxt.TabIndex = 26;
            // 
            // AddCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1280, 851);
            Controls.Add(grpBoxCustomer);
            Name = "AddCustomers";
            Text = "AddCustomers";
            ((System.ComponentModel.ISupportInitialize)dataContextBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)customerDetailsBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataContextBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)customerDetailsModelBindingSource).EndInit();
            grpBoxCustomer.ResumeLayout(false);
            grpBoxCustomer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private BindingSource dataContextBindingSource;
        private BindingSource customerDetailsBindingSource;
        private BindingSource dataContextBindingSource1;
        private BindingSource customerDetailsModelBindingSource;
        private Label cusStatusLbl;
        private RadioButton cusOn;
        private RadioButton cusOff;
        private Label cusNameLbl;
        private TextBox cusNameTxt;
        private Label cusUnameLbl;
        private GroupBox grpBoxCustomer;
        private Label cusTokeLbl;
        private TextBox cusUnameTxt;
        private TextBox cusApiTokenTxt;
        private Label cusQueueLbl;
        private TextBox cusDocIdTxt;
        private Label cusDocIdLbl;
        private TextBox cusTempIdTxt;
        private Label cusTempKeyLbl;
        private TextBox cusProjIdTxt;
        private Label cusProjectLbl;
        private TextBox cusQueuTxt;
        private TextBox cusMaxBatchTxt;
        private Label cusMaxBatchLbl;
        private Label cusDocEncoLbl;
        private TextBox cusDocencTxt;
        private TextBox cusFonValTxt;
        private Label cusFoneValLbl;
        private TextBox cusFonNameVal;
        private Label cusFoneNameLbl;
        private TextBox custFtwoValTxt;
        private Label cusFtwoNameLbl;
    }
}