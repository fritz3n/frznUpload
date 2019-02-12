using frznUpload.Client.Hotkey;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    public partial class HotkeyConfigControl : GroupBox
    {
        private Keys Key = 0;
        private ModifierKeys Modifiers = 0;

        private Keys BackupKey = 0;
        private ModifierKeys BackupModifiers = 0;

        private bool Capturing = false;

        SettingsForm Form;

        public HotkeyConfigControl(SettingsForm form)
        {
            Form = form;
            InitializeComponent();
            UpdateShareEnables();
            UpdateButtonText();

        }

        public void Update(HotkeyConfig config)
        {
            FormatText.Text = config.Format;
            WhitelistText.Text = config.Whitelist;

            ShareBox.Checked = (config.Share) != 0;
            PublicBox.Checked = (config.Share & ShareType.Public) != 0;
            PublicRegisteredBox.Checked = (config.Share & ShareType.PublicRegistered) != 0;
            FirstViewBox.Checked = (config.Share & ShareType.FirstView) != 0;
            WhitelistedBox.Checked = (config.Share & ShareType.Whitelisted) != 0;
            UpdateShareEnables();

            Key = config.Key;
            Modifiers = config.Modifier;
            UpdateButtonText();
        }

        public void KeyUpEvent(KeyEventArgs e)
        {
            if (!Capturing)
                return;
            
            // Set up a variable with all bits set
            ModifierKeys and = (ModifierKeys)uint.MaxValue;

            if (e.KeyCode == Keys.Menu)
            {
                // If Alt is pressed, set the corresponding bit to 0
                and ^= Hotkey.ModifierKeys.Alt;
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                // If Control is pressed, set the corresponding bit to 0
                and ^= Hotkey.ModifierKeys.Control;
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                // If Shift is pressed, set the corresponding bit to 0
                and ^= Hotkey.ModifierKeys.Shift;
            }

            // And the Modifiers with the previously set up variable; if a modifierkey was released, the corresponding bit will be guaranteed to be 0
            Modifiers &= and;

            UpdateButtonText();
            e.Handled = true;
        }

        public void KeyDownEvent(KeyEventArgs e)
        {
            if (!Capturing)
                return;
            bool modifier = false;

            if (e.KeyCode == Keys.Menu)
            {
                modifier = true;
                Modifiers |= Hotkey.ModifierKeys.Alt;
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                modifier = true;
                Modifiers |= Hotkey.ModifierKeys.Control;
            }

            if (e.KeyCode == Keys.ShiftKey)
            {
                modifier = true;
                Modifiers |= Hotkey.ModifierKeys.Shift;
            }

            if (!modifier)
            {
                Key = e.KeyCode;
                StopCapturing();
            }

            UpdateButtonText();
            e.Handled = true;
        }

        private void StartCapturing()
        {
            if (Capturing)
                return;
            
            BackupKey = Key;
            BackupModifiers = Modifiers;

            Key = Keys.None;
            Modifiers = 0;

            Capturing = Form.StartCapturing(this);
            
            if(Capturing)
                HotkeyButton.BackColor = SystemColors.ControlLightLight;

            UpdateButtonText();
        }

        public void StopCapturing(bool canceled = false)
        {
            if (!Capturing)
                return;

            Form.StopCapturing();
            Capturing = false;
            HotkeyButton.BackColor = SystemColors.ControlLight;

            if (Modifiers == 0 & !canceled)
            {
                MessageBox.Show("Please press a modifier key!");
                canceled = true;
            }

            if (canceled)
            {
                Key = BackupKey;
                Modifiers = BackupModifiers;
            }
            else
            {
                
            }

            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            /*List<string> mods = new List<string>();

            if ((Modifiers & Hotkey.ModifierKeys.Control) > 0)
                mods.Add("Ctrl");

            if ((Modifiers & Hotkey.ModifierKeys.Alt) > 0)
                mods.Add("Alt");

            if ((Modifiers & Hotkey.ModifierKeys.Control) > 0)
                mods.Add("Ctrl");*/

            HotkeyButton.Text = Modifiers.ToString("G").Replace(",", " +").Replace("Control", "Ctrl") + " + " + (Capturing ? "..." : Key.ToString("G"));
        }

        private void UpdateShareEnables()
        {
            PublicBox.Enabled = ShareBox.Checked;
            PublicRegisteredBox.Enabled = ShareBox.Checked;
            FirstViewBox.Enabled = ShareBox.Checked;
            WhitelistedBox.Enabled = ShareBox.Checked;
            WhitelistText.Enabled = ShareBox.Checked & WhitelistedBox.Checked;
        }

        private void HotkeyButton_Click(object sender, EventArgs e)
        {
            if (Capturing)
                StopCapturing(true);
            else
                StartCapturing();
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            this.Initialize();
            ResumeLayout(false);
        }


    }
}
