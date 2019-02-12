using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    public partial class SettingsForm : Form
    {
        private bool showing = false;
        private HotkeyContainer Hotkeys;
        private List<HotkeyConfigControl> HotkeyControls = new List<HotkeyConfigControl>();
        private HotkeyConfigControl Capturer;
        private bool Capturing = false;

        public SettingsForm(HotkeyContainer hotkeys)
        {
            InitializeComponent();
            Hotkeys = hotkeys;
            
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
            Hide();
            showing = false;
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

        private void AddButton_Click(object sender, EventArgs e)
        {
            HotkeyLayout.SuspendLayout();
            var hk = new HotkeyConfigControl(this);
            HotkeyControls.Add(hk);
            HotkeyLayout.Controls.Add(hk);
            HotkeyLayout.ResumeLayout(true);
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
    }
}
