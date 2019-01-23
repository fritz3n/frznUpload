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

        public IconHandler()
        {
            MenuItem consoleMenuItem = new MenuItem("Console", new EventHandler(ShowConsole));
            MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));


            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); ;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { consoleMenuItem, configMenuItem, exitMenuItem });
            notifyIcon.MouseClick += new MouseEventHandler(LeftClick);
            notifyIcon.Visible = true;

            
            mainForm = new MainForm();
        }

        void ShowConfig(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void Exit(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void ShowConsole(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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
