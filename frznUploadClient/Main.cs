using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    public partial class MainForm : Form
    {
        private bool showing;
        private LoginForm loginForm;
        private ClientManager Client;
        bool Uploading = false;
        private FileUploader fileUploader = null;
        private System.Timers.Timer UploadTimer = new System.Timers.Timer(100);

        public MainForm(ClientManager client)
        {
            Client = client;
            InitializeComponent();
            Shown += MainForm_Shown;
            FormClosing += MainForm_FormClosing;
            loginForm = new LoginForm(Client);

            UploadTimer.AutoReset = true;
            UploadTimer.Enabled = false;
            UploadTimer.Elapsed += UploadTimer_Elapsed;
        }

        private void UploadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (fileUploader.Running)
            {
                ProgressBar.Value = (int)(fileUploader.Progress * 100);
            }
            else
            {
                Uploading = false;
                ProgressBar.Value = 0;
                UploadTimer.Stop();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            showing = false;
            loginForm.Hide();
            e.Cancel = true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            showing = true;
        }

        public new void Show()
        {
            if (!showing)
            {
                base.Show();
            }
            else
            {
                BringToFront();
                Activate();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void AccountButton_Click(object sender, EventArgs e)
        {
            loginForm.Show();
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                PathText.Text = openFileDialog.FileName;
            }
        }

        private void PathText_TextChanged(object sender, EventArgs e)
        {
            FilenameText.Text = Path.GetFileName(PathText.Text);
            UploadButton.Enabled = File.Exists(PathText.Text);
        }

        private void WhitelistedBox_CheckedChanged(object sender, EventArgs e)
        {
            WhitelistText.Enabled = WhitelistedBox.Checked;
        }

        private void ShareBox_CheckedChanged(object sender, EventArgs e)
        {
            SharePanel.Enabled = ShareBox.Checked;
        }

        private async void UploadButton_Click(object sender, EventArgs e)
        {
            if (File.Exists(PathText.Text) && !Uploading)
            {
                Uploading = true;
                fileUploader = await Client.UploadFile(PathText.Text);
                UploadTimer.Start();
            }
        }
    }
}
