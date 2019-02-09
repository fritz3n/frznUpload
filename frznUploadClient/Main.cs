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
        private SettingsForm settingsForm;
        private ClientManager Client;
        public bool Uploading { get; private set; } = false;
        public event EventHandler UploadFinished;
        private UploadContract FileUpload;
        private System.Timers.Timer UploadTimer = new System.Timers.Timer(100);
        private RemoteFile RightClicked = null;

        public MainForm(ClientManager client)
        {
            Client = client;
            InitializeComponent();
            Shown += MainForm_Shown;
            FormClosing += MainForm_FormClosing;
            loginForm = new LoginForm(Client);
            settingsForm = new SettingsForm();

            UploadTimer.AutoReset = true;
            UploadTimer.Enabled = false;
            UploadTimer.Elapsed += UploadTimer_Elapsed;

            //Check if the client is logged in, if not -> Display the login form
            if(client.LoggedIn == false)
            {
                loginForm.Owner = this;
                loginForm.ControlBox = false;
                loginForm.Show();
            }
        }

        private async void UploadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (FileUpload.Uploader.Running)
            {
                ProgressBar.Invoke(new Action(() => ProgressBar.Value = (int)(FileUpload.Uploader.Progress * 100)));
            }
            else
            {
                ProgressBar.Invoke(new Action(() => StatusLabel.Text = "Finished"));
                Uploading = false;
                ProgressBar.Invoke(new Action(() => ProgressBar.Value = 0));
                UploadTimer.Stop();

                if (FileUpload.Share && FileUpload.Uploader.Finished)
                {
                    var s = await Client.ShareFile(
                        FileUpload.Uploader.Identifier,
                        FileUpload.FirstView,
                        FileUpload.Public,
                        FileUpload.PublicRegistered,
                        FileUpload.Whitelisted,
                        FileUpload.Whitelist
                        );

                    LinkText.Invoke(new Action(() => LinkText.Text = "fritzen.tk/view.php?id=" + s));
                }

                UploadFinished?.Invoke(this, EventArgs.Empty);
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

        public void ShowLogin()
        {
            Show();
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
                var u = await Client.UploadFile(PathText.Text, FilenameText.Text);
                var c  = new UploadContract
                {
                    Uploader = u,
                    Path = PathText.Text,
                    Filename = FilenameText.Text,

                    Share = ShareBox.Checked,
                    FirstView = FirstViewBox.Checked,
                    Public = PublicBox.Checked,
                    PublicRegistered = PublicRegisteredBox.Checked,
                    Whitelisted = WhitelistedBox.Checked,
                    Whitelist = WhitelistText.Text
                };

                StartUpload(c);
            }
        }

        public void StartUpload(UploadContract contract)
        {
            StatusLabel.Text = "Uploading: " + contract.Filename;
            Uploading = true;
            FileUpload = contract;
            UploadTimer.Start();
        }

        private void CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(LinkText.Text);
        }

        private void Tab_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(e.TabPage == FileTab)
            {
                var t = UpdateFileList();
            }
        }

        private async Task UpdateFileList()
        {
            FileView.Items.Clear();
            var l = await Client.GetFiles();

            foreach(RemoteFile file in l)
            {
                var Item = new ListViewItem(new string[] { file.Filename + "." + file.File_extension, file.SizeString });
                Item.Tag = file;
                if (FileView.InvokeRequired)
                    FileView.Invoke(new Action(() => FileView.Items.Add(Item)));
                else
                    FileView.Items.Add(Item);
            }

        }

        private void FileView_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                contextMenu.Show(FileView, e.Location);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            settingsForm.Show();
        }
    }
}
