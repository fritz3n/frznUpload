using frznUpload.Client.Handlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
			notifyIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); ;
			notifyIcon.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
			notifyIcon.ContextMenuStrip.Items.Add("Show", null, Show);
			notifyIcon.ContextMenuStrip.Items.Add("Configuration", null, ShowConfig);
			notifyIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

			notifyIcon.MouseClick += new MouseEventHandler(LeftClick);
			//Dont show icon untill everything is set ups
			notifyIcon.Visible = false;

			CertificateHandler.Load();

			Client = new ClientManager();

			try
			{
				Task.Run(() => Client.Connect()).Wait();
			}
			catch (AggregateException ex)
			{
				MessageBox.Show("Agg Couldn´t connect:\n" + ex.InnerException.Message);



				string text = $"AggregateException with {ex.InnerExceptions.Count} InnerExceptions:\n" + ex.ToString() + "\nInnerExceptions:";

				foreach (Exception exception in ex.InnerExceptions)
					text += "\n" + exception.ToString();

				File.WriteAllText("crashLog " + DateTime.Now.ToString("dd-MM-yy HH_mm") + ".txt", text);

				Thread.Sleep(1000);

				Exit();
				return;
			}
			catch (Exception e)
			{
				MessageBox.Show("Couldn´t connect:\n" + e.Message);
				File.WriteAllText("crashLog " + DateTime.Now.ToString("dd-MM-yy HH_mm") + ".txt", e.ToString());

				Exit();
				return;
			}

			mainForm = new MainForm(Client);

			//if client is not logged in -> show a login prompt
			if (Client.LoggedIn == false)
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
			mainForm.WindowState = FormWindowState.Normal;
			mainForm.BringToFront();
			mainForm.Activate();
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
