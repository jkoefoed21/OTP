using System.Drawing;

namespace OTPStuff
{
    partial class OTPImplantForm
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
            this.picOutBox = new System.Windows.Forms.TextBox();
            this.pass1Box = new System.Windows.Forms.TextBox();
            this.pass2Box = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.pictureInSelectButton = new System.Windows.Forms.Button();
            this.messageInSelectButton = new System.Windows.Forms.Button();
            this.messageOutSelectButton = new System.Windows.Forms.Button();
            this.picOutSelectButton = new System.Windows.Forms.Button();
            this.picInHeaderLabel = new System.Windows.Forms.Label();
            this.msgInHeaderLabel = new System.Windows.Forms.Label();
            this.msgOutHeaderLabel = new System.Windows.Forms.Label();
            this.picOutHeaderLabel = new System.Windows.Forms.Label();
            this.pass1HeaderLabel = new System.Windows.Forms.Label();
            this.pass2HeaderLabel = new System.Windows.Forms.Label();
            this.picInStatusLabel = new System.Windows.Forms.Label();
            this.msgInStatusLabel = new System.Windows.Forms.Label();
            this.picOutStatusLabel = new System.Windows.Forms.Label();
            this.msgOutStatusLabel = new System.Windows.Forms.Label();
            this.primaryStatusLabel = new System.Windows.Forms.Label();
            this.pass1StatusLabel = new System.Windows.Forms.Label();
            this.pass2StatusLabel = new System.Windows.Forms.Label();
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
            this.messageInBox.Name = "messageInBox";
            this.messageInBox.Size = new System.Drawing.Size(600, 26);
            this.messageInBox.TabIndex = 1;
            this.messageInBox.TextChanged += new System.EventHandler(this.messageInBox_TextChanged);
            // 
            // picOutBox
            // 
            this.picOutBox.Location = new System.Drawing.Point(40, 180);
            this.picOutBox.Name = "picOutBox";
            this.picOutBox.Size = new System.Drawing.Size(600, 26);
            this.picOutBox.TabIndex = 2;
            this.picOutBox.TextChanged += new System.EventHandler(this.picOutBox_TextChanged);
            // 
            // msgOutBox
            // 
            this.messageOutBox.Location = new System.Drawing.Point(40, 240);
            this.messageOutBox.Name = "messageOutBox";
            this.messageOutBox.Size = new System.Drawing.Size(600, 26);
            this.messageOutBox.TabIndex = 2;
            this.messageOutBox.TextChanged += new System.EventHandler(this.messageOutBox_TextChanged);
            // 
            // pass1Box
            // 
            this.pass1Box.Location = new System.Drawing.Point(40, 300);
            this.pass1Box.Name = "pass1Box";
            this.pass1Box.PasswordChar = '*';
            this.pass1Box.Size = new System.Drawing.Size(600, 26);
            this.pass1Box.TabIndex = 3;
            this.pass1Box.TextChanged += new System.EventHandler(this.pass1Box_TextChanged);
            // 
            // pass2Box
            // 
            this.pass2Box.Location = new System.Drawing.Point(40, 360);
            this.pass2Box.Name = "pass2Box";
            this.pass2Box.PasswordChar = '*';
            this.pass2Box.Size = new System.Drawing.Size(600, 26);
            this.pass2Box.TabIndex = 4;
            this.pass2Box.TextChanged += new System.EventHandler(this.pass2Box_TextChanged);
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(40, 420);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(600, 50);
            this.runButton.TabIndex = 5;
            this.runButton.Text = "Implant";
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
            this.messageInSelectButton.TabIndex = 7;
            this.messageInSelectButton.Text = "Select File";
            this.messageInSelectButton.UseVisualStyleBackColor = true;
            this.messageInSelectButton.Click += new System.EventHandler(this.messageInSelectButton_Click);
            // 
            // picOutSelectButton
            // 
            this.picOutSelectButton.Location = new System.Drawing.Point(700, 175);
            this.picOutSelectButton.Name = "picOutSelectButton";
            this.picOutSelectButton.Size = new System.Drawing.Size(150, 40);
            this.picOutSelectButton.TabIndex = 8;
            this.picOutSelectButton.Text = "Select File";
            this.picOutSelectButton.UseVisualStyleBackColor = true;
            this.picOutSelectButton.Click += new System.EventHandler(this.picOutSelectButton_Click);
            // 
            // messageOutSelectButton
            // 
            this.messageOutSelectButton.Location = new System.Drawing.Point(700, 235);
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
            this.msgInHeaderLabel.Size = new System.Drawing.Size(185, 20);
            this.msgInHeaderLabel.TabIndex = 10;
            this.msgInHeaderLabel.Text = "Message Input File Path:";
            // 
            // picOutHeaderLabel
            // 
            this.picOutHeaderLabel.AutoSize = true;
            this.picOutHeaderLabel.Location = new System.Drawing.Point(40, 160);
            this.picOutHeaderLabel.Name = "picOutHeaderLabel";
            this.picOutHeaderLabel.Size = new System.Drawing.Size(181, 20);
            this.picOutHeaderLabel.TabIndex = 11;
            this.picOutHeaderLabel.Text = "Picture Output File Path:";
            // 
            // msgOutHeaderLabel
            // 
            this.msgOutHeaderLabel.AutoSize = true;
            this.msgOutHeaderLabel.Location = new System.Drawing.Point(40, 220);
            this.msgOutHeaderLabel.Name = "msgOutHeaderLabel";
            this.msgOutHeaderLabel.Size = new System.Drawing.Size(181, 20);
            this.msgOutHeaderLabel.TabIndex = 11;
            this.msgOutHeaderLabel.Text = "Message Output File Path:";
            // 
            // pass1HeaderLabel
            // 
            this.pass1HeaderLabel.AutoSize = true;
            this.pass1HeaderLabel.Location = new System.Drawing.Point(40, 280);
            this.pass1HeaderLabel.Name = "pass1HeaderLabel";
            this.pass1HeaderLabel.Size = new System.Drawing.Size(125, 20);
            this.pass1HeaderLabel.TabIndex = 12;
            this.pass1HeaderLabel.Text = "Enter Password:";
            // 
            // pass2HeaderLabel
            // 
            this.pass2HeaderLabel.AutoSize = true;
            this.pass2HeaderLabel.Location = new System.Drawing.Point(40, 340);
            this.pass2HeaderLabel.Name = "pass2HeaderLabel";
            this.pass2HeaderLabel.Size = new System.Drawing.Size(144, 20);
            this.pass2HeaderLabel.TabIndex = 13;
            this.pass2HeaderLabel.Text = "Reenter Password:";
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
            this.msgInStatusLabel.TabIndex = 17;
            this.msgInStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // picOutStatusLabel
            // 
            this.picOutStatusLabel.AutoSize = true;
            this.picOutStatusLabel.Location = new System.Drawing.Point(900, 180);
            this.picOutStatusLabel.Name = "picOutStatusLabel";
            this.picOutStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.picOutStatusLabel.TabIndex = 18;
            this.picOutStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // msgOutStatusLabel
            // 
            this.msgOutStatusLabel.AutoSize = true;
            this.msgOutStatusLabel.Location = new System.Drawing.Point(900, 240);
            this.msgOutStatusLabel.Name = "msgOutStatusLabel";
            this.msgOutStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.msgOutStatusLabel.TabIndex = 18;
            this.msgOutStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // primaryStatusLabel
            // 
            this.primaryStatusLabel.AutoSize = true;
            this.primaryStatusLabel.Location = new System.Drawing.Point(700, 420);
            this.primaryStatusLabel.MaximumSize = new System.Drawing.Size(550, 0);
            this.primaryStatusLabel.Name = "primaryStatusLabel";
            this.primaryStatusLabel.Size = new System.Drawing.Size(0, 20);
            this.primaryStatusLabel.TabIndex = 19;
            // 
            // pass1StatusLabel
            // 
            this.pass1StatusLabel.AutoSize = true;
            this.pass1StatusLabel.Location = new System.Drawing.Point(700, 300);
            this.pass1StatusLabel.Name = "pass1StatusLabel";
            this.pass1StatusLabel.Size = new System.Drawing.Size(0, 20);
            this.pass1StatusLabel.TabIndex = 20;
            // 
            // pass2StatusLabel
            // 
            this.pass2StatusLabel.AutoSize = true;
            this.pass2StatusLabel.Location = new System.Drawing.Point(700, 360);
            this.pass2StatusLabel.Name = "pass2StatusLabel";
            this.pass2StatusLabel.Size = new System.Drawing.Size(0, 20);
            this.pass2StatusLabel.TabIndex = 21;
            // 
            // ImplantForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1300, 510);
            this.Controls.Add(this.pass2StatusLabel);
            this.Controls.Add(this.pass1StatusLabel);
            this.Controls.Add(this.primaryStatusLabel);
            this.Controls.Add(this.picOutStatusLabel);
            this.Controls.Add(this.msgOutStatusLabel);
            this.Controls.Add(this.msgInStatusLabel);
            this.Controls.Add(this.msgOutHeaderLabel);
            this.Controls.Add(this.picInStatusLabel);
            this.Controls.Add(this.pass2HeaderLabel);
            this.Controls.Add(this.pass1HeaderLabel);
            this.Controls.Add(this.picOutHeaderLabel);
            this.Controls.Add(this.msgInHeaderLabel);
            this.Controls.Add(this.picInHeaderLabel);
            this.Controls.Add(this.picOutSelectButton);
            this.Controls.Add(this.messageInSelectButton);
            this.Controls.Add(this.messageOutSelectButton);
            this.Controls.Add(this.pictureInSelectButton);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.pass2Box);
            this.Controls.Add(this.pass1Box);
            this.Controls.Add(this.picOutBox);
            this.Controls.Add(this.messageInBox);
            this.Controls.Add(this.messageOutBox);
            this.Controls.Add(this.pictureInBox);
            this.Name = "OTPImplantForm";
            this.Text = "Stegonography Encoder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pictureInBox;
        private System.Windows.Forms.TextBox messageInBox;
        private System.Windows.Forms.TextBox messageOutBox;
        private System.Windows.Forms.TextBox picOutBox;
        private System.Windows.Forms.TextBox pass1Box;
        private System.Windows.Forms.TextBox pass2Box;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button pictureInSelectButton;
        private System.Windows.Forms.Button messageInSelectButton;
        private System.Windows.Forms.Button messageOutSelectButton;
        private System.Windows.Forms.Button picOutSelectButton;
        private System.Windows.Forms.Label picInHeaderLabel;
        private System.Windows.Forms.Label msgInHeaderLabel;
        private System.Windows.Forms.Label msgOutHeaderLabel;
        private System.Windows.Forms.Label picOutHeaderLabel;
        private System.Windows.Forms.Label pass1HeaderLabel;
        private System.Windows.Forms.Label pass2HeaderLabel;
        private System.Windows.Forms.Label picOutStatusLabel;
        private System.Windows.Forms.Label msgOutStatusLabel;
        private System.Windows.Forms.Label msgInStatusLabel;
        private System.Windows.Forms.Label picInStatusLabel;
        private System.Windows.Forms.Label primaryStatusLabel;
        private System.Windows.Forms.Label pass2StatusLabel;
        private System.Windows.Forms.Label pass1StatusLabel;
    }
}