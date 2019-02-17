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

        public IconHandler(string[] args)
        {
            MenuItem consoleMenuItem = new MenuItem("Show", new EventHandler(Show));
            MenuItem configMenuItem = new MenuItem("Configuration", new EventHandler(ShowConfig));
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));


            notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); ;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { consoleMenuItem, configMenuItem, exitMenuItem });
            notifyIcon.MouseClick += new MouseEventHandler(LeftClick);
            //Dont show icon untill everything is set ups
            notifyIcon.Visible = false;

            Client = new ClientManager();

            try
            {
                Client.Connect().Wait();
            }catch(AggregateException ex)
            {
                MessageBox.Show("Couldn´t connect:\n" + ex.InnerException.Message);

                Exit();
                return;
            }
            catch(Exception e)
            {
                MessageBox.Show("Couldn´t connect:\n" + e.Message);

                Exit();
                return;
            }

            mainForm = new MainForm(Client);

            //if client is not logged in -> show a login prompt
            if(Client.LoggedIn == false)
            {
                mainForm.Show();
            }

            FileUploadHandler.Init(mainForm, Client);
            mainForm.CreateControl();

            //Everything is ready -> show icon
            notifyIcon.Visible = true;

            var argumentsHandler = new ArgumentsHandler(mainForm);

            if (args.Length != 0)
                argumentsHandler.HandleArguments(args);

            PipeHandler.Start(argumentsHandler);
        }

        void ShowConfig(object sender, EventArgs e)
        {
            mainForm.ShowLogin();
        }

        void Exit(object sender, EventArgs e)
        {
            Exit();
        }

        void Exit()
        {
            Application.Exit();
            Environment.Exit(0);
        }

        void Show(object sender, EventArgs e)
        {
            mainForm.Show();
        }

        public void ShowMain()
        {
            //if(Invok)
        }

        void LeftClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mainForm?.Show();
            }
        }
    }
}
