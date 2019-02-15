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
        private ClientManager clientManager;

        public SettingsForm(ClientManager manager)
        {
            InitializeComponent();
            clientManager = manager;
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
        * Tow factor 
        *    
        */
        private async void TowFaCheckbox_CheckedChangedAsync(object sender, EventArgs e)
        {
            if (towFaCheckbox.Checked)
            {
                //enable tow fa
                string secret = await clientManager.GetTowFaSecret();
                MessageBox.Show("Enter this secret into your app: " + secret);
            }
            else
            {
                //remove tow fa
                await clientManager.RemoveTowFa();
            }
        }


        private async void SettingsTabCtrl_TabIndexChangedAsync(object sender, EventArgs e)
        {
            if(SettingsTabCtrl.SelectedIndex == 3)
            {
                //account tab -> load settings
                towFaCheckbox.Checked = await clientManager.GetHasTowFaEnabled();

                //settings loaded enable buttons
                towFaCheckbox.Enabled = true;
            }
        }
    }
}
