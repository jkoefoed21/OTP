using System.Drawing;

namespace OTPStuff
{
    partial class OTPExtractForm
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
            this.pictureInBox = new System.Windows.Forms.TextBox();
            this.messageInBox = new System.Windows.Forms.TextBox();
            this.messageOutBox = new System.Windows.Forms.TextBox();
            this.passBox = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.pictureInSelectButton = new System.Windows.Forms.Button();
            this.messageInSelectButton = new System.Windows.Forms.Button();
            this.messageOutSelectButton = new System.Windows.Forms.Button();
            this.picInHeaderLabel = new System.Windows.Forms.Label();
            this.msgInHeaderLabel = new System.Windows.Forms.Label();
            this.msgOutHeaderLabel = new System.Windows.Forms.Label();
            this.passwordHeaderLabel = new System.Windows.Forms.Label();
            this.picInStatusLabel = new System.Windows.Forms.Label();
            this.msgInStatusLabel = new System.Windows.Forms.Label();
            this.msgOutStatusLabel = new System.Windows.Forms.Label();
            this.primaryStatusLabel = new System.Windows.Forms.Label();
            this.passwordStatusLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pictureInBox
            // 
            this.pictureInBox.Location = new System.Drawing.Point(40, 60);
            this.pictureInBox.Name = "pictureInBox";
            this.pictureInBox.Size = new System.Drawing.Size(600, 26);
            this.pictureInBox.TabIndex = 0;
            this.pictureInBox.TextChanged += new System.EventHandler(this.pictureInBox_TextChanged);
            // 
            // messageInBox
            // 
            this.messageInBox.Location = new System.Drawing.Point(40, 120);
            this.messageInBox.Name = "pictureInBox";
            this.messageInBox.Size = new System.Drawing.Size(600, 26);
            this.messageInBox.TabIndex = 0;
            this.messageInBox.TextChanged += new System.EventHandler(this.messageInBox_TextChanged);
            // 
            // messageOutBox
            // 
            this.messageOutBox.Location = new System.Drawing.Point(40, 180);
            this.messageOutBox.Name = "messageOutBox";
            this.messageOutBox.Size = new System.Drawing.Size(600, 26);
            this.messageOutBox.TabIndex = 1;
            this.messageOutBox.TextChanged += new System.EventHandler(this.messageOutBox_TextChanged);
            // 
            // passBox
            // 
            this.passBox.Location = new System.Drawing.Point(40, 240);
            this.passBox.Name = "passBox";
            this.passBox.PasswordChar = '*';
            this.passBox.Size = new System.Drawing.Size(600, 26);
            this.passBox.TabIndex = 2;
            this.passBox.TextChanged += new System.EventHandler(this.passBox_TextChanged);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(40, 300);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(600, 50);
            this.runButton.TabIndex = 5;
            this.runButton.Text = "Extract";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // pictureInSelectButton
            // 
            this.pictureInSelectButton.Location = new System.Drawing.Point(700, 55);
            this.pictureInSelectButton.Name = "pictureInSelectButton";
            this.pictureInSelectButton.Size = new System.Drawing.Size(150, 40);
            this.pictureInSelectButton.TabIndex = 6;
            this.pictureInSelectButton.Text = "Select File";
            this.pictureInSelectButton.UseVisualStyleBackColor = true;
            this.pictureInSelectButton.Click += new System.EventHandler(this.pictureInSelectButton_Click);
            // 
            // messageInSelectButton
            // 
            this.messageInSelectButton.Location = new System.Drawing.Point(700, 115);
            this.messageInSelectButton.Name = "messageInSelectButton";
            this.messageInSelectButton.Size = new System.Drawing.Size(150, 40);
            this.messageInSelectButton.TabIndex = 6;
            this.messageInSelectButton.Text = "Select File";
            this.messageInSelectButton.UseVisualStyleBackColor = true;
            this.messageInSelectButton.Click += new System.EventHandler(this.messageInSelectButton_Click);
            // 
            // messageOutSelectButton
            // 
            this.messageOutSelectButton.Location = new System.Drawing.Point(700, 175);
            this.messageOutSelectButton.Name = "messageOutSelectButton";
            this.messageOutSelectButton.Size = new System.Drawing.Size(150, 40);
            this.messageOutSelectButton.TabIndex = 7;
            this.messageOutSelectButton.Text = "Select File";
            this.messageOutSelectButton.UseVisualStyleBackColor = true;
            this.messageOutSelectButton.Click += new System.EventHandler(this.messageOutSelectButton_Click);
            // 
            // picInHeaderLabel
            // 
            this.picInHeaderLabel.AutoSize = true;
            this.picInHeaderLabel.Location = new System.Drawing.Point(40, 40);
            this.picInHeaderLabel.Name = "picInHeaderLabel";
            this.picInHeaderLabel.Size = new System.Drawing.Size(169, 20);
            this.picInHeaderLabel.TabIndex = 9;
            this.picInHeaderLabel.Text = "Picture Input File Path:";
            // 
            // msgInHeaderLabel
            // 
            this.msgInHeaderLabel.AutoSize = true;
            this.msgInHeaderLabel.Location = new System.Drawing.Point(40, 100);
            this.msgInHeaderLabel.Name = "msgInHeaderLabel";
            this.msgInHeaderLabel.Size = new System.Drawing.Size(169, 20);
            this.msgInHeaderLabel.TabIndex = 9;
            this.msgInHeaderLabel.Text = "Message Input File Path:";
            // 
            // msgOutHeaderLabel
            // 
            this.msgOutHeaderLabel.AutoSize = true;
            this.msgOutHeaderLabel.Location = new System.Drawing.Point(40, 160);
            this.msgOutHeaderLabel.Name = "msgOutHeaderLabel";
            this.msgOutHeaderLabel.Size = new System.Drawing.Size(197, 20);
            this.msgOutHeaderLabel.TabIndex = 10;
            this.msgOutHeaderLabel.Text = "Message Output File Path:";
            // 
            // passwordHeaderLabel
            // 
            this.passwordHeaderLabel.AutoSize = true;
            this.passwordHeaderLabel.Location = new System.Drawing.Point(40, 220);
            this.passwordHeaderLabel.Name = "passwordHeaderLabel";
            this.passwordHeaderLabel.Size = new System.Drawing.Size(125, 20);
            this.passwordHeaderLabel.TabIndex = 11;
            this.passwordHeaderLabel.Text = "Enter Password:";
            // 
            // picInStatusLabel
            // 
            this.picInStatusLabel.AutoSize = true;
            this.picInStatusLabel.Location = new System.Drawing.Point(900, 60);
            this.picInStatusLabel.Name = "picInStatusLabel";
            this.picInStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.picInStatusLabel.TabIndex = 16;
            this.picInStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // msgInStatusLabel
            // 
            this.msgInStatusLabel.AutoSize = true;
            this.msgInStatusLabel.Location = new System.Drawing.Point(900, 120);
            this.msgInStatusLabel.Name = "msgInStatusLabel";
            this.msgInStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.msgInStatusLabel.TabIndex = 16;
            this.msgInStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // msgOutStatusLabel
            // 
            this.msgOutStatusLabel.AutoSize = true;
            this.msgOutStatusLabel.Location = new System.Drawing.Point(900, 180);
            this.msgOutStatusLabel.Name = "msgOutStatusLabel";
            this.msgOutStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.msgOutStatusLabel.TabIndex = 17;
            this.msgOutStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // primaryStatusLabel
            // 
            this.primaryStatusLabel.AutoSize = true;
            this.primaryStatusLabel.Location = new System.Drawing.Point(700, 300);
            this.primaryStatusLabel.MaximumSize = new System.Drawing.Size(550, 0);
            this.primaryStatusLabel.Name = "primaryStatusLabel";
            this.primaryStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.primaryStatusLabel.TabIndex = 19;
            // 
            // passwordStatusLabel
            // 
            this.passwordStatusLabel.AutoSize = true;
            this.passwordStatusLabel.Location = new System.Drawing.Point(700, 240);
            this.passwordStatusLabel.Name = "passwordStatusLabel";
            this.passwordStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.passwordStatusLabel.TabIndex = 20;
            // 
            // OTPExtractForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 390);
            this.Controls.Add(this.passwordStatusLabel);
            this.Controls.Add(this.primaryStatusLabel);
            this.Controls.Add(this.msgOutStatusLabel);
            this.Controls.Add(this.msgInStatusLabel);
            this.Controls.Add(this.picInStatusLabel);
            this.Controls.Add(this.passwordHeaderLabel);
            this.Controls.Add(this.msgOutHeaderLabel);
            this.Controls.Add(this.msgInHeaderLabel);
            this.Controls.Add(this.picInHeaderLabel);
            this.Controls.Add(this.messageOutSelectButton);
            this.Controls.Add(this.messageInSelectButton);
            this.Controls.Add(this.pictureInSelectButton);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.passBox);
            this.Controls.Add(this.messageOutBox);
            this.Controls.Add(this.messageInBox);
            this.Controls.Add(this.pictureInBox);
            this.Name = "OTPExtractForm";
            this.Text = "OTP Decoder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pictureInBox;
        private System.Windows.Forms.TextBox messageInBox;
        private System.Windows.Forms.TextBox messageOutBox;
        private System.Windows.Forms.TextBox passBox;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button pictureInSelectButton;
        private System.Windows.Forms.Button messageInSelectButton;
        private System.Windows.Forms.Button messageOutSelectButton;
        private System.Windows.Forms.Label picInHeaderLabel;
        private System.Windows.Forms.Label msgInHeaderLabel;
        private System.Windows.Forms.Label msgOutHeaderLabel;
        private System.Windows.Forms.Label passwordHeaderLabel;
        private System.Windows.Forms.Label msgOutStatusLabel;
        private System.Windows.Forms.Label msgInStatusLabel;
        private System.Windows.Forms.Label picInStatusLabel;
        private System.Windows.Forms.Label primaryStatusLabel;
        private System.Windows.Forms.Label passwordStatusLabel;
    }
}