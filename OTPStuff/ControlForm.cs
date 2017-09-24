using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
//using System.Security.Cryptography;
using encryption;
using Stego_Stuff;

namespace OTPStuff
{
    public partial class ControlForm : Form
    {
        
        /// <summary>
        /// Creates the UI.
        /// </summary>
        public ControlForm()
        {
            InitializeComponent();
            //runningFiles = new List<String>();
            //checkIsCurrentlyRunning();
        }

        /// <summary>
        /// Called when the encrypt button is clicked.
        /// </summary>
        /// <param name="sender"> The object calling this method </param>
        /// <param name="e"> Args for the event </param>
        private void implantButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            ImplantForm f = new ImplantForm();
            f.ShowDialog();
            this.Show();
        }
        
        /// <summary>
        /// Called when the decrypt button is clicked.
        /// </summary>
        /// <param name="sender"> The object calling this method </param>
        /// <param name="e"> Args for the event </param>
        private void extractButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            ExtractForm f = new ExtractForm();
            f.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Called when the encrypt button is clicked.
        /// </summary>
        /// <param name="sender"> The object calling this method </param>
        /// <param name="e"> Args for the event </param>
        private void implantOTPButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            OTPImplantForm f = new OTPImplantForm();
            f.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Called when the decrypt button is clicked.
        /// </summary>
        /// <param name="sender"> The object calling this method </param>
        /// <param name="e"> Args for the event </param>
        private void extractOTPButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            OTPExtractForm f = new OTPExtractForm();
            f.ShowDialog();
            this.Show();
        }

        /// <summary>
        /// Redirects to the old encryption mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void straightEncryptionButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Rijndael f = new Rijndael();
            f.ShowDialog();
            this.Show();
        }
    }
}
