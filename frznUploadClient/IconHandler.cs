using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace frznUploadClient
{
    class IconHandler : ApplicationContext
    {
        NotifyIcon notifyIcon = new NotifyIcon();

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
                
            }
        }
    }
}
