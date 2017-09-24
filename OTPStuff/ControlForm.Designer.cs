using System.Windows.Forms;
namespace OTPStuff
{
    partial class ControlForm
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
            this.implantButton = new System.Windows.Forms.Button();
            this.extractButton = new System.Windows.Forms.Button();
            this.implantOTPButton = new System.Windows.Forms.Button();
            this.extractOTPButton = new System.Windows.Forms.Button();
            this.straightEncryptionButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // implantButton
            // 
            this.implantButton.Location = new System.Drawing.Point(100, 80);
            this.implantButton.Name = "implantButton";
            this.implantButton.Size = new System.Drawing.Size(150, 50);
            this.implantButton.TabIndex = 5;
            this.implantButton.Text = "Implant";
            this.implantButton.UseVisualStyleBackColor = true;
            this.implantButton.Click += new System.EventHandler(this.implantButton_Click);
            // 
            // extractButton
            // 
            this.extractButton.Location = new System.Drawing.Point(300, 80);
            this.extractButton.Name = "extractButton";
            this.extractButton.Size = new System.Drawing.Size(150, 50);
            this.extractButton.TabIndex = 6;
            this.extractButton.Text = "Extract";
            this.extractButton.UseVisualStyleBackColor = true;
            this.extractButton.Click += new System.EventHandler(this.extractButton_Click);
            // 
            // implantOTPButton
            // 
            this.implantOTPButton.Location = new System.Drawing.Point(100, 150);
            this.implantOTPButton.Name = "implantOTPButton";
            this.implantOTPButton.Size = new System.Drawing.Size(150, 50);
            this.implantOTPButton.TabIndex = 5;
            this.implantOTPButton.Text = "Implant w/ OTP";
            this.implantOTPButton.UseVisualStyleBackColor = true;
            this.implantOTPButton.Click += new System.EventHandler(this.implantOTPButton_Click);
            // 
            // extractOTPButton
            // 
            this.extractOTPButton.Location = new System.Drawing.Point(300, 150);
            this.extractOTPButton.Name = "extractOTPButton";
            this.extractOTPButton.Size = new System.Drawing.Size(150, 50);
            this.extractOTPButton.TabIndex = 6;
            this.extractOTPButton.Text = "Extract w/ OTP";
            this.extractOTPButton.UseVisualStyleBackColor = true;
            this.extractOTPButton.Click += new System.EventHandler(this.extractOTPButton_Click);
            // 
            // straightEncryptionButton
            // 
            this.straightEncryptionButton.Location = new System.Drawing.Point(200, 220);
            this.straightEncryptionButton.Name = "straightEncryptionButton";
            this.straightEncryptionButton.Size = new System.Drawing.Size(150, 50);
            this.straightEncryptionButton.TabIndex = 7;
            this.straightEncryptionButton.Text = "Encryption Tools";
            this.straightEncryptionButton.UseVisualStyleBackColor = true;
            this.straightEncryptionButton.Click += new System.EventHandler(this.straightEncryptionButton_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 370);
            this.Controls.Add(this.straightEncryptionButton);
            this.Controls.Add(this.extractButton);
            this.Controls.Add(this.implantButton);
            this.Controls.Add(this.extractOTPButton);
            this.Controls.Add(this.implantOTPButton);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "ControlForm";
            this.Text = "Stegonography";
            this.ResumeLayout(false);

        }

        #endregion
        private Button implantButton;
        private Button extractButton;
        private Button implantOTPButton;
        private Button extractOTPButton;
        private Button straightEncryptionButton;
    }
}

