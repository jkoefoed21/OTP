using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using encryption;
using System.Diagnostics;
using Stego_Stuff;

namespace OTPStuff
{
    public partial class OTPImplantForm : Form
    {
        public OTPImplantForm() //If you want to make settings for various encodings--do this in control--pass parameter
        {
            InitializeComponent();
        }

        /// <summary>
        /// This is stored like this because I don't want to have to open and close the image all the time.
        /// </summary>
        private int possibleSize = 0;

        /// <summary>
        /// For threading in order to set primary status label
        /// </summary>
        /// <param name="text"> The text to change the label to </param>
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
                if (!(Path.GetExtension(filePath).ToLower().Equals(".png") || Path.GetExtension(filePath).ToLower().Equals(".jpg")))
                {
                    return "Error: File must be a PNG or a JPG";
                }
                else
                {
                    Bitmap b = new Bitmap(filePath);
                    int size = b.Height * b.Width;
                    calculatePossibleSize(size);
                    b.Dispose();
                    return "Size: " + size + " px. " + possibleSize + " bytes available.";
                }
            }
        }

        private void calculatePossibleSize(int imgSize)
        {
            this.possibleSize = StegoHandler.availableBytes(imgSize);
        }

        private void messageInBox_TextChanged(object sender, EventArgs e)
        {
            msgInStatusLabel.Text = checkMessageInBox();
        }

        private string checkMessageInBox()
        {
            pictureInBox_TextChanged(null, null);
            String filePath = this.messageInBox.Text;
            if (!File.Exists(filePath))
            {
                return "Error: File does not exist";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (f.Length > this.possibleSize-OTPHandler.EOF1_LENGTH-1) //have to leave room in the message for the OTP to hit the message.ear
                {
                    return "Error: File size exceeds that available in image";
                }
                else
                {
                    return "File Size=" + f.Length + " bytes";
                }
            }
        }

        private void picOutBox_TextChanged(object sender, EventArgs e)
        {
            picOutStatusLabel.Text = checkPictureOutBox();
        }

        private string checkPictureOutBox()
        {
            String filePath = this.picOutBox.Text;
            if (filePath == null || filePath == "")
            {
                return "Error: No file path specified";
            }
            if (filePath.Equals(messageInBox.Text) || filePath.Equals(pictureInBox.Text))
            {
                return "Error: Cannot use same file";
            }
            else
            {
                FileInfo f = new FileInfo(filePath);
                if (!(f.Directory.Exists))
                {
                    return "Error: Directory does not exist";
                }
                else if (!Path.GetExtension(filePath).ToLower().Equals(".png"))
                {
                    return "Error: File must be a PNG";
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
            if (filePath.Equals(messageInBox.Text) || filePath.Equals(pictureInBox.Text) || filePath.Equals(picOutBox.Text))
            {
                return "Error: Cannot use same file";
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

        private void pictureInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg; *.jpg)|*.jpeg; *.jpg|All Files (*.*)|*.*";
            od.ShowDialog();
            pictureInBox.Text = od.FileName;
        }

        private void messageInSelectButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog od = new OpenFileDialog();
            od.ShowDialog();
            messageInBox.Text = od.FileName;
        }

        private void picOutSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "PNG (*.png)|*.png|All Files (*.*)|*.*";
            sf.DefaultExt = "png";
            sf.ShowDialog();
            picOutBox.Text = sf.FileName;
        }

        private void messageOutSelectButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.DefaultExt = "txt";
            sf.ShowDialog();
            messageOutBox.Text = sf.FileName;
        }


        private void pass1Box_TextChanged(object sender, EventArgs e) //this is whack--should honestly add two more labels for passwords
        {
            pass1StatusLabel.Text = checkPass1Box();
        }

        private string checkPass1Box()
        {
            if (pass1Box.Text.Length < 8)
            {
                return "Error--password must be at least 8 characters";
            }
            else
            {
                return "";
            }
        }

        private void pass2Box_TextChanged(object sender, EventArgs e)
        {
            pass2StatusLabel.Text = checkPass2Box();
        }

        private string checkPass2Box()
        {
            if (!pass2Box.Text.Equals(pass1Box.Text))
            {
                return "Error--passwords do not match";
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Error checks, then opens a new thread to run the implantation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void runButton_Click(object sender, EventArgs e) //error checking needed here
        {
            if (checkPictureInBox().StartsWith("Error") ||
            checkMessageInBox().StartsWith("Error") ||
            checkPictureOutBox().StartsWith("Error") ||
            checkMessageOutBox().StartsWith("Error") ||
            checkPass1Box().StartsWith("Error") ||
            checkPass2Box().StartsWith("Error"))
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
                Thread t = new Thread(implantClick);
                t.IsBackground = true; //when the whole program--read--the MAIN SCREEN is closed, will kill stego.
                t.Start();
            }
        }

        private void implantClick() //error checking needed here
        {
            SetPrimaryStatusLabelText("Implantation Running");
            Stopwatch s = new Stopwatch();
            s.Start();
            string imgPath = pictureInBox.Text;
            string msgPath = messageInBox.Text;
            string picOutPath = picOutBox.Text;
            string msgOutPath = messageOutBox.Text;
            string password = pass1Box.Text;
            Bitmap b = new Bitmap(imgPath);
            byte[] OTP = OTPHandler.generateRandomBytes(StegoHandler.availableBytes(b.Width * b.Height));

            byte[] unencryptedMsg = File.ReadAllBytes(msgPath);

            if (unencryptedMsg.Length>OTP.Length-OTPHandler.EOF1_LENGTH-1)
            {
                SetPrimaryStatusLabelText("Error: Message cannot be longer than image can handle.");
                return;
            }
            byte[] messageBytes = OTPHandler.addEOF(unencryptedMsg); //some nuances need to be resolved here when message approaches the length of the OTP.
            //byte[] encryptedMsg = AES.encryptionMain(password, messageBytes);
            byte[] finalMessage=OTPHandler.getEncryptedMessage(OTP, messageBytes); //this saves to finalMessage.
            
            byte[] OTPwEOF = OTPHandler.addEOF(OTP);
            OTPwEOF=AES.encryptionMain(password, OTPwEOF);
            Console.WriteLine("Time to encryption: " + s.ElapsedMilliseconds);
            b = StegoHandler.implantMain(password, b, OTPwEOF);
            Console.WriteLine("Time to implant: " + s.ElapsedMilliseconds);

            b.Save(picOutPath, ImageFormat.Png);
            File.WriteAllBytes(msgOutPath, finalMessage);
            s.Stop();
            SetPrimaryStatusLabelText("Implantation Complete. Time: " + s.ElapsedMilliseconds + "ms.");
        }

        /// <summary>
        /// Goes through and acts like the user just reset every box to refresh everything. Not always needed but won't lag at all, so why not?
        /// </summary>
        private void refreshAllLabels()
        {
            pictureInBox_TextChanged(null, null);
            messageInBox_TextChanged(null, null);
            picOutBox_TextChanged(null, null);
            messageOutBox_TextChanged(null, null);
            pass1Box_TextChanged(null, null);
            pass2Box_TextChanged(null, null);
        }
    }
}
