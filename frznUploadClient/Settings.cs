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

        public SettingsForm()
        {
            InitializeComponent();
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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
    }
}
