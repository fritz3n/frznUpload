﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    partial class LoginForm : Form
    {
        private bool showing;
        ClientManager Client;
        
        public LoginForm(ClientManager clientManager)
        {
            Client = clientManager;

            InitializeComponent();
            Shown += MainForm_Shown;
            FormClosing += MainForm_FormClosing;

            ShowInTaskbar = false;
            SetEnables();
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
            SetEnables();
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
            SetEnables();
        }

        private void SetEnables()
        {
            LoginButton.Enabled = !Client.LoggedIn;
            LogoutButton.Enabled = Client.LoggedIn;


            if (Client.LoggedIn)
            {
                StatusText.Text = $"Logged in as:\n{Client.Username}";
            }
            else
            {
                StatusText.Text = "Not logged in.";
            }
        }

        private async void LoginButton_Click(object sender, EventArgs e)
        {
            if (userBox.Text == "" || passBox.Text == "")
            {
                StatusText.Text = "Enter Username/Password";
                SystemSounds.Hand.Play();
            }
            else
            {
                await Client.Login(userBox.Text, passBox.Text);
                SetEnables();
                if (!Client.LoggedIn)
                {
                    StatusText.Text = "Wrong Username/Password";
                    SystemSounds.Hand.Play();
                }
                else
                {
                    SystemSounds.Question.Play();
                    this.ControlBox = true;
                }
            }
        }

        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            await Client.Logout();
            SetEnables();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = LoginButton;
        }
    }
}
