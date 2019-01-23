using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        private ClientManager Client;

        public MainForm()
        {
            Client = new ClientManager();
            InitializeComponent();
            Shown += MainForm_Shown;
            FormClosing += MainForm_FormClosing;
            loginForm = new LoginForm(Client);
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
    }
}
