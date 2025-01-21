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
            CusName = new Label();
            SuspendLayout();
            // 
            // CusName
            // 
            CusName.AutoSize = true;
            CusName.Location = new Point(12, 9);
            CusName.Name = "CusName";
            CusName.Size = new Size(105, 15);
            CusName.TabIndex = 0;
            CusName.Text = "Customer Remove";
            // 
            // RemoveCustomers
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(800, 450);
            Controls.Add(CusName);
            Name = "RemoveCustomers";
            Text = "RemoveCustomers";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label CusName;
    }
}