namespace DEA.UI
{
    partial class EditCustomers
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
            CusStus = new Label();
            SuspendLayout();
            // 
            // CusStus
            // 
            CusStus.AutoSize = true;
            CusStus.Location = new Point(12, 9);
            CusStus.Name = "CusStus";
            CusStus.Size = new Size(94, 15);
            CusStus.TabIndex = 0;
            CusStus.Text = "Customer Status";
            // 
            // EditCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(800, 450);
            Controls.Add(CusStus);
            Name = "EditCustomers";
            Text = "EditCustomers";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label CusStus;
    }
}