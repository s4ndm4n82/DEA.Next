namespace DEA.UI.Forms
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            pictureBox1 = new PictureBox();
            abtGrpBox = new GroupBox();
            abtLink = new LinkLabel();
            abtVersion = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            abtGrpBox.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.DEA_Small;
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(400, 156);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(452, 112);
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // abtGrpBox
            // 
            abtGrpBox.Controls.Add(abtLink);
            abtGrpBox.Controls.Add(abtVersion);
            abtGrpBox.Controls.Add(pictureBox1);
            abtGrpBox.Location = new Point(12, 12);
            abtGrpBox.Name = "abtGrpBox";
            abtGrpBox.Size = new Size(1218, 552);
            abtGrpBox.TabIndex = 1;
            abtGrpBox.TabStop = false;
            // 
            // abtLink
            // 
            abtLink.AutoSize = true;
            abtLink.Font = new Font("Times New Roman", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            abtLink.Location = new Point(548, 305);
            abtLink.Name = "abtLink";
            abtLink.Size = new Size(115, 25);
            abtLink.TabIndex = 3;
            abtLink.TabStop = true;
            abtLink.Text = "edentri.com";
            // 
            // abtVersion
            // 
            abtVersion.AutoSize = true;
            abtVersion.Font = new Font("Times New Roman", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            abtVersion.Location = new Point(521, 281);
            abtVersion.Name = "abtVersion";
            abtVersion.Size = new Size(0, 25);
            abtVersion.TabIndex = 1;
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.AppWorkspace;
            ClientSize = new Size(1242, 576);
            Controls.Add(abtGrpBox);
            Name = "AboutForm";
            Text = "About";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            abtGrpBox.ResumeLayout(false);
            abtGrpBox.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private GroupBox abtGrpBox;
        public Label abtVersion;
        private LinkLabel abtLink;
    }
}