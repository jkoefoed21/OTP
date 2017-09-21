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

namespace Stego_Stuff
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
        /*
        /// <summary>
        /// A List of file paths currently being operated on.
        /// </summary>
        private List<String> runningFiles;

        /// <summary>
        /// Called when the user clicks the X button.
        /// </summary>
        /// <param name="e"> The closing event potentially being cancelled.</param>
        protected override void OnFormClosing(FormClosingEventArgs e) //this only addresses this thread and no other threads...encryption/decryption
        {                                                             //will keep going.
            if (runningFiles.Count != 0)
            {
                var result = MessageBox.Show("Program is still running. Are you sure that you want to exit?",
                                            "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// A delegate for setting the indicator.
        /// </summary>
        /// <param name="text"> The text for the indicator to be set to </param>
        delegate void invokeSetIndicatorText(string text);

        /// <summary>
        /// A delegate for setting the password box.
        /// </summary>
        /// <param name="text"> The text for the box to be set to </param>
        delegate void invokeSetPasswordText(string text);

        /// <summary>
        /// A delegate for setting the file box.
        /// </summary>
        /// <param name="text"> The text for the box to be set to </param>
        delegate void invokeSetFileText(string text);

        /// <summary>
        /// A delegate for setting the password check box.
        /// </summary>
        /// <param name="text"> The text for the box to be set to </param>
        delegate void invokeSetPasswordCheckText(string text);

        /// <summary>
        /// A delegate for setting the running indicator..
        /// </summary>
        /// <param name="text"> The text for the indicator to be set to </param>
        delegate void invokeIsRunningText(string text);

        /// <summary>
        /// A method to set the message indicator text in a thread-safe way.
        /// </summary>
        /// <param name="text"> The text for it to be set to</param>
        private void SetIndicatorText(string text)
        {
            if (runIndicatorLabel.InvokeRequired)
            {
                invokeSetIndicatorText d = new invokeSetIndicatorText(SetIndicatorText);
                Invoke(d, new object[] { text });
            }
            else
            {
                runIndicatorLabel.Text = text;
                runIndicatorLabel.Refresh();
            }
        }

        /// <summary>
        /// A method to set the password box text in a thread-safe way.
        /// </summary>
        /// <param name="text"> The text for it to be set to</param>
        private void SetPasswordText(string text)
        {
            if (passwordBox.InvokeRequired)
            {
                invokeSetPasswordText d = new invokeSetPasswordText(SetPasswordText);
                Invoke(d, new object[] { text });
            }
            else
            {
                passwordBox.Text = text;
                passwordBox.Refresh();
            }
        }

        /// <summary>
        /// A method to set the file path box text in a thread-safe way.
        /// </summary>
        /// <param name="text"> The text for it to be set to</param>
        private void SetFileText(string text)
        {
            if (filePathBox.InvokeRequired)
            {
                invokeSetFileText d = new invokeSetFileText(SetFileText);
                Invoke(d, new object[] { text });
            }
            else
            {
                filePathBox.Text = text;
                filePathBox.Refresh();
            }
        }

        /// <summary>
        /// A method to set the password check box text in a thread-safe way.
        /// </summary>
        /// <param name="text"> The text for it to be set to</param>
        private void SetPasswordCheckText(string text)
        {
            if (passwordCheckBox.InvokeRequired)
            {
                invokeSetPasswordCheckText d = new invokeSetPasswordCheckText(SetPasswordCheckText);
                Invoke(d, new object[] { text });
            }
            else
            {
                passwordCheckBox.Text = text;
                passwordCheckBox.Refresh();
            }
        }

        /// <summary>
        /// A method to set the running indicator text in a thread-safe way.
        /// </summary>
        /// <param name="text"> The text for it to be set to</param>
        private void SetIsRunningText(string text)
        {
            if (isRunningLabel.InvokeRequired)
            {
                invokeIsRunningText d = new invokeIsRunningText(SetIsRunningText);
                Invoke(d, new object[] { text });
            }
            else
            {
                isRunningLabel.Text = text;
                isRunningLabel.Refresh();
            }
        }

        /// <summary>
        /// Manages the actions taken when the encrypt button is clicked.
        /// </summary>
        private void encryptClick()
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            string password = passwordBox.Text;
            string passwordCheck = passwordCheckBox.Text;
            if (!password.Equals(passwordCheck))
            {
                SetIndicatorText("Error: Passwords do not match.");
                return;
            }
            string filePath = filePathBox.Text;
            if (!(checkAndClearBoxTexts(filePath) && checkPasswordAndPath(password, filePath)))
            {
                return;
            }
            SetIsRunningText("Running");
            runningFiles.Add(filePath);
            SetIndicatorText("");
            try
            {
                if (File.Exists(filePath))
                {
                    AES.encryptionMain(password, filePath);
                    s.Stop();
                    SetIndicatorText("Encryption Complete. Time: " + s.ElapsedMilliseconds + " ms.");
                }
                else if (Directory.Exists(filePath))
                {
                    String newPath = filePath + ".aes";
                    Console.WriteLine(newPath);
                    ZipFile.CreateFromDirectory(filePath, newPath); //throws error if newPath already exists
                    Stopwatch z = new Stopwatch();
                    z.Start();
                    wipeDirectory(filePath);
                    z.Stop();
                    Console.WriteLine("Wipe time: " + z.ElapsedMilliseconds);
                    AES.encryptionMain(password, newPath);
                    SetIndicatorText("Encryption Complete. Time: " + s.ElapsedMilliseconds + " ms.");
                }
                else
                {
                    SetIndicatorText("Error: Invalid Path Name.\n Please try again.");
                }
            }
            catch (UnauthorizedAccessException)
            {
                SetIndicatorText("Error: You do not have permission\n to access this file. Please try again.");
            }
            runningFiles.Remove(filePath);
            checkIsCurrentlyRunning();
        }

        /// <summary>
        /// Recursively wipes every file in the directory, then deletes the directory.
        /// </summary>
        /// <param name="path"> the path to be wiped </param>
        private void wipeDirectory(String path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            di.Attributes = FileAttributes.Normal;
            String[] subDirs = Directory.GetDirectories(path);
            for (int ii = 0; ii < subDirs.Length; ii++)
            {
                wipeDirectory(subDirs[ii]);
            }
            String[] subFiles = Directory.GetFiles(path);
            for (int ii = 0; ii < subFiles.Length; ii++)
            {
                wipeFile(subFiles[ii], 7);
            }
            Directory.Delete(path);
        }

        /// <summary>
        /// See: http://www.codeproject.com/Articles/22736/Securely-Delete-a-File-using-NET
        /// </summary>
        /// <param name="filename"> The path to overwrite </param>
        /// <param name="timesToWrite"> The number of times to write </param>
        public void wipeFile(string filename, int timesToWrite)
        {
            try
            {
                if (File.Exists(filename))
                {
                    // Set the files attributes to normal in case it's read-only.

                    File.SetAttributes(filename, FileAttributes.Normal);

                    // Calculate the total number of sectors in the file.
                    double sectors = Math.Ceiling(new FileInfo(filename).Length / 512.0);

                    // Create a dummy-buffer the size of a sector.

                    byte[] dummyBuffer = new byte[512];

                    // Create a cryptographic Random Number Generator.
                    // This is what I use to create the garbage data.

                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

                    // Open a FileStream to the file.
                    FileStream inputStream = new FileStream(filename, FileMode.Open);
                    for (int currentPass = 0; currentPass < timesToWrite; currentPass++)
                    {
                        //UpdatePassInfo(currentPass + 1, timesToWrite);

                        // Go to the beginning of the stream

                        inputStream.Position = 0;

                        // Loop all sectors
                        for (int sectorsWritten = 0; sectorsWritten < sectors; sectorsWritten++)
                        {
                            //UpdateSectorInfo(sectorsWritten + 1, (int)sectors);

                            // Fill the dummy-buffer with random data

                            rng.GetBytes(dummyBuffer);

                            // Write it to the stream
                            inputStream.Write(dummyBuffer, 0, dummyBuffer.Length);
                        }
                    }

                    // Truncate the file to 0 bytes.
                    // This will hide the original file-length if you try to recover the file.

                    inputStream.SetLength(0);

                    // Close the stream.
                    inputStream.Close();

                    // As an extra precaution I change the dates of the file so the
                    // original dates are hidden if you try to recover the file.

                    DateTime dt = new DateTime(2037, 1, 1, 0, 0, 0);
                    File.SetCreationTime(filename, dt);
                    File.SetLastAccessTime(filename, dt);
                    File.SetLastWriteTime(filename, dt);

                    // Finally, delete the file

                    File.Delete(filename);

                    //WipeDone();
                }
            }
            catch (Exception)
            {
                //WipeError(e);
            }
        }
        */

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
            /*
            Thread newThread = new Thread(encryptClick);
            newThread.IsBackground = true;
            newThread.Start();
            */
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
            /*
            Thread newThread = new Thread(decryptClick);
            newThread.IsBackground = true;
            newThread.Start();*/
        }

        private void straightEncryptionButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            encryption.Rijndael f = new encryption.Rijndael();
            f.ShowDialog();
            this.Show();
        }
        /*
/// <summary>
/// Manages the events when the decrypt button is clicked.
/// </summary>
private void decryptClick()
{
   Stopwatch s = new Stopwatch();
   s.Start();
   string password = passwordBox.Text; //I don't get thread errors here
   string filePath = filePathBox.Text;
   if (!(checkAndClearBoxTexts(filePath) && checkPasswordAndPath(password, filePath)))
   {
       return;
   }
   runningFiles.Add(filePath);
   SetIsRunningText("Running");
   SetIndicatorText("");
   try
   {
       if (File.Exists(filePath))
       {
           AES.decryptionMain(password, filePath);
           if (filePath.EndsWith(".aes"))
           {
               ZipFile.ExtractToDirectory(filePath, filePath.Replace(".aes", ""));
               File.Delete(filePath);
           }
           s.Stop();
           SetIndicatorText("Decryption Complete. Time: " + s.ElapsedMilliseconds + " ms.");
       }
       else
       {
           SetIndicatorText("Error: Invalid path name.\n Please try again.");
       }
   }
   catch (UnauthorizedAccessException)
   {
       SetIndicatorText("Error: You do not have permission\n to access this file.");
   }
   catch (ApplicationException)
   {
       SetIndicatorText("Error: Invalid password or not \n a valid encrypted file. Please try again.");
   }
   runningFiles.Remove(filePath);
   checkIsCurrentlyRunning();
}

/// <summary>
/// Called when the file browse button is clicked.
/// </summary>
/// <param name="sender"> The object calling this method </param>
/// <param name="e"> Args for the event </param>
private void fileBrowseButton_Click(object sender, EventArgs e)
{   //this is not multiThreaded because somebody can just close this if needed
   OpenFileDialog od = new OpenFileDialog();
   od.ShowDialog();
   filePathBox.Text = od.FileName;
}

/// <summary>
/// Checks the password and the path for length violations.
/// </summary>
/// <param name="password"> The password being inputted. </param>
/// <param name="filePath"> The file path being inputted. </param>
/// <returns> False if there are any problems </returns>
private bool checkPasswordAndPath(string password, string filePath)
{
   if (password.Length < 8)
   {
       SetIndicatorText("Error: Please enter a password \n longer than 8 characters and try again.");
       return false;
   }
   if (filePath.Length == 0)
   {
       SetIndicatorText("Error: Please enter a \n non-empty path name.");
       return false;
   }
   return true;
}

/// <summary>
/// Checks the file for concurrent modification problems and clears the boxes.
/// </summary>
/// <param name="filePath"> The file path being inputted. </param>
/// <returns> False if there are any problems. </returns>
private Boolean checkAndClearBoxTexts(string filePath)
{
   if (runningFiles.Contains(filePath))
   {
       SetIndicatorText("File is currently in use. Please Try Again.");
       return false;
   }
   //SetPasswordText("");  //these are commented out for the sake of ease of testability
   //SetFileText("");      //On release, these would clear each box upon running
   //SetPasswordCheckText(""); //but in order to save the tester time, I won't make you re-enter the password
   return true;                //every time.
}

/// <summary>
/// Checks and resets the indicator based on whether the program is running.
/// </summary>
private void checkIsCurrentlyRunning()
{
   if (runningFiles.Count == 0)
   {
       SetIsRunningText("Currently Inactive");
   }
   else
   {
       SetIsRunningText("Running");
   }
}
*/
    }
}
