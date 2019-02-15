using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
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
        private HotkeyContainer hotkeyContainer;

        public MainForm(ClientManager client)
        {
            Client = client;
            InitializeComponent();
            Shown += MainForm_Shown;
            FormClosing += MainForm_FormClosing;
            loginForm = new LoginForm(Client);
            hotkeyContainer = new HotkeyContainer(Client, this);
            settingsForm = new SettingsForm(hotkeyContainer);

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
                try
                {
                    if (Created)
                        ProgressBar.Invoke(new Action(() => ProgressBar.Value = (int)(FileUpload.Uploader.Progress * 100)));
                }
                catch { }
            }
            else
            {
                if (Created)
                    ProgressBar.Invoke(new Action(() => StatusLabel.Text = "Finished"));
                Uploading = false;
                if (Created)
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

                    if (Created)
                        LinkText.Invoke(new Action(() => LinkText.Text = @"https://fritzen.tk/view.php?id=" + s));

                    SetClipboardText(@"https://fritzen.tk/view.php?id=" + s);
                    SystemSounds.Asterisk.Play();
                }

                UploadFinished?.Invoke(this, EventArgs.Empty);
            }
        }

        private void SetClipboardText(string text)
        {
            var state = Thread.CurrentThread.GetApartmentState();

            if(state == ApartmentState.STA)
            {
                Clipboard.SetText(text);
            }
            else
            {
                var t = new Thread(() => SetClipboardText(text));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
            }

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            showing = false;
            loginForm.Hide();
            settingsForm.Hide();
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
            SetClipboardText(LinkText.Text);
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
                var Item = new ListViewItem(new string[] { file.Filename + "." + file.File_extension, file.SizeString })
                {
                    Tag = file
                };
                if (FileView.InvokeRequired)
                    FileView.Invoke(new Action(() => FileView.Items.Add(Item)));
                else
                    FileView.Items.Add(Item);
            }

        }

        public async Task DeletSelectedItems()
        {
            foreach (ListViewItem sel in FileView.SelectedItems)
            {
                try
                {
                    string id = ((RemoteFile)(sel.Tag)).Identifier;
                    await Client.DeleteFile(id);
                }
                catch
                {
                    MessageBox.Show("Error deleting file " + sel.SubItems[0]);
                }
            }
            await UpdateFileList();
        }

        private void FileView_MouseClick(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                contextMenu.Show(FileView, e.Location);
            }
        }

        private async void DeleteToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            await DeletSelectedItems();
        }

        private void SettingsButton_Click(object sender, EventArgs e)
        {
            settingsForm.Show();
        }

    }
}
