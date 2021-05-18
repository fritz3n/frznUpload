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

		Dictionary<string, FileProvider> providers = new Dictionary<string, FileProvider>()
		{
			{ "Clipboard", FileProvider.Clipboard },
			{ "ScreenClip", FileProvider.ScreenClip },
			{ "Screenshot", FileProvider.Screenshot },
			{ "Cb > Sc > Ss", FileProvider.Clipboard | FileProvider.ScreenClip | FileProvider.Screenshot},
			{ "Cb > Ss", FileProvider.Clipboard | FileProvider.Screenshot },
			{ "Sc > Ss",  FileProvider.ScreenClip | FileProvider.Screenshot },
		};

		SettingsForm Form;

		public HotkeyConfigControl(SettingsForm form)
		{
			Form = form;
			InitializeComponent();
			UpdateShareEnables();
			UpdateButtonText();
			FileProviderBox.SelectedIndex = 0;
		}

		public void Update(HotkeyConfig config)
		{
			FormatText.Text = config.Format;
			WhitelistText.Text = config.Whitelist;

			ShareBox.Checked = (config.Share & ShareType.Share) != 0;
			PublicBox.Checked = (config.Share & ShareType.Public) != 0;
			PublicRegisteredBox.Checked = (config.Share & ShareType.PublicRegistered) != 0;
			FirstViewBox.Checked = (config.Share & ShareType.FirstView) != 0;
			WhitelistedBox.Checked = (config.Share & ShareType.Whitelisted) != 0;
			UpdateShareEnables();

			FileProviderBox.SelectedItem = providers.First(p => p.Value == config.Provider).Key;

			Key = config.Key;
			Modifiers = config.Modifier;
			UpdateButtonText();
		}

		public HotkeyConfig GetConfig()
		{
			ShareType share = ShareType.DontShare;

			if (ShareBox.Checked)
			{
				share |= ShareType.Share;

				if (FirstViewBox.Checked)
					share |= ShareType.FirstView;

				if (PublicBox.Checked)
					share |= ShareType.Public;

				if (PublicRegisteredBox.Checked)
					share |= ShareType.PublicRegistered;

				if (WhitelistedBox.Checked)
					share |= ShareType.Whitelisted;
			}

			var config = new HotkeyConfig
			{
				Format = FormatText.Text,
				Whitelist = WhitelistText.Text,
				Share = share,
				Key = Key,
				Modifier = Modifiers,
				Provider = GetSelectedFileProvider()
			};

			return config;
		}

		private void Save()
		{
			if (!IsValid().Item1)
				return;

			HotkeyConfig config = GetConfig();

			Form.SaveHotkey(config);
		}

		/// <summary>
		/// Indicates whether the hotkey is valid and can be/was saved and if not, a message to display to the user
		/// </summary>
		/// <returns>A tuple of (is valid, optional error message)</returns>
		public (bool, string) IsValid()
		{
			if (Key == 0 | Modifiers == 0)
				return (false, "no Hotkey set!");

			if (GetSelectedFileProvider() == FileProvider.None)
				return (false, "no File Provider selected!");

			if (WhitelistedBox.Checked & WhitelistText.Text == "")
				return (false, "no valid Whitelist was provided!");

			return (true, null);
		}

		private FileProvider GetSelectedFileProvider()
		{
			return providers[(string)FileProviderBox.SelectedItem];
		}

		#region Hotkey Capturing
		public void KeyUpEvent(KeyEventArgs e)
		{
			if (!Capturing)
				return;

			// Set up a variable with all bits set
			var and = (ModifierKeys)uint.MaxValue;

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

			if (Capturing)
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
				Save();
			}

			UpdateButtonText();
		}

		private void UpdateButtonText()
		{


			HotkeyButton.Text = Modifiers.ToString("G").Replace(",", " +").Replace("Control", "Ctrl") + " + " + (Capturing ? "..." : Key.ToString("G"));
		}

		#endregion

		private void UpdateShareEnables()
		{
			PublicBox.Enabled = ShareBox.Checked;
			PublicRegisteredBox.Enabled = ShareBox.Checked;
			FirstViewBox.Enabled = ShareBox.Checked;
			WhitelistedBox.Enabled = ShareBox.Checked;
			WhitelistText.Enabled = ShareBox.Checked & WhitelistedBox.Checked;
			WhitelistLabel.Enabled = ShareBox.Checked & WhitelistedBox.Checked;
		}

		private void HotkeyButton_Click(object sender, EventArgs e)
		{
			if (Capturing)
				StopCapturing(true);
			else
				StartCapturing();
		}

		private void RemoveButton_Click(object sender, EventArgs e)
		{
			Form.RemoveHotkey(this);
		}

		private void InitializeComponent()
		{
			SuspendLayout();
			Initialize();
			ResumeLayout(false);
		}

		private void PublicBox_CheckedChanged(object sender, EventArgs e)
		{
			Save();
		}

		private void FirstViewBox_CheckedChanged(object sender, EventArgs e)
		{
			Save();

		}

		private void PublicRegisteredBox_CheckedChanged(object sender, EventArgs e)
		{
			Save();
		}

		private void WhitelistedBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateShareEnables();
		}

		private void WhitelistText_TextChanged(object sender, EventArgs e)
		{
			Save();

		}

		private void ShareBox_CheckedChanged(object sender, EventArgs e)
		{
			UpdateShareEnables();
		}

		private void FileProviderBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			Save();

		}

		private void FormatText_TextChanged(object sender, EventArgs e)
		{
			Save();
		}
	}
}
