using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUpload.Client
{
    class IconHandler : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();
        MainForm mainForm;
        public static ClientManager Client { get; private set; }

        public IconHandler()
        {
            MenuItem consoleMenuItem = new MenuItem("Show", new EventHandler(Show));
            MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));


            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); ;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { consoleMenuItem, configMenuItem, exitMenuItem });
            notifyIcon.MouseClick += new MouseEventHandler(LeftClick);
            notifyIcon.Visible = true;

            Client = new ClientManager();
            Task.Run(() => Client.Connect());

            mainForm = new MainForm(Client);

            //if client is not logged in -> show a login prompt
            if(Client.LoggedIn == false)
            {
                mainForm.Show();
            }
        }

        void ShowConfig(object sender, EventArgs e)
        {
            mainForm.ShowLogin();
        }

        void Exit(object sender, EventArgs e)
        {
            Application.Exit();
            Environment.Exit(0);
        }

        void Show(object sender, EventArgs e)
        {
            mainForm.Show();
        }

        void LeftClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mainForm.Show();
            }
        }
    }
}
