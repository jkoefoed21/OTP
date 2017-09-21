using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using encryption;
using System.Diagnostics;

namespace Stego_Stuff
{
    public partial class ExtractForm : Form
    {
        public ExtractForm()
        {
            InitializeComponent();
        }

        delegate void invokeSetPSLabelText(string text);

        /// <summary>
        /// To be able to adjust the PSL from other threads
        /// </summary>
        /// <param name="text"> text to change to </param>
        private void SetPrimaryStatusLabelText(string text)
        {
            if (primaryStatusLabel.InvokeRequired)
            {
                invokeSetPSLabelText d = new invokeSetPSLabelText(SetPrimaryStatusLabelText);
                Invoke(d, new object[] { text });
            }
            else
            {
                primaryStatusLabel.Text = text;
                primaryStatusLabel.Refresh();
            }
        }

        private void pictureInBox_TextChanged(object sender, EventArgs e)
        {
            picInStatusLabel.Text = checkPictureInBox();
        }

        private string checkPictureInBox()
        {
            String filePath = this.pictureInBox.Text;
            if (!File.Exists(filePath))
            {
                return "Error: File does not exist";
            }
            else
            {
                if (!Path.GetExtension(filePath).ToLower().Equals(".png"))
                {
                    return "Error: File must be a PNG";
                }
                return "";
            }
        }

        private void messageOutBox_TextChanged(object sender, EventArgs e)
        {
            msgOutStatusLabel.Text = checkMessageOutBox();
        }

        private string checkMessageOutBox()
        {
            String filePath = this.messageOutBox.Text;
            if (filePath == null || filePath == "")
            {
                return "Error: No file path specified";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (!(f.Directory.Exists))
                {
                    return "Error: Directory does not exist";
                }
                else if (f.Exists && f.IsReadOnly)
                {
                    return "Error: File is read only";
                }
                else if (f.Exists)
                {
                    return "Warning: File will be overwritten";
                }
                else
                {
                    return "";
                }
            }
        }

        private void passBox_TextChanged(object sender, EventArgs e) //this is whack--should honestly add two more labels for passwords
        {
            passwordStatusLabel.Text = checkPassBox();
        }

        private string checkPassBox()
        {
            if (passBox.Text.Length < 8)
            {
                return "Error--password must be at least 8 characters";
            }
            else
            {
                return "";
            }
        }

        private void pictureInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "PNG (*.png)|*.png|All Files (*.*)|*.*";
            od.ShowDialog();
            pictureInBox.Text = od.FileName;
        }

        private void messageOutSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sf.DefaultExt = ".txt";
            sf.ShowDialog();
            messageOutBox.Text = sf.FileName;
        }

        private void runButton_Click(object sender, EventArgs e) //as much error checking as possible here
        {
            if (checkPictureInBox().StartsWith("Error") ||
                checkMessageOutBox().StartsWith("Error") ||
                checkPassBox().StartsWith("Error"))
            {
                refreshAllLabels();
                primaryStatusLabel.Text = "Error--See messages above for details";
            }
            else if (primaryStatusLabel.Text.Contains("Running"))
            {
                refreshAllLabels();
                primaryStatusLabel.Text = "Error--only one operation is allowed at a time";
            }
            else
            {
                Thread t = new Thread(extractClick);
                t.IsBackground = true;
                t.Start();
            }
        }

        private void extractClick() //error checking needed here
        {
            SetPrimaryStatusLabelText("Extraction Running");
            Stopwatch s = new Stopwatch();
            s.Start();
            string imgPath = pictureInBox.Text;
            string msgPath = messageOutBox.Text;
            string password = passBox.Text;
            Bitmap b = new Bitmap(imgPath);
            if (!StegoHandler.checkHash(password, b))
            {
                SetPrimaryStatusLabelText("Error: Wrong password or not a Stego File");
            }
            else
            {
                byte[] msg = StegoHandler.extractMain(password, b);
                byte[] decrypted =  AES.decryptionMain(password, msg);
                decrypted = StegoHandler.chopEOF(decrypted);
                File.WriteAllBytes(msgPath, decrypted);
                s.Stop();
                SetPrimaryStatusLabelText("Extraction Complete. Time: " + s.ElapsedMilliseconds + "ms.");
            }
        }

        private void refreshAllLabels()
        {
            pictureInBox_TextChanged(null, null);
            messageOutBox_TextChanged(null, null);
            passBox_TextChanged(null, null);
        }
    }
}