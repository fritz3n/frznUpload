using frznUpload.Client.Hotkey;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    public partial class SettingsForm : Form
    {
        private bool showing = false;
        private ClientManager clientManager;
        private HotkeyContainer Hotkeys;
        private List<HotkeyConfigControl> HotkeyControls = new List<HotkeyConfigControl>();
        private HotkeyConfigControl Capturer;
        private bool Capturing = false;

        public SettingsForm(ClientManager manager,HotkeyContainer hotkeys)
        {
            InitializeComponent();
            clientManager = manager;
            Hotkeys = hotkeys;
        }

        private void InitHotkeys()
        {
            HotkeyLayout.SuspendLayout();
            HotkeyLayout.Controls.Clear();
            HotkeyControls.Clear();

            foreach (HotkeyConfig Hotkey in Hotkeys)
            {
                var hk = new HotkeyConfigControl(this);
                hk.Update(Hotkey);

                HotkeyControls.Add(hk);
                HotkeyLayout.Controls.Add(hk);
            }

            HotkeyLayout.Controls.Add(AddButton);

            HotkeyLayout.ResumeLayout(true);
        }

        public bool StartCapturing(HotkeyConfigControl capturer)
        {
            if (Capturing)
            {
                Capturer.StopCapturing(true);
                return false;
            }

            Capturer = capturer;
            Capturing = true;
            return true;
        }

        public void StopCapturing()
        {
            Capturing = false;
            Capturer = null;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool discard = false;
            string message = "";

            int id = 0;

            foreach(HotkeyConfigControl control in HotkeyControls)
            {
                id++;

                (bool, string) valid = control.IsValid();

                if (!valid.Item1)
                {
                    discard = true;
                    message += $"Hotkey #{id}´s changes have not been saved and will be discarded:\n\t{valid.Item2}\n\n";
                }
            }

            if (discard)
            {
                var result = MessageBox.Show(message, "Discard changes?", MessageBoxButtons.OKCancel);

                if(result == DialogResult.OK)
                {
                    Hide();
                    showing = false;
                }
            }
            else
            {
                Hide();
                showing = false;
            }
            
            e.Cancel = true;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            showing = true;
        }

        public new void Show()
        {
            InitHotkeys();

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

        private void AddButton_Click(object sender, EventArgs e)
        {
            HotkeyLayout.SuspendLayout();
            var hk = new HotkeyConfigControl(this);
            
            HotkeyControls.Add(hk);

            HotkeyLayout.Controls.Remove(AddButton); ;
            HotkeyLayout.Controls.Add(hk);
            HotkeyLayout.Controls.Add(AddButton);

            HotkeyLayout.ResumeLayout(true);
        }

        public void RemoveHotkey(HotkeyConfigControl control)
        {
            HotkeyLayout.Controls.Remove(control);
            Hotkeys.Remove(control.GetConfig());
            HotkeyControls.Remove(control);
        }

        public void SaveHotkey(HotkeyConfig config)
        {
            Hotkeys.Add(config);
        }

        private void SettingsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (Capturing)
            {
                Capturer.KeyDownEvent(e);
            }
        }

        private void SettingsForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (Capturing)
            {
                Capturer.KeyUpEvent(e);
            }
        }

        private void SettingsForm_Deactivate(object sender, EventArgs e)
        {
            if (Capturing)
                Capturer.StopCapturing(true);
        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (autostartCheckbox.Checked)
            {
                AutostartHandler.SetToCurrentPath();
            }
            else
            {
                AutostartHandler.DeleteKey();
            }
        }


        /*
        * Two factor 
        *    
        */
        private async void TwoFaCheckbox_CheckedChangedAsync(object sender, EventArgs e)
        {
            if (TwoFaCheckbox.Checked)
            {
                //enable Two fa
                string qr = await clientManager.GetTwoFaSecret();
                // Display the qrcode
                var base64Data = Regex.Match(qr, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);

                using (var stream = new MemoryStream(binData))
                {
                    picture_qr.Image = new Bitmap(stream);
                }
                scanLable.Visible = true;
                picture_qr.Visible = true;
            }
            else
            {
                //remove Two fa
                await clientManager.RemoveTwoFa();
            }
        }


        private async void SettingsTabCtrl_TabIndexChangedAsync(object sender, EventArgs e)
        {
            if(SettingsTabCtrl.SelectedIndex == 3)
            {
                //account tab -> load settings
                TwoFaCheckbox.Checked = await clientManager.GetHasTwoFaEnabled();
                //settings loaded enable buttons
                TwoFaCheckbox.Enabled = true;
            }
            else
            {
                //hide the qrcode and lable
                picture_qr.Visible = false;
                scanLable.Visible = false;
            }
        }
    }
}
